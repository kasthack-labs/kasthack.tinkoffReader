namespace kasthack.TinkoffReader
{
    public enum InputFormat
    {
        /// <summary>
        /// Tinkoff original report in XLSX format.
        /// </summary>
        TinkoffXlsx,

        /// <summary>
        /// JSON generated using output format=Raw.
        /// </summary>
        RawJson,
    }
}
