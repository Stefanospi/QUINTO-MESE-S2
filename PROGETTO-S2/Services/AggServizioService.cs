using PROGETTO_S2.Models;
using System.Data.SqlClient;

namespace PROGETTO_S2.Services
{
    public class AggServizioService : IAggServizioService
    {
        private readonly string _connectionString;
        private readonly ILogger<AggServizioService> _logger;
        private const string AGG_SERVIZIO_COMMAND = "" +
            "INSERT INTO PrenotazioniServiziAgg(IdPrenotazione, IdServizioAgg, Data, Quantita, Prezzo) " +
            "VALUES (@IdPrenotazione, @IdServizioAgg, @Data, @Quantita, @Prezzo);";
        private const string GET_ALL_SERVIZIAGG_COMMAND = "SELECT IdServizioAgg,Descrizione FROM ServiziAgg";

        public AggServizioService(IConfiguration configuration, ILogger<AggServizioService> logger)
        {
            _connectionString = configuration.GetConnectionString("Authdb");
            _logger = logger;
        }

        public PrenotazioneServizioAgg AddServizioAgg(PrenotazioneServizioAgg prenotazioneServizioAgg, int IdPrenotazione)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(AGG_SERVIZIO_COMMAND, connection))
                    {
                        command.Parameters.AddWithValue("@IdPrenotazione", IdPrenotazione);
                        command.Parameters.AddWithValue("@IdServizioAgg", prenotazioneServizioAgg.IdServizioAgg);
                        command.Parameters.AddWithValue("@Data", prenotazioneServizioAgg.Data);
                        command.Parameters.AddWithValue("@Quantita", prenotazioneServizioAgg.Quantita);
                        command.Parameters.AddWithValue("@Prezzo", prenotazioneServizioAgg.Prezzo);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return prenotazioneServizioAgg;
                        }
                        else
                        {
                            _logger.LogWarning("Nessuna riga inserita per la prenotazione {IdPrenotazione}", IdPrenotazione);
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'inserimento del servizio aggiuntivo per la prenotazione {IdPrenotazione}", IdPrenotazione);
                return null;
            }
        }

        public List<ServizioAgg> GetServiziAgg()
        {
            List<ServizioAgg> servizi = new List<ServizioAgg>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(GET_ALL_SERVIZIAGG_COMMAND, conn);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    var s = new ServizioAgg
                    {
                        IdServizioAgg = r.GetInt32(0),
                        Descrizione = r.GetString(1),
                    };
                    servizi.Add(s);
                }
                return servizi;
            }
            catch (Exception ex) { }
            return null;
        }
    }
}
