namespace Common.Models
{
    /// <summary>
    /// Model to log data to logger.
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        /// <value>
        /// The request id.
        /// </value>
        public string Request { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public double Duration { get; set; }
    }
}