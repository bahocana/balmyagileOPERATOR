using System;
using System.Data;
using System.Data.SqlClient;

namespace BalmyAgilev1
{
    public class DatabaseHelper
    {
        private readonly string connectionString;

        // Constructor: Veritabanı bağlantı dizesi alır
        public DatabaseHelper(string connString)
        {
            connectionString = connString;
        }

        // Yetkilendirme kontrolü metodu
        private bool CheckAuthorization(int userId, string procedureName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_CheckAuthorization", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@ProcedureName", procedureName);

                    SqlParameter outputParam = new SqlParameter("@IsAuthorized", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (bool)outputParam.Value;
                }
            }
        }

        // Stored Procedure çalıştırır ve sonuçları bir DataTable olarak döner
        public DataTable ExecuteStoredProcedure(int userId, string procedureName, SqlParameter[] parameters = null)
        {
            // Yetkilendirme kontrolü
            if (!CheckAuthorization(userId, procedureName))
            {
                throw new UnauthorizedAccessException($"Kullanıcı {userId}, {procedureName} prosedürünü çalıştırmak için yetkili değil.");
            }

            DataTable resultTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parametreleri ekle
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(resultTable);
                    }
                }
            }

            return resultTable;
        }

        // Stored Procedure çalıştırır, yalnızca bir değer döner (örneğin, bir COUNT veya ID)
        public object ExecuteScalar(int userId, string procedureName, SqlParameter[] parameters = null)
        {
            // Yetkilendirme kontrolü
            if (!CheckAuthorization(userId, procedureName))
            {
                throw new UnauthorizedAccessException($"Kullanıcı {userId}, {procedureName} prosedürünü çalıştırmak için yetkili değil.");
            }

            object result;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parametreleri ekle
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    result = command.ExecuteScalar();
                }
            }

            return result;
        }
        public SqlDataReader ExecuteReader(string storedProcedure, SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        // Stored Procedure çalıştırır, sadece etkilenen satır sayısını döner
        public int ExecuteNonQuery(int userId, string procedureName, SqlParameter[] parameters = null)
        {
            // Yetkilendirme kontrolü
            if (!CheckAuthorization(userId, procedureName))
            {
                throw new UnauthorizedAccessException($"Kullanıcı {userId}, {procedureName} prosedürünü çalıştırmak için yetkili değil.");
            }

            int rowsAffected;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parametreleri ekle
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
            }

            return rowsAffected;
        }
    }
}
