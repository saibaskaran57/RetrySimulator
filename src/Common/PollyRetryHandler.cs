namespace Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Models;
    using Polly;
    using Polly.Contrib.WaitAndRetry;

    /// <summary>
    /// Polly documentation for Wait and retry
    /// </summary>
    /// <remarks>
    ///  https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry#wait-and-retry-with-exponential-back-off
    /// </remarks>
    public static class PollyRetryHandler
    {
        /// <summary>
        /// Create an retry after backoff retry delay which returned by server.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <param name="option">The retry option.</param>
        /// <param name="context">Request context.</param>
        /// <param name="action">Action to execute.</param>
        /// <param name="predicate">Handle result predicate.</param>
        /// <param name="sleepDurationProvider">Sleep duration provider.</param>
        /// <param name="onRetryAsync">Handle custom on retries.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task<T> RetryAfterBackOff<T>(
            RetryOption option,
            Context context,
            Func<Context, Task<T>> action,
            Func<T, bool> predicate,
            Func<int, DelegateResult<T>, Context, TimeSpan> sleepDurationProvider,
            Func<DelegateResult<T>, TimeSpan, int, Context, Task> onRetryAsync)
        {
            if (option is null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            return Policy
                .HandleResult(predicate)
                .WaitAndRetryAsync(
                        retryCount: option.MaxRetry,
                        sleepDurationProvider,
                        onRetryAsync)
                .ExecuteAsync(async (arg) => await action(arg).ConfigureAwait(false), context);
        }

        /// <summary>
        /// Create an constant backoff retry delay of 1, 1, 1, 1s.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <param name="option">The retry option.</param>
        /// <param name="context">Request context.</param>
        /// <param name="action">Action to execute.</param>
        /// <param name="predicate">Handle result predicate.</param>
        /// <param name="onRetry">Handle custom on retries.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task<T> ConstantBackoff<T>(
            RetryOption option,
            Context context,
            Func<Context, Task<T>> action,
            Func<T, bool> predicate,
            Action<DelegateResult<T>, TimeSpan, Context> onRetry)
        {
            if (option is null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            var delay = Backoff.ConstantBackoff(TimeSpan.FromMilliseconds(option.MinDelayIsMs), retryCount: option.MaxRetry);

            return HandleRetry(context, action, predicate, onRetry, delay);
        }

        /// <summary>
        /// Create a linear retry delay of 1, 2, 3, 4s.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <param name="option">The retry option.</param>
        /// <param name="context">Request context.</param>
        /// <param name="action">Action to execute.</param>
        /// <param name="predicate">Handle result predicate.</param>
        /// <param name="onRetry">Handle custom on retries.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task<T> LinearBackoff<T>(
            RetryOption option,
            Context context,
            Func<Context, Task<T>> action,
            Func<T, bool> predicate,
            Action<DelegateResult<T>, TimeSpan, Context> onRetry)
        {
            if (option is null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            var delay = Backoff.LinearBackoff(TimeSpan.FromMilliseconds(option.MinDelayIsMs), retryCount: option.MaxRetry);

            return HandleRetry(context, action, predicate, onRetry, delay);
        }

        /// <summary>
        /// This will create an exponentially increasing retry delay of 100, 200, 400, 800, 1600ms.
        /// The default exponential growth factor is 2.0. However, can can provide our own.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <param name="option">The retry option.</param>
        /// <param name="context">Request context.</param>
        /// <param name="action">Action to execute.</param>
        /// <param name="predicate">Handle result predicate.</param>
        /// <param name="onRetry">Handle custom on retries.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Snippet:
        /// Formula = initial delay * 2 * attempt
        /// </remarks>
        public static Task<T> ExponentialBackoff<T>(
            RetryOption option,
            Context context,
            Func<Context, Task<T>> action,
            Func<T, bool> predicate,
            Action<DelegateResult<T>, TimeSpan, Context> onRetry)
        {
            if (option is null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            var delay = Backoff.ExponentialBackoff(TimeSpan.FromMilliseconds(option.MinDelayIsMs), retryCount: option.MaxRetry);

            return HandleRetry(context, action, predicate, onRetry, delay);
        }

        /// <summary>
        /// Exponentially with jitter on each retries
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <param name="option">The retry option.</param>
        /// <param name="context">Request context.</param>
        /// <param name="action">Action to execute.</param>
        /// <param name="predicate">Handle result predicate.</param>
        /// <param name="onRetry">Handle custom on retries.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// exponential back-off: 2, 4, 8 etc
        /// plus some jitter: up to 1 second
        /// </remarks>
        public static Task<T> SimpleExponentialJitterBackoff<T>(
           RetryOption option,
           Context context,
           Func<Context, Task<T>> action,
           Func<T, bool> predicate,
           Action<DelegateResult<T>, TimeSpan, Context> onRetry)
        {
            if (option is null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            var jitterer = new Random();
            return Policy
              .HandleResult(predicate)
              .WaitAndRetryAsync(
                  5,
                  retryAttempt =>
                  {
                      return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1))
                            + TimeSpan.FromMilliseconds(jitterer.Next(option.JitterStart, option.JitterEnd));
                  },
                  onRetry)
              .ExecuteAsync(async (arg) => await action(arg).ConfigureAwait(false), context);
        }

        /// <summary>
        /// This will set up a policy that will retry five times. Each retry will delay for a random
        /// amount of time between the minimum of 1s and the maximum of 5s.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <param name="option">The retry option.</param>
        /// <param name="context">Request context.</param>
        /// <param name="action">Action to execute.</param>
        /// <param name="predicate">Handle result predicate.</param>
        /// <param name="onRetry">Handle custom on retries.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Snippet:
        /// https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry/blob/master/src/Polly.Contrib.WaitAndRetry/Backoff.AwsDecorrelatedJitter.cs
        /// Formula - random(minDelay, min(maxDelay, minDelay * 3)))
        /// </remarks>
        public static Task<T> AwsDecorrelatedJitterBackoff<T>(
            RetryOption option,
            Context context,
            Func<Context, Task<T>> action,
            Func<T, bool> predicate,
            Action<DelegateResult<T>, TimeSpan, Context> onRetry)
        {
            if (option is null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            var delay = Backoff.AwsDecorrelatedJitterBackoff(
                        minDelay: TimeSpan.FromMilliseconds(option.MinDelayIsMs),
                        maxDelay: TimeSpan.FromMilliseconds(option.MaxDelayIsMs),
                        retryCount: option.MaxRetry);

            return HandleRetry(context, action, predicate, onRetry, delay);
        }

        /// <summary>
        /// First, new Random() is not quite random, when it comes to reducing concurrency.
        /// You can get multiple instances with the same seed, if many retries are issued simultaneously.
        /// https://github.com/App-vNext/Polly/issues/530.
        /// </summary>
        /// <typeparam name="T">The type of response.</typeparam>
        /// <param name="option">The retry option.</param>
        /// <param name="context">Request context.</param>
        /// <param name="action">Action to execute.</param>
        /// <param name="predicate">Handle result predicate.</param>
        /// <param name="onRetry">Handle custom on retries.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task<T> DecorrelatedJitterBackoffV2<T>(
            RetryOption option,
            Context context,
            Func<Context, Task<T>> action,
            Func<T, bool> predicate,
            Action<DelegateResult<T>, TimeSpan, Context> onRetry)
        {
            if (option is null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            var maxDelay = TimeSpan.FromSeconds(option.MaxDelayIsMs);
            var delay = Backoff
                .DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromMilliseconds(option.MinDelayIsMs), retryCount: option.MaxRetry)
                .Select(s => TimeSpan.FromTicks(Math.Min(s.Ticks, maxDelay.Ticks)));

            return HandleRetry(context, action, predicate, onRetry, delay);
        }

        private static Task<T> HandleRetry<T>(
            Context context,
            Func<Context, Task<T>> action,
            Func<T, bool> predicate,
            Action<DelegateResult<T>, TimeSpan, Context> onRetry,
            IEnumerable<TimeSpan> delay)
        {
            return Policy
               .HandleResult(predicate)
               .WaitAndRetryAsync(delay, onRetry)
               .ExecuteAsync(action, context);
        }
    }
}