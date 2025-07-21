//using Cashicart.Common.Responses;
//using Cashicart.Domain.Exceptions;
//using System.ComponentModel.DataAnnotations;
//using System.Net;
//using System.Text.Json;

//namespace Cashicart.API.Middleware
//{
//    public class ApiExceptionMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ApiExceptionMiddleware> _logger;

//        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (DomainException ex)
//            {
//                _logger.LogWarning(ex, "Domain validation error");
//                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unexpected error");
//                await HandleExceptionAsync(context, "An unexpected error occurred.", HttpStatusCode.InternalServerError);
//            }
//        }

//        private async Task HandleExceptionAsync(HttpContext context, string message, HttpStatusCode statusCode)
//        {
//            context.Response.ContentType = "application/json";
//            context.Response.StatusCode = (int)statusCode;

//            var response = new ApiErrorResponse
//            {
//                Message = message
//            };

//            var result = JsonSerializer.Serialize(response);
//            await context.Response.WriteAsync(result);
//        }
//    }
//}

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


//using System.Net;
//using Cashicart.Common.Responses;
//using Cashicart.Domain.Exceptions;
//using FluentValidation;
//using Microsoft.EntityFrameworkCore;

//namespace Cashicart.API.Middleware
//{
//    public class ApiExceptionMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ApiExceptionMiddleware> _logger;

//        public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (DomainException ex)
//            {
//                _logger.LogWarning(ex, "Business rule violation");
//                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//                await context.Response.WriteAsJsonAsync(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = ex.Message,
//                    Errors = null
//                });
//            }
//            catch (ValidationException ex)
//            {
//                _logger.LogWarning("Validation error: {Errors}", string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)));
//                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

//                var errorDetails = ex.Errors
//                    .Select(e => new { e.PropertyName, e.ErrorMessage })
//                    .ToList();

//                await context.Response.WriteAsJsonAsync(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "Validation failed.",
//                    Errors = errorDetails
//                });
//            }
//            catch (DbUpdateConcurrencyException ex)
//            {
//                _logger.LogWarning(ex, "Concurrency conflict detected");
//                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
//                await context.Response.WriteAsJsonAsync(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "The product was updated or deleted by another process. Please refresh and try again.",
//                    Errors = null
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unhandled exception occurred");
//                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                await context.Response.WriteAsJsonAsync(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "An unexpected error occurred.",
//                    Errors = null
//                });
//            }
//        }
//    }
//}


