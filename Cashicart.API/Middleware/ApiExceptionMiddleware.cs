using System.Net;
using System.Text.Json;
using Cashicart.Common.Responses;
using Cashicart.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Cashicart.API.Middleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict detected");
                await WriteResponse(context, HttpStatusCode.Conflict, "The product was modified or deleted by another process. Please refresh and try again.");
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Business rule violation");
                await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error");
                await WriteResponse(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private async Task WriteResponse(HttpContext context, HttpStatusCode status, string message)
        {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}





