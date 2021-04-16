namespace kasthack.TinkoffReader.Reports.Converters.Xlsx
{
    using System;
    using System.IO;
    using System.Linq;

    using kasthack.TinkoffReader.Reports.Models.Raw;

    using OfficeOpenXml;

    public static class Raw
    {

        public static void Transform(RawReport rawReport, FileInfo outputPath)
        {
            var package = new ExcelPackage();
            GenerateTitleSheet(rawReport, package);
            GenerateSectionSheets(rawReport, package);

            package.SaveAs(outputPath);
        }

        private static void GenerateSectionSheets(RawReport rawReport, ExcelPackage package)
        {
            foreach (var section in rawReport.Sections)
            {
                var sectionSheet = package.Workbook.Worksheets.Add(section.Name);

                var xOffset = 1;
                var yOffset = 1;
                foreach (var table in section.Tables)
                {
                    xOffset = WriteParentTable(sectionSheet, xOffset, yOffset, table);
                }

                sectionSheet.Cells[sectionSheet.Dimension.Address].AutoFitColumns();
            }
        }

        private static int WriteParentTable(ExcelWorksheet sectionSheet, int xOffset, int yOffset, RawTable table)
        {
            var hasRows = table.Rows.Any();
            var hasTables = table.ChildTables.Any();

            if (hasRows && hasTables)
            {
                throw new NotImplementedException($"Formatting tables with both rows and subtables is not supported");
            }

            FormatTable(sectionSheet, xOffset, yOffset, table);

            if (hasTables)
            {
                foreach (var subtable in table.ChildTables)
                {
                    FormatTable(sectionSheet, xOffset, yOffset + 1, subtable);
                    xOffset += subtable.Columns.Count + 2;
                }
            }

            if (!hasTables)
            {
                xOffset += table.Columns.Count + 2;
            }

            return xOffset;
        }

        private static void GenerateTitleSheet(RawReport rawReport, ExcelPackage package)
        {
            var titleSheet = package.Workbook.Worksheets.Add("Intro");
            titleSheet.Cells["A1"].Value = "Compiled report by kasthack.tinkoffReader";
            titleSheet.Cells["A1"].Style.Font.Size *= 2;
            titleSheet.Row(1).Height *= 2;
            var rows = new[]
            {
                    "Метаданные:",

                    rawReport.Header.BrokerInfo,
                    rawReport.Header.Investor,
                    rawReport.Header.ReportDate,
                    rawReport.Header.ReportRange,
                    rawReport.Footer.BrokerName,
                    rawReport.Footer.BrokerDivisionHead,
                    rawReport.Footer.AccountingPerson,
            };

            for (var i = 0; i < rows.Length; i++)
            {
                titleSheet.Cells[3 + i, 1].Value = rows[i];
            }

            titleSheet.Cells[titleSheet.Dimension.Address].AutoFitColumns();
        }

        private static void FormatTable(ExcelWorksheet sectionSheet, int xOffset, int yOffset, RawTable table)
        {
            for (var columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
            {
                sectionSheet.Cells[yOffset, xOffset + columnIndex].Value = table.Columns[columnIndex];
            }

            yOffset++;

            for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                {
                    if (!table.Rows[rowIndex].Fields.TryGetValue(table.Columns[columnIndex], out var fieldValue))
                    {
                        Console.WriteLine($"Warning: missing field {table.Columns[columnIndex]} at row {table.Rows[rowIndex].StartRow}");
                        fieldValue = null;
                    }

                    sectionSheet.Cells[yOffset + rowIndex, xOffset + columnIndex].Value = fieldValue;
                }
            }
        }

    }
}
