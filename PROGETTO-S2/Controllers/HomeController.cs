using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROGETTO_S2.Models;
using PROGETTO_S2.Services;
using System.Diagnostics;

namespace PROGETTO_S2.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPrenotazioniService _prenotazioniService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CercaPrenotazioni(string CF)
        {
            {
                try
                {
                    if (!string.IsNullOrEmpty(CF))
                    {
                        var prenotazione = _prenotazioniService.GetPrenotazioni(CF);
                        return View("CercaPrenotazioni", prenotazione);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Inserisci un codice fiscale");
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Si è verificato un errore durante la ricerca della prenotazione. Riprova più tardi.");
                    _logger.LogError(ex, "Errore durante la ricerca della prenotazione.");
                    return RedirectToAction("Index");
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
