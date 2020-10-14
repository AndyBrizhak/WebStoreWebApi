using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebStore.Domain;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Service;
using WebStore.Interfaces.Services;
using WebStore.Services.Mapping;

namespace WebStore.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductData _ProductData;
        private readonly IConfiguration _Configuration;
        private const string __PageSize = "PageSize";

        public CatalogController(IProductData ProductData, IConfiguration Configuration)
        {
            _ProductData = ProductData;
            _Configuration = Configuration;
        }

        public IActionResult Shop(int? BrandId, int? SectionId, int Page = 1)
        {
            var page_size = int.TryParse(_Configuration[__PageSize], out var size)
                ? size
                : (int?)null;

            var filter = new ProductFilter
            {
                BrandId = BrandId,
                SectionId = SectionId,
                Page = Page,
                PageSize = page_size
            };

            var products = _ProductData.GetProducts(filter);

            return View(new CatalogViewModel
            {
                SectionId = SectionId,
                BrandId = BrandId,
                Products = products.Products.FromDTO().ToView().OrderBy(p => p.Order),
                PageViewModel = new PageViewModel
                {
                    PageSize = page_size ?? 0,
                    PageNumber = Page,
                    TotalItems = products.TotalCount
                }
            });
        }

        public IActionResult Details(int id)
        {
            var product = _ProductData.GetProductById(id);

            if (product is null)
                return NotFound();

            return View(product.FromDTO().ToView());
        }

        #region API

        public IActionResult GetCatalogHtml(int? BrandId, int? SectionId, int Page) =>
            PartialView("Partial/_FeaturesItems", GetProducts(BrandId, SectionId, Page));

        private IEnumerable<ProductViewModel> GetProducts(int? BrandId, int? SectionId, in int Page) =>
            _ProductData.GetProducts(
                    new ProductFilter
                    {
                        SectionId = SectionId,
                        BrandId = BrandId,
                        Page = Page,
                        PageSize = int.Parse(_Configuration[__PageSize])
                    }).Products
                .FromDTO()
                .ToView()
                .OrderBy(p => p.Order);

        #endregion
    }
}