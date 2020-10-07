using System.Collections.Generic;
using WebStore.Domain;
using WebStore.Domain.DTO.Products;
using WebStore.Domain.Entities;

namespace WebStore.Interfaces.Service
{
    public interface IProductData
    {
        //IEnumerable<Section> GetSections();

        //IEnumerable<Brand> GetBrands();

        //IEnumerable<Product> GetProducts(ProductFilter Filter = null);

        //Product GetProductById(int id);

        IEnumerable<SectionDTO> GetSections();

        SectionDTO GetSectionById(int id);

        IEnumerable<BrandDTO> GetBrands();

        BrandDTO GetBrandById(int id);

        IEnumerable<ProductDTO> GetProducts(ProductFilter Filter = null);
        ProductDTO GetProductById(int id);
    }
}
