namespace Server
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Primitives;

    /// <summary>
    /// Service route controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private const string RequestAllowedKey = "NumberOfRequestAllowed";
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceController"/> class.
        /// </summary>
        /// <param name="memoryCache">In memory cache for allowed request.</param>
        public ServiceController(IMemoryCache memoryCache)
        {
            this.cache = memoryCache;
            this.cache.GetOrCreate(RequestAllowedKey, entry => 1);
        }

        /// <summary>
        /// POST request route.
        /// </summary>
        /// <param name="request">POST request data.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        public Task<IActionResult> Post([FromBody] RequestOption request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return this.HandlePostRequest(request);
        }

        private async Task<IActionResult> HandlePostRequest(RequestOption request)
        {
            Console.WriteLine($"Request {request.Id} received");

            await Semaphore.WaitAsync().ConfigureAwait(false);

            // Critical code to ensure only single request goes in asynchrously.
            try
            {
                int numberOfRequestAllowed = this.cache.Get<int>(RequestAllowedKey);
                if (numberOfRequestAllowed == 0)
                {
                    var res = new ObjectResult(new Response() { Id = request.Id })
                    {
                        StatusCode = 429,
                    };

                    this.HttpContext.Response.Headers.Add("Retry-After", new StringValues(TimeSpan.FromSeconds(5).ToString()));
                    Console.WriteLine($"Request {request.Id} rejected with 429");

                    return res;
                }
                else
                {
                    this.cache.Set(RequestAllowedKey, numberOfRequestAllowed - 1);
                }
            }
            finally
            {
                Semaphore.Release();
            }

            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            this.cache.Set(RequestAllowedKey, this.cache.Get<int>(RequestAllowedKey) + 1);

            Console.WriteLine($"Request {request.Id} completed with 200");

            return new OkObjectResult(new Response() { Id = request.Id });
        }
    }
}