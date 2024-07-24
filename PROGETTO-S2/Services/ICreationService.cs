using PROGETTO_S2.Models;
using System.Numerics;

namespace PROGETTO_S2.Services
{
    public interface ICreationService
    {
        public Persona CreatePersona(Persona persona);
        public Prenotazione CreatePrenotazione(Prenotazione prenotazione);
        public List<Persona> GetPersona();
        public List<Prenotazione> GetPrenotazione();
        public Prenotazione GetPrenotazioneById(int id);
    }
}
