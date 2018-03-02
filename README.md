# README.md

This branch is an attempt to reproduce the https://github.com/dotnet/coreclr/issues/16438 issue in a reproducible manner.

The problem has thus far only been reproduced on macOS.

## Prerequisites

Install [Docker for Mac](https://www.docker.com/docker-mac). It includes the `docker-compose` command line utility, which lets you start Docker-based applications easily.

To start the prerequisities (CouchDB):

```shell
$ docker-compose up -d
```

Create the database:

```shell
$ curl -X PUT http://localhost:5984/pg2couch
```

## Running the test

You can now run the program that reproduces the bug:

```shell
$ ./debug.sh
```
