using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace ErrorReportExtractor
{
    public class ConnectionScope : IDisposable
    {
        public ConnectionScope()
        {
            count++;
        }

        ~ConnectionScope()
        {
            this.CommonDispose();
        }

        public static SqlConnection Connection
        {
            get
            {
                if ( ConnectionScope.connection == null )
                {
                    ConnectionScope.connection = new SqlConnection( ConnectionScope.connectionString );
                    connection.Open();
                }
                return ConnectionScope.connection;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize( this );
            this.CommonDispose();
        }

        private void CommonDispose()
        {
            count--;
            if ( count == 0 && ConnectionScope.connection != null )
            {
                ConnectionScope.connection.Close();
                ConnectionScope.connection = null;
            }
        }

        [ThreadStatic]
        private static SqlConnection connection;

        [ThreadStatic]
        private static int count;

        public static void SetConnectionString( string connectionString )
        {
            ConnectionScope.connectionString = connectionString;

        }
        
        private static string connectionString;


        


    }
}
