using Domain.DTOs.Category;
using Domain.Filtres;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface ICategoryService
{
    Task<Response<List<CategoryDto>>> GetAllAsync(CategoryFilter filter);
    Task<Response<CategoryDto>> GetByIdAsync(int id);
    Task<Response<CategoryDto>> CreateAsync(CreateCategoryDto dto);
    Task<Response<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<Response<string>> DeleteAsync(int id);
}
