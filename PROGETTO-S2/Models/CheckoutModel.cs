namespace PROGETTO_S2.Models
{
    public class CheckoutModel
    {
        public int NumeroCamera { get; set; }
        public DateTime SoggiornoDal { get; set; }
        public DateTime SoggiornoAl { get; set; }
        public decimal Tariffa { get; set; }
        public decimal Caparra { get; set; }
        public List<ServizioAggViewModel> ServiziAgg { get; set; } = new List<ServizioAggViewModel>();
        public decimal ImportoDaSaldare { get; set; }
    }

    public class StanzaViewModel
    {
        public int NumeroCamera { get; set; }
        public DateTime SoggiornoDal { get; set; }
        public DateTime SoggiornoAl { get; set; }
        public decimal Tariffa { get; set; }
        public decimal Caparra { get; set; }
    }

    public class ServizioAggViewModel
    {
        public string ServizioAgg { get; set; }
        public DateTime? DataServizio { get; set; }
        public int? Quantita { get; set; }
        public decimal? Prezzo { get; set; }
    }

}