namespace Common.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Model to represent request options.
    /// </summary>
    public class RequestOption
    {
        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        /// <value>
        /// The request id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the request method.
        /// </summary>
        /// <value>
        /// The request method.
        /// </value>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the request uri.
        /// </summary>
        /// <value>
        /// The request uri.
        /// </value>
        public Uri RequestUri { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        /// <value>
        /// The request headers.
        /// </value>
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the request content headers.
        /// </summary>
        /// <value>
        /// The request headers.
        /// </value>
        public IDictionary<string, string> ContentHeaders { get; set; }

        /// <summary>
        /// Gets or sets the request body.
        /// </summary>
        /// <value>
        /// The request headers.
        /// </value>
        public string Body { get; set; }
    }
}