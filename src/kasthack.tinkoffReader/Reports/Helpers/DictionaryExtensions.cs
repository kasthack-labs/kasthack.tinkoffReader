namespace kasthack.TinkoffReader.Reports.Helpers
{
    using System;
    using System.Collections.Generic;

    internal static class DictionaryExtensions
    {
        public static TValue GetValueAtAnyKey<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, params TKey[] keys)
        {
            if (keys.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(keys), "You must pass at least one key");
            }

            foreach (var key in keys)
            {
                if (dictionary.TryGetValue(key, out var value))
                {
                    return value;
                }
            }

            throw new KeyNotFoundException($"None of the keys passed were present in the dictionary");
        }
    }
}
