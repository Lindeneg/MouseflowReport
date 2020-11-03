Mouseflow Report
- Generate CSV reports for Mouseflow connected websites

</br>

Requirements:

- [Access to Mouseflow REST API](https://app.mouseflow.com/settings/api)

__________

</br>

Installation

`$ git clone https://github.com/Lindeneg/MouseflowReport && cd MouseflowReport`

</br>

Run unit tests

`$ dotnet test`

</br>

Build

`$ dotnet build --output Build`

__________

</br>

Usage

`$ ./Build/MouseflowReport ARGS [ ...FLAGS ]`


Arguments:

    -u USER       -> Mouseflow user
    -k KEY        -> Mouseflow token
    -l LOCATION   -> Mouseflow server location
    -p PROJECTS   -> Comma-separated Mouseflow website Ids
    -f FROMDATE   -> ISO 8601 date string specifying start of date range
    -t TODATE     -> ISO 8601 date string specifying end of date range
    -o OUTPUT     -> Path to output directory

Flags:

    -T            -> Generate a total row for accumulative data
    -R            -> Remove all empty rows
    -C            -> Convert millisecond time measures to minutes
    -K            -> Keep all entries in most seen maps
    -D            -> Output debug information


*Note:* 

*- Mouseflow token and Mouseflow server location can be found [here](https://app.mouseflow.com/settings/api)*

*- Mouseflow website ID can be found [here](https://app.mouseflow.com/websites/settings)*

__________

</br>

Estimated Performance

*tested in three batches of 9000, 75000 and 400000 recordings, respectively*

```
n                          = amount of recordings
oneRecordingParseInSec    ~= 0.0000549978

allRecordingParseInSec    ~= n * 0.0000549978
```

However for every 10.000th recording, a new GET request have to be exercised,
where a response can take seconds to be received. The estimated runtime,
for n amount of recordings with an arbitrary response time of 4 seconds is:

```
totalRunTimeInSec         ~= ((n / 10000) * 4) + (n * 0.0000549978)
```
