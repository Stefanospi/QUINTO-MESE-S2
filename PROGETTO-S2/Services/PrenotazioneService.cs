using PROGETTO_S2.Models;
using System.Data.SqlClient;

namespace PROGETTO_S2.Services
{
    public class PrenotazioneService : IPrenotazioniService
    {
        private readonly string _connectionString;
        private const string GET_PRENOTAZIONI_COMMAND = @"
            SELECT pr.*, p.Nome, p.Cognome, c.NumeroCamera, c.Descrizione, c.Tipologia 
            FROM Prenotazioni pr 
            JOIN Persone p ON p.IdPersona = pr.IdPersona 
            JOIN Camere c ON c.IdCamera = pr.IdCamera 
            WHERE p.CF = @CF;";
        private const string GET_PRENOTAZIONI_BY_TIPO_PENSIONE_COMMAND = @"
            SELECT pr.*, p.Nome, p.Cognome, c.NumeroCamera, c.Descrizione, c.Tipologia 
            FROM Prenotazioni pr 
            JOIN Persone p ON p.IdPersona = pr.IdPersona 
            JOIN Camere c ON c.IdCamera = pr.IdCamera 
            WHERE pr.TipoPensione = @TipoPensione;";

        public PrenotazioneService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Authdb");
        }

        public async Task<List<Prenotazione>> GetPrenotazioni(string CF)
        {
            var prenotazioni = new List<Prenotazione>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(GET_PRENOTAZIONI_COMMAND, connection))
                {
                    command.Parameters.AddWithValue("@CF", CF);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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
                                Nome = reader.GetString(reader.GetOrdinal("Nome")),
                                Cognome = reader.GetString(reader.GetOrdinal("Cognome")),
                                IdCamera = reader.GetInt32(reader.GetOrdinal("IdCamera")),
                            };
                            prenotazioni.Add(prenotazione);
                        }
                    }
                }
            }

            return prenotazioni;
        }

        public async Task<List<Prenotazione>> GetPrenotazioneByTipoPensione(string tipoPensione)
        {
            var prenotazioni = new List<Prenotazione>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(GET_PRENOTAZIONI_BY_TIPO_PENSIONE_COMMAND, connection))
                {
                    command.Parameters.AddWithValue("@TipoPensione", tipoPensione);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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
                                Nome = reader.GetString(reader.GetOrdinal("Nome")),
                                Cognome = reader.GetString(reader.GetOrdinal("Cognome")),
                                IdCamera = reader.GetInt32(reader.GetOrdinal("IdCamera")),
                            };
                            prenotazioni.Add(prenotazione);
                        }
                    }
                }
            }
            return prenotazioni;
        }
    }
}
