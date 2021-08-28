$('[data-loading-text').on('click', function () {

	var btn = $(this);
	var html = btn.data('loading-text');
	btn.html(html);
	btn.addClass('disabled');
});

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

$(document).ready(function () {

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

				let streetNumber = place.address_components.find(x => x.types.indexOf('street_number') > -1).long_name;
				let streetName = place.address_components.find(x => x.types.indexOf('route') > -1).long_name;
				let suburb = place.address_components.find(x => x.types.indexOf('sublocality') > -1).long_name;
				let city = place.address_components.find(x => x.types.indexOf('locality') > -1).long_name;
				let province = place.address_components.find(x => x.types.indexOf('administrative_area_level_1') > -1).long_name;
				let postalCode = place.address_components.find(x => x.types.indexOf('postal_code') > -1).long_name;

				if (province === 'KwaZulu-Natal') province = 'KwaZulu Natal';

				$('#' + prefix + 'Address1').val(streetNumber + ' ' + streetName);
				$('#' + prefix + 'Address3').val(suburb);
				$('#' + prefix + 'AddressTown').val(city);
				$('#' + prefix + 'AddressCity').val(city);
				$('#' + prefix + 'AddressProvince').val(province);
				$('#' + prefix + 'AddressPostalCode').val(postalCode);
				$('#' + prefix + 'AddressCode').val(postalCode);
			});

		});

		
	}
});