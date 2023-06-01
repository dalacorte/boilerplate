using Business.Api.Extensions;
using Microsoft.AspNetCore.Localization;

namespace Business.Api.Providers
{
    public class UrlRequestCultureProvider : IRequestCultureProvider
    {
        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            string culture = httpContext.GetLanguage();
            return Task.FromResult(new ProviderCultureResult(culture));
        }
    }
}
