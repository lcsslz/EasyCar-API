using System.Data;
using System.Data.SqlClient;

namespace EasyCar_API
{
    public static class SQL
    {
        private const string CONNECTION_STRING = "Server = .\\SQLEXPRESS; Database = EasyCar; Trusted_Connection = True; trustServerCertificate = True";

        public static DataTable GetData(SqlCommand command)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                command.Connection = connection;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);
                return dt;
            }
        }

        public static void SetData(SqlCommand command)
        {
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                command.Connection = connection;
                command.ExecuteNonQuery();
            }
        }

        public static object? SetDataWithReturn(SqlCommand command)
        {
            object? return_value;
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                command.Connection = connection;
                return_value = command.ExecuteScalar();
                return return_value;
            }
        }
    }
}