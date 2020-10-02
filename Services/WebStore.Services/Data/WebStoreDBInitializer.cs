using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;

namespace WebStore.Services.Data
{
    public class WebStoreDBInitializer
    {
        private readonly WebStoreDB _db;
        private readonly UserManager<User> _UserManager;
        private readonly RoleManager<Role> _RoleManager;
        private readonly ILogger<WebStoreDBInitializer> _Logger;

        public WebStoreDBInitializer(WebStoreDB db, UserManager<User> UserManager, RoleManager<Role> RoleManager, ILogger<WebStoreDBInitializer> Logger)
        {
            _db = db;
            _UserManager = UserManager;
            _RoleManager = RoleManager;
            _Logger = Logger;
        }

        public void Initialize()
        {
            _Logger.LogInformation("Database initialization ....");

            var db = _db.Database;

            //if(db.EnsureDeleted())
            //    if(!db.EnsureCreated())
            //        throw new InvalidOperationException("Ошибка при создании БД");

            try
            {
                _Logger.LogInformation("Database migration");
                db.Migrate();

                _Logger.LogInformation("Product catalog initialization");
                InitializeProducts();

                _Logger.LogInformation("Employee Directory Initialization");
                InitializeEmployees();

                _Logger.LogInformation("Identity System Data Initialization");
                InitializeIdentityAsync().Wait();
            }
            catch (Exception error)
            {
                _Logger.LogCritical(new EventId(0), error, "Database initialization process error");

                throw;
            }

            _Logger.LogInformation("Database initialization completed successfully");
        }

        private void InitializeProducts()
        {
            if (_db.Products.Any())
            {
                _Logger.LogInformation("The product catalog is already initialized");
                return;
            }

            var db = _db.Database;
            using (db.BeginTransaction())
            {
                _db.Sections.AddRange(TestData.Sections);

                db.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ProductSections] ON");
                _db.SaveChanges();
                db.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ProductSections] OFF");

                db.CommitTransaction();
            }

            using (db.BeginTransaction())
            {
                _db.Brands.AddRange(TestData.Brands);

                db.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ProductBrands] ON");
                _db.SaveChanges();
                db.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[ProductBrands] OFF");

                db.CommitTransaction();
            }

            using (db.BeginTransaction())
            {
                _db.Products.AddRange(TestData.Products);

                db.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Products] ON");
                _db.SaveChanges();
                db.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Products] OFF");

                db.CommitTransaction();
            }

            //var products = TestData.Products;
            //var sections = TestData.Sections;
            //var brands = TestData.Brands;

            //var product_section = products.Join(
            //    sections, 
            //    p => p.SectionId, 
            //    s => s.Id, 
            //    (product, section) => (product, section));

            //foreach (var (product, section) in product_section)
            //{
            //    product.Section = section;
            //    product.SectionId = 0;
            //}

            //var product_brand = products.Join(
            //    brands,
            //    p => p.BrandId,
            //    b => b.Id,
            //    (product, brand) => (product, brand));

            //foreach (var (product, brand) in product_brand)
            //{
            //    product.Brand = brand;
            //    product.BrandId = null;
            //}

            //foreach (var product in products)
            //    product.Id = 0;

            //var child_sections = sections.Join(
            //    sections,
            //    child => child.ParentId,
            //    parent => parent.Id,
            //    (child, parent) => (child, parent));

            //foreach (var (child, parent) in child_sections)
            //{
            //    child.ParentSection = parent;
            //    child.ParentId = null;
            //}

            //foreach (var section in sections)
            //    section.Id = 0;

            //foreach (var brand in brands)
            //    brand.Id = 0;


            //using (db.BeginTransaction())
            //{
            //    _db.Sections.AddRange(sections);
            //    _db.Brands.AddRange(brands);
            //    _db.Products.AddRange(products);
            //    _db.SaveChanges();
            //    db.CommitTransaction();
            //}
        }

        private void InitializeEmployees()
        {
            if (_db.Employees.Any())
            {
                _Logger.LogInformation("The employee section has already been initialized");
                return;
            }

            using (_db.Database.BeginTransaction())
            {
                TestData.Employees.ForEach(employee => employee.Id = 0);

                _db.Employees.AddRange(TestData.Employees);

                _db.SaveChanges();

                _db.Database.CommitTransaction();
            }
        }

        private async Task InitializeIdentityAsync()
        {
            async Task CheckRoleExist(string RoleName)
            {
                if (!await _RoleManager.RoleExistsAsync(RoleName))
                {
                    _Logger.LogInformation("Adding user roles {0}", RoleName);
                    await _RoleManager.CreateAsync(new Role { Name = RoleName });
                }
            }

            await CheckRoleExist(Role.Administrator);
            await CheckRoleExist(Role.User);

            if (await _UserManager.FindByNameAsync(User.Administrator) is null)
            {
                var admin = new User { UserName = User.Administrator };
                var creation_result = await _UserManager.CreateAsync(admin, User.DefaultAdminPassword);
                if (creation_result.Succeeded)
                {
                    _Logger.LogInformation("User {0} added", User.Administrator);
                    await _UserManager.AddToRoleAsync(admin, Role.Administrator);
                    _Logger.LogInformation("Role added for user {0} {1}", User.Administrator, Role.Administrator);
                }
                else
                {
                    var errors = creation_result.Errors.Select(e => e.Description);
                    throw new InvalidOperationException($"Error creating user Administrator: {string.Join(", ", errors)}");
                }
            }
        }
    }
}