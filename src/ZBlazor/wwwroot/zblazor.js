var zb = {
	scrollToId: function(id, containerId) {
		var element = document.getElementById(id);
		var container = document.getElementById(containerId);

		if (element && container) {
			if (element.offsetTop < container.scrollTop) {
				container.scrollTop = element.offsetTop;
			} else {
				var offsetBottom = element.offsetTop + element.offsetHeight;
				var scrollBottom = container.scrollTop + container.offsetHeight;
				if (offsetBottom > scrollBottom) {
					container.scrollTop = offsetBottom - container.offsetHeight;
				}
			}
		}

	}


}
