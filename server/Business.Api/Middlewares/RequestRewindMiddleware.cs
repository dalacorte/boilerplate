using Microsoft.Extensions.Options;

namespace Business.Api.Middlewares
{
    public class RequestRewindMiddleware
    {
        private readonly RequestRewindConfigs _options;
        private readonly RequestDelegate _next;

        public RequestRewindMiddleware(IOptions<RequestRewindConfigs> options, RequestDelegate next)
        {
            _options = options.Value;
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (_options.AllowedContentTypes.Contains(context.Request.ContentType) &&
               context.Request.ContentLength < _options.MaxSizeBytes)
            {
                context.Request.EnableBuffering();
            }

            return _next(context);
        }
    }

    public class RequestRewindConfigs
    {
        public ISet<string> AllowedContentTypes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "application/json" };

        public long MaxSizeBytes { get; set; } = 1024 * 1024;
    }
}
