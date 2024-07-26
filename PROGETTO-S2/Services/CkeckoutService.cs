using PROGETTO_S2.Models;
using System.Data.SqlClient;

namespace PROGETTO_S2.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly string _connectionString;
        private readonly ILogger<CheckoutService> _logger;

        private const string GET_STANZA_PERIODO_TARIFFA = @"
            SELECT 
                C.NumeroCamera, 
                P.SoggiornoDal, 
                P.SoggiornoAl, 
                P.Tariffa,
                P.Caparra
            FROM 
                Prenotazioni AS P 
            INNER JOIN 
                Camere AS C ON P.IdCamera = C.IdCamera 
            WHERE 
                P.IdPrenotazione = @IdPrenotazione";

        private const string GET_SERVIZI_BY_PRENOTAZIONE = @"
            SELECT 
                S.Descrizione, 
                PS.Data, 
                PS.Quantita, 
                PS.Prezzo 
            FROM 
                ServiziAgg AS S 
            INNER JOIN 
                PrenotazioniServiziAgg AS PS ON S.IdServizioAgg = PS.IdServizioAgg 
            WHERE 
                PS.IdPrenotazione = @IdPrenotazione";

        private const string GET_IMPORTO = @"
            SELECT 
                (p.Tariffa - p.Caparra + ISNULL(SUM(ps.Quantita * ps.Prezzo), 0)) AS ImportoDaSaldare
            FROM 
                Prenotazioni AS p 
            LEFT JOIN 
                PrenotazioniServiziAgg AS ps ON p.IdPrenotazione = ps.IdPrenotazione 
            WHERE 
                p.IdPrenotazione = @IdPrenotazione 
            GROUP BY 
                p.Tariffa, p.Caparra";

        public CheckoutService(IConfiguration configuration, ILogger<CheckoutService> logger)
        {
            _connectionString = configuration.GetConnectionString("Authdb");
            _logger = logger;
        }

        public async Task<CheckoutModel> GetPrenotazioneConImportoDaSaldare(int idPrenotazione)
        {
            var model = new CheckoutModel();

            try
            {
                var stanzaDetails = await ExecuteReaderAsync<StanzaViewModel>(
                    GET_STANZA_PERIODO_TARIFFA,
                    cmd => cmd.Parameters.AddWithValue("@IdPrenotazione", idPrenotazione),
                    reader => new StanzaViewModel
                    {
                        NumeroCamera = reader.GetInt32(reader.GetOrdinal("NumeroCamera")),
                        SoggiornoDal = reader.GetDateTime(reader.GetOrdinal("SoggiornoDal")),
                        SoggiornoAl = reader.GetDateTime(reader.GetOrdinal("SoggiornoAl")),
                        Tariffa = reader.GetDecimal(reader.GetOrdinal("Tariffa")),
                        Caparra = reader.GetDecimal(reader.GetOrdinal("Caparra"))
                    });

                if (stanzaDetails.Any())
                {
                    model.NumeroCamera = stanzaDetails.First().NumeroCamera;
                    model.SoggiornoDal = stanzaDetails.First().SoggiornoDal;
                    model.SoggiornoAl = stanzaDetails.First().SoggiornoAl;
                    model.Tariffa = stanzaDetails.First().Tariffa;
                    model.Caparra = stanzaDetails.First().Caparra;
                }

                var serviziAgg = await ExecuteReaderAsync<ServizioAggViewModel>(
                    GET_SERVIZI_BY_PRENOTAZIONE,
                    cmd => cmd.Parameters.AddWithValue("@IdPrenotazione", idPrenotazione),
                    reader => new ServizioAggViewModel
                    {
                        ServizioAgg = reader.GetString(reader.GetOrdinal("Descrizione")),
                        DataServizio = reader.IsDBNull(reader.GetOrdinal("Data")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Data")),
                        Quantita = reader.IsDBNull(reader.GetOrdinal("Quantita")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Quantita")),
                        Prezzo = reader.IsDBNull(reader.GetOrdinal("Prezzo")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Prezzo"))
                    });

                model.ServiziAgg = serviziAgg;

                model.ImportoDaSaldare = await ExecuteScalarAsync<decimal>(
                    GET_IMPORTO,
                    cmd => cmd.Parameters.AddWithValue("@IdPrenotazione", idPrenotazione));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei dettagli di prenotazione per ID {idPrenotazione}", idPrenotazione);
                throw;
            }

            return model;
        }

        private async Task<List<T>> ExecuteReaderAsync<T>(string commandText, Action<SqlCommand> parameterize, Func<SqlDataReader, T> handleReader)
        {
            var results = new List<T>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(commandText, connection))
                {
                    parameterize(command);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(handleReader(reader));
                        }
                    }
                }
            }

            return results;
        }

        private async Task<T> ExecuteScalarAsync<T>(string commandText, Action<SqlCommand> parameterize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(commandText, connection))
                {
                    parameterize(command);
                    var result = await command.ExecuteScalarAsync();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
        }
    }
}