# SheepChat

 VS2019, .NET Framework 4.7.2

## Installation

 1. Clone & Build from source

 __-OR-__

 1. Download binaries from Releases.
 2. Either run once to generate a config file or create `config.json` with the following settings:
```
{
  "Database": "LiteDb",
  "ConnectionString": "",
  "Port": 23
}
```
 3. Adjust settings per your stack.
 4. Run SheepChat.exe

## Supported Databases

 Currently support is provided for flatfile storage through LiteDb or SQLite, and relational storage through MySql.
 
### LiteDb

To use LiteDb's BSON document storage, set Database to `LiteDb`. ConnectionString can be empty but is required to be present in the config.

### SQLite

To use flatfile SQL storage, set Database to `SQLite` and ConnectionString to a valid SQLite connection string.

### MySql

To use a full-fledged MySQL or MariaDB server as storage, set Database to `MySql` and ConnectionString to a valid MySql connection string.

## Connection Strings

For reference on constructing a proper connection string, I use https://www.connectionstrings.com/
