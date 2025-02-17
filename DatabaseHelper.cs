using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Windows.Forms;

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

        // İnternet ve VPN bağlantısını kontrol eden metot
        private bool CheckInternetAndVPN()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send("8.8.8.8", 1000); // Google DNS ile test
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        // SQL Server'a bağlantıyı test eden metot
        private bool CheckSQLConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // Yetkilendirme kontrolü metodu
        private bool CheckAuthorization(int userId, string procedureName)
        {
            if (!CheckInternetAndVPN() || !CheckSQLConnection())
            {
                MessageBox.Show("İnterneti ve VPN'in açık olup olmadığını kontrol ediniz.", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

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

        // Stored Procedure çalıştırır, yalnızca bir değer döner
        public object ExecuteScalar(int userId, string procedureName, SqlParameter[] parameters = null)
        {
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
            if (!CheckInternetAndVPN() || !CheckSQLConnection())
            {
                MessageBox.Show("İnterneti ve VPN'in açık olup olmadığını kontrol ediniz.", "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

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
