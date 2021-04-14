namespace kasthack.TinkoffReader
{
    /// <summary>
    /// Reader output format.
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// Simply extract data into JSON without any modifications.
        /// </summary>
        Raw,

        /// <summary>
        /// Extract data into a typed JSON[WIP].
        /// </summary>
        Parsed,
    }
}
