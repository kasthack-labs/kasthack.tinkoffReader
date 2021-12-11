namespace kasthack.TinkoffReader.Reports.Models.Typed
{
    using System.Collections.Generic;

    public record Report(Header Header, Sections Sections, Footer Footer);

    public record Header();

    public record Footer();

    public record Sections(
        SettledTradesSection SettedTrades,                  // 1.1 Информация о совершенных и исполненных сделках на конец отчетного периода
        UnsettledTradesSection UnsettledTrades,             // 1.2 Информация о неисполненных сделках на конец отчетного периода
        OtherTradesSection OtherTrades,                     // 1.3 Сделки за расчетный период, обязательства из которых прекращены  не в результате исполнения
                                                            // 1.4 Информация об изменении расчетных параметров сделок РЕПО
        CashOperationsSection CashOperations,               // 2. Операции с денежными средствами
        ExchangeInformationSection ExchangeInformation,     // 5. Информация о торговых площадках
        CodeDefinitionsSection CodeDefinitions);            // 6. Расшифровка дополнительных кодов используемых в отчете

    // 1.1 Информация о совершенных и исполненных сделках на конец отчетного периода
    public record SettledTradesSection(string Name, IReadOnlyList<SettledTradeRow> Rows)
        : NamedTableSection<SettledTradeRow>(Name, Rows);

    // 1.2 Информация о неисполненных сделках на конец отчетного периода
    public record UnsettledTradesSection(string Name, IReadOnlyList<UnsettledTradeRow> Rows)
        : NamedTableSection<UnsettledTradeRow>(Name, Rows);

    // 1.3 Сделки за расчетный период, обязательства из которых прекращены  не в результате исполнения
    public record OtherTradesSection(string Name, IReadOnlyList<OtherTradeRow> Rows)
        : NamedTableSection<OtherTradeRow>(Name, Rows);

    //public record 

    // 2. Операции с денежными средствами
    public record CashOperationsSection(string Name,
        IReadOnlyList<CashFlowSummaryRow> Summary,

        IReadOnlyDictionary<string, IReadOnlyList<CashFlowOperationRow>> OperationsByCurrency)
        : NamedSection(Name);

    // 6. Расшифровка дополнительных кодов используемых в отчете
    public record CodeDefinitionsSection(string Name, IReadOnlyList<CodeDefinitionRow> Rows)
        : NamedTableSection<CodeDefinitionRow>(Name, Rows);

    // 5. Информация о торговых площадках
    public record ExchangeInformationSection(string Name, IReadOnlyList<ExchangeInformationRow> Rows)
        : NamedTableSection<ExchangeInformationRow>(Name, Rows);

    public abstract record NamedTableSection<TRow>(string Name, IReadOnlyList<TRow> Rows)
        : NamedSection(Name);

    public abstract record NamedSection(string Name);
}
