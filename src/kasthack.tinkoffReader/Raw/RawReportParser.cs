namespace kasthack.tinkoffReader.Raw
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    using kasthack.tinkoffReader.Raw.Models;

    //Extracts data from a Tinkoff broker report in XLSX format
    //not threadsafe, create separate instances for parallel processing
    public class RawReportParser
    {
        private IDictionary<int, (int x1, int y1, int x2, int y2)[]> mergeLookupTable;

        public RawReport ParseRawReport(ExcelPackage package)
        {
            var sheet = package.Workbook.Worksheets[0];
            this.mergeLookupTable = this.BuildFastMergedCellLookupTable(sheet);

            var (header, footer) = this.ParseFooterAndHeader(sheet);
            var rawSections = this.ParseSections(sheet).ToArray();
            return new RawReport(header, rawSections, footer);
        }
        private IEnumerable<RawSection> ParseSections(ExcelWorksheet sheet)
        {
            var rowEnum = this.GetRowEnumerator(sheet);
            if (!rowEnum.MoveNext())
            {
                yield break;
            }

            //todo: check if it's missing rows after first+ section
            while (rowEnum.Current != default)
            {
                if (this.IsSeparatorRow(rowEnum.Current))
                {
                    if (!rowEnum.MoveNext())
                    {
                        yield break;
                    }
                    continue;
                }
                else if (this.IsSectionTitle(rowEnum.Current))
                {
                    yield return this.ParseSection(rowEnum);
                }
                else
                {
                    throw new InvalidDataException($"Unexpected row type at row {rowEnum.Current.Start.Row}");
                }
            }
        }
        private IEnumerator<ExcelRange> GetRowEnumerator(ExcelWorksheet sheet) => Enumerable
            // 7 => size of header
            // 3 => size of footer
            // 1 => offset due to 1-based indexes
            // TODO: replaces with calls to header/footer.Height
            .Range(7, sheet.Dimension.Rows - 7 - 3)
            .Select(row => sheet.Cells[row, 1])
            .GetEnumerator();

        private RawSection ParseSection(IEnumerator<ExcelRange> rowEnum)
        {
            var headerRow = rowEnum.Current;
            var sectionTitle = headerRow.Text.Trim();
            var sectionRow = headerRow.Start.Row;

            var tables = rowEnum.MoveNext() ? this.ParseSectionTables(rowEnum).ToArray() : Array.Empty<RawTable>();
            return new RawSection(sectionTitle, tables, sectionRow);
        }

        private IEnumerable<RawTable> ParseSectionTables(IEnumerator<ExcelRange> rowEnum)
        {
            while (rowEnum.Current != default)
            {
                if (this.IsSeparatorRow(rowEnum.Current))
                {
                    if (!rowEnum.MoveNext())
                    {
                        yield break;
                    }
                    continue;
                }
                else if (this.IsTableHeader(rowEnum.Current))
                {
                    yield return this.ParseTable(rowEnum);
                }
                else
                {
                    yield break;
                }
            }
        }
        private RawTable ParseTable(IEnumerator<ExcelRange> rowEnum)
        {
            var row = rowEnum.Current;
            var tableHeader = this.ParseTableHeader(row);
            var tableRow = row.Start.Row;

            var (rows, tables) = rowEnum.MoveNext() ? this.ParseSectionTableBody(rowEnum, tableHeader) : (Array.Empty<RawRow>(), Array.Empty<RawTable>());
            return new RawTable(tableHeader, rows, tables, tableRow);
        }

        private (IReadOnlyList<RawRow> rows, IReadOnlyList<RawTable> tables) ParseSectionTableBody(IEnumerator<ExcelRange> rowEnum, IReadOnlyList<string> tableHeader)
        {
            var rows = new List<RawRow>();
            var tables = new List<RawTable>();
            while (rowEnum.Current != default)
            {
                var row = rowEnum.Current;
                if (this.IsSeparatorRow(rowEnum.Current))
                {
                    if (!rowEnum.MoveNext())
                    {
                        break;
                    }
                    continue;
                }
                else if (this.IsDataRow(row))
                {
                    rows.Add(this.ParseDataRow(row, tableHeader));
                    if (!rowEnum.MoveNext())
                    {
                        break;
                    }
                    continue;
                }
                else if (this.IsTableHeader(row))
                {
                    var nextTableHeader = this.ParseTableHeader(row);
                    if (tableHeader.SequenceEqual(nextTableHeader))
                    {
                        if (!rowEnum.MoveNext())
                        {
                            break;
                        }
                        continue;
                    }
                }
                else if (this.IsSubTableHeader(row))
                {
                    tables.Add(this.ParseSubTable(rowEnum, tableHeader));
                    continue;
                }

                break;
            }
            return (rows, tables);
        }

        private RawTable ParseSubTable(IEnumerator<ExcelRange> rowEnum, IReadOnlyList<string> parentTableHeader)
        {
            var headerRow = rowEnum.Current;
            var tableHeader = this.ParseTableHeader(headerRow);
            var tableRow = headerRow.Start.Row;

            var rows = new List<RawRow>();
            if (rowEnum.MoveNext())
            {
                while (rowEnum.Current != default)
                {
                    var row = rowEnum.Current;
                    if (this.IsSeparatorRow(rowEnum.Current))
                    {
                        if (!rowEnum.MoveNext())
                        {
                            break;
                        }
                        continue;
                    }
                    else if (this.IsDataRow(row))
                    {
                        rows.Add(this.ParseDataRow(row, tableHeader));
                        if (!rowEnum.MoveNext())
                        {
                            break;
                        }
                        continue;
                    }
                    else if (this.IsTableHeader(row))
                    {
                        var nextTableHeader = this.ParseTableHeader(row);
                        if (parentTableHeader.SequenceEqual(nextTableHeader))
                        {
                            if (!rowEnum.MoveNext())
                            {
                                break;
                            }
                            continue;
                        }
                    }
                    else if (this.IsSubTableHeader(row))
                    {
                        var nextTableHeader = this.ParseTableHeader(row);
                        if (tableHeader.SequenceEqual(nextTableHeader))
                        {
                            if (!rowEnum.MoveNext())
                            {
                                break;
                            }
                            continue;
                        }
                    }

                    break;
                }
            }
            return new RawTable(tableHeader, rows, Array.Empty<RawTable>(), tableRow);
        }

        private (RawHeader header, RawFooter footer) ParseFooterAndHeader(ExcelWorksheet sheet)
        {
            var rows = sheet.Dimension.Rows;

            //header and footer are located at fixed addresses
            //numbers are well known data
            var header = new RawHeader(
                BrokerInfo: sheet.Cells[2, 1].Text,
                ReportDate: sheet.Cells[3, 1].Text,
                Investor: sheet.Cells[5, 1].Text,
                ReportRange: sheet.Cells[6, 1].Text,
                StartRow: 2
            );
            var footer = new RawFooter(
                AccountingPerson: sheet.Cells[rows - 1, 1].Text,
                BrokerDivisionHead: sheet.Cells[rows - 2, 1].Text,
                BrokerName: sheet.Cells[rows - 3, 1].Text,
                StartRow: rows - 3
            );

            if (!(footer.IsValid() && header.IsValid()))
            {
                Console.WriteLine("Header / footer is invalid. Failed to parse table.");
                throw new InvalidDataException();
            }
            else
            {
                Console.WriteLine("Passed header validation");
                return (header, footer);
            }
        }

        private RawRow ParseDataRow(ExcelRange row, IReadOnlyList<string> columns)
        {
            var columnValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int columnIndex = 1, visibleColumn = 0; columnIndex < row.Worksheet.Dimension.Columns; visibleColumn++)
            {
                var cell = this.GetParentMergeRegion(row.Worksheet.Cells[row.Start.Row, columnIndex]);
                columnValues.Add(columns[visibleColumn], cell.RichText.Text.Replace("\n", "").Trim());

                if (cell.Columns > 1)
                {
                    columnIndex += cell.Columns;
                }
            }
            return new RawRow(columnValues, row.Start.Row);
        }

        private IReadOnlyList<string> ParseTableHeader(ExcelRange row)
        {
            var columns = new List<string>();
            for (var i = 1; i < row.Worksheet.Dimension.Columns;)
            {
                var cell = this.GetParentMergeRegion(row.Worksheet.Cells[row.Start.Row, i]);
                columns.Add(
                    cell.RichText.Text.Replace("\n", "").Trim()
                );
                if (cell.Columns > 1)
                {
                    i += cell.Columns;
                }
            }
            return columns;
        }
        #region Classifiers
        private bool IsSectionTitle(ExcelRange cell) => cell.Style.Font.Size == 9;
        private bool IsSeparatorRow(ExcelRange cell)
        {
            if (cell.Value != null)
            {
                return false;
            }

            //empty except for two merged cells on the right
            var rightColumnIndex = cell.Worksheet.Dimension.End.Column;
            if (rightColumnIndex < 1)
            {
                return false;
            }
            var rightMostBorder = this.GetParentMergeRegion(cell.Worksheet.Cells[cell.Start.Row, rightColumnIndex]).Start.Column - 1;
            if (rightMostBorder < 1)
            {
                return false;
            }
            var lastEmptyCell = this.GetParentMergeRegion(cell.Worksheet.Cells[cell.Start.Row, rightMostBorder]).Start.Column - 1;
            if (lastEmptyCell < 1)
            {
                return false;
            }

            var result = Enumerable
                .Range(1, lastEmptyCell)
                .All(column =>
                {
                    var currentCell = cell.Worksheet.Cells[cell.Start.Row, column];
                    var brd = currentCell.Style.Border;
                    var isEmpty = new[] { brd.Bottom, brd.Top, brd.Left, brd.Right }
                        .All(a => a.Color.Theme == null && a.Color.Rgb == null && a.Style == ExcelBorderStyle.None)
                        && currentCell.Value == null
                        && !currentCell.Merge;
                    return isEmpty;
                });
            return result;
        }
        private bool IsTableHeader(ExcelRange row) => row.Style.Fill.BackgroundColor.Rgb == "C1D3EC";
        private bool IsDataRow(ExcelRange row) =>
            row.Style.Font.Size < 6
            && (row.Style.Fill.BackgroundColor.Rgb is "FFFFFF" or null)
            && (row.Style.Border.Bottom.Style != ExcelBorderStyle.None || row.Style.Border.Top.Style != ExcelBorderStyle.None);
        private bool IsSubTableHeader(ExcelRange row) => row.Style.Fill.BackgroundColor.Rgb == "D9D9D9";

        #endregion

        #region MergeExtensions

        //Looking up parent cells for merged regions is horribly broken( O(spreadsheet size) instead of O(1) or similar, takes up to 200ms per lookup on the test dataset)
        //in EPPlus, so we have to reinvent the wheel.

        //creates fast merged cell lookup table
        //key: row index
        //value: array of merge regions in that row sorted by x asceding
        //multirow merge regions are not supported
        //the right way to do this is building an R-tree but I'm not a CS grad
        private IDictionary<int, (int x1, int y1, int x2, int y2)[]> BuildFastMergedCellLookupTable(ExcelWorksheet worksheet)
        {
            var mergeRegions = worksheet
                .MergedCells
                .Select(a => worksheet.Cells[a])
                .Select(a => (x1: a.Start.Column, y1: a.Start.Row, x2: a.End.Column, y2: a.End.Row))
                .ToArray();

            if (mergeRegions.Any(a => a.y1 != a.y2))
            {
                throw new NotSupportedException("Multirow merge regions are not supported for fast lookups");
            }

            return mergeRegions.GroupBy(a => a.y1).ToDictionary(a => a.Key, a => a.OrderBy(a => a.x1).ToArray());
        }

        private ExcelRange GetParentMergeRegion(ExcelRange cell)
        {
            if (cell.Merge)
            {
                //custom lookup code working close to O(1) as there's a fixed small number of columns
                var row = this.mergeLookupTable[cell.Start.Row];
                var (x1, y1, x2, y2) = row.Single(a => a.x1 <= cell.Start.Column && a.x2 >= cell.End.Column);
                return cell.Worksheet.Cells[y1, x1, y2, x2];

                /*
                    there's a recommended way from stackoverlow but it's too slow(would take half an hour to process an average report)


                    //https://stackoverflow.com/a/47695698
                    var idx = @this.Worksheet.GetMergeCellId(@this.Start.Row, @this.Start.Column);
                    return @this.Worksheet.Cells[
                        @this.Worksheet.MergedCells[idx - 1] //the array is 0-indexed but the mergeId is 1-indexed...
                    ];
                */
            }
            else
            {
                return cell;
            }
        }
        #endregion

    }
}
