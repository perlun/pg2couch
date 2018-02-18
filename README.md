[![Build Status](https://travis-ci.org/perlun/pg2couch.svg?branch=master)](https://travis-ci.org/perlun/pg2couch)

# pg2couch

`pg2couch` is a little utility for one-way data synchronization from Postgres to CouchDB.

At the moment, it only supports doing a "full synchronization" on startup, but the idea is to also support streaming of changes via Posgres' built-in `NOTIFY`/`LISTEN` functionality. This should allow you to have a live, read-only replica of your Postgres data.

**TODO**: Only records which are actually changed compared to their CouchDB representation will be updated in CouchDB, to avoid creating superfluous document revisions.

**TODO**: The format of the data in CouchDB will be optimized to suit [`ember-pouch`](https://github.com/pouchdb-community/ember-pouch) since this is our primary consumer of the CouchDB data.

## TODO/unresolved issues

- Implement the TODO points noted above
- Think about how to support data being changed in CouchDB, replicated back to Postgres (using some 3rd party tool). How will we avoid data "looping"? Will the change detection be enough to support these scenarios?
