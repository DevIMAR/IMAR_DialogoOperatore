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

	function trySearch(target, methodName) {
		var now = Date.now();
		if (now - lastSearchTime < DEBOUNCE_MS) return;

		var wrapper = target.closest('.' + (methodName === 'OnBollaSearch' ? 'bolla-input' : 'odp-input'));
		if (!wrapper) return;

		var inputEl = wrapper.querySelector('input');
		if (!inputEl) return;

		var value = inputEl.value.replace(/[_ ]/g, '').trim();
		if (!value) return;

		lastSearchTime = now;
		dotNetRef.invokeMethodAsync(methodName, value);
	}

	// Capture phase: intercetta prima che DevExpress possa bloccare
	document.addEventListener('keydown', function (e) {
		if (e.key !== 'Enter' && e.key !== 'Tab') return;

		if (e.target.closest('.bolla-input')) {
			trySearch(e.target, 'OnBollaSearch');
		} else if (e.target.closest('.odp-input')) {
			trySearch(e.target, 'OnOdpSearch');
		}
	}, true);

	document.addEventListener('focusout', function (e) {
		if (e.target.closest('.bolla-input')) {
			trySearch(e.target, 'OnBollaSearch');
		} else if (e.target.closest('.odp-input')) {
			trySearch(e.target, 'OnOdpSearch');
		}
	}, true);
}
