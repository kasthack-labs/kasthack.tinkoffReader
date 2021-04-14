namespace kasthack.TinkoffReader
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    using kasthack.TinkoffReader.Raw;
    using kasthack.TinkoffReader.Raw.Models;
    using kasthack.TinkoffReader.Typed;
    using kasthack.TinkoffReader.Xlsx;

    using OfficeOpenXml;

    public static class Program
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// Reads tinkoff XLSX broker report and converts it to a machine-readble json file or a usable XLSX.
        /// Check out https://github.com/kasthack-labs/kasthack.tinkoffReader for more info and updates.
        /// </summary>
        /// <param name="inputPath">Input file path.</param>
        /// <param name="outputPath">Output file path.</param>
        /// <param name="inputFormat">Input format.</param>
        /// <param name="outputFormat">Output format.</param>
        public static async Task Main(FileInfo inputPath, FileInfo outputPath, InputFormat inputFormat = InputFormat.TinkoffXlsx, OutputFormat outputFormat = OutputFormat.Raw)
        {
            var rawReport = inputFormat switch
            {
                InputFormat.TinkoffXlsx => ReadTinkoffXlsx(inputPath),
                InputFormat.RawJson => await ReadRawJson(inputPath).ConfigureAwait(false),
                _ => throw new Exception($"Invalid input format: {inputFormat}"),
            };

            switch (outputFormat)
            {
                case OutputFormat.Raw:
                    {
                        var json = JsonSerializer.Serialize(rawReport, JsonOptions);
                        File.WriteAllText(outputPath.FullName, json);
                    }

                    break;
                case OutputFormat.Parsed:
                    {
                        var report = ReportParser.ParseReport(rawReport);
                        var json = JsonSerializer.Serialize(report, JsonOptions);
                        File.WriteAllText(outputPath.FullName, json);
                    }

                    break;
                case OutputFormat.Xlsx:
                    {
                        XlsxReportGenerator.GenerateXlsxReport(rawReport, outputPath);
                    }
                    break;
                default:
                    throw new Exception($"Invalid output format: {outputFormat}");
            }

            Console.WriteLine($"Parsed the report and saved it to {outputPath.FullName}");

            Console.WriteLine();
        }

        private static RawReport ReadTinkoffXlsx(FileInfo inputPath)
        {
            using var package = new ExcelPackage(inputPath);
            return new RawReportParser().ParseRawReport(package);
        }

        private static async Task<RawReport> ReadRawJson(FileInfo inputPath)
        {
            using var file = File.OpenRead(inputPath.FullName);
            return await JsonSerializer.DeserializeAsync<RawReport>(file, JsonOptions).ConfigureAwait(false);
        }
    }
}
