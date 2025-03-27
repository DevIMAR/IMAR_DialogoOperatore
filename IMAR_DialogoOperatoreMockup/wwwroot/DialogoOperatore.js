function FocusElement(elementId) {
	var element = document.querySelector(`#${elementId} td`);
	if (element) {
		element.setAttribute("tabindex", 0)
		element.focus();
	}
}
