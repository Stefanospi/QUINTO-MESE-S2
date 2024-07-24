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
        private readonly ICreationService _creationService;

        public HomeController(ILogger<HomeController> logger, IPrenotazioniService prenotazioniService,ICreationService creationService)
        {
            _logger = logger;
            _prenotazioniService = prenotazioniService;
            _creationService = creationService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CercaPrenotazioni(string codiceFiscale)
        {
            try
            {
                if (!string.IsNullOrEmpty(codiceFiscale))
                {
                    var prenotazioni = await _prenotazioniService.GetPrenotazioni(codiceFiscale);
                    if (prenotazioni != null && prenotazioni.Any())
                    {
                        return View("CercaPrenotazioni", prenotazioni);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Nessuna prenotazione trovata.");
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Inserisci un codice fiscale.");
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
        [HttpPost]
        public IActionResult CercaPrenotazioniByTipo(string tipoPensione)
        {
            try
            {
                if (!string.IsNullOrEmpty(tipoPensione))
                {
                    var prenotazioni = _prenotazioniService.GetPrenotazioneByTipoPensione(tipoPensione);
                    if (prenotazioni != null && prenotazioni.Any())
                    {
                        return View("CercaPrenotazioniByTipo", prenotazioni);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Nessuna prenotazione trovata.");
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Inserisci un tipo di pensione.");
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
        public IActionResult CreatePersona()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreatePersona(Persona persona)
        {
            if (ModelState.IsValid)
            {
                _creationService.CreatePersona(persona);
                return RedirectToAction("Index");
            }
            return View(persona);
        }
        public IActionResult ElencoPersone()
        {
            var persona = _creationService.GetPersona();
            return View(persona);
        }
        public IActionResult CreatePrenotazione()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreatePrenotazione(Prenotazione prenotazione)
        {
            if (ModelState.IsValid)
            {
                _creationService.CreatePrenotazione(prenotazione);
                return RedirectToAction("Index");
            }
            return View(prenotazione);

        }
        public IActionResult ElencoPrenotazioni()
        {
            var prenotazione = _creationService.GetPrenotazione();
            return View(prenotazione);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}