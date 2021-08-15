$(document).ready(function () {
	$('[data-loading-text').on('click', function () {

		var btn = $(this);
		var html = btn.data('loading-text');
		btn.html(html);
		btn.addClass('disabled');
	});


	try {
		var r = Math.floor(Math.random() * 100);

		$("form").attr("autocomplete", "nope-" + r);

		$("input[type='text']").each(function () {
			$(this).attr("autocomplete", "nope-" + r);
		});
	}
	catch (e) { }
});