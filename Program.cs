using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyCouch;
using NLog;
using Npgsql;

namespace Pg2Couch
{
    public class Program
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        private string CouchDbUrl =
            Environment.GetEnvironmentVariable("COUCHDB_URL") ??
                throw new TerminateApplicationException("Error: The COUCHDB_URL env variable must be set");

        private string CouchDbDatabaseName =
            Environment.GetEnvironmentVariable("COUCHDB_DB_NAME") ??
                throw new TerminateApplicationException("Error: The COUCHDB_DB_NAME env variable must be set");

        public static void Main(string[] args)
        {
            try
            {
                LogHelper.ConfigureLogging();
                new Program().Run();
            }
            catch (Exception e)
            {
                HandleGlobalException(e);
            }
        }

        private void Run()
        {
            var json = "{ \"foo\": \"bar\" }";
            var rows = new List<string>();
            for (int i = 0; i < 20000; i++)
            {
                rows.Add(json);
            }

            InsertIntoCouchDB(CouchDbDatabaseName, rows);
        }

        private void InsertIntoCouchDB(string databaseName, IEnumerable<string> rows)
        {
            using (var client = new MyCouchClient(CouchDbUrl, databaseName))
            {
                // DON'T DO THIS AT HOME. This creates a huge number of threads, which is likely to crash
                // your program if the number of rows is high.
                var tasks = new List<Task>();
                foreach (var row in rows)
                {
                    tasks.Add(client.Documents.PostAsync(row));
                }

                Task.WaitAll(tasks.ToArray());
            }
        }

        private static void HandleGlobalException(Exception exception)
        {
            Console.Error.WriteLine(exception.Message);

            if (!(exception is TerminateApplicationException))
            {
                Console.Error.WriteLine(exception.StackTrace);
            }

            Environment.Exit(1);
        }
    }
}
