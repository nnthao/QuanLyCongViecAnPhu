using System;
using System.Data.SqlClient;

namespace Dashboard.Common
{
    public class SqlServeHelper
    {
        public static string LayGiaTriCuaCotLaString(SqlDataReader reader, string colName)
        {
            int ordinal = reader.GetOrdinal(colName);
            if (reader.IsDBNull(ordinal))
            {
                return "";
            }
            return reader.GetString(ordinal);
        }

        public static int? LayGiaTriCuaCotLaInteger(SqlDataReader reader, string colName)
        {
            int ordinal = reader.GetOrdinal(colName);
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }
            return reader.GetInt32(ordinal);
        }

        public static int LayGiaTriCuaCotLaIntegerDefaultIsZero(SqlDataReader reader, string colName)
        {
            int ordinal = reader.GetOrdinal(colName);
            if (reader.IsDBNull(ordinal))
            {
                return 0;
            }
            return reader.GetInt32(ordinal);
        }

        public static DateTime? LayGiaTriCuaCotLaDateTime(SqlDataReader reader, string colName)
        {
            int ordinal = reader.GetOrdinal(colName);
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }
            return reader.GetDateTime(ordinal);
        }

        public static bool? LayGiaTriCuaCotLaBoolean(SqlDataReader reader, string colName)
        {
            int ordinal = reader.GetOrdinal(colName);
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }
            return reader.GetBoolean(ordinal);
        }
    }
}