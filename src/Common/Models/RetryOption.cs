namespace Common.Models
{
    /// <summary>
    /// Model to represent retry options.
    /// </summary>
    public class RetryOption
    {
        /// <summary>
        /// Gets or sets minimum delay in milliseconds.
        /// </summary>
        /// <value>
        /// Minimum delay in milliseconds.
        /// </value>
        public int MinDelayIsMs { get; set; }

        /// <summary>
        /// Gets or sets maximum delay in milliseconds.
        /// </summary>
        /// <value>
        /// Maximum delay in milliseconds.
        /// </value>
        public int MaxDelayIsMs { get; set; }

        /// <summary>
        /// Gets or sets maximum retry counts.
        /// </summary>
        /// <value>
        /// Maximum retry counts.
        /// </value>
        public int MaxRetry { get; set; }

        /// <summary>
        /// Gets or sets jitter random start.
        /// </summary>
        /// <value>
        /// Jitter random start.
        /// </value>
        public int JitterStart { get; set; }

        /// <summary>
        /// Gets or sets jitter random end.
        /// </summary>
        /// <value>
        /// Jitter random end.
        /// </value>
        public int JitterEnd { get; set; }
    }
}