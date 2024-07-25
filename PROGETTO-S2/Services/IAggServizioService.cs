using PROGETTO_S2.Models;

namespace PROGETTO_S2.Services
{
    public interface IAggServizioService
    {
        public PrenotazioneServizioAgg AddServizioAgg(PrenotazioneServizioAgg prenotazioneServizioAgg, int IdPrenotazione);
    }
}
