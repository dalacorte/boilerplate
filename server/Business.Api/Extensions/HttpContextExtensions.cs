using System.Text.RegularExpressions;

namespace Business.Api.Extensions
{
    public static class HttpContextExtensions
    {
        const string DefaultLanguage = "pt-BR";

        public static string GetLanguage(this HttpContext httpContext)
        {
            PathString url = httpContext.Request.Path;
            List<string> parts = httpContext.Request.Path.Value
                         .Split('/')
                         .Where(p => !string.IsNullOrWhiteSpace(p))
                         .ToList();

            if (!parts.Any())
                return DefaultLanguage;

            if (!Regex.IsMatch(parts[0], @"^[a-z]{2}(?:-[A-Z]{2})?$"))
                return DefaultLanguage;

            return parts[0];
        }
    }
}
