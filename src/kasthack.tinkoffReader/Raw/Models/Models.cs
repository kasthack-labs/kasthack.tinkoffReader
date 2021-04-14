namespace kasthack.TinkoffReader.Raw.Models
{
    using System.Collections.Generic;

    public interface IRowAddressable
    {
        int StartRow { get; }
    }

    public record RawReport(RawHeader Header, IReadOnlyList<RawSection> Sections, RawFooter Footer);

    public record RawSection(string Name, IReadOnlyList<RawTable> Tables, int StartRow) : IRowAddressable;

    public record RawTable(IReadOnlyList<string> Columns, IReadOnlyList<RawRow> Rows, IReadOnlyList<RawTable> ChildTables, int StartRow) : IRowAddressable;

    public record RawRow(IReadOnlyDictionary<string, string> Fields, int StartRow) : IRowAddressable;

    public record RawHeader(string BrokerInfo, string ReportDate, string Investor, string ReportRange, int StartRow) : IRowAddressable
    {
        public bool IsValid() => new[]
        {
            (this.BrokerInfo, nameof(this.BrokerInfo), "брокер"),
            (this.ReportDate, nameof(this.ReportDate), "дата расчета"),
            (this.Investor, nameof(this.Investor), "инвестор"),
            (this.ReportRange, nameof(this.ReportRange), "отчет о сделках и операциях за период"),
        }.ValidatePrefixes();
    }

    public record RawFooter(string AccountingPerson, string BrokerDivisionHead, string BrokerName, int StartRow) : IRowAddressable
    {
        public bool IsValid() => new[]
        {
            (this.BrokerName, nameof(this.BrokerName), "АО «Тинькофф Банк»"),
            (this.AccountingPerson, nameof(this.AccountingPerson), "Сотрудник банка, ответственный за ведение внутреннего учета (ФИО)"),
            (this.BrokerDivisionHead, nameof(this.BrokerDivisionHead), "Руководитель отдела брокерских услуг"),
        }.ValidatePrefixes();
    }
}
