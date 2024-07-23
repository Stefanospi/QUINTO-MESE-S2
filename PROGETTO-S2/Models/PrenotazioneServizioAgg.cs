namespace PROGETTO_S2.Models
{
    public class PrenotazioneServizioAgg
    {
        public int IdPrenotazioneServizioAgg { get; set; }
        public int IdPrenotazione { get; set; }
        public int IdServizioAgg { get; set; }
        public DateTime Data { get; set; }
        public int Quantita { get; set; }
        public decimal Prezzo { get; set; }
    }
}
