
using PROGETTO_S2.Models;
using System.Data.SqlClient;

namespace PROGETTO_S2.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly string _connectionString;
        private readonly ILogger<CheckoutService> _logger;
        private const string GET_PRENOTAZIONE_CON_IMPORTO_DA_SALDARE_COMMAND = @"
       SELECT 
            p.IdPrenotazione,
            c.NumeroCamera,
            p.SoggiornoDal,
            p.SoggiornoAl,
            p.Tariffa,
            p.Caparra,
            sa.Descrizione AS ServizioAggiuntivo,
            psa.Data AS DataServizio,
            psa.Quantita,
            psa.Prezzo,
            (p.Tariffa - p.Caparra + COALESCE(SUM(psa.Prezzo * psa.Quantita), 0)) AS ImportoDaSaldare
        FROM 
            Prenotazioni p
        JOIN 
            Camere c ON p.IdCamera = c.IdCamera
        LEFT JOIN 
            PrenotazioniServiziAgg psa ON p.IdPrenotazione = psa.IdPrenotazione
        LEFT JOIN 
            ServiziAgg sa ON psa.IdServizioAgg = sa.IdServizioAgg
        WHERE 
            p.IdPrenotazione = @IdPrenotazione
        GROUP BY 
            p.IdPrenotazione, c.NumeroCamera, p.SoggiornoDal, p.SoggiornoAl, 
            p.Tariffa, p.Caparra, sa.Descrizione, psa.Data, psa.Quantita, psa.Prezzo
        ORDER BY 
            p.IdPrenotazione;";

        public CheckoutService(IConfiguration configuration, ILogger<CheckoutService> logger)
        {
            _connectionString = configuration.GetConnectionString("Authdb");
            _logger = logger;
        }

        public async Task<CheckoutModel> GetPrenotazioneConImportoDaSaldare(int idPrenotazione)
        {
            var prenotazione = new CheckoutModel();

            try
            {
                await ExecuteReaderAsync(GET_PRENOTAZIONE_CON_IMPORTO_DA_SALDARE_COMMAND,
                    command => command.Parameters.AddWithValue("@IdPrenotazione", idPrenotazione),
                    async reader =>
                    {
                        if (!await reader.ReadAsync())
                        {
                            return null;
                        }

                        prenotazione.IdPrenotazione = reader.GetInt32(reader.GetOrdinal("IdPrenotazione"));
                        prenotazione.NumeroCamera = reader.GetInt32(reader.GetOrdinal("NumeroCamera"));
                        prenotazione.SoggiornoDal = reader.GetDateTime(reader.GetOrdinal("SoggiornoDal"));
                        prenotazione.SoggiornoAl = reader.GetDateTime(reader.GetOrdinal("SoggiornoAl"));
                        prenotazione.Tariffa = reader.GetDecimal(reader.GetOrdinal("Tariffa"));
                        prenotazione.Caparra = reader.GetDecimal(reader.GetOrdinal("Caparra"));
                        prenotazione.ImportoDaSaldare = reader.GetDecimal(reader.GetOrdinal("ImportoDaSaldare"));

                        if (!reader.IsDBNull(reader.GetOrdinal("ServizioAggiuntivo")))
                        {
                            var servizio = new ServizioAggViewModel
                            {
                                ServizioAgg = reader.GetString(reader.GetOrdinal("ServizioAggiuntivo")),
                                DataServizio = reader.IsDBNull(reader.GetOrdinal("DataServizio")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DataServizio")),
                                Quantita = reader.IsDBNull(reader.GetOrdinal("Quantita")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Quantita")),
                                Prezzo = reader.IsDBNull(reader.GetOrdinal("Prezzo")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Prezzo"))
                            };

                            prenotazione.ServiziAgg.Add(servizio);
                        }

                        return prenotazione;
                    });

                if (prenotazione.ServiziAgg.Count == 0)
                {
                    _logger.LogInformation("No additional services found for reservation ID {idPrenotazione}", idPrenotazione);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reservation with outstanding amount for ID {idPrenotazione}", idPrenotazione);
            }

            return prenotazione;
        }

        private async Task ExecuteReaderAsync(string commandText, Action<SqlCommand> parameterize, Func<SqlDataReader, Task<CheckoutModel>> handleReader)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(commandText, connection))
                {
                    parameterize(command);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        await handleReader(reader);
                    }
                }
            }
        }
    }

    }
