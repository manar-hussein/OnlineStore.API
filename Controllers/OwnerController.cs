using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.Interfaces;

namespace OnlineStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnerController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet("GetAllOwners")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult GetAllOwners()
        {
            try
            {
                // Get all owners
                var owners = _ownerService.GetAllOwners();

                if (owners == null)
                {
                    return Ok(new GeneralResponse<string>(false, "No owners found."));
                }

                return Ok(new GeneralResponse<IEnumerable<User>>(true, "Owners retrieved successfully.", owners));
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse<string>(false, $"An error occurred while retrieving owners: {ex.Message}"));
            }
        }

        [HttpPost("DemoteOwner")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult DemoteOwner(string ownerEmail)
        {
            try
            {
                _ownerService.DemoteOwnerToCustomer(ownerEmail);
                return Ok(new GeneralResponse<string>(true, "Owner demoted to customer successfully."));
            }
            catch (ArgumentException ex)
            {
                // Handle case where the owner was not found
                return BadRequest(new GeneralResponse<string>(false, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new GeneralResponse<string>(false, $"An error occurred while demoting the owner: {ex.Message}"));
            }
        }
    }
}
