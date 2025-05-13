using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DataAccess.EF.Repositories;
using AudiophileAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AudiophileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly CategoryRepository _categoryRepository;

        public CategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("all-users")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            try
            {
                var category = await _categoryRepository.GetAllCategories();
                if (category == null || !category.Any())
                {
                    return NotFound("No categories found");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while fetching categories.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("find-by-id/{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryById(id);

                if (category == null)
                {
                    return NotFound(new
                    {
                        Message = $"Category with ID {id} was not found."
                    });
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return Problem(
                     detail: ex.Message,
                     title: "An error occurred while fetching Category.",
                     statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("create-category")]
        public async Task<ActionResult> CreateUser([FromBody] Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "All fields are required."
                    });
                }
                var createdCategory = await _categoryRepository.AddCategory(category);

                return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.CategoryId }, createdCategory);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while creating the category.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("update-category/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            try
            {
                
                if (category.CategoryId != 0 && category.CategoryId != id)
                {
                    return BadRequest("Category ID in the body does not match URL.");
                }

                if (string.IsNullOrWhiteSpace(category.CategoryName))
                {
                    return BadRequest("Name cannot be empty.");
                }

                var updated = await _categoryRepository.UpdateCategory(id, category.CategoryName);

                if (updated == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while updating the category.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("delete-category/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                await _categoryRepository.DeleteCategory(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while deleting the category.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
