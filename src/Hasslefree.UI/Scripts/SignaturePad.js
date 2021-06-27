var SignaturePad = (function ($) {

	var functions = {

		Initialize: function (uniqueId) {

			var canvas = $('#' + uniqueId + '_element');
			window.addEventListener('orientationchange', function () {
				canvas.find('canvas').attr({
					height: canvas.height() - 25,
					width: canvas.width() - 5
				});
			}, false);
			window.addEventListener('resize', function () {
				canvas.find('canvas').attr({
					height: canvas.height() - 25,
					width: canvas.width() - 5
				});
			}, false);

			canvas.find('canvas').attr({
				height: canvas.height() - 25,
				width: canvas.width() - 5
			});

			canvas.signaturePad({
				drawOnly: true,
				defaultAction: 'drawIt',
				validateFields: false,
				lineWidth: 0,
				lineColour: 'black',
				penColour: 'black',
				sigNav: null,
				name: null,
				typed: null,
				typeIt: null,
				drawIt: null,
				typeItDesc: null,
				drawItDesc: null,
				onDrawEnd: function () {
					var instance = canvas.signaturePad();
					$('#' + uniqueId).val(instance.getSignatureImage());
				}
			});

		},

		Clear: function (uniqueId) {
			var canvas = $('#' + uniqueId + '_element');
			var instance = canvas.signaturePad();
			instance.clearCanvas();
			$('#' + uniqueId).val('');
		}
	}

	return {
		Functions: functions
	}

})(jQuery);