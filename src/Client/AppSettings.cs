namespace Client
{
    using Common.Models;

    /// <summary>
    /// Model to represent app configuration.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets retry settings.
        /// </summary>
        /// <value>
        /// <code>
        /// "retry": {
        ///     "minDelayIsMs": 1000,
        ///     "maxDelayIsMs": 5000,
        ///     "maxRetry": 50,
        ///     "jitterStart": 1,
        ///     "jitterEnd": 1000
        /// },
        /// </code>
        /// </value>
        public RetryOption Retry { get; set; }

        /// <summary>
        /// Gets or sets request settings.
        /// </summary>
        /// <value>
        /// <code>
        /// "request": {
        ///     "verb": "POST",
        ///    "endpoint": "https://localhost:5001/api/service",
        ///    "headers": {
        ///     "Content-Type": "application/json"
        ///    },
        ///    "body": "{\"id\":\"{{ id }}\"}"
        /// }
        /// </code>
        /// </value>
        public RequestOption Request { get; set; }
    }
}