using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyCouch;
using Npgsql;

namespace PostgresToCouchDB
{
    public class Program
    {
        private string PostgresConnectionString =
            Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING") ??
                throw new TerminateApplicationException("Error: The POSTGRES_CONNECTION_STRING env variable must be set");

        private string CouchDbUrl =
            Environment.GetEnvironmentVariable("COUCHDB_URL") ??
                throw new TerminateApplicationException("Error: The COUCHDB_URL env variable must be set");

        private string CouchDbDatabaseName =
            Environment.GetEnvironmentVariable("COUCHDB_DB_NAME") ??
                throw new TerminateApplicationException("Error: The COUCHDB_DB_NAME env variable must be set");

        private static void HandleGlobalException(Exception exception)
        {
            Console.Error.WriteLine(exception.Message);

            if (!(exception is TerminateApplicationException))
            {
                Console.Error.WriteLine(exception.StackTrace);
            }

            Environment.Exit(1);
        }

        public static void Main(string[] args)
        {
            try
            {
                new Program().Run();
            }
            catch (Exception e)
            {
                HandleGlobalException(e);
            }
        }

        private void Run()
        {
            using (var connection = new NpgsqlConnection(PostgresConnectionString))
            {
                // We need to start off by performing an initial sync, since the data in our CouchDB database is not
                // necessarily in sync with the data in the Postgres database.
                connection.Open();

                var tables = GetTables(connection);
                LogInfo("Retrieved table list");

                foreach (var table in tables)
                {
                    SynchronizeTableToCouchDB(connection, table);
                }
            }
        }

        private IEnumerable<string> GetTables(NpgsqlConnection conn)
        {
            var queryString = @"
                SELECT table_name
                  FROM information_schema.tables
                 WHERE table_schema='public'
                   AND table_type='BASE TABLE';
            ";
            using (var command = new NpgsqlCommand(queryString, conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    return reader.GetFirstColumnAs<string>();
                }
            }
        }

        private void SynchronizeTableToCouchDB(NpgsqlConnection connection, string table)
        {
            var rows = GetRowsFromTableAsJson(connection, table);
            LogInfo($"Transferring {rows.Count()} records in table '{table}' from Postgres to CouchDB");

            InsertIntoCouchDB(CouchDbDatabaseName, rows);
        }

        private void InsertIntoCouchDB(string databaseName, IEnumerable<string> rows)
        {
            using (var client = new MyCouchClient(CouchDbUrl, databaseName))
            {
                Parallel.ForEach(rows, new ParallelOptions { MaxDegreeOfParallelism = 10 }, row =>
                {
                    client.Documents.PostAsync(row).Wait();
                });
            }
        }

        private IEnumerable<string> GetRowsFromTableAsJson(NpgsqlConnection connection, string tableName)
        {
            // ROW_TO_JSON() requires Postgres 9.2 or greater:
            // https://www.postgresql.org/docs/current/static/functions-json.html
            //
            // Note: directly inserting strings in SQL is clearly an antipattern. However,
            // command.Parameters.AddWithValue() does not work in this case because of limitations in PostgreSQL.
            // More information: https://stackoverflow.com/questions/37752836/postgresql-npgsql-returning-42601-syntax-error-at-or-near-1
            var queryString = $@"
                SELECT ROW_TO_JSON({tableName})
                FROM {tableName}
            ";
            using (var command = new NpgsqlCommand(queryString, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    return reader.GetFirstColumnAs<string>();
                }
            }
        }

        private void LogInfo(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }
    }
}
