﻿using PROGETTO_S2.Models;
using System.Data.SqlClient;

namespace PROGETTO_S2.Services
{
    public class PrenotazioneService:IPrenotazioniService
    {
        private readonly string _connectionString;
        private const string GET_PRENOTAZIONI_COMMAND = "" +
            "SELECT * FROM Prenotazioni pr " +
            "JOIN Persone p ON p.IdPersona = pr.IdPersona" +
            "JOIN Camere c ON c.IdCamera = pr.IdCamera" +
            "WHERE p.CF = @CF;";

        public PrenotazioneService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Authdb");
        }

        public List<Prenotazione> GetPrenotazioni(string CF)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(GET_PRENOTAZIONI_COMMAND, connection))
                    {
                        command.Parameters.AddWithValue("@CF", CF);
                        using (var reader = command.ExecuteReader())
                        {
                            List<Prenotazione> prenotazioni = new List<Prenotazione>();
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
                                    IdCamera = reader.GetInt32(reader.GetOrdinal("IdCamera")),
                                };
                                prenotazioni.Add(prenotazione);
                            }
                            reader.Close();
                            return prenotazioni;
                        }
                    }
                }
                

            }
            catch(Exception ex)
            {
                throw new Exception("Errore durante il recupero delle prenotazioni", ex);
            }
            
        }
    }
}
