using AudiophileAPI.DataAccess.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();

        Task<Product> GetProductById(int id);

        Task<Product> AddProduct(DTO.ProductDTO product);

        Task<Product> UpdateProduct(int productId, string productName, string productDescription, string productFeatures, decimal productPrice,
            int productStock, int categoryId, string imageURL);

        Task DeleteProduct(int id);
    }
}
