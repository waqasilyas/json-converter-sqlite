# JSON Converter for SQLite
A simple command line utility that converts an SQLite file to text based JSON

This is not meant to be an API and doesn't offer a lot of configurability. Just a quick way to dump a database into a JSON file for quick inspection or for integrating it into diff tools for on-the-fly conversions.

# Usage

```
JsonConverterSqlite.exe [-hf] SOURCE [TARGET]

SOURCE    An SQLite file to convert.
TARGET    The destination file to save the JSON output. If not give, the JSON is emitted on standard output.
-f        Force overwrite if TARGET exists
-h        Prints this description.
```

# Legal

Copyright (c) 2017 Waqas Ilyas
Licenced under [MIT License](LICENSE.md)

This software uses third-party libraries that are distributed under their own terms, see [LICENSE-3RD-PARTY](LICENSE-3RD-PARTY.md)