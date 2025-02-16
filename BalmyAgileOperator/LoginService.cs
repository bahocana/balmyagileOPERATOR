using System;
using System.Data;
using System.Data.SqlClient;

namespace BalmyAgilev1
{
    public class LoginService
    {
        private readonly string connectionString;

        public LoginService(string connString)
        {
            connectionString = connString;
        }

        public (int UserID, int RoleID) Login(string username, string password)
        {
            // DatabaseHelper örneği
            DatabaseHelper dbHelper = new DatabaseHelper(connectionString);

            // Parametreleri tanımla
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", SqlDbType.NVarChar) { Value = username },
                new SqlParameter("@Password", SqlDbType.NVarChar) { Value = password },
                new SqlParameter("@UserID", SqlDbType.Int) { Direction = ParameterDirection.Output }, // Çıkış parametresi
                new SqlParameter("@RoleID", SqlDbType.Int) { Direction = ParameterDirection.Output }  // Çıkış parametresi
            };

            try
            {
                // Prosedürü çalıştır
                dbHelper.ExecuteNonQuery(5,"sp_UserLogin", parameters);

                // Çıktı parametrelerini al
                int userID = parameters[2].Value != DBNull.Value ? Convert.ToInt32(parameters[2].Value) : -1;
                int roleID = parameters[3].Value != DBNull.Value ? Convert.ToInt32(parameters[3].Value) : -1;

                return (userID, roleID); // Başarılı giriş
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Login failed: " + ex.Message);
                return (-1, -1); // Giriş başarısız
            }
        }
    }
}
