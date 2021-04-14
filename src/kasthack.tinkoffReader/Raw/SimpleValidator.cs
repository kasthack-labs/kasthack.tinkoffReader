namespace kasthack.tinkoffReader.Raw
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class SimpleValidator
    {
        public static bool ValidatePrefixes(this IEnumerable<(string value, string name, string expectedPrefix)> records) => records.Aggregate(true, (result, record) =>
        {
            var currentResult = record.value.StartsWith(record.expectedPrefix, StringComparison.OrdinalIgnoreCase);
            if (!currentResult)
            {
                Console.WriteLine($"Validation error: field {record.name} has value '{record.value}' that doesn't start with the expected prefix '{record.expectedPrefix}'");
            }
            return result && currentResult;
        });
    }
}
