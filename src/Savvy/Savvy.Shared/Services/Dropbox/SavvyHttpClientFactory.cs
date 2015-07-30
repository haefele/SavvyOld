using System;
using System.Net;
using System.Net.Http;
using DropboxRestAPI.Http;
using DropboxRestAPI.Utils;

namespace Savvy.Services.Dropbox
{
    public class SavvyHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateHttpClient(HttpClientOptions options)
        {
            HttpMessageHandler httpMessageHandler = new HttpClientHandler
            {
                AllowAutoRedirect = options.AllowAutoRedirect
            };
            if (((HttpClientHandler)httpMessageHandler).SupportsAutomaticDecompression)
                ((HttpClientHandler)httpMessageHandler).AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            if (options.AddTokenToRequests)
                httpMessageHandler = new AccessTokenAuthenticator(options.TokenRetriever, httpMessageHandler);

            TimeSpanSemaphore readTimeSpanSemaphore = null;
            TimeSpanSemaphore writeTimeSpanSemaphore = null;

            if (options.ReadRequestsPerSecond.HasValue)
                readTimeSpanSemaphore = new TimeSpanSemaphore(1, TimeSpan.FromSeconds(1.0 / options.ReadRequestsPerSecond.Value));

            if (options.WriteRequestsPerSecond.HasValue)
                writeTimeSpanSemaphore = new TimeSpanSemaphore(1, TimeSpan.FromSeconds(1.0 / options.WriteRequestsPerSecond.Value));

            if (readTimeSpanSemaphore != null || writeTimeSpanSemaphore != null)
                httpMessageHandler = new ThrottlingMessageHandler(readTimeSpanSemaphore, writeTimeSpanSemaphore, httpMessageHandler);

            httpMessageHandler = new RemoveEtagDelegatingHandler(httpMessageHandler);

            return new HttpClient(httpMessageHandler);
        }
    }
}
