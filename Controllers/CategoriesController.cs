using Microsoft.AspNetCore.Authorization;
using OnlineStore.Infrastructure.DTOs;

namespace OnlineStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private ICategoryServices _categoryServices;
        private IMapper _Mapper;

        public CategoriesController(ICategoryServices categoryServices, IMapper mapper)
        {
            _categoryServices = categoryServices;
            _Mapper = mapper;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public IActionResult Categories()
        {
            IEnumerable<Category> Categories=_categoryServices.GetCategories();

            
            return Ok(_Mapper.Map<IEnumerable<CategoriesDTO>>(Categories));
        }
    }
}
