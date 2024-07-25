using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public IActionResult CreatePersona()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public IActionResult CreatePrenotazione(Prenotazione prenotazione)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine("Creating Prenotazione with data: " + JsonConvert.SerializeObject(prenotazione));
                    _creationService.CreatePrenotazione(prenotazione);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Errore durante la creazione della prenotazione: " + ex.Message);
                    Console.WriteLine("Errore durante la creazione della prenotazione: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("ModelState is not valid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("ModelState error: " + error.ErrorMessage);
                }
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