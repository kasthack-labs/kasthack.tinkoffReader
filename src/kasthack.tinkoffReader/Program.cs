namespace kasthack.TinkoffReader
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    using kasthack.TinkoffReader.Reports.Models.Raw;

    using OfficeOpenXml;

    public static class Program
    {
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
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

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
                        var json = JsonSerializer.Serialize(rawReport, jsonOptions);
                        File.WriteAllText(outputPath.FullName, json);
                    }

                    break;
                case OutputFormat.Parsed:
                    {
                        var report = kasthack.TinkoffReader.Reports.Converters.Typed.Raw.Transform(rawReport);
                        var json = JsonSerializer.Serialize(report, jsonOptions);
                        File.WriteAllText(outputPath.FullName, json);
                    }

                    break;
                case OutputFormat.Xlsx:
                    {
                        kasthack.TinkoffReader.Reports.Converters.Xlsx.Raw.Transform(rawReport, outputPath);
                    }
                    break;
                default:
                    throw new Exception($"Invalid output format: {outputFormat}");
            }

            Console.WriteLine($"Parsed the report and saved it to {outputPath.FullName}");

            Console.WriteLine();

            // ----------------------------------

            RawReport ReadTinkoffXlsx(FileInfo inputPath)
            {
                using var package = new ExcelPackage(inputPath);
                return new kasthack.TinkoffReader.Reports.Converters.Raw.TinkoffXlsx().Transform(package);
            }

            async Task<RawReport> ReadRawJson(FileInfo inputPath)
            {
                using var file = File.OpenRead(inputPath.FullName);
                return await JsonSerializer.DeserializeAsync<RawReport>(file, jsonOptions).ConfigureAwait(false);
            }
        }

    }
}
