$('[data-loading-text').on('click', function () {

	var btn = $(this);
	var html = btn.data('loading-text');
	btn.html(html);
	btn.addClass('disabled');
});

var r = Math.floor(Math.random() * 100);

$("form").attr("autocomplete", "nope-" + r);

$("input[type='text']").each(function (i, e) {

	console.log(e);

	var observerHack = new MutationObserver(function () {
		observerHack.disconnect();
		
		$(e).attr("autocomplete", "nope-" + r);
	});

	observerHack.observe(e, {
		attributes: true,
		attributeFilter: ['autocomplete']
	});

});