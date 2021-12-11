# Конвертер брокерских отчётов Тинькофф

## Зачем?

Тинькофф отдаёт потрясающе убогие документы, с которыми невозможно работать. Конвертер решает проблему, позволяя получить машинночитаемый JSON или корректный XLSX-файл.
На момент написания [Investments](https://github.com/KonishchevDmitry/investments) не мог обработать некоторые отчёты, а налоги нужно было подать.

Работающие секции для типизированных отчётов:

* 1.1 Информация о совершенных и исполненных сделках на конец отчетного периода
* 1.2 Информация о неисполненных сделках на конец отчетного периода
* 1.3 Сделки за расчетный период, обязательства из которых прекращены  не в результате исполнения
* 2\. Операции с денежными средствами
* 5\. Информация о торговых площадках
* 6\. Расшифровка дополнительных кодов используемых в отчете

TODO:
 * 1.4 Информация об изменении расчетных параметров сделок РЕПО
 * 3.1 Движение по ценным бумагам инвестора
 * 3.2 Движение по производным финансовым инструментам
 * 3.3 Информация о позиционном состоянии по производным финансовым инструментам из таблицы
 * 4.1 Информация о ценных бумагах
 * 4.2 Информация об инструментах, не квалифицированных в качестве ценной бумаги
 * 4.3 Информация о производных финансовых инструментах

[![Github All Releases](https://img.shields.io/github/downloads/kasthack-labs/kasthack.tinkoffReader/total.svg)](https://github.com/kasthack-labs/kasthack.tinkoffReader/releases/latest)
[![GitHub release](https://img.shields.io/github/release/kasthack-labs/kasthack.tinkoffReader.svg)](https://github.com/kasthack-labs/kasthack.tinkoffReader/releases/latest)
[![license](https://img.shields.io/github/license/kasthack-labs/kasthack.tinkoffReader.svg)](LICENSE)
[![.NET Status](https://github.com/kasthack-labs/kasthack.tinkoffReader/workflows/.NET/badge.svg)](https://github.com/kasthack-labs/kasthack.tinkoffReader/actions?query=workflow%3A.NET)
[![CodeQL](https://github.com/kasthack-labs/kasthack.tinkoffReader/workflows/CodeQL/badge.svg)](https://github.com/kasthack-labs/kasthack.tinkoffReader/actions?query=workflow%3ACodeQL)
[![Patreon pledges](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Fshieldsio-patreon.vercel.app%2Fapi%3Fusername%3Dkasthack%26type%3Dpledges&style=flat)](https://patreon.com/kasthack)
[![Patreon patrons](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Fshieldsio-patreon.vercel.app%2Fapi%3Fusername%3Dkasthack%26type%3Dpatrons&style=flat)](https://patreon.com/kasthack)

## Использование

### Установка

#### Windows

Свежие бинари для Windows [лежат тут](https://github.com/kasthack-labs/kasthack.tinkoffReader/releases/latest).

#### Unix

* Если у вас MacOS / Linux, придётся собрать из сорцов.

* Для начала, поставьте [.NET SDK](https://dotnet.microsoft.com/download).

* Соберите конвертер:

```
git clone https://github.com/kasthack-labs/kasthack.tinkoffReader/
cd kasthack.tinkoffReader/src
dotnet build
```

* Запускайте через `dotnet run -- [параметры приложения]`

## Выполнение

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
* RawJson — JSON, сгенерированный ранее через `--output-format=Raw`

### Выходной
* Raw — документ в JSON без модификаций.
* Xlsx — конвертированный XLSX. Один лист на секцию, никаких объединённых ячеек / разделителей / повторяющихся заголовков таблиц.
* Parsed — строго типизированный JSON. На момент написания документации покрывает только секции с операциями по ценным бумагам и движениям наличных средств.
