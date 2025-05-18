using AudiophileAPI.DataAccess.EF.Context;
using AudiophileAPI.DataAccess.EF.Interfaces;
using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly AudiophileAPIDbContext _context;

        public ProductRepository(AudiophileAPIDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();

        }

        public async Task<Product> GetProductById(int id)
        {
            var user = await _context.Products.FindAsync(id);

            if (user == null)
            {
                throw new Exception("Product not found");
            }

            return user;
        }

        public async Task<Product> AddProduct(ProductDTO productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Features = productDto.Features,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId,
                ImageUrl = productDto.ImageURL
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> UpdateProduct(int productId, string productName, string productDescription, string productFeatures, decimal productPrice,
            int productStock, int categoryId, string imageUrl)
        {
            var existingProduct = await _context.Products.FindAsync(productId);

            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            existingProduct.Name = productName;
            existingProduct.Description = productDescription;
            existingProduct.Features = productFeatures;
            existingProduct.Price = productPrice;
            existingProduct.Stock = productStock;
            existingProduct.CategoryId = categoryId;
            existingProduct.ImageUrl = imageUrl;

            await _context.SaveChangesAsync();

            return existingProduct;
        }

        public async Task DeleteProduct(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
