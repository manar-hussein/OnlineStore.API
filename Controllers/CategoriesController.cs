using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;
        private readonly IMapper _Mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(ICategoryServices categoryServices, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _categoryServices = categoryServices;
            _Mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAll")]
        //[Authorize(AuthenticationSchemes ="Bearer")]
        public IActionResult GetAll()
        {
            try
            {
                var categories = _categoryServices.GetCategories();
                if (categories.Any())
                {
                    return Ok(new GeneralResponse<IEnumerable<CategoriesDTO>>(true, "Categories retrieved successfully", _Mapper.Map<IEnumerable<CategoriesDTO>>(categories)));
                }
                return Ok(new GeneralResponse<string>(false, "No categories available"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>(false, "An error occurred while retrieving categories.", ex.Message));
            }
        }

        [HttpGet("Get/{id:int}")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult GetById(int id)
        {
            try
            {
                var category = _categoryServices.GetCategory(id);
                if (category != null)
                {
                    return Ok(new GeneralResponse<CategoriesDTO>(true, "Category retrieved successfully", _Mapper.Map<CategoriesDTO>(category)));
                }
                return Ok(new GeneralResponse<string>(false, $"No category found with ID: {id}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>(false, "An error occurred while retrieving the category.", ex.Message));
            }
        }

        [HttpPost("Add")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult Add(CategoriesDTO newCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var addedCategory = _Mapper.Map<Category>(newCategory);
                    _categoryServices.AddCategory(addedCategory);
                    _unitOfWork.Commit();
                    return Ok(new GeneralResponse<Category>(true, "Category added successfully", addedCategory));
                }
                return Ok(new GeneralResponse<string>(false, "Failed to add category. Invalid data."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>(false, "An error occurred while adding the category.", ex.Message));
            }
        }

        [HttpPut("Update")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult Update(int id, CategoriesDTO updatedCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCategory = _categoryServices.GetCategory(id);
                    if (existingCategory != null)
                    {
                        // Map updated data to the existing entity
                        _Mapper.Map(updatedCategory, existingCategory);

                        _categoryServices.UpdateCategory(existingCategory);
                        _unitOfWork.Commit();

                        return Ok(new GeneralResponse<Category>(true, "Category updated successfully", existingCategory));
                    }
                    return Ok(new GeneralResponse<string>(false, $"No category found with ID: {id}"));
                }
                return Ok(new GeneralResponse<string>(false, "Failed to update category. Invalid data."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>(false, "An error occurred while updating the category.", ex.Message));
            }
        }

        [HttpDelete("Delete/{id:int}")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult Delete(int id)
        {
            try
            {
                var category = _categoryServices.GetCategory(id);
                if (category != null)
                {
                    _categoryServices.RemoveCategory(id);
                    _unitOfWork.Commit();
                    return Ok(new GeneralResponse<string>(true, "Category deleted successfully"));
                }
                return Ok(new GeneralResponse<string>(false, $"Failed to delete category. No category found with ID: {id}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>(false, "An error occurred while deleting the category.", ex.Message));
            }
        }
    }
}
