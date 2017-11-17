/*jslint unparam: true, regexp: true */
/*global window, $ */
$(function ()
{
    'use strict';
    // Change this to the location of your server-side upload handler:


    $('.fileinput').on('click', 'input', function ()
    {
        data.DomName = m_galleryImageMainModel.ModelName;

        $(this).fileupload({
            filesContainer: $('#GalleryImageList'),
            url: url,
            dataType: 'json',
            autoUpload: true,
            acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
            maxFileSize: 5000000, // 5 MB
            // Enable image resizing, except for Android and Opera,
            // which actually support image resizing, but fail to
            // send Blob objects via XHR requests:
            disableImageResize: /Android(?!.*Chrome)|Opera/
                .test(window.navigator.userAgent),
            previewMaxWidth: 280,
            previewMaxHeight: 280,
            previewCrop: true,
            formData: data,
            prependFiles: true,
            add: function(e, data) {
                var uploadErrors = [];
                var acceptFileTypes = /^image\/(gif|jpe?g|png)$/i;
                if(data.originalFiles[0]['type'].length && !acceptFileTypes.test(data.originalFiles[0]['type'])) {
                    uploadErrors.push('Not an accepted file type');
                }
                if(data.originalFiles[0]['size'].length && data.originalFiles[0]['size'] > 5000000) {
                    uploadErrors.push('Filesize is too big');
                }
                if(uploadErrors.length > 0) {
                    alert(uploadErrors.join("\n"));
                } else {
                    data.submit();
                }
            },
            process: [
            {
                action: 'load',
                fileTypes: /^image\/(gif|jpeg|png)$/,
                maxFileSize: 20000000 // 20MB
            },
            {
                action: 'resize',
                maxWidth: 1440,
                maxHeight: 900
            },
                add: function(e, data) {
                    var uploadErrors = [];
                    var acceptFileTypes = /^image\/(gif|jpe?g|png)$/i;
                    if(data.originalFiles[0]['type'].length && !acceptFileTypes.test(data.originalFiles[0]['type'])) {
                        uploadErrors.push('Not an accepted file type');
                    }
                    if(data.originalFiles[0]['size'].length && data.originalFiles[0]['size'] > 5000000) {
                        uploadErrors.push('Filesize is too big');
                    }
                    if(uploadErrors.length > 0) {
                        alert(uploadErrors.join("\n"));
                    } else {
                        data.submit();
                    }
                },
                            {
                                action: 'save'
                            }
    ]
        }).bind('fileuploaddone', function (e, data)
        {

        }).bind('fileuploadfail', function (e, data)
        {
            console.log(data.files[data.index]);
            //console.log(data);

        });
});
});
