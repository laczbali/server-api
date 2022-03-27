using server_api.Services;

namespace server_api.Middleware
{
    /// <summary>
    /// Performs bearer token based authentication
    /// </summary>
    public class TokenAuthMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Called by app, at startup
        /// </summary>
        /// <param name="next"></param>
        public TokenAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Checks that the request has a Berarer token, that matches the stored token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // no auth needed for test endpoint
            if(context.Request.Path == "/admin/test")
            {
                await _next(context);
                return;
            }

            context.Request.Headers.TryGetValue("Authorization", out var authHeader);

            var authToken = authHeader.ToString().Replace("Bearer ", "");
            if (authToken == null || authToken == "")
            {
                // no token was provided
                context.Response.StatusCode = 401;
                return;
            }

            // check if the token is valid
            if (!AuthService.IsTokenValid(authToken))
            {
                context.Response.StatusCode = 401;
                return;
            }

            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

    /// <summary>
    /// Makes the TokenAuthMiddleware available to the app
    /// </summary>
    public static class TokenAuthMiddlewareExtensions
    {
        /// <summary>
        /// Checks that the request has a Berarer token, that matches the stored token
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTokenAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthMiddleware>();
        }
    }
}