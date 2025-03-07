var allowGeoRecall = true;
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

			SignaturePad.Functions.InitLocation(uniqueId);
		},

		Clear: function (uniqueId) {
			var canvas = $('#' + uniqueId + '_element');
			var instance = canvas.signaturePad();
			instance.clearCanvas();
			$('#' + uniqueId).val('');
		},

		InitLocation: function (uniqueId) {

			if (navigator.geolocation) {
				navigator.geolocation.getCurrentPosition(showPosition, positionError);
			} else {
				swal({
					title: 'Geolocation Error',
					text: 'Please allow this site for using your location.',
					icon: 'warning',
				});
			}

			function positionError() {
				swal({
					title: 'Geolocation Error',
					text: 'Please allow this site for using your location.',
					icon: 'warning',
				});

				if (allowGeoRecall) SignaturePad.Functions.InitLocation();
			}

			function showPosition(position) {
				$.ajax({
					url: '/location',
					data: {
						lat: position.coords.latitude,
						lng: position.coords.longitude
					}
				}).done(function (data) {
					var coordinatesModel = JSON.parse(data).data[0];
					var signedAt = coordinatesModel.county;
					$('[name=SignedAt' + uniqueId + ']').val(signedAt);
				});
			}

		}
	}

	return {
		Functions: functions
	}

})(jQuery);