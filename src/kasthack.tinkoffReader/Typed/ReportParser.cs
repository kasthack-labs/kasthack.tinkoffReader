namespace kasthack.TinkoffReader.Typed
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using kasthack.TinkoffReader.Raw.Models;

    public class ReportParser
    {
        private static readonly CultureInfo russianCulture = CultureInfo.GetCultureInfo("ru");
        private static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;

        public static Report ParseReport(RawReport report) => new Report(
            new Header(),
            new Sections(
                ParseExecutedTradesSection(report.Sections.Single(a => a.Name == "1.1 Информация о совершенных и исполненных сделках на конец отчетного периода")),
                ParseUnexecutedTradesSection(report.Sections.Single(a => a.Name == "1.2 Информация о неисполненных сделках на конец отчетного периода")),
                ParseOtherTradesSection(report.Sections.Single(a => a.Name == "1.3 Сделки за расчетный период, обязательства из которых прекращены  не в результате исполнения")),
                //todo: 1.4 repo contract changes sections
                ParseCashFlowOperationsSection(report.Sections.Single(a => a.Name == "2. Операции с денежными средствами"))
            ), new Footer());

        public static ExecutedTradesSection ParseExecutedTradesSection(RawSection section) => new ExecutedTradesSection(
            section.Name,
            section.Tables.Single().Rows.Select(a => ParseExecutedTradeRow(a.Fields)).ToArray());

        public static UnexecutedTradesSection ParseUnexecutedTradesSection(RawSection section) => new UnexecutedTradesSection(
            section.Name,
            section.Tables.Single().Rows.Select(a => ParseUnexecutedTradeRow(a.Fields)).ToArray());

        public static OtherTradesSection ParseOtherTradesSection(RawSection section) => new OtherTradesSection(
            section.Name,
            section.Tables.Single().Rows.Select(a => ParseOtherTradeRow(a.Fields)).ToArray());

        public static CashOperationsSection ParseCashFlowOperationsSection(RawSection section) => new CashOperationsSection(
            section.Name,
            section.Tables.First().Rows.Select(a => ParseCashFlowSummaryRow(a.Fields)).ToArray(),
            section
                .Tables
                .Skip(1)
                .ToDictionary(
                    a => a.Columns.Single(),
                    a => (IReadOnlyList<CashFlowOperationRow>)a.ChildTables.Single().Rows.Select(a => ParseCashFlowOperationRow(a.Fields)).ToArray()));

        public static CashFlowSummaryRow ParseCashFlowSummaryRow(IReadOnlyDictionary<string, string> rawRow) => new CashFlowSummaryRow(
            Currency: ConvertString(rawRow["Валюта"]),
            StartingBalance: ConvertDecimal(rawRow["Входящий остаток на начало периода:"]),
            EndingBalance: ConvertDecimal(rawRow["Исходящий остаток на конец периода:"]),
            EndingBalanceIncludingUnsettled: ConvertDecimal(rawRow["Плановый исходящий остаток на конец периода (с учетом неисполненных на дату"]),
            DebtToBroker: ConvertDecimal(rawRow["Задолженность клиента перед брокером:"]),
            UncoveredBalance: ConvertDecimal(rawRow["Сумма непокрытого остатка:"]),
            DebtToDepository: ConvertDecimal(rawRow["Задолженность клиента перед Депозитарием (справочно)"]));

        public static CashFlowOperationRow ParseCashFlowOperationRow(IReadOnlyDictionary<string, string> rawRow) => new CashFlowOperationRow(
            Date: ConvertDate(rawRow["Дата"]),
            Time: ConvertTime(rawRow["Время совершения"]),
            ExecutionDate: ConvertDate(rawRow["Дата исполнения"]),
            Operation: ConvertString(rawRow["Операция"]),
            Credit: ConvertDecimal(rawRow["Сумма зачисления"]),
            Debit: ConvertDecimal(rawRow["Сумма списания"]),
            Comment: rawRow["Примечание"]);

        //public static RepoContractChangesRow ParseRepoContractChangesRow(IDictionary<string, string> rawRow) => new OtherTradeRow(ParseSettledTradeRowBase(rawRow));
        public static OtherTradeRow ParseOtherTradeRow(IReadOnlyDictionary<string, string> rawRow) => new OtherTradeRow(ParseSettledTradeRowBase(rawRow));

        public static ExecutedTradeRow ParseExecutedTradeRow(IReadOnlyDictionary<string, string> rawRow) => new ExecutedTradeRow(ParseSettledTradeRowBase(rawRow));

        public static UnexecutedTradeRow ParseUnexecutedTradeRow(IReadOnlyDictionary<string, string> rawRow) => new UnexecutedTradeRow(ParseTradeRowBase(rawRow));

        private static SettledTradeRowBase ParseSettledTradeRowBase(IReadOnlyDictionary<string, string> rawRow) => new SettledTradeRowBase(
            ParseTradeRowBase(rawRow),
            settlementType: ConvertString(rawRow["Тип расчета по сделке"]));

        private static TradeRowBase ParseTradeRowBase(IReadOnlyDictionary<string, string> rawRow) => new TradeRowBase(
            ParsePartialTradeRowBase(rawRow),
            price: ConvertDecimal(rawRow["Цена за единицу"]),
            priceCurrency: ConvertString(rawRow["Валюта цены"]),
            quantity: ConvertLong(rawRow["Количество"]).Value,
            amountExceptACY: ConvertDecimal(rawRow["Сумма (без НКД)"]),
            aCY: ConvertDecimal(rawRow["НКД"]),
            tradeAmount: ConvertDecimal(rawRow["Сумма сделки"]),
            exchangeFee: ConvertDecimal(rawRow["Комиссия биржи"]),
            exchangeFeeCurrency: ConvertString(rawRow["Валюта комиссии биржи"]),
            clearingFee: ConvertDecimal(rawRow["Комиссия клир. центра"]),
            clearingFeeCurrency: ConvertString(rawRow["Валюта комиссии клир. центра"]));

        private static PartialTradeRowBase ParsePartialTradeRowBase(IReadOnlyDictionary<string, string> rawRow) => new PartialTradeRowBase(
            tradeId: ConvertLong(rawRow["Номер сделки"]).Value,
            orderId: ConvertLong(rawRow["Номер поручения"]),
            tradeDate: ConvertDate(rawRow["Дата заключения"]).Value,
            tradeTime: ConvertTime(rawRow["Время"]).Value,
            exchange: ConvertString(rawRow["Торговая площадка"]),
            operation: ConvertString(rawRow["Вид сделки"]),
            instrumentName: ConvertString(rawRow["Сокращенное наименование актива"]),
            instrumentCode: ConvertString(rawRow["Код актива"]),
            settlementCurrency: ConvertString(rawRow["Валюта расчетов"]),
            brokersFee: ConvertDecimal(rawRow["Комиссия брокера"]),
            brokersFeeCurrency: ConvertString(rawRow["Валюта комиссии"]),
            repoInterestRate: ConvertDecimal(rawRow["Ставка РЕПО(%)"]),
            centralCounterparty: ConvertString(rawRow["Контрагент"]),
            settlementDate: ConvertDate(rawRow["Дата расчетов"]).Value,
            deliveryDate: ConvertDate(rawRow["Дата поставки"]),
            brokerStaus: ConvertString(rawRow["Статус брокера"]),
            contractType: ConvertString(rawRow["Тип дог."]),
            contractId: ConvertString(rawRow["Номер дог."]),
            contractDate: ConvertDate(rawRow["Дата дог."]));

        private static long? ConvertLong(string value)
        {

            try
            {
                return string.IsNullOrWhiteSpace(value) ? null : long.Parse(value, russianCulture);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static string? ConvertString(string value) => string.IsNullOrWhiteSpace(value) ? default : value.Trim();

        private static DateTimeOffset? ConvertDate(string value) => string.IsNullOrWhiteSpace(value) ? null : DateTimeOffset.Parse(value, russianCulture);

        private static TimeSpan? ConvertTime(string value) => string.IsNullOrWhiteSpace(value) ? null : TimeSpan.Parse(value, russianCulture);

        private static decimal ConvertDecimal(string value)
        {
            try
            {
                return string.IsNullOrWhiteSpace(value)
                    ? 0
                    : decimal.TryParse(value, NumberStyles.Any, russianCulture, out var result)
                        ? result
                        : decimal.Parse(value, invariantCulture);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
