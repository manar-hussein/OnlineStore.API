using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs.Admin;
using OnlineStore.Application.Interfaces;

namespace OnlineStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleServices _roleServices;
        private readonly IUserService _userServices;

        public RoleController(IRoleServices roleServices, IUserService userServices)
        {
            _roleServices = roleServices;
            _userServices = userServices;
        }

        [HttpPost("Create")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult CreateRole(RolteDTO roleDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var roleExists = _roleServices.RoleExists(roleDto.Name);
                    if (roleExists)
                    {
                        return BadRequest(new GeneralResponse<string>(false, "Role already exists."));
                    }

                    _roleServices.CreateRole(roleDto.Name);
                    return Ok(new GeneralResponse<string>(true, "Role created successfully"));
                }
                return BadRequest(new GeneralResponse<string>(false, "Invalid role data."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>(false, "An error occurred while creating the role.", ex.Message));
            }
        }

        [HttpPost("Assign")]
        //[Authorize(AuthenticationSchemes = "Bearer", Roles = Roles.Admin)]
        public IActionResult AssignRoleToUser(string userEmail, string roleName)
        {
            try
            {
                if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(roleName))
                {
                    return BadRequest(new GeneralResponse<string>(false, "User email and role name are required."));
                }

                var user = _userServices.GetUserByEmail(userEmail);
                if (user == null)
                {
                    return NotFound(new GeneralResponse<string>(false, $"No user found with email: {userEmail}"));
                }

                var role = _roleServices.GetRoleByName(roleName);
                if (role == null)
                {
                    return NotFound(new GeneralResponse<string>(false, $"No role found with name: {roleName}"));
                }

                _userServices.AssignRoleToUser(userEmail, roleName);
                return Ok(new GeneralResponse<string>(true, "Role assigned to user successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new GeneralResponse<string>(false, "An error occurred while assigning the role.", ex.Message));
            }
        }
    }
}
