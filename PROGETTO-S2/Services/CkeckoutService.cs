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
            p.IdPrenotazione, c.NumeroCamera, p.SoggiornoDal, p.SoggiornoAl, p.Tariffa, p.Caparra, sa.Descrizione, psa.Data, psa.Quantita, psa.Prezzo
        ORDER BY 
            p.IdPrenotazione;";

        public CheckoutService(IConfiguration configuration, ILogger<CheckoutService> logger)
        {
            _connectionString = configuration.GetConnectionString("Authdb");
            _logger = logger;
        }

        public async Task<CheckoutModel> GetPrenotazioneConImportoDaSaldare(int idPrenotazione)
        {
            CheckoutModel prenotazione = null;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(GET_PRENOTAZIONE_CON_IMPORTO_DA_SALDARE_COMMAND, connection))
                    {
                        command.Parameters.AddWithValue("@IdPrenotazione", idPrenotazione);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                prenotazione = new CheckoutModel
                                {
                                    IdPrenotazione = reader.GetInt32(reader.GetOrdinal("IdPrenotazione")),
                                    NumeroCamera = reader.GetString(reader.GetOrdinal("NumeroCamera")),
                                    SoggiornoDal = reader.GetDateTime(reader.GetOrdinal("SoggiornoDal")),
                                    SoggiornoAl = reader.GetDateTime(reader.GetOrdinal("SoggiornoAl")),
                                    Tariffa = reader.GetDecimal(reader.GetOrdinal("Tariffa")),
                                    Caparra = reader.GetDecimal(reader.GetOrdinal("Caparra")),
                                    ServizioAggiuntivo = reader.IsDBNull(reader.GetOrdinal("ServizioAggiuntivo")) ? null : reader.GetString(reader.GetOrdinal("ServizioAggiuntivo")),
                                    DataServizio = reader.IsDBNull(reader.GetOrdinal("DataServizio")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DataServizio")),
                                    Quantita = reader.IsDBNull(reader.GetOrdinal("Quantita")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Quantita")),
                                    Prezzo = reader.IsDBNull(reader.GetOrdinal("Prezzo")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Prezzo")),
                                    ImportoDaSaldare = reader.GetDecimal(reader.GetOrdinal("ImportoDaSaldare"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della prenotazione con importo da saldare per ID {IdPrenotazione}", idPrenotazione);
            }

            return prenotazione;
        }
    }

    }
