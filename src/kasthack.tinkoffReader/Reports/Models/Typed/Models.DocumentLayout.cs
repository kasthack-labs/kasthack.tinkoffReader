namespace kasthack.TinkoffReader.Reports.Models.Typed
{
    using System.Collections.Generic;

    public record Report(Header Header, Sections Sections, Footer Footer);

    public record Header();

    public record Footer();

    public record Sections(
        SettledTradesSection SettedTrades,
        UnsettledTradesSection UnsettledTrades,
        OtherTradesSection OtherTrades,
        CashOperationsSection CashOperations,
        ExchangeInformationSection ExchangeInformation,
        CodeDefinitionsSection CodeDefinitions);

    public record SettledTradesSection(string Name, IReadOnlyList<SettledTradeRow> Rows)
        : NamedTableSection<SettledTradeRow>(Name, Rows);

    public record UnsettledTradesSection(string Name, IReadOnlyList<UnsettledTradeRow> Rows)
        : NamedTableSection<UnsettledTradeRow>(Name, Rows);

    public record OtherTradesSection(string Name, IReadOnlyList<OtherTradeRow> Rows)
        : NamedTableSection<OtherTradeRow>(Name, Rows);

    public record CashOperationsSection(string Name, IReadOnlyList<CashFlowSummaryRow> Summary, IReadOnlyDictionary<string, IReadOnlyList<CashFlowOperationRow>> OperationsByCurrency)
        : NamedSection(Name);

    public record CodeDefinitionsSection(string Name, IReadOnlyList<CodeDefinitionRow> Rows)
        : NamedTableSection<CodeDefinitionRow>(Name, Rows);

    public record ExchangeInformationSection(string Name, IReadOnlyList<ExchangeInformationRow> Rows)
        : NamedTableSection<ExchangeInformationRow>(Name, Rows);

    public abstract record NamedTableSection<TRow>(string Name, IReadOnlyList<TRow> Rows)
        : NamedSection(Name);

    public abstract record NamedSection(string Name);
}
