using Newtonsoft.Json.Linq;
using PROGETTO_S2.Models;
using System.Data.SqlClient;

namespace PROGETTO_S2.Services
{
    public class CreateService : ICreationService
    {
        private readonly string _connectionString;
        private const string CREATE_PERSONA_COMMAND= "" +
            "INSERT INTO Persone (Nome, Cognome, CF, Email, Telefono, Cellulare, Città, Provincia) " +
            "OUTPUT INSERTED.IdPersona " +
            "VALUES (@Nome, @Cognome, @CF, @Email, @Telefono, @Cellulare, @Città, @Provincia)";
        private const string CREATE_PRENOTAZIONE_COMMAND = "" +
            "INSERT INTO [dbo].[Prenotazioni] (DataPrenotazione, NumProgressivo, Anno, SoggiornoDal, SoggiornoAl, Caparra, Tariffa, TipoPensione, IdPersona, IdCamera) " +
            "OUTPUT INSERTED.IdPrenotazione " +
            "VALUES (@DataPrenotazione,@NumProgressivo, @Anno, @SoggiornoDal,@SoggiornoAl, @Caparra, @Tariffa, @TipoPensione, @IdPersona, @IdCamera)";
        private const string GET_PERSONA_COMMAND = "SELECT * FROM Persone;";
        private const string GET_PRENOTAZIONE_COMMAND = "SELECT * FROM Prenotazioni;";
        private const string GET_PRENOTAZIONE_BY_ID = "SELECT * FROM Prenotazioni WHERE IdPrenotazione = @IdPrenotazione";
        public CreateService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Authdb");
        }
        public Persona CreatePersona(Persona persona)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(CREATE_PERSONA_COMMAND, connection))
                    {
                        command.Parameters.AddWithValue("@Nome", persona.Nome);
                        command.Parameters.AddWithValue("@Cognome", persona.Cognome);
                        command.Parameters.AddWithValue("@CF", persona.CF);
                        command.Parameters.AddWithValue("@Email", persona.Email);
                        command.Parameters.AddWithValue("@Telefono", persona.Telefono);
                        command.Parameters.AddWithValue("@Cellulare", persona.Cellulare);
                        command.Parameters.AddWithValue("@Città", persona.Città);
                        command.Parameters.AddWithValue("@Provincia", persona.Provincia);
                        persona.IdPersona = (int)command.ExecuteScalar();

                    }
                    return persona;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante la creazione della persona", ex);
            }
        }

        public Prenotazione CreatePrenotazione(Prenotazione prenotazione)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(CREATE_PRENOTAZIONE_COMMAND, connection))
                    {
                        // Aggiungi parametri
                        command.Parameters.AddWithValue("@DataPrenotazione", prenotazione.DataPrenotazione);
                        command.Parameters.AddWithValue("@NumProgressivo", prenotazione.NumProgressivo);
                        command.Parameters.AddWithValue("@Anno", prenotazione.Anno);
                        command.Parameters.AddWithValue("@SoggiornoDal", prenotazione.SoggiornoDal);
                        command.Parameters.AddWithValue("@SoggiornoAl", prenotazione.SoggiornoAl);
                        command.Parameters.AddWithValue("@Caparra", prenotazione.Caparra);
                        command.Parameters.AddWithValue("@Tariffa", prenotazione.Tariffa);
                        command.Parameters.AddWithValue("@TipoPensione", prenotazione.TipoPensione);
                        command.Parameters.AddWithValue("@IdPersona", prenotazione.IdPersona);
                        command.Parameters.AddWithValue("@IdCamera", prenotazione.IdCamera);

                        // Esegui il comando
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            prenotazione.IdPrenotazione = Convert.ToInt32(result);
                            Console.WriteLine("Prenotazione created with Id: " + prenotazione.IdPrenotazione);
                        }
                        else
                        {
                            throw new Exception("Failed to retrieve the inserted IdPrenotazione.");
                        }
                    }
                }
                return prenotazione;
            }
            catch (SqlException ex)
            {
                // Log SQL-specific errors
                Console.WriteLine("SQL Exception: " + ex.Message);
                throw new Exception("Errore durante la creazione della prenotazione: SQL error", ex);
            }
            catch (Exception ex)
            {
                // Log general errors
                Console.WriteLine("General Exception: " + ex.Message);
                throw new Exception("Errore durante la creazione della prenotazione", ex);
            }
        }
        public List<Persona> GetPersona()
        {
            List<Persona> persone = new List<Persona>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GET_PERSONA_COMMAND, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var persona = new Persona
                                {
                                    IdPersona = reader.GetInt32(reader.GetOrdinal("IdPersona")),
                                    Nome = reader.GetString(reader.GetOrdinal("Nome")),
                                    Cognome = reader.GetString(reader.GetOrdinal("Cognome")),
                                    CF = reader.GetString(reader.GetOrdinal("CF")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                                    Cellulare = reader.GetString(reader.GetOrdinal("Cellulare")),
                                    Città = reader.GetString(reader.GetOrdinal("Città")),
                                    Provincia = reader.GetString(reader.GetOrdinal("Provincia"))
                                };
                                persone.Add(persona);
                            }
                            reader.Close();
                        }
                    }
                    return persone;

                }

            }
            catch(Exception ex)
            {
                throw new Exception("Errore durante il recupero della persona", ex);
            }
        }

        public List<Prenotazione> GetPrenotazione()
        {
            try
            {
                List<Prenotazione> prenotazioni = new List<Prenotazione>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GET_PRENOTAZIONE_COMMAND, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var prenotazione = new Prenotazione
                                {
                                    IdPrenotazione = reader.GetInt32(reader.GetOrdinal("IdPrenotazione")),
                                    DataPrenotazione = reader.GetDateTime(reader.GetOrdinal("DataPrenotazione")),
                                    NumProgressivo = reader.GetInt32(reader.GetOrdinal("NumProgressivo")),
                                    Anno = reader.GetInt32(reader.GetOrdinal("Anno")),
                                    SoggiornoDal = reader.GetDateTime(reader.GetOrdinal("SoggiornoDal")),
                                    SoggiornoAl = reader.GetDateTime(reader.GetOrdinal("SoggiornoAl")),
                                    Caparra = reader.GetDecimal(reader.GetOrdinal("Caparra")),
                                    Tariffa = reader.GetDecimal(reader.GetOrdinal("Tariffa")),
                                    TipoPensione = reader.GetString(reader.GetOrdinal("TipoPensione")),
                                    IdPersona = reader.GetInt32(reader.GetOrdinal("IdPersona")),
                                    IdCamera = reader.GetInt32(reader.GetOrdinal("IdCamera"))
                                };
                                prenotazioni.Add(prenotazione);
                            }
                            reader.Close();
                        }
                    }
                    return prenotazioni;
                }

            }
            catch(Exception ex)
            {
                   throw new Exception("Errore durante il recupero della prenotazione", ex);
            }
        }
        public Prenotazione GetPrenotazioneById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GET_PRENOTAZIONE_BY_ID, connection))
                    {
                        command.Parameters.AddWithValue("@IdPrenotazione", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var prenotazione = new Prenotazione
                                {
                                    IdPrenotazione = reader.GetInt32(reader.GetOrdinal("IdPrenotazione")),
                                    DataPrenotazione = reader.GetDateTime(reader.GetOrdinal("DataPrenotazione")),
                                    NumProgressivo = reader.GetInt32(reader.GetOrdinal("NumProgressivo")),
                                    Anno = reader.GetInt32(reader.GetOrdinal("Anno")),
                                    SoggiornoDal = reader.GetDateTime(reader.GetOrdinal("SoggiornoDal")),
                                    SoggiornoAl = reader.GetDateTime(reader.GetOrdinal("SoggiornoAl")),
                                    Caparra = reader.GetDecimal(reader.GetOrdinal("Caparra")),
                                    Tariffa = reader.GetDecimal(reader.GetOrdinal("Tariffa")),
                                    TipoPensione = reader.GetString(reader.GetOrdinal("TipoPensione")),
                                    IdPersona = reader.GetInt32(reader.GetOrdinal("IdPersona")),
                                    IdCamera = reader.GetInt32(reader.GetOrdinal("IdCamera"))
                                };
                                return prenotazione;
                            }
                            else
                            {
                                throw new Exception("Prenotazione not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante il recupero della prenotazione", ex);
            }
        }
    }
}
