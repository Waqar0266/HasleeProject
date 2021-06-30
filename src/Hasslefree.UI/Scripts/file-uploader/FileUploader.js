var FileUploader = (function ($) {

    var uploadsDone = false;

    var functions = {

        Initialize: function (uniqueId) {

            var $downloadIds = $('#' + uniqueId);
            var url = '/documentupload';
            var $fileInput = $('#' + uniqueId + '_Uploader');

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

            $fileInput.closest('form').on('submit',
                function () {

                    if (!uploadsDone) swal({
                        title: 'Validation Error',
                        text: 'Please wait for all the uploads to complete!',
                        icon: 'warning',
                    });
                    else {
                        $fileInput.closest('form').find('button').html("<i class='fa fa-spinner fa-spin'></i> Saving...");
                        $fileInput.closest('form').find('button').addClass('disabled');
                    }

                    return uploadsDone;
                });

            function removeElement(arrayName, arrayElement) {
                for (var i = 0; i < arrayName.length; i++) {
                    if (arrayName[i] == arrayElement)
                        arrayName.splice(i, 1);
                }
            }

            function adjustIds(id) {
                if (id == 0) return;

                var downloadIds = $downloadIds.val().split(',').map(i => Number(i));
                //remove zeros
                removeElement(downloadIds, 0);
                downloadIds.push(id);
                $downloadIds.val(downloadIds.join(','));
            }

            $fileInput.on('fileuploaddone', function (e, data) {

                adjustIds(data.result.files[0].downloadId);

                var activeUploads = $fileInput.fileupload('active');
                if (activeUploads == 1) {
                    uploadsDone = true;
                }
            });

        }

    }

    return {
        Functions: functions
    }

})(jQuery);