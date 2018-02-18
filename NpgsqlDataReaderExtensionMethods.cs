using System.Collections.Generic;
using Npgsql;

namespace PostgresToCouchDB
{
    public static class NpgsqlDataReaderExtensionMethods
    {
        public static List<T> GetFirstColumnAs<T>(this NpgsqlDataReader reader)
        {
            var result = new List<T>();
            while (reader.Read())
            {
                result.Add((T)reader[0]);
            }
            return result;
        }
    }
}
