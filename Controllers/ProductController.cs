using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs.Products;
using OnlineStore.Application.Interfaces;

namespace OnlineStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }
        [HttpGet]
        public ActionResult<GeneralResponse<List<ProductElementDTO>>> All()
        {
            GeneralResponse<List<ProductElementDTO>> Response = new GeneralResponse<List<ProductElementDTO>>(true ,"");
            Response.Data = _productServices.AllProducts().ToList();
            return Response;
        }

        [HttpGet("Var")]
        public ActionResult<GeneralResponse<List<ProductVariants>>> Allvar()
        {
            GeneralResponse<List<ProductVariants>> Response = new GeneralResponse<List<ProductVariants>>(true, "");
            Response.Data = _productServices.AllProductsVariants().ToList();
            return Response;
        }


    }
}
