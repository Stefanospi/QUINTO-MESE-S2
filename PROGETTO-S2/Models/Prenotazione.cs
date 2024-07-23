namespace PROGETTO_S2.Models
{
    public class Prenotazione
    {
        public int IdPrenotazione { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public int NumProgressivo { get; set; }
        public int Anno { get; set; }
        public DateTime SoggiornoDal { get; set; }
        public DateTime SoggiornoAl { get; set; }
        public decimal Caparra { get; set; }
        public decimal Tariffa { get; set; }
        public string TipoPensione { get; set; }
        public int IdPersona { get; set; }
        public int IdCamera { get; set; }
    }
}
