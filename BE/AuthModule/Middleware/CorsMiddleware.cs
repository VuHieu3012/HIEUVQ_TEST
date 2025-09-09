using Microsoft.AspNetCore.Http;

namespace AuthModule.Middleware
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;

        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Set CORS headers
            context.Response.Headers["Access-Control-Allow-Origin"] = "http://localhost:3000";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization, X-Requested-With, Accept, Origin";
            context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
            context.Response.Headers["Access-Control-Max-Age"] = "86400";

            // Handle preflight requests
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(string.Empty);
                return;
            }

            await _next(context);
        }
    }
}
