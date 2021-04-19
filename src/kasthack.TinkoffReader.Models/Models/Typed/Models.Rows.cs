namespace kasthack.TinkoffReader.Reports.Models.Typed
{
    using System;

    public record CodeDefinitionRow(string Code, string Definition);

    public record ExchangeInformationRow(string TradeMode, string Description);

    public record CashFlowSummaryRow(
        string Currency,
        decimal StartingBalance,
        decimal EndingBalance,
        decimal EndingBalanceIncludingUnsettled,
        decimal DebtToBroker,
        decimal UncoveredBalance,
        decimal DebtToDepository);

    public record CashFlowOperationRow(
        DateTimeOffset? Date,
        /*
        [System.Text.Json.Serialization.JsonIgnore()]
        TimeSpan? Time,*/
        DateTimeOffset? SettleDate,
        string Operation,
        decimal Credit,
        decimal Debit,
        string Comment);
}
