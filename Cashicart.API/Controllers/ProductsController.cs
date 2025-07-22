using Asp.Versioning;
using Cashicart.Application.Features.Products.Commands;
using Cashicart.Application.Features.Products.Queries;
using Cashicart.Common.Responses;
using MediatR;
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

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProduct), new { id = productId }, ApiResponse<Guid>.SuccessResponse(productId, "Product created successfully."));
    }


    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(ApiResponse<object>.SuccessResponse(products, "Products fetched successfully."));
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _mediator.Send(new GetProductQuery { ProductId = id });
        return Ok(ApiResponse<object>.SuccessResponse(product, "Product fetched successfully."));
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.ProductId)
            return BadRequest(ApiResponse<string>.FailureResponse("Product ID in URL and payload do not match."));

        await _mediator.Send(command);
        return Ok(ApiResponse<string>.SuccessResponse(null, "Product updated successfully."));
    }


    [HttpPost("{id}/upload-images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImages(Guid id, [FromForm] List<IFormFile> files)
    {
        var command = new UploadProductImagesCommand
        {
            ProductId = id,
            Files = files
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Images uploaded successfully."));
    }


    [HttpDelete("{productId}/images/{imageId}")]
    public async Task<IActionResult> DeleteProductImage(Guid productId, Guid imageId)
    {
        await _mediator.Send(new DeleteProductImageCommand
        {
            ProductId = productId,
            ImageId = imageId
        });

        return Ok(ApiResponse<string>.SuccessResponse(null, "Image deleted successfully."));
    }


    [HttpPost("{productId}/images/{imageId}/set-primary")]
    public async Task<IActionResult> SetPrimaryImage(Guid productId, Guid imageId)
    {
        await _mediator.Send(new SetPrimaryProductImageCommand
        {
            ProductId = productId,
            ImageId = imageId
        });

        return Ok(ApiResponse<string>.SuccessResponse(null, "Primary image set successfully."));
    }


    [HttpPost("{productId}/variants")]
    public async Task<IActionResult> CreateVariant(Guid productId, [FromBody] CreateProductVariantCommand command)
    {
        if (productId != command.ProductId)
            return BadRequest(ApiResponse<string>.FailureResponse("ProductId mismatch"));

        var variantId = await _mediator.Send(command);
        return Ok(ApiResponse<Guid>.SuccessResponse(variantId, "Variant created successfully."));
    }


    [HttpPut("{productId}/variants/{variantId}")]
    public async Task<IActionResult> UpdateVariant(Guid productId, Guid variantId, [FromBody] UpdateProductVariantCommand command)
    {
        if (productId != command.ProductId || variantId != command.VariantId)
            return BadRequest(ApiResponse<string>.FailureResponse("Mismatched product or variant ID."));

        await _mediator.Send(command);
        return Ok(ApiResponse<string>.SuccessResponse(null, "Variant updated successfully."));
    }

    
    [HttpDelete("{productId}/variants/{variantId}")]
    public async Task<IActionResult> DeleteVariant(Guid productId, Guid variantId)
    {
        await _mediator.Send(new DeleteProductVariantCommand
        {
            ProductId = productId,
            VariantId = variantId
        });

        return Ok(ApiResponse<string>.SuccessResponse(null, "Variant deleted successfully."));
    }
}