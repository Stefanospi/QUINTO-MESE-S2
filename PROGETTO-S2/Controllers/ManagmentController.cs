using Microsoft.AspNetCore.Mvc;
using PROGETTO_S2.Models;
using PROGETTO_S2.Services;

namespace PROGETTO_S2.Controllers
{

    [Route("Managment/[controller]")]
    public class ManagmentController : Controller
    {
        private readonly IPrenotazioniService _prenotazioniService;
        private readonly ILogger<ManagmentController> _logger;
        private readonly IAggServizioService _aggServizioService;
        private readonly ICheckoutService _checkoutService;

        public ManagmentController(IPrenotazioniService prenotazioniService, ILogger<ManagmentController> logger, IAggServizioService aggServizioService, ICheckoutService checkoutService)
        {
            _prenotazioniService = prenotazioniService;
            _logger = logger;
            _aggServizioService = aggServizioService;
            _checkoutService = checkoutService;
        }

        [HttpGet("CercaPrenotazioni")]
        public IActionResult CercaPrenotazioni()
        {
            return View();
        }

        [HttpPost("CercaPrenotazioni")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CercaPrenotazioni(string CF)
        {
            if (string.IsNullOrWhiteSpace(CF))
            {
                ModelState.AddModelError("", "Il codice fiscale non può essere nullo o vuoto.");
                return View();
            }

            try
            {
                var prenotazioni = await _prenotazioniService.GetPrenotazioni(CF);

                if (prenotazioni != null && prenotazioni.Any())
                {
                    return Json(new { success = true, redirectUrl = Url.Action("RisultatiRicerca", new { CF }) });
                }
                else
                {
                    ModelState.AddModelError("", "Nessuna prenotazione trovata per il codice fiscale fornito.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la ricerca delle prenotazioni per il codice fiscale: {CF}", CF);
                return StatusCode(500, "Errore interno del server. Si prega di riprovare più tardi.");
            }
        }

        [HttpGet("RisultatiRicerca")]
        public async Task<IActionResult> RisultatiRicerca(string CF)
        {
            if (string.IsNullOrWhiteSpace(CF))
            {
                return RedirectToAction("CercaPrenotazioni");
            }

            try
            {
                var prenotazioni = await _prenotazioniService.GetPrenotazioni(CF);

                if (prenotazioni == null || !prenotazioni.Any())
                {
                    ViewBag.Message = "Nessuna prenotazione trovata per il codice fiscale fornito.";
                }

                return View(prenotazioni);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei risultati di ricerca per il codice fiscale: {CF}", CF);
                return StatusCode(500, "Errore interno del server. Si prega di riprovare più tardi.");
            }
        }
        [HttpGet("CercaPrenotazioniByTipo")]
        public IActionResult CercaPrenotazioniByTipo()
        {
            return View();
        }

        [HttpPost("CercaPrenotazioniByTipo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CercaPrenotazioniByTipo(string TipoPensione)
        {
            if (string.IsNullOrWhiteSpace(TipoPensione))
            {
                ModelState.AddModelError("", "Il Tipo pensione non può essere nullo o vuoto.");
                return View();
            }

            try
            {
                var prenotazioni = await _prenotazioniService.GetPrenotazioneByTipoPensione(TipoPensione);

                if (prenotazioni != null && prenotazioni.Any())
                {
                    return Json(new { success = true, redirectUrl = Url.Action("RisultatiRicercaByTipo", new { TipoPensione }) });
                }
                else
                {
                    ModelState.AddModelError("", "Nessuna prenotazione trovata per il tipo pensione fornito.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la ricerca delle prenotazioni per il tipo pensione: {TipoPensione}", TipoPensione);
                return StatusCode(500, "Errore interno del server. Si prega di riprovare più tardi.");
            }
        }

        [HttpGet("RisultatiRicercaByTipo")]
        public async Task<IActionResult> RisultatiRicercaByTipo(string TipoPensione)
        {
            if (string.IsNullOrWhiteSpace(TipoPensione))
            {
                return RedirectToAction("CercaPrenotazioniByTipo");
            }

            try
            {
                var prenotazioni = await _prenotazioniService.GetPrenotazioneByTipoPensione(TipoPensione);

                if (prenotazioni == null || !prenotazioni.Any())
                {
                    ViewBag.Message = "Nessuna prenotazione trovata per il tipo pensione fornito.";
                }

                return View(prenotazioni);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei risultati di ricerca per il tipo pensione: {TipoPensione}", TipoPensione);
                return StatusCode(500, "Errore interno del server. Si prega di riprovare più tardi.");
            }
        }
        [HttpGet("AddServizioAgg")]
        public IActionResult AddServizioAgg(int idPrenotazione)
        {
            var model = new PrenotazioneServizioAgg
            {
                IdPrenotazione = idPrenotazione
            };
            return View(model);
        }

        [HttpPost("AddServizioAgg")]
        [ValidateAntiForgeryToken]
        public IActionResult AddServizioAgg(int idPrenotazione, PrenotazioneServizioAgg prenotazioneServizioAgg)
        {
            if (ModelState.IsValid)
            {
                var result = _aggServizioService.AddServizioAgg(prenotazioneServizioAgg, idPrenotazione);
                if (result != null)
                {
                    return RedirectToAction("CheckoutPrenotazione", new { idPrenotazione });
                }
                else
                {
                    ModelState.AddModelError("", "Errore durante l'inserimento del servizio aggiuntivo.");
                }
            }

            return View(prenotazioneServizioAgg);
        }

        [HttpGet("CheckoutPrenotazione")]
        public async Task<IActionResult> CheckoutPrenotazione(int idPrenotazione)
        {
            var prenotazione = await _checkoutService.GetPrenotazioneConImportoDaSaldare(idPrenotazione);
            return View("CheckoutPrenotazioni", prenotazione);
        }
    }

 }
