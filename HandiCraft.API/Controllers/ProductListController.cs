using HandiCraft.Application.DTOs.ProductList;
using HandiCraft.Application.Interfaces;
using HandiCraft.Application.Specificatoins;
using HandiCraft.Domain.ProductList;
using HandiCraft.Infrastructure.Services.ProductList;
using HandiCraft.Presentation;
using HandiCraft.Presistance.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HandiCraft.API.Controllers
{
    
    public class ProductListController : APIControllerBase
    {
        private readonly IProductListServices _productListService;
        private readonly HandiCraftDbContext _context;

        public ProductListController(IProductListServices productListService, HandiCraftDbContext context)
        {
            _productListService = productListService;
            _context = context;
        }




        [HttpPost("AddProduct")]
        [Authorize]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new Response(401));

            

            var result = await _productListService.AddProductAsync(userId,dto);
           
            if (result ==null)
                return BadRequest(new Response(400));

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {

            var result = await _productListService.GetProductByIdAsync(id);

            if (result == null)
                return NotFound(new Response(404));

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{productId}/react")]
        public async Task<IActionResult> AddReaction(Guid productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new Response(401,"User not logged in"));

            var success = await _productListService.AddProductReactionAsync(userId, productId);

            if (!success)
                return BadRequest(new Response(400,"Could not add reaction (maybe already reacted or product not found)"));

            return Ok("Reaction added successfully");
        }

        [Authorize]
        [HttpDelete("{productId}/react")]
        public async Task<IActionResult> RemoveReaction(Guid productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(new Response(401,"User not logged in"));

            var success = await _productListService.RemoveProductReactionAsync(userId, productId);

            if (!success)
                return NotFound(new Response(404,"Reaction not found for this product"));

            return Ok("Reaction removed successfully");
        }
        [HttpGet("Products")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductSpecParams productParams)
        {
            if (productParams.PageSize <= 0) productParams.PageSize = 10;
            if (productParams.PageIndex <= 0) productParams.PageIndex = 1;

            var result = await _productListService.GetAllProductsAsync(productParams);
            return Ok(result);
        }
        [HttpPut("update/{productId}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(Guid productId,[FromForm] UpdateProductDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return NotFound(new Response(400));
            }

            var result = await _productListService.UpdateProductAsync(userId,productId, dto);

            if (result == null)
                return BadRequest(new Response(400,"Product Not found "));

            return Ok(result);
        } 

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId is null)
            {
                return NotFound(new Response(400));
            }
            
            var deleted = await _productListService.DeleteProductAsync(userId,productId);

            if (!deleted)
                return NotFound(new Response(404));

            return NoContent();
        }

    }
}
