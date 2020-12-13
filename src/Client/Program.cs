namespace Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Common;
    using Common.Models;
    using Newtonsoft.Json;
    using Polly;

    /// <summary>
    /// In a high-throughput scenario, processing many requests at once, a fixed-progression wait - and - retry
    /// strategy can have disadvantages.Sudden issues affecting performance, combined with a fixed-progression
    /// wait - and - retry, can lead to subsequent retries being highly correlated.For example, if there are 50
    /// concurrent failures, and all 50 requests enter a wait - and - retry for 10ms, then all 50 requests will hit
    /// the service again in 10ms; potentially overwhelming the service again.One way to address this is to add
    /// some randomness to the wait delay. This will cause each request to vary slightly on retry, which decorrelates
    /// the retries from each other.
    /// </summary>
    public class Program
    {
        private const string CorrelationIdKey = "correlationid";
        private static readonly ServiceClient ServiceClient = new ServiceClient();
        private static readonly CsvLogger Logger = new CsvLogger($@"{Guid.NewGuid()}.csv");
        private static readonly ConcurrentBag<LogData> Data = new ConcurrentBag<LogData>();

        private static async Task Main(string[] args)
        {
            var (option, numberOfRequest) = CreateRetryOption();

            await MeasureRetries(option, numberOfRequest, ProcessRetry).ConfigureAwait(false);
        }

        private static async Task MeasureRetries(string option, int numberOfRequest, Func<string, int, Task> processRetry)
        {
            var watch = new Stopwatch();
            watch.Start();
            Console.WriteLine($"Start time - {DateTime.UtcNow}");
            Console.WriteLine($"Number of request - {numberOfRequest}");

            await processRetry(option, numberOfRequest).ConfigureAwait(false);

            watch.Stop();
            Console.WriteLine($"End time - {DateTime.UtcNow}");
            Console.WriteLine($"Total Time Taken - {watch.ElapsedMilliseconds / 1000}s");
        }

        private static (string, int) CreateRetryOption()
        {
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("Please select types of retries");
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("1. Constant Backoff (e.g. 1s, 1s, 1s)");
            Console.WriteLine("2. Linear Backoff (e.g. 1s, 2s, 3s)");
            Console.WriteLine("3. Exponential Backoff (e.g. 1s, 2s, 4s, 8s)");
            Console.WriteLine("4. Simple Exponential Jitter Backoff (e.g. 1.123s, 2.456s, 4.768s, 8.125s)");
            Console.WriteLine("5. RetryAfter Backoff (e.g. 5s, 5s, 5s)");
            Console.WriteLine("6. Aws Decorrelated Jitter Backoff (e.g. 1.123s, 2.456s, 2.768s, 4.125s)");
            Console.WriteLine("7. Decorrelated Jitter Backoff V2 (e.g. 1.555s, 2.223s, 2.123s, 3.233s)");
            Console.WriteLine();
            Console.Write($"Your option ([Ctrl + C] to exit):");
            var option = Console.ReadLine();

            Console.Write($"Number of requests to send:");
            var numberOfRequest = int.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            Console.WriteLine();

            return (option, numberOfRequest);
        }

        private static Task<HttpResponseMessage> CreateRequest(string option, string rawSettings)
        {
            const string DefaultMessage = "The retry option is not supported";

            var requestId = Guid.NewGuid().ToString();
            var context = new Context
            {
                { CorrelationIdKey, requestId },
            };

            // Tokenize id in settings for correlation.
            rawSettings = rawSettings.Replace("{{ id }}", requestId, StringComparison.InvariantCulture);
            var settings = JsonConvert.DeserializeObject<AppSettings>(rawSettings);
            var request = settings.Request;
            var retry = settings.Retry;

            return option switch
            {
                "1" => PollyRetryHandler.ConstantBackoff(retry, context, (context) => ServiceClient.SendAsync(request), HandleError, HandleRetry),
                "2" => PollyRetryHandler.LinearBackoff(retry, context, (context) => ServiceClient.SendAsync(request), HandleError, HandleRetry),
                "3" => PollyRetryHandler.ExponentialBackoff(retry, context, (context) => ServiceClient.SendAsync(request), HandleError, HandleRetry),
                "4" => PollyRetryHandler.SimpleExponentialJitterBackoff(retry, context, (context) => ServiceClient.SendAsync(request), HandleError, HandleRetry),
                "5" => PollyRetryHandler.RetryAfterBackOff(retry, context, (context) => ServiceClient.SendAsync(request), HandleError, HandleWait, HandleRetry),
                "6" => PollyRetryHandler.AwsDecorrelatedJitterBackoff(retry, context, (context) => ServiceClient.SendAsync(request), HandleError, HandleRetry),
                "7" => PollyRetryHandler.DecorrelatedJitterBackoffV2(retry, context, (context) => ServiceClient.SendAsync(request), HandleError, HandleRetry),
                _ => throw new NotSupportedException(DefaultMessage),
            };
        }

        private static async Task ProcessRetry(string option, int numberOfRequest)
        {
            var rawSettings = GetRawAppSettings();
            var tasks = new List<Task<HttpResponseMessage>>();
            for (int i = 1; i <= numberOfRequest; i++)
            {
                tasks.Add(CreateRequest(option, rawSettings));
            }

            await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);

            Logger.Log(Data);
        }

        private static bool HandleError(HttpResponseMessage response)
        {
            return (int)response.StatusCode is 408 or 429 or >= 500;
        }

        private static async Task HandleRetry(DelegateResult<HttpResponseMessage> response, TimeSpan timeSpan, int retryCount, Context context)
        {
            HandleRetry(response, timeSpan, context);

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private static void HandleRetry(DelegateResult<HttpResponseMessage> response, TimeSpan timeSpan, Context context)
        {
            var requestId = context.GetValueOrDefault(CorrelationIdKey);
            Data.Add(new LogData
            {
                Request = requestId.ToString(),
                Duration = timeSpan.TotalSeconds,
            });

            Console.WriteLine($"Request {requestId} will retry in {timeSpan.TotalSeconds}s");
        }

        private static TimeSpan HandleWait(int retryCount, DelegateResult<HttpResponseMessage> response, Context context)
        {
            const string retryAfterHeaderKey = "Retry-After";
            var result = response.Result;
            if (result.StatusCode == HttpStatusCode.TooManyRequests && result.Headers.Contains(retryAfterHeaderKey))
            {
                var retryAfter = TimeSpan.Parse(result.Headers.GetValues(retryAfterHeaderKey).FirstOrDefault(), CultureInfo.InvariantCulture);

                return retryAfter;
            }

            return TimeSpan.Zero;
        }

        private static string GetRawAppSettings()
        {
            var appSettingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            return File.ReadAllText(appSettingPath);
        }
    }
}