using System;
using System.Collections.Generic;
using System.Data.Common;
using Humanizer;

namespace Pg2Couch
{
    public class DatabaseRecord : Dictionary<string, object>
    {
        public DatabaseRecord TransformKeys(Transformation transformation)
        {
            var databaseRecord = new DatabaseRecord();

            if (transformation == Transformation.CamelizeLower)
            {
                foreach (var key in Keys)
                {
                    databaseRecord[key.Camelize()] = this[key];
                }

                return databaseRecord;
            }
            else if (transformation == Transformation.CamelizeUpper)
            {
                foreach (var key in Keys)
                {
                    databaseRecord[key.Pascalize()] = this[key];
                }

                return databaseRecord;
            }
            else if (transformation == Transformation.SnakeCase)
            {
                foreach (var key in Keys)
                {
                    databaseRecord[key.Underscore()] = this[key];
                }

                return databaseRecord;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        internal static DatabaseRecord FromReader(DbDataReader reader, List<string> columnNames)
        {
            var databaseRecord = new DatabaseRecord();

            for (int i = 0; i < columnNames.Count; i++)
            {
                databaseRecord[columnNames[i]] = reader[i];
            }

            return databaseRecord;
        }
    }
}
