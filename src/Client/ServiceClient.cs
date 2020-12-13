namespace Client
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common.Models;

    /// <summary>
    /// A class to orchestrate REST api communication.
    /// </summary>
    public class ServiceClient
    {
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceClient"/> class.
        /// </summary>
        public ServiceClient()
        {
            this.client = new HttpClient();
        }

        /// <summary>
        /// Post data to service.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns><see cref="HttpResponseMessage"/> class.</returns>
        public Task<HttpResponseMessage> SendAsync(RequestOption request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return this.ExecuteAsync(request);
        }

        private static HttpRequestMessage CreateRequestMessage(RequestOption request)
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = request.RequestUri,
                Method = new HttpMethod(request.Method),
            };

            AddHeaders(request, requestMessage);
            AddBody(request, requestMessage);

            return requestMessage;
        }

        private static void AddBody(RequestOption request, HttpRequestMessage requestMessage)
        {
            if (!string.IsNullOrWhiteSpace(request.Body))
            {
                requestMessage.Content = new StringContent(request.Body);

                AddContentHeaders(request, requestMessage);
            }
        }

        private static void AddContentHeaders(RequestOption request, HttpRequestMessage requestMessage)
        {
            if (request.ContentHeaders != null && request.ContentHeaders.Count > 0)
            {
                var headers = request.ContentHeaders;
                foreach (var header in headers)
                {
                    requestMessage.Content.Headers.Clear();
                    requestMessage.Content.Headers.Add(header.Key, header.Value);
                }
            }
        }

        private static void AddHeaders(RequestOption request, HttpRequestMessage requestMessage)
        {
            if (request.Headers != null && request.Headers.Count > 0)
            {
                var headers = request.Headers;
                foreach (var header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }
        }

        private Task<HttpResponseMessage> ExecuteAsync(RequestOption request)
        {
            var requestMessage = CreateRequestMessage(request);

            return this.client.SendAsync(requestMessage);
        }
    }
}