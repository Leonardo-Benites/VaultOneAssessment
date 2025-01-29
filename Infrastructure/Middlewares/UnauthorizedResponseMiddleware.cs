using Microsoft.AspNetCore.Http;
using System.Text.Json; 


public class UnauthorizedResponseMiddleware
{
    private readonly RequestDelegate _next;

    public UnauthorizedResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == 401)
        {
            var apiResponse = new 
            {
                Message = "O Token informado é inválido ou expirou.",
                Code = 401,
                Success = false
            };

            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, apiResponse); 
        }
    }
}