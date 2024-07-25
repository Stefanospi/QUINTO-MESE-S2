//RICERCA PER CODICE FISCALE
$(document).ready(function () {
    $('#RicercaBYCF').submit(function (event) {
        event.preventDefault();

        var form = $(this);
        var url = form.attr('action');
        var data = form.serialize();

        console.log('URL:', url);
        console.log('Data:', data);

        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            success: function (response) {
                if (response.redirectUrl) {
                    window.location.href = response.redirectUrl;
                } else {
                    alert('Nessuna prenotazione trovata.');
                }
            },
            error: function (xhr, status, error) {
                console.error('Errore:', status, error);
                console.error('Dettagli:', xhr.responseText);
                alert('Si è verificato un errore. Per favore, riprova.');
            }
        });
    });
});

//RICERCA PER TIPO DI PENSIONE
$(document).ready(function () {
    $('#RicercaByTipo').submit(function (event) {
        event.preventDefault();

        var form = $(this);
        var url = form.attr('action');
        var data = form.serialize();

        console.log('URL:', url);
        console.log('Data:', data);

        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            success: function (response) {
                if (response.redirectUrl) {
                    window.location.href = response.redirectUrl;
                } else {
                    alert('Nessuna prenotazione trovata.');
                }
            },
            error: function (xhr, status, error) {
                console.error('Errore:', status, error);
                console.error('Dettagli:', xhr.responseText);
                alert('Si è verificato un errore. Per favore, riprova.');
            }
        });
    });
});