using System;
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
	public interface IProductService
    {
		Task<ResponseDto?> GetAllProductAsync();
        Task<ResponseDto?> GetProductByIdAsync(int id);
        Task<ResponseDto?> CreateProductAsync(ProductDto productDto);
        Task<ResponseDto?> UpadateProductAsync(ProductDto productDto);
        Task<ResponseDto?> DeleteProductAsync(int id);
    }
}

