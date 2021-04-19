namespace kasthack.TinkoffReader.Reports.Models.Typed
{
    using System;

    public class UnsettledTradeRow : TradeRowBase
    {
        public UnsettledTradeRow() { }
        public UnsettledTradeRow(TradeRowBase value)
            : base(value)
        {
        }
    }

    public class OtherTradeRow : SettledTradeRowBase
    {
        public OtherTradeRow() { }
        public OtherTradeRow(TradeRowBase row, string settlementType)
            : base(row, settlementType)
        {
        }

        public OtherTradeRow(SettledTradeRowBase row)
            : base(row)
        {
        }
    }

    public class SettledTradeRow : SettledTradeRowBase
    {
        public SettledTradeRow() { }

        public SettledTradeRow(TradeRowBase row, string settlementType)
            : base(row, settlementType)
        {
        }

        public SettledTradeRow(SettledTradeRowBase row)
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

        public SettledTradeRowBase() { }

        public SettledTradeRowBase(SettledTradeRowBase row)
            : this(row, row.SettlementType)
        {
        }

        public string SettlementType { get; init; }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), this.SettlementType);
    }

    public class TradeRowBase : PartialTradeRowBase
    {

        public decimal Price { get; init; }

        public string PriceCurrency { get; init; }

        public long Quantity { get; init; }

        public decimal AmountExceptACY { get; init; }

        public decimal ACY { get; init; }

        public decimal TradeAmount { get; init; }

        public decimal ExchangeFee { get; init; }

        public string? ExchangeFeeCurrency { get; init; }

        public decimal ClearingFee { get; init; }

        public string? ClearingFeeCurrency { get; init; }

        public TradeRowBase() { }

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
        public long TradeId { get; init; }

        public long? OrderId { get; init; }

        public DateTimeOffset TradeDate { get; init; }

        [System.Text.Json.Serialization.JsonIgnore()]
        public TimeSpan TradeTime { get; init; }

        public string Exchange { get; init; }

        public string Operation { get; init; }

        public string InstrumentName { get; init; }

        public string InstrumentCode { get; init; }

        public string SettlementCurrency { get; init; }

        public decimal BrokersFee { get; init; }

        public string? BrokersFeeCurrency { get; init; }

        public decimal? RepoInterestRate { get; }

        public string CentralCounterparty { get; init; }

        public DateTimeOffset SettlementDate { get; init; }

        public DateTimeOffset? DeliveryDate { get; init; }

        public string BrokerStaus { get; init; }

        public string? ContractType { get; init; }

        public string? ContractId { get; init; }

        public DateTimeOffset? ContractDate { get; init; }

        public PartialTradeRowBase() { }

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

}
