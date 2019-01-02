using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Playground.Core.Responses;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Playground.Core.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException businessError)
            {
                await HandleBusinessExceptionAsync(context, businessError);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        public static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var jsonMessage = JsonConvert.SerializeObject(new ApiResponse<string> { IsSuccess = false, ErrorMessages = exception.Message, Data = null });
            return context.Response.WriteAsync(jsonMessage);
        }

        public static Task HandleBusinessExceptionAsync(HttpContext context, BusinessException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            var jsonMessage = JsonConvert.SerializeObject(new ApiResponse<string> { IsSuccess = false, Data = null, ErrorMessages = exception.Message });
            return context.Response.WriteAsync(jsonMessage);
        }
    }
}
