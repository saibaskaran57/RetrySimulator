namespace Common
{
    using System;

    /// <summary>
    /// An exception to orchestrate 429 Too Many Requests.
    /// </summary>
    public class TooManyRequestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyRequestException"/> class.
        /// </summary>
        public TooManyRequestException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyRequestException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public TooManyRequestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyRequestException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="retryAfter">Retry after header.</param>
        public TooManyRequestException(string message, TimeSpan retryAfter)
            : base(message)
        {
            this.RetryAfter = retryAfter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyRequestException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public TooManyRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Gets or sets retry after header.
        /// </summary>
        /// <value>
        /// Retry after header.
        /// </value>
        public TimeSpan RetryAfter { get; set; }
    }
}