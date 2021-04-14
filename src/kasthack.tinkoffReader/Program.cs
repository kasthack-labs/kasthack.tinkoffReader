namespace kasthack.tinkoffReader
{
    using System;
    using System.IO;
    using System.Text.Json;

    using kasthack.tinkoffReader.Raw;

    using OfficeOpenXml;

    class Program
    {
        /// <summary>
        /// Reads tinkoff XLSX broker report and converts it to a machine-readble json file
        /// </summary>
        /// <param name="inputPath">Input file path</param>
        /// <param name="outputPath">Output json path</param>
        /// <param name="format">Output format. Currently, only raw is supported</param>
        static void Main(FileInfo inputPath, FileInfo outputPath, OutputFormat format = OutputFormat.Raw)
        {
            using var package = new ExcelPackage(inputPath);
            var rawReport = new RawReportParser().ParseRawReport(package);
            var json = System.Text.Json.JsonSerializer.Serialize(rawReport, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            File.WriteAllText(outputPath.FullName, json);

            Console.WriteLine($"Parsed the report and saved it to {outputPath.FullName}");

            Console.WriteLine();
        }
    }
}
