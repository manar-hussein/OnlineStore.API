using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs.Products;
using OnlineStore.Application.Interfaces;
using OnlineStore.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

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
            try
            {
                var products = _productServices.AllProducts().ToList();

                if (products == null || products.Count == 0)
                {
                    var notFoundResponse = new GeneralResponse<List<ProductElementDTO>>(false, "No products found");
                    return NotFound(notFoundResponse);
                }

                var response = new GeneralResponse<List<ProductElementDTO>>(true, "Products retrieved successfully", products);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new GeneralResponse<List<ProductElementDTO>>(false, $"Error retrieving products: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }


        [HttpGet("Var")]
        public ActionResult<GeneralResponse<List<ProductVariants>>> Allvar()
        {
            try
            {
                var productVariants = _productServices.AllProductsVariants().ToList();
                var response = new GeneralResponse<List<ProductVariants>>(true, "Product variants retrieved successfully", productVariants);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new GeneralResponse<List<ProductVariants>>(false, $"Error retrieving product variants: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<GeneralResponse<ProductDetailsDTO>> ProductById(int id)
        {
            try
            {
                var productDetails = _productServices.ProductDetails(id);
                if (productDetails == null)
                {
                    var notFoundResponse = new GeneralResponse<ProductDetailsDTO>(false, "Product not found");
                    return NotFound(notFoundResponse);
                }

                var response = new GeneralResponse<ProductDetailsDTO>(true, "Product details retrieved successfully", productDetails);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new GeneralResponse<ProductDetailsDTO>(false, $"Error retrieving product details: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}
