# README.md

This branch is an attempt to reproduce the https://github.com/dotnet/coreclr/issues/16438 issue in a reproducible manner.

The problem has thus far only been reproduced on macOS.

To start the prerequisities (CouchDB):

```shell
$ docker-compose up -d
```

Create the database:

```shell
$ curl -X PUT http://localhost:5984/pg2couch
```

You can now run the program that reproduces the bug:

```shell
$ ./debug.sh
```
