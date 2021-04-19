namespace kasthack.tinkoffReader.TaxCalculator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    using kasthack.TinkoffReader.Reports.Models.Typed;

    class Program
    {
        static void Main(FileInfo[] inputPath)
        {
            var reports = inputPath
                .Select(a =>
                {
                    using (var file = a.OpenRead())
                    {
                        return JsonSerializer.DeserializeAsync<Report>(file).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                })
                .ToArray();
            var trades = reports.SelectMany(a => a.Sections.SettedTrades.Rows).ToArray();
            trades.Select(a => a.Operation).GroupBy(a => a)
        }
    }
}
