$(document).ready(function () {

  $('#FFC').on('change', function () {

    var checked = $(this).is(':checked');

    if (checked === true) {
      $('.fcc').show('fast');
    } else {
      $('.fcc').hide('fast');
    }
  });
});