var zb = {
	scrollToId: function(id) {
		var element = document.getElementById(id);

		if (element) {
			element.scrollIntoView(false);
		}
	}
}
