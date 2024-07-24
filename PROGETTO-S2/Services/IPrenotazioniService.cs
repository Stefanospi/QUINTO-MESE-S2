using PROGETTO_S2.Models;

namespace PROGETTO_S2.Services
{
    public interface IPrenotazioniService
    {
        public Task<List<Prenotazione>> GetPrenotazioni(string CF);
        public List<Prenotazione> GetPrenotazioneByTipoPensione(string tipoPensione);
    }
}
