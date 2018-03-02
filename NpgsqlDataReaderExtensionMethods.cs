using System;
using System.Collections.Generic;
using Npgsql;

namespace Pg2Couch
{
    public static class NpgsqlDataReaderExtensionMethods
    {
        public static List<T> GetFirstColumnAs<T>(this NpgsqlDataReader reader)
        {
            return reader.MapResult(innerReader => (T)innerReader[0]);
        }

        public static List<T> MapResult<T>(this NpgsqlDataReader reader, Func<NpgsqlDataReader, T> mapper)
        {
            var result = new List<T>();
            while (reader.Read())
            {
                result.Add(mapper(reader));
            }
            return result;
        }
    }
}
