namespace PROGETTO_S2.Models
{
    public class PrenotazioneServizioAgg
    {
        public int IdPrenotazione { get; set; }
        public int IdServizioAgg { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int Quantita { get; set; }
        public decimal Prezzo { get; set; }
    }
}
