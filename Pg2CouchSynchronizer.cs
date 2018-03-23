using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using MyCouch;
using MyCouch.Requests;
using Newtonsoft.Json;
using NLog;
using Npgsql;

namespace Pg2Couch
{
    public delegate DatabaseRecord RowTransformer(DatabaseRecord row);

    public class Pg2CouchSynchronizer : IDisposable
    {

        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private DbConnection connection;
        private Dictionary<string, RowTransformer> tables = new Dictionary<string, RowTransformer>();

        private string CouchDbUrl =
            Environment.GetEnvironmentVariable("COUCHDB_URL") ??
                throw new TerminateApplicationException("Error: The COUCHDB_URL env variable must be set");

        private string CouchDbDatabaseName =
            Environment.GetEnvironmentVariable("COUCHDB_DB_NAME") ??
                throw new TerminateApplicationException("Error: The COUCHDB_DB_NAME env variable must be set");

        private string PostgresConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING") ??
            throw new TerminateApplicationException("Error: The POSTGRES_CONNECTION_STRING env variable must be set");

        public Pg2CouchSynchronizer()
        {
            this.connection = new NpgsqlConnection(PostgresConnectionString);

            LogHelper.ConfigureLogging();
        }

        public void AddTable(string tableName, RowTransformer rowTransformer = null)
        {
            tables[tableName] = rowTransformer;
        }

        public void PerformInitialSync()
        {
            // We need to start off by performing an initial sync, since the data in our CouchDB database is not
            // necessarily in sync with the data in the Postgres database.
            connection.Open();

            Logger.Info($"Beginning transfer of {tables.Count} table(s).");

            foreach (var table in tables)
            {
                SynchronizeTableToCouchDB(table);
            }
        }

        private void SynchronizeTableToCouchDB(KeyValuePair<string, RowTransformer> table)
        {
            var rows = GetRowsFromTable(table.Key).ToList();
            Logger.Info($"{table.Key}: Transferring {rows.Count()} records in table.");
            var timeTaken = Benchmark(() => InsertIntoCouchDB(CouchDbDatabaseName, rows, table.Value));
            var rowsPerSecond = Math.Round(rows.Count() / timeTaken, 1);

            Logger.Info($"{table.Key}: Transferred. ({rowsPerSecond} rows/s)");
        }

        private IEnumerable<DatabaseRecord> GetRowsFromTable(string tableName)
        {
            using (var command = connection.CreateCommand())
            {
                // Note: directly inserting strings in SQL is clearly an antipattern. However,
                // command.Parameters.AddWithValue() does not work in this case because of limitations in PostgreSQL.
                // More information: https://stackoverflow.com/questions/37752836/postgresql-npgsql-returning-42601-syntax-error-at-or-near-1
                command.CommandText = $@"
                    SELECT *
                    FROM {tableName}
                ";

                using (var reader = command.ExecuteReader())
                {
                    var columnNames = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames.Add(reader.GetName(i));
                    }

                    while (reader.Read())
                    {
                        var databaseRecord = DatabaseRecord.FromReader(reader, columnNames);
                        yield return databaseRecord;
                    }
                }
            }
        }

        private void InsertIntoCouchDB(string databaseName, List<DatabaseRecord> rows, RowTransformer rowTransformer)
        {
            const int chunkSize = 1000;

            using (var client = new MyCouchClient(CouchDbUrl, databaseName))
            {
                Parallel.ForEach(rows.Chunk(chunkSize), new ParallelOptions { MaxDegreeOfParallelism = 20 }, chunkEnumerable =>
                {
                    var chunk = chunkEnumerable.ToArray();
                    IEnumerable<DatabaseRecord> transformedChunk;

                    if (rowTransformer == null)
                    {
                        transformedChunk = chunk;
                    }
                    else
                    {
                        transformedChunk = chunk.Select(row => rowTransformer(row));
                    }

                    var documents = transformedChunk.Select(JsonConvert.SerializeObject).ToArray();

                    var request = new BulkRequest();
                    request.Include(documents);

                    client.Documents.BulkAsync(request).Wait();
                });
            }
        }

        private double Benchmark(Action callback)
        {
            var startTime = DateTime.Now;
            callback();
            var endTime = DateTime.Now;
            var timeTaken = endTime - startTime;

            return timeTaken.TotalSeconds;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.connection.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}
