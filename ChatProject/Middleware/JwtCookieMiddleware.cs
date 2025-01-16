namespace ChatProject.Middleware;

public class JwtCookieMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtCookieMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies["AuthToken"]; // Get JWT from cookie

        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers["Authorization"] = "Bearer " + token; // Set it in the Authorization header
        }

        await _next(context); // Continue the request pipeline
    }
}