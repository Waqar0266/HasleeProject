$(document).ready(function () {
	$('[data-loading-text').on('click', function () {

		var btn = $(this);
		var html = btn.data('loading-text');
		btn.html(html);

	});
});