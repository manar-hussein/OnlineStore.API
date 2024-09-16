using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Interfaces;

namespace OnlineStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly IMapper _mapper;

        public DiscountController(IDiscountService discountService, IMapper mapper)
        {
            _discountService = discountService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult GetAll()
        {
            try
            {
                var discounts = _discountService.GetAllDiscounts();
                var discountDTOs = _mapper.Map<IEnumerable<DiscountDTO>>(discounts);

                return Ok(new GeneralResponse<IEnumerable<DiscountDTO>>(
                    IsSuccess: true,
                    Message: "Discounts retrieved successfully",
                    data: discountDTOs));
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse<string>(
                    IsSuccess: false,
                    Message: $"Error retrieving discounts: {ex.Message}"));
            }
        }

        [HttpGet("GetById/{id:int}")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult GetById(int id)
        {
            try
            {
                var discount = _discountService.GetDiscountById(id);
                var discountDTO = _mapper.Map<DiscountDTO>(discount);

                return Ok(new GeneralResponse<DiscountDTO>(
                    IsSuccess: true,
                    Message: "Discount retrieved successfully",
                    data: discountDTO));
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse<string>(
                    IsSuccess: false,
                    Message: $"Error retrieving discount: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult Add([FromBody] DiscountDTO discountDTO)
        {
            try
            {
                var discount = _mapper.Map<Discount>(discountDTO);
                _discountService.AddDiscount(discount);

                return Ok(new GeneralResponse<string>(
                    IsSuccess: true,
                    Message: "Discount added successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse<string>(
                    IsSuccess: false,
                    Message: $"Error adding discount: {ex.Message}"));
            }
        }

        [HttpPut("Update/{id:int}")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult Update(int id, [FromBody] DiscountDTO discountDTO)
        {
            try
            {
                var discount = _discountService.GetDiscountById(id);
                if (discount == null)
                {
                    return NotFound(new GeneralResponse<string>(
                        IsSuccess: false,
                        Message: "Discount not found"));
                }

                _mapper.Map(discountDTO, discount);
                _discountService.UpdateDiscount(discount);

                return Ok(new GeneralResponse<string>(
                    IsSuccess: true,
                    Message: "Discount updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse<string>(
                    IsSuccess: false,
                    Message: $"Error updating discount: {ex.Message}"));
            }
        }

        [HttpDelete("Delete/{id:int}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult Delete(int id)
        {
            try
            {
                _discountService.DeleteDiscount(id);

                return Ok(new GeneralResponse<string>(
                    IsSuccess: true,
                    Message: "Discount deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse<string>(
                    IsSuccess: false,
                    Message: $"Error deleting discount: {ex.Message}"));
            }
        }

        [HttpPost("ApplyDiscount")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.BrandOwner)]
        public IActionResult ApplyDiscountToProduct(int productId, int discountId)
        {
            try
            {
                _discountService.ApplyDiscountToProduct(productId, discountId);

                return Ok(new GeneralResponse<string>(
                    IsSuccess: true,
                    Message: "Discount applied to product successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse<string>(
                    IsSuccess: false,
                    Message: $"Error applying discount to product: {ex.Message}"));
            }
        }
    }
}
