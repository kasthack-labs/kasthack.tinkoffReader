# Конвертер брокерских отчётов Тинькофф

## Зачем?

Тинькофф отдаёт потрясающе убогие документы, с которыми невозможно работать. Конвертер решает проблему, позволяя получить машинночитаемый JSON или корректный XLSX-файл.
На момент написания [Investments](https://github.com/KonishchevDmitry/investments) не мог обработать некоторые отчёты, а налоги нужно было подать.


[![Github All Releases](https://img.shields.io/github/downloads/kasthack-labs/kasthack.tinkoffReader/total.svg)](https://github.com/kasthack-labs/kasthack.tinkoffReader/releases/latest)
[![GitHub release](https://img.shields.io/github/release/kasthack-labs/kasthack.tinkoffReader.svg)](https://github.com/kasthack-labs/kasthack.tinkoffReader/releases/latest)
[![license](https://img.shields.io/github/license/kasthack-labs/kasthack.tinkoffReader.svg)](LICENSE)
[![.NET Status](https://github.com/kasthack-labs/kasthack.tinkoffReader/workflows/.NET/badge.svg)](https://github.com/kasthack-labs/kasthack.tinkoffReader/actions?query=workflow%3A.NET)
[![CodeQL](https://github.com/kasthack-labs/kasthack.tinkoffReader/workflows/CodeQL/badge.svg)](https://github.com/kasthack-labs/kasthack.tinkoffReader/actions?query=workflow%3ACodeQL)
[![Patreon pledges](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Fshieldsio-patreon.vercel.app%2Fapi%3Fusername%3Dkasthack%26type%3Dpledges&style=flat)](https://patreon.com/kasthack)
[![Patreon patrons](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Fshieldsio-patreon.vercel.app%2Fapi%3Fusername%3Dkasthack%26type%3Dpatrons&style=flat)](https://patreon.com/kasthack)

## Usage

```
kasthack.TinkoffReader:
  Reads tinkoff XLSX broker report and converts it to a machine-readble json file or a usable XLSX.
   Check out https://github.com/kasthack-labs/kasthack.tinkoffReader for more info and updates.

Usage:
  kasthack.TinkoffReader [options]

Options:
  --input-path <input-path>               Input file path.
  --output-path <output-path>             Output file path.
  --input-format <RawJson|TinkoffXlsx>    Input format. [default: TinkoffXlsx]
  --output-format <Parsed|Raw|Xlsx>       Output format. [default: Raw]
  --version                               Show version information
  -?, -h, --help                          Show help and usage information
```

## Форматы

### Входной

* TinkoffXlsx — XLSX, который отдаёт Тинькофф
* RawJson — JSON, сгенерированный ранее через --output-format=Raw

### Выходной
* Raw — документ в JSON без модификаций.
* Xlsx — конвертированный XLSX. Один лист на секцию, никаких объединённых ячеек / разделителей / повторяющихся заголовков таблиц.
* Parsed — строго типизированный JSON. На момент написания документации покрывает только секции с операциями по ценным бумагам и движениям наличных средств.
