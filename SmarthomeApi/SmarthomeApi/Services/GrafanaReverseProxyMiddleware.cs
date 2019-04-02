using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmarthomeApi.Services
{
    public class GrafanaReverseProxyMiddleware
    {
        private static readonly HttpClientHandler _handler = new HttpClientHandler() { AllowAutoRedirect = false };
        private static readonly HttpClient _httpClient = new HttpClient(_handler);
        private readonly RequestDelegate _nextMiddleware;

        public GrafanaReverseProxyMiddleware(RequestDelegate nextMiddleware)
        {
            _nextMiddleware = nextMiddleware;
        }
        
        private Uri BuildTargetUri(HttpRequest request)
        {
            // redirect to static file provider (as configured in Startup.cs)
            if (request.Path.StartsWithSegments("/dashboard/public/build"))
                return null;

            // redirect to grafana via reverse proxy
            if (request.Path.StartsWithSegments("/dashboard", out var remainingPath))
                return new Uri("http://localhost:8088" + remainingPath);

            // redirect to all other controllers and middlewares
            return null;
        }

        public async Task Invoke(HttpContext context)
        {
            var targetUri = BuildTargetUri(context.Request);
            if (targetUri == null)
            {
                await _nextMiddleware(context);
                return;
            }

            var targetRequestMessage = CreateTargetMessage(context, targetUri);

            using (var responseMessage = await _httpClient.SendAsync(targetRequestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                context.Response.StatusCode = (int)responseMessage.StatusCode;
                CopyFromTargetResponseHeaders(context, responseMessage);
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
        }

        private HttpRequestMessage CreateTargetMessage(HttpContext context, Uri targetUri)
        {
            var requestMessage = new HttpRequestMessage();
            CopyFromOriginalRequestContentAndHeaders(context, requestMessage);

            requestMessage.RequestUri = targetUri;
            requestMessage.Headers.Host = targetUri.Host;
            requestMessage.Method = GetMethod(context.Request.Method);

            return requestMessage;
        }

        private void CopyFromOriginalRequestContentAndHeaders(HttpContext context, HttpRequestMessage requestMessage)
        {
            var requestMethod = context.Request.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
              !HttpMethods.IsHead(requestMethod) &&
              !HttpMethods.IsDelete(requestMethod) &&
              !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in context.Request.Headers)
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            context.Response.Headers.Remove("transfer-encoding");
        }

        private static HttpMethod GetMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }
    }
}