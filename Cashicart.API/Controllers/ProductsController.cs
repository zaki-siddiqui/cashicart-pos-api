using Asp.Versioning;
using Cashicart.Application.Features.Products.Commands;
using Cashicart.Application.Features.Products.Queries;
using Cashicart.Domain.Entities;
using Cashicart.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Cashicart.API.Controllers;

//[Route("api/[controller]")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
//[Authorize(Roles = "Admin,Manager")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        try
        {
            var productId = await _mediator.Send(command);
            _logger.LogInformation("Product created with ID {ProductId}", productId);
            return CreatedAtAction(nameof(GetProduct), new { id = productId }, new { ProductId = productId });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation error");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error");
            return StatusCode(500, "An internal server error occurred.");
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsQuery query)
    {
        try
        {
            var products = await _mediator.Send(query);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
            return StatusCode(500, "An error occurred while fetching products.");
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        try
        {
            var product = await _mediator.Send(new GetProductQuery { ProductId = id });
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product with ID {ProductId}", id);
            return StatusCode(500, "An error occurred while fetching the product.");
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.ProductId)
            return BadRequest("Product ID in URL and payload do not match.");

        try
        {
            await _mediator.Send(command);
            _logger.LogInformation("Product updated with ID {ProductId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
            return StatusCode(500, ex.Message);
        }
    }


    [HttpPost("{id}/upload-images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImages(Guid id, [FromForm] List<IFormFile> files)
    {
        try
        {
            var command = new UploadProductImagesCommand
            {
                ProductId = id,
                Files = files
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading images for product {ProductId}", id);
            return StatusCode(500, ex.Message);
        }
    }


    [HttpDelete("{productId}/images/{imageId}")]
    public async Task<IActionResult> DeleteProductImage(Guid productId, Guid imageId)
    {
        try
        {
            var command = new DeleteProductImageCommand
            {
                ProductId = productId,
                ImageId = imageId
            };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product image");
            return StatusCode(500, ex.Message);
        }
    }


    [HttpPost("{productId}/images/{imageId}/set-primary")]
    public async Task<IActionResult> SetPrimaryImage(Guid productId, Guid imageId)
    {
        try
        {
            await _mediator.Send(new SetPrimaryProductImageCommand
            {
                ProductId = productId,
                ImageId = imageId
            });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting primary image");
            return StatusCode(500, ex.Message);
        }
    }


    [HttpPost("{productId}/variants")]
    public async Task<IActionResult> CreateVariant(Guid productId, [FromBody] CreateProductVariantCommand command)
    {
        try
        {
            if (productId != command.ProductId)
                return BadRequest("ProductId mismatch");

            var variantId = await _mediator.Send(command);
            _logger.LogInformation("Created variant {VariantId} for product {ProductId}", variantId, productId);
            return Ok(new { VariantId = variantId });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product variant");
            return StatusCode(500, "An error occurred while creating the variant.");
        }
    }


    [HttpPut("{productId}/variants/{variantId}")]
    public async Task<IActionResult> UpdateVariant(Guid productId, Guid variantId, [FromBody] UpdateProductVariantCommand command)
    {
        try
        {
            if (productId != command.ProductId || variantId != command.VariantId)
                return BadRequest("Mismatched product or variant ID.");

            await _mediator.Send(command);
            return Ok(new { message = "Variant updated successfully." });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating variant");
            return StatusCode(500, "An error occurred while updating the variant.");
        }
    }

    
    [HttpDelete("{productId}/variants/{variantId}")]
    public async Task<IActionResult> DeleteVariant(Guid productId, Guid variantId)
    {
        try
        {
            await _mediator.Send(new DeleteProductVariantCommand
            {
                ProductId = productId,
                VariantId = variantId
            });

            return Ok(new { message = "Variant deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting variant");
            return StatusCode(500, "An error occurred while deleting the variant.");
        }
    }


}