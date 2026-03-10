function FocusElement(elementId) {
	var element = document.querySelector(`#${elementId} td`);
	if (element) {
		element.setAttribute("tabindex", 0)
		element.focus();
	}
}

// Event delegation sul document per intercettare Enter/Tab/FocusOut
// sugli input Bolla e ODP. Usa il document come target perché Blazor/DevExpress
// ricrea gli <input> dopo ogni StateHasChanged.
function setupSearchInputs(dotNetRef) {
	var lastSearchTime = 0;
	var DEBOUNCE_MS = 500;
	var pendingEnter = null; // timer per Enter differito (barcode scanner)

	function trySearch(cssClass, methodName) {
		var now = Date.now();
		if (now - lastSearchTime < DEBOUNCE_MS) return;

		var wrapper = document.querySelector('.' + cssClass);
		if (!wrapper) return;

		var inputEl = wrapper.querySelector('input');
		if (!inputEl) return;

		var value = inputEl.value.replace(/[_ ]/g, '').trim();
		if (!value) return;

		lastSearchTime = now;
		dotNetRef.invokeMethodAsync(methodName, value);
	}

	// Capture phase: intercetta prima che DevExpress possa bloccare.
	// Per Enter: ritarda di 150ms per dare tempo al DxMaskedInput di processare
	// tutti i caratteri dal barcode scanner, poi legge il valore aggiornato.
	document.addEventListener('keydown', function (e) {
		if (e.key !== 'Enter' && e.key !== 'Tab') return;

		var bollaTarget = e.target.closest('.bolla-input');
		var odpTarget = e.target.closest('.odp-input');

		if (!bollaTarget && !odpTarget) return;

		var cssClass = bollaTarget ? 'bolla-input' : 'odp-input';
		var method = bollaTarget ? 'OnBollaSearch' : 'OnOdpSearch';

		if (e.key === 'Enter') {
			e.preventDefault();
			e.stopPropagation();

			// Cancella eventuale timer precedente (barcode che manda Enter multipli)
			if (pendingEnter) clearTimeout(pendingEnter);

			// Ritarda la lettura del valore per dare tempo al masked input
			pendingEnter = setTimeout(function () {
				pendingEnter = null;
				trySearch(cssClass, method);
			}, 150);
		} else {
			// Tab: esegui subito
			trySearch(cssClass, method);
		}
	}, true);

	// Se arrivano caratteri mentre c'è un Enter pendente, rinvia il timer.
	// Questo gestisce il caso in cui il barcode scanner invia Enter
	// prima che tutti i caratteri siano stati processati dal masked input.
	document.addEventListener('input', function (e) {
		if (!pendingEnter) return;
		if (!e.target.closest('.bolla-input') && !e.target.closest('.odp-input')) return;

		// Caratteri ancora in arrivo: resetta il timer
		var bollaTarget = e.target.closest('.bolla-input');
		var cssClass = bollaTarget ? 'bolla-input' : 'odp-input';
		var method = bollaTarget ? 'OnBollaSearch' : 'OnOdpSearch';

		clearTimeout(pendingEnter);
		pendingEnter = setTimeout(function () {
			pendingEnter = null;
			trySearch(cssClass, method);
		}, 150);
	}, true);

	document.addEventListener('focusout', function (e) {
		if (e.target.closest('.bolla-input')) {
			trySearch('bolla-input', 'OnBollaSearch');
		} else if (e.target.closest('.odp-input')) {
			trySearch('odp-input', 'OnOdpSearch');
		}
	}, true);
}

// --- Speech-to-Text (Web Speech API) ---
var _speechRecognition = null;

function startSpeechRecognition(dotNetRef) {
	var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
	if (!SpeechRecognition) {
		dotNetRef.invokeMethodAsync('OnSpeechError', 'Speech recognition non supportato in questo browser');
		return;
	}

	_speechRecognition = new SpeechRecognition();
	_speechRecognition.lang = 'it-IT';
	_speechRecognition.continuous = true;
	_speechRecognition.interimResults = false;

	_speechRecognition.onresult = function (event) {
		var transcript = '';
		for (var i = event.resultIndex; i < event.results.length; i++) {
			if (event.results[i].isFinal) {
				transcript += event.results[i][0].transcript;
			}
		}
		if (transcript) {
			dotNetRef.invokeMethodAsync('OnSpeechResult', transcript);
		}
	};

	_speechRecognition.onerror = function (event) {
		dotNetRef.invokeMethodAsync('OnSpeechError', event.error);
	};

	_speechRecognition.onend = function () {
		dotNetRef.invokeMethodAsync('OnSpeechEnded');
	};

	_speechRecognition.start();
}

function stopSpeechRecognition() {
	if (_speechRecognition) {
		_speechRecognition.stop();
		_speechRecognition = null;
	}
}
