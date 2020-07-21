using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace NetCodeExample
{

    //http://www.queryprocessor.ru/fast-in-ssms-slow-in-app-part1/

    //https://dba.stackexchange.com/questions/2500/make-sqlclient-default-to-arithabort-on


    //https://docs.microsoft.com/ru-ru/ef/core/extensions/
    internal class CheckSQL_ARITHABORT
    {
        internal static void PrintSample()
        {
            string connection = "Server=localhost;Database=ContainerEditor;Integrated Security=SSPI;persist security info=True;";

            SqlConnection connect = new SqlConnection(connection);
            connect.Open();

            string cmd = "SELECT SESSIONPROPERTY('ARITHABORT');";

            SqlCommand sqlCommand = new SqlCommand(cmd, connect);
            SqlDataReader reader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

            var colCnt = reader.GetColumnSchema().Count;

            while (reader.Read())
            {
                string result = "ARITHABORT = ";
                for (int i =0; i< colCnt; i++)
                {
                    result += reader.GetInt32(i) + ";  ";
                }
                Console.WriteLine(result);
            }
        }
    }
}