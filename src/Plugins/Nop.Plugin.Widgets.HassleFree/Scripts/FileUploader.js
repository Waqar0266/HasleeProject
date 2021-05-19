var uploadsDone = false;

$(document).ready(function () {

  var url = '/documents/upload';
  var $fileInput = $('#' + uniqueId);

  // Initialize the jQuery File Upload widget:
  $fileInput.fileupload({
    // Setup Doka Image Editor:
    doka: window.Doka && Doka.create(),
    edit:
      window.Doka &&
      Doka.supported() &&
      function (file) {
        return this.doka.edit(file).then(function (output) {
          return output && output.file;
        });
      },
    // Uncomment the following to send cross-domain cookies:
    //xhrFields: {withCredentials: true},
    url: url,
    autoUpload: true,
    singleFileUploads: true
  });

  $fileInput.on('submit',
    function () {
      return uploadsDone;
    });

  $fileInput.on('fileuploaddone', function (e, data) {

    var activeUploads = $fileInput.fileupload('active');
    if (activeUploads == 1) {
      uploadsDone = true;
    }
  });

  // Load existing files:
  $fileInput.addClass('fileupload-processing');
  $.ajax({
    url: $fileInput.fileupload('option', 'url'),
    dataType: 'json',
    context: $fileInput[0]
  })
    .always(function () {
      $(this).removeClass('fileupload-processing');
    })
    .done(function (result) {
      console.log('finished');
      $(this)
        .fileupload('option', 'done')
        // eslint-disable-next-line new-cap
        .call(this, $.Event('done'), { result: result });
    });

})