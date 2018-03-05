[![Build Status](https://travis-ci.org/perlun/pg2couch.svg?branch=master)](https://travis-ci.org/perlun/pg2couch)

# pg2couch

`pg2couch` is a little utility for one-way data synchronization from Postgres to CouchDB.

At the moment, it only supports doing a "full synchronization" on startup, but the idea is to also support streaming of changes via Posgres' built-in `NOTIFY`/`LISTEN` functionality. This should allow you to have a live, read-only replica of your Postgres data.

For now, it's just a proof of concept and **not production ready in any way**. See the list of [issues](https://github.com/perlun/pg2couch/issues) to get a view of what we are aiming for in the somewhat-near future

## Supported features

- Initial synchronization of data.
- **TODO**: Only transfer modified records (https://github.com/perlun/pg2couch/issues/3)
- **TODO**: Implement transformation of data (https://github.com/perlun/pg2couch/issues/2)
- **TODO**: Be usable as a library (possibly a NuGet package)
- **TODO**: Support the `NOTIFY`/`LISTEN` commands in PostgreSQL.

## Not in scope

- Supporting other source databases (MySQL, MSSQL, other ADO.NET.) This is clearly doable, but we have chosen the simple path here of only supporting what we need, i.e. PostgreSQL, which is the simplest way to get started in our use case. Feel free to fork the project and add support for other databases if you like.

## License

MIT
