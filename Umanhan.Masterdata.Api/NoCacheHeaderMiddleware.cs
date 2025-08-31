using Microsoft.AspNetCore.OutputCaching;

namespace Umanhan.Masterdata.Api
{
    public class NoCacheHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public NoCacheHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check for "no-cache: true" in request header
            if (context.Request.Headers.TryGetValue("no-cache", out var value) &&
                value.ToString().ToLowerInvariant() == "true")
            {
                // Tell the OutputCache middleware to skip this request
                context.Features.Set<IOutputCacheFeature>(new BypassOutputCacheFeature(context));
            }

            await _next(context);
        }
    }

    public class BypassOutputCacheFeature : IOutputCacheFeature
    {
        public bool AllowCacheStorage { get; set; } = false;
        public bool AllowCacheLookup { get; set; } = false;
        public bool AllowLocking { get; set; } = false;

        public OutputCacheContext Context { get; set; }

        public BypassOutputCacheFeature(HttpContext httpContext)
        {
            Context = new OutputCacheContext
            {
                HttpContext = httpContext
            };
        }
    }

}
