using PROGETTO_S2.Models;
namespace PROGETTO_S2.Services
{
    public interface IAuthService
    {
        Utente Login(string username, string password);
        Utente Register(string username, string password);
    }
}
