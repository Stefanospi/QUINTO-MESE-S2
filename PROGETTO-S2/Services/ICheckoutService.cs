using PROGETTO_S2.Models;


namespace PROGETTO_S2.Services
{
    public interface ICheckoutService
    {
       public Task<CheckoutModel> GetPrenotazioneConImportoDaSaldare(int idPrenotazione);
    }


}
