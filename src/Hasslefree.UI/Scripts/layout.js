var Hasslefree = (function ($) {

	var initialize = function () {

		var r = Math.floor(Math.random() * 100);

		$("form").attr("autocomplete", "nope-" + r);

		$("input[type='text']").each(function (i, e) {

			var observerHack = new MutationObserver(function () {
				observerHack.disconnect();

				$(e).attr("autocomplete", "nope-" + r);
			});

			observerHack.observe(e, {
				attributes: true,
				attributeFilter: ['autocomplete']
			});

		});

		if (window.location.pathname != '/account/login') {
			// prevent forms from auto submitting on all inputs
			$(document).on("keydown", "input", function (e) {
				if (e.which == 13) e.preventDefault();
			});
		}

		if ($('[google-address]').length > 0) {

			$('[google-address]').each(function (i, e) {

				var address = e;
				var prefix = address.id.replace('Address1', '');

				const addressOptions = {
					componentRestrictions: { country: "za" },
					types: ["address"]
				};

				const addressAutocomplete = new google.maps.places.Autocomplete(address, addressOptions);

				addressAutocomplete.addListener("place_changed", () => {

					const place = addressAutocomplete.getPlace();

					var streetNumber = place.address_components.find(x => x.types.indexOf('street_number') > -1).long_name;
					var streetName = place.address_components.find(x => x.types.indexOf('route') > -1).long_name;
					var suburb = place.address_components.find(x => x.types.indexOf('sublocality') > -1).long_name;
					var city = place.address_components.find(x => x.types.indexOf('locality') > -1).long_name;
					var province = place.address_components.find(x => x.types.indexOf('administrative_area_level_1') > -1).long_name;
					var postalCode = place.address_components.find(x => x.types.indexOf('postal_code') > -1).long_name;

					if (province === 'KwaZulu-Natal') province = 'KwaZulu Natal';

					$('#' + prefix + 'Address1').val(streetNumber + ' ' + streetName);
					$('#' + prefix + 'Address3').val(suburb);
					$('#' + prefix + 'AddressTown').val(city);
					$('#' + prefix + 'AddressCity').val(city);
					$('#' + prefix + 'AddressProvince').val(province);
					$('#' + prefix + 'AddressRegion').val(province);
					$('#' + prefix + 'AddressPostalCode').val(postalCode);
					$('#' + prefix + 'AddressCode').val(postalCode);
				});
			});
		}
	};

	var functions = {

		//Validate
		Validate: function (form, settings) {
			_valid = true;

			/*Settings*/
			_settings = $.extend({
				scrollLeft: 30,
				scrollTop: 100,
				validClass: "valid",
				invalidClass: "invalid",
				fadeDelay: 2500,
				scrollIntoView: true,
				validHandler: $.noop,
				invalidHandler: function (input, invalidClass, fadeDelay) {
					$(input).addClass(invalidClass);
					setTimeout(function () { $(input).removeClass(invalidClass); }, fadeDelay);
				}
			}, settings),

				/*Validators*/
				_validators = {
					required: function (value) {
						return (value != null) && (value.trim() != '');
					},

					email: function (value) {
						var pat = /\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b/gi;
						if ((value == null) || (value.trim() == '')) return true;
						return pat.test(value);
					},

					number: function (value) {
						if ((value == null) || (value.trim() == '')) return true;
						return $.isNumeric(value);
					},

					greater: function (value, greater) {
						if (!$.isNumeric(value)) return true;
						if (!$.isNumeric(greater)) return true;
						return parseFloat(value) >= parseFloat(greater);
					}
				}

			/*Validation*/
			var count = 0;
			$(form).find(':input,select,textarea').each(function () {

				if (!$(this).is(':visible')) return;

				var val = $(this).val();
				var validate = $(this).attr('data-validate');
				var required = $(this).attr('data-validate-required');
				var email = $(this).attr('data-validate-email');
				var number = $(this).attr('data-validate-number');
				var greater = $(this).attr('data-validate-greater-than');

				var valid = true;
				if (validate) {
					if (required) valid = valid && _validators.required(val);
					if (email) valid = valid && _validators.email(val);
					if (number) valid = valid && _validators.number(val);
					if (greater) valid = valid && _validators.greater(val, greater);
				}

				/*Invalid Input*/
				if (!valid) _settings.invalidHandler($(this), _settings.invalidClass, _settings.fadeDelay);

				//Scroll into view
				if (!valid && count == 0 && _settings.scrollIntoView) {
					count++;
					var $this = this;
					if ($(this).prop('tagName') == 'SELECT')
						$this = $(this).nextAll('div').find('a');

					var offset = $(this).offset();
					offset.left -= _settings.scrollLeft;
					offset.top -= _settings.scrollTop;

					$('html, body').animate({
						scrollTop: offset.top,
						scrollLeft: offset.left
					});
				}

				/*Invalid Form*/
				_valid = _valid && valid;
			});

			if (_valid) {
				//show loading indicator
				var loadingText = $(form).find('button[type=submit]').data('loading-text');
				$(form).find('button[type=submit]').html(loadingText);
				$(form).find('button[type=submit]').addClass('disabled');
			}

			return _valid;
		}
	};

	return {
		Initialize: initialize,
		Functions: functions
	};

})(jQuery);

$(function () {
	Hasslefree.Initialize();
});
