using Business.Domain.Interfaces.Repositories;
using Business.Domain.Models.Others;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;

namespace Business.Api.Filters
{
    public class RequestLoggingFilter : IAsyncActionFilter
    {
        private readonly ILogRequestRepository _logRequestRepository;
        private Stopwatch _stopWatch;

        private const int DefaultBuffer = 81920;

        public RequestLoggingFilter(ILogRequestRepository logRequestRepository)
        {
            _logRequestRepository = logRequestRepository;
            _stopWatch = new Stopwatch();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Path.Value.Contains("/api/"))
            {
                _stopWatch.Reset();
                _stopWatch.Start();
            }

            ActionExecutedContext actionContext = await next.Invoke();

            if (context.HttpContext.Request.Path.Value.Contains("/api/"))
            {
                _stopWatch.Stop();
                CancellationToken cancellationToken = context.HttpContext.RequestAborted;
                ClaimsPrincipal user = context.HttpContext.User;
                long executionTime = _stopWatch.ElapsedMilliseconds;
                PathString path = context.HttpContext.Request.Path;
                string controller = context.Controller.ToString().Replace("Business.Api.Controllers.", string.Empty);
                string httpVerb = context.HttpContext.Request.Method;
                string queryString = context.HttpContext.Request.QueryString.ToString();
                string userId = user.Claims.Count() > 0 ? user.Identities.FirstOrDefault().Claims.FirstOrDefault(w => w.Type == "Id").Value : string.Empty;
                string userIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
                int statusCode = context.HttpContext.Response.StatusCode;
                context.HttpContext.Request.EnableBuffering();
                string requestBody = await GetRequestBody(context, cancellationToken);
                if (userIp == "::1")
                    userIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == AddressFamily.InterNetwork).ToString();
                LogRequest logRequisicao = new LogRequest(path, controller, httpVerb, queryString, requestBody, userId, userIp, statusCode, executionTime);
                await _logRequestRepository.Post(logRequisicao);
            }

        }

        protected static async Task<string> GetRequestBody(ActionExecutingContext actionContext, CancellationToken cancellationToken)
        {
            Stream body = actionContext.HttpContext.Request.Body;
            if (body is not null && body.CanRead)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    int bufferSize = DefaultBuffer;
                    if (body.CanSeek)
                    {
                        body.Seek(0, SeekOrigin.Begin);
                        bufferSize = (int)Math.Min(bufferSize, body.Length < 2 ? 1 : body.Length);
                    }
                    await body.CopyToAsync(stream, bufferSize, cancellationToken);
                    if (body.CanSeek)
                    {
                        body.Seek(0, SeekOrigin.Begin);
                    }
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            return string.Empty;
        }
    }

    public static class HttpRequestExtension
    {
        public static string GetHeader(this HttpRequest request, string key)
            => request.Headers.FirstOrDefault(x => x.Key == key).Value.FirstOrDefault();
    }
}
