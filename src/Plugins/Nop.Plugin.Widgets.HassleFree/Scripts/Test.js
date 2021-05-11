$(document).ready(function () {

  $('#picker').on('change', function() {

    var url = $(this).val();

    if (url == '') {
      $('.page-body').html('');
      return;
    }

    $.ajax({
      type: 'GET',
      url: url,
      cache: false,
      dataType: 'html',
      success: function (result) {
        $('.page-body').html(result);
      }
    });
  });

  
});