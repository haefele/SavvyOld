using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Savvy.Services.Dropbox
{
    public class RemoveEtagDelegatingHandler : DelegatingHandler
    {
        public RemoveEtagDelegatingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Contains("ETag"))
                request.Headers.Remove("ETag");

            return base.SendAsync(request, cancellationToken);
        }
    }
}