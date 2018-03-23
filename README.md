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

## Is it fast?

It's fast enough for my use case. Feel free to suggest performance improvements; I care _a lot_ about performance and am definitely willing to sacrifice some code readability/maintainability if the performance gain is significant.

Having that said, this is how it performed at one time, when no row transformation was taking place and no "do-we-need-to-update" check was in place. I intend to update this once I have more recent figures with more features implemented.

```
INFO 23:56:50.73 Pg2Couch.Pg2CouchSynchronizer: Beginning transfer of 4 table(s).
INFO 23:56:50.871 Pg2Couch.Pg2CouchSynchronizer: trello_boards: Transferring 293 records in table.
INFO 23:56:51.761 Pg2Couch.Pg2CouchSynchronizer: trello_boards: Transferred. (329.4 rows/s)
INFO 23:56:52.066 Pg2Couch.Pg2CouchSynchronizer: trello_cards: Transferring 19693 records in table.
INFO 23:56:57.688 Pg2Couch.Pg2CouchSynchronizer: trello_cards: Transferred. (3503 rows/s)
INFO 23:56:57.697 Pg2Couch.Pg2CouchSynchronizer: trello_lists: Transferring 2819 records in table.
INFO 23:56:58.574 Pg2Couch.Pg2CouchSynchronizer: trello_lists: Transferred. (3216.1 rows/s)
INFO 23:56:58.574 Pg2Couch.Pg2CouchSynchronizer: trello_members: Transferring 16 records in table.
INFO 23:56:58.631 Pg2Couch.Pg2CouchSynchronizer: trello_members: Transferred. (284.2 rows/s)
```

## Not in scope

- Supporting other source databases (MySQL, MSSQL, other ADO.NET.) This is clearly doable, but we have chosen the simple path here of only supporting what we need, i.e. PostgreSQL, which is the simplest way to get started in our use case. Feel free to fork the project and add support for other databases if you like.

## License

MIT
