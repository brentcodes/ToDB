using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDB;
using ToDB.SqlClient;

namespace System.Data.SqlClient
{
    public static class Extension
    {
        public static string ToSql(this Command command)
        {
            TSqlSerializer serializer = new TSqlSerializer();
            return serializer.ToSql(command);
        }

        public static string ToSql(this Command command, TSqlSerializerOptions options)
        {
            TSqlSerializer serializer = new TSqlSerializer(options);
            return serializer.ToSql(command);
        }

    }
}
