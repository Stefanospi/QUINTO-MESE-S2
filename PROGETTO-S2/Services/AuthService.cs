using System.Data.SqlClient;
using PROGETTO_S2.Models;

namespace PROGETTO_S2.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _connectionString;
        private const string LOGIN_COMMAND = "SELECT * FROM Utenti WHERE Username = @Username AND Password = @Password";
        private const string REGISTER_COMMAND = "INSERT INTO Utenti (Username, Password) VALUES (@Username, @Password)";

        public AuthService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Authdb");
        }

        public Utente Login(string username, string password)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(LOGIN_COMMAND, connection))
                    {
                        var hashedPassword = PasswordHelper.HashPassword(password);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", hashedPassword);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new Utente
                                {
                                    IdUtente = reader.GetInt32(reader.GetOrdinal("IdUtente")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    Password = reader.GetString(reader.GetOrdinal("Password"))
                                };
                                reader.Close();
                                return user;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante il login", ex);
            }
        }

        public Utente Register(string username, string password)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(REGISTER_COMMAND, connection))
                    {
                        var hashedPassword = PasswordHelper.HashPassword(password);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", hashedPassword);
                        var userId = Convert.ToInt32(command.ExecuteScalar());
                        return new Utente
                        {
                            IdUtente = userId,
                            Username = username,
                            Password = hashedPassword
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Registration failed: " + ex.Message);
            }
        }
    }
}
