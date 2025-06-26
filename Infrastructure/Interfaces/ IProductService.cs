using Domain.DTOs.Product;
using Domain.Filtres;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    Task<Response<List<ProductDto>>> GetAllAsync(ProductFilter filter);
    Task<Response<ProductDto>> GetByIdAsync(int id);
    Task<Response<ProductDto>> CreateAsync(CreateProductDto productDto);
    Task<Response<ProductDto>> UpdateAsync(int id, UpdateProductDto productDto);
    Task<Response<string>> DeleteAsync(int id);

}
