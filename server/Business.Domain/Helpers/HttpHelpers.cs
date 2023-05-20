using Polly;
using Polly.Contrib.WaitAndRetry;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Business.Domain.Helpers
{
    public static class HttpHelpers
    {
        private static readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
            Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => r.StatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3));

        public static async Task<HttpResponseMessage> SendRequestRaw(string path, HttpMethod method, AuthenticationHeaderValue authentication = null, string content = null, string queryString = null)
        {
            using (HttpClient client = new HttpClient())
            {
                if (authentication is not null)
                    client.DefaultRequestHeaders.Authorization = authentication;

                UriBuilder builder = new UriBuilder(path);

                if (queryString is not null)
                    builder.Query += queryString;

                HttpRequestMessage httpRequest = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(builder.ToString())
                };

                if (content is not null)
                    httpRequest.Content = new StringContent(content, Encoding.UTF8, "application/json");

                HttpResponseMessage resultado = await _retryPolicy.ExecuteAsync(() => client.SendAsync(httpRequest));

                return resultado;
            }
        }
    }
}
