namespace kasthack.TinkoffReader.Typed
{
    using System;
    using System.Collections.Generic;

    public record Report(Header Header, Sections Sections, Footer Footer);

    public record Header();

    public record Sections(
        ExecutedTradesSection ExectutedTrades,
        UnexecutedTradesSection UnexectutedTrades,
        OtherTradesSection OtherTrades,
        CashOperationsSection CashOperations);

    public record Footer();

    public record ExecutedTradesSection(string Name, IReadOnlyList<ExecutedTradeRow> Rows);

    public record UnexecutedTradesSection(string Name, IReadOnlyList<UnexecutedTradeRow> Rows);

    public record OtherTradesSection(string Name, IReadOnlyList<OtherTradeRow> Rows);

    public record CashOperationsSection(string Name, IReadOnlyList<CashFlowSummaryRow> Summary, IReadOnlyDictionary<string, IReadOnlyList<CashFlowOperationRow>> OperationsByCurrency);

    public class UnexecutedTradeRow : TradeRowBase
    {
        public UnexecutedTradeRow(TradeRowBase value)
            : base(value)
        {
        }
    }

    public class OtherTradeRow : SettledTradeRowBase
    {
        public OtherTradeRow(TradeRowBase row, string settlementType)
            : base(row, settlementType)
        {
        }

        public OtherTradeRow(SettledTradeRowBase row)
            : base(row)
        {
        }
    }

    public class ExecutedTradeRow : SettledTradeRowBase
    {
        public ExecutedTradeRow(TradeRowBase row, string settlementType)
            : base(row, settlementType)
        {
        }

        public ExecutedTradeRow(SettledTradeRowBase row)
            : base(row)
        {
        }
    }

    public class SettledTradeRowBase : TradeRowBase
    {

        public SettledTradeRowBase(TradeRowBase row, string settlementType)
            : base(row)
        {
            this.SettlementType = settlementType;
        }

        public SettledTradeRowBase(SettledTradeRowBase row)
            : this(row, row.SettlementType)
        {
        }

        public string SettlementType { get; }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), this.SettlementType);
    }

    public class TradeRowBase : PartialTradeRowBase
    {

        public decimal Price { get; }

        public string PriceCurrency { get; }

        public long Quantity { get; }

        public decimal AmountExceptACY { get; }

        public decimal ACY { get; }

        public decimal TradeAmount { get; }

        public decimal ExchangeFee { get; }

        public string? ExchangeFeeCurrency { get; }

        public decimal ClearingFee { get; }

        public string? ClearingFeeCurrency { get; }

        public TradeRowBase(PartialTradeRowBase partialTradeRowBase, decimal price, string priceCurrency, long quantity, decimal amountExceptACY, decimal aCY, decimal tradeAmount, decimal exchangeFee, string? exchangeFeeCurrency, decimal clearingFee, string? clearingFeeCurrency)
            : base(partialTradeRowBase)
                =>
                    (this.Price, this.PriceCurrency, this.Quantity, this.AmountExceptACY, this.ACY, this.TradeAmount, this.ExchangeFee, this.ExchangeFeeCurrency, this.ClearingFee, this.ClearingFeeCurrency)
                        =
                    (price, priceCurrency, quantity, amountExceptACY, aCY, tradeAmount, exchangeFee, exchangeFeeCurrency, clearingFee, clearingFeeCurrency);

        public TradeRowBase(TradeRowBase value)
            : this(value, value.Price, value.PriceCurrency, value.Quantity, value.AmountExceptACY, value.ACY, value.TradeAmount, value.ExchangeFee, value.ExchangeFeeCurrency, value.ClearingFee, value.ClearingFeeCurrency)
        {
        }

        public override int GetHashCode() => HashCode.Combine(
            base.GetHashCode(),
            HashCode.Combine(this.Price, this.PriceCurrency, this.Quantity, this.AmountExceptACY, this.ACY, this.TradeAmount, this.ExchangeFee),
            HashCode.Combine(this.ExchangeFeeCurrency, this.ClearingFee, this.ClearingFeeCurrency));
    }

    public class PartialTradeRowBase
    {
        public long TradeId { get; }

        public long? OrderId { get; }

        public DateTimeOffset TradeDate { get; }

        public TimeSpan TradeTime { get; }

        public string Exchange { get; }

        public string Operation { get; }

        public string InstrumentName { get; }

        public string InstrumentCode { get; }

        public string SettlementCurrency { get; }

        public decimal BrokersFee { get; }

        public string? BrokersFeeCurrency { get; }

        public decimal? RepoInterestRate { get; }

        public string CentralCounterparty { get; }

        public DateTimeOffset SettlementDate { get; }

        public DateTimeOffset? DeliveryDate { get; }

        public string BrokerStaus { get; }

        public string? ContractType { get; }

        public string? ContractId { get; }

        public DateTimeOffset? ContractDate { get; }

        public PartialTradeRowBase(long tradeId, long? orderId, DateTimeOffset tradeDate, TimeSpan tradeTime, string exchange, string operation, string instrumentName,
            string instrumentCode, string settlementCurrency, decimal brokersFee, string? brokersFeeCurrency, decimal? repoInterestRate, string centralCounterparty,
            DateTimeOffset settlementDate, DateTimeOffset? deliveryDate, string brokerStaus, string? contractType, string? contractId, DateTimeOffset? contractDate)
                =>
                    (
                        this.TradeId, this.OrderId, this.TradeDate, this.TradeTime, this.Exchange, this.Operation, this.InstrumentName, this.InstrumentCode,
                        this.SettlementCurrency, this.BrokersFee, this.BrokersFeeCurrency, this.RepoInterestRate, this.CentralCounterparty, this.SettlementDate,
                        this.DeliveryDate, this.BrokerStaus, this.ContractType, this.ContractId, this.ContractDate
                    )
                    =
                    (
                        tradeId, orderId, tradeDate, tradeTime, exchange, operation, instrumentName, instrumentCode, settlementCurrency, brokersFee,
                        brokersFeeCurrency, repoInterestRate, centralCounterparty, settlementDate, deliveryDate, brokerStaus, contractType, contractId, contractDate
                    );

        public PartialTradeRowBase(PartialTradeRowBase value)
            : this(value.TradeId, value.OrderId, value.TradeDate, value.TradeTime, value.Exchange, value.Operation, value.InstrumentName, value.InstrumentCode,
                  value.SettlementCurrency, value.BrokersFee, value.BrokersFeeCurrency, value.RepoInterestRate, value.CentralCounterparty, value.SettlementDate,
                  value.DeliveryDate, value.BrokerStaus, value.ContractType, value.ContractId, value.ContractDate)
        {
        }

        public override int GetHashCode() => HashCode.Combine(
            HashCode.Combine(this.TradeId, this.OrderId, this.TradeDate, this.TradeTime, this.Exchange, this.Operation, this.InstrumentName),
            HashCode.Combine(this.InstrumentCode, this.SettlementCurrency, this.BrokersFee, this.BrokersFeeCurrency, this.RepoInterestRate, this.CentralCounterparty),
            HashCode.Combine(this.SettlementDate, this.DeliveryDate, this.BrokerStaus, this.ContractType, this.ContractId, this.ContractDate));
    }


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
        TimeSpan? Time,
        DateTimeOffset? ExecutionDate,
        string Operation,
        decimal Credit,
        decimal Debit,
        string Comment);
}
