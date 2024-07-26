namespace PROGETTO_S2.Models
{
    public class CheckoutModel
    {
        public int IdPrenotazione { get; set; }
        public int NumeroCamera { get; set; }
        public DateTime SoggiornoDal { get; set; }
        public DateTime SoggiornoAl { get; set; }
        public decimal Tariffa { get; set; }
        public decimal Caparra { get; set; }
        public string ServizioAggiuntivo { get; set; }
        public DateTime? DataServizio { get; set; }
        public int? Quantita { get; set; }
        public decimal? Prezzo { get; set; }
        public decimal ImportoDaSaldare { get; set; }
    }
}
