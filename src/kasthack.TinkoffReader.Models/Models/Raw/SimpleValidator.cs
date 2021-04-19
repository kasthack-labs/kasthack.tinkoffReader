namespace kasthack.TinkoffReader.Reports.Models.Raw
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class SimpleValidator
    {
        public static bool ValidatePrefixes(this IEnumerable<(string Value, string Name, string ExpectedPrefix)> records) => records.Aggregate(true, (result, record) =>
        {
            var currentResult = record.Value.StartsWith(record.ExpectedPrefix, StringComparison.OrdinalIgnoreCase);
            if (!currentResult)
            {
                Console.WriteLine($"Validation error: field {record.Name} has value '{record.Value}' that doesn't start with the expected prefix '{record.ExpectedPrefix}'");
            }

            return result && currentResult;
        });
    }
}
