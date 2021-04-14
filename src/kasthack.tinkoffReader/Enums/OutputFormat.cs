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

        /// <summary>
        /// Actually usable XLSX document. 1 sheet per section, no merged cells, no bullshit.
        /// </summary>
        Xlsx,
    }
}
