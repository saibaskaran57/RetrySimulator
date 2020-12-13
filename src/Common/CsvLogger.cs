namespace Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using Core;
    using CsvHelper;

    /// <summary>
    /// A class to write logs to csv.
    /// </summary>
    public class CsvLogger : ILogger
    {
        private readonly string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvLogger"/> class.
        /// </summary>
        /// <param name="fileName">CSV file name.</param>
        public CsvLogger(string fileName)
        {
            this.fileName = fileName;
        }

        /// <inheritdoc/>
        public void Log<T>(IEnumerable<T> data)
        {
            var path = this.CreateDirectoryIfNotExist();
            using (var writer = new StreamWriter($"{path}{this.fileName}"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data);
            }
        }

        private string CreateDirectoryIfNotExist()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var baseDirectory = Path.GetDirectoryName(path);
            var directory = Directory.CreateDirectory($"{baseDirectory}/Result/");

            return directory.FullName;
        }
    }
}