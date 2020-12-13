namespace Core
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface to represent logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log data in a batch.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="data">Generic enumerable list.</param>
        void Log<T>(IEnumerable<T> data);
    }
}