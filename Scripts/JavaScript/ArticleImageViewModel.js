var m_articleImageMainModel;


//Main Model
function ArticleImageMainModel(id) {

    this.parent.Init.call(this, "ArticleImage",1,id);

    var edit = this.EditableItem;
        $('#fileupload').fileupload({
            dataType: "json",
            url: "/api/tempimage",
            progressInterval: 100,
            maxNumberOfFiles: 1,
            autoUpload: true,
            add: function (e, data) {
                var fileCount = parseInt(edit.FileCount());
                if (fileCount == 1) {
                    alert('Only one file is allowed.\nPlease remove the current file and upload a new file.');
                    return;
                }

                var randomID = Math.floor(Math.random() * 11);
                data.files[0].id = randomID;

                $('#filelistholder').removeClass('hide');
                data.context = $('<div id="' + randomID + '" />').html(data.files[0].name + '<a onclick="RemoveItem(' + randomID + ')" class="pull-right btn btn-xs btn-primary">Remove</a>').appendTo('#filelistholder');
                $('</div><div class="progress progress-striped active"><div class="progress-bar" style="width:0%"></div></div>').appendTo(data.context);

                $('#cmdFileUpload').addClass('hide');

                if (isIE() < 10 && navigator.appName == 'Microsoft Internet Explorer') {
                    $('#imgLoader').show();
                }

                var uploadErrors = [];
                var acceptFileTypes = /(\.|\/)(gif|jpe?g|png|pdf|eps|tiff|cdr|PDF|CDR|JPG|presentation|sheet|ms-excel|msword|document|EPS|TIFF)$/i;
                if (data.originalFiles[0]['type'].length && !acceptFileTypes.test(data.originalFiles[0]['type'])) {
                    uploadErrors.push('Not an accepted file type');
                }

                if (data.originalFiles[0]['size'] && data.originalFiles[0]['size'].toString().length && data.originalFiles[0]['size'] > 5000000)
                {
                    uploadErrors.push('Filesize is too big');
                }

                if (data.originalFiles[0]['type'].length == 0 && uploadErrors.length == 0) {
                    uploadErrors.push('Not an accepted file type');
                }

                if (uploadErrors.length > 0) {
                    alert(uploadErrors.join("\n"));

                    $('#' + data.files[0].id).remove();
                    $('#cmdFileUpload').removeClass('hide');
                    var fileCount = parseInt($('#FileCount').val());
                    fileCount--;
                    $('#FileCount').val(fileCount).change();

                } else {
                    data.submit();
                }
            },
            done: function (e, data) {
                data.context.html(data.files[0].name + '... <span class="label label-success"><i class="fa fa-check"></i> Completed</span><a onclick="RemoveItem(' + data.files[0].id + ')" class="pull-right btn btn-xs btn-primary"><i class="fa fa-trash-o"> Remove</a>');

                edit.TempID(data.result.ID);
                edit.Filename(data.files[0].name);
                edit.Data(null);

                var fileCount = parseInt(edit.FileCount());
                fileCount++;
                edit.FileCount(fileCount);
                $('#imgLoader').hide();

                $("#ArticleImageIDVal").val(data.result.ID);
            },
            progressall: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('#overallbar').css('width', progress + '%');
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                data.context.find('.progress-bar').css('width', progress + '%');
            }
        });

}

ArticleImageMainModel.InheritsFrom(MainModel);

ArticleImageMainModel.prototype.Save = function () {

    var unmapped = ko.mapping.toJSON(this.EditableItem);
    var id = this.EditableItem.GetPrimaryKey();

    if (id == 0)
        this.ServiceProxy.Post(this.PostUrl, unmapped);
    else
        this.ServiceProxy.Put(this.PutUrl.replace('{0}', id), unmapped);

    return true;
}

ArticleImageMainModel.prototype.HandleLoad = function (result) {

    this.parent.HandleLoad.call(this, result);

    if (result.ArticleImageID > 0) {
        $('#filelistholder').html('<div id=1><a href="/image.png?id=' + result.ArticleImageID + '"><img src="/image.png?id=' + result.ArticleImageID + '&type=big" /></a>... Completed<a onclick="RemoveItem(1)" class="pull-right"><i class="fa fa-trash-o"> Remove</a></div>');
        $('#filelistholder').removeClass('hide');
        $('#cmdFileUpload').addClass('hide');
        this.EditableItem.FileCount(1);
    }
    else {
        $('#filelistholder').html('');
        $('#filelistholder').addClass('hide');
        $('#cmdFileUpload').removeClass('hide');
        this.EditableItem.FileCount(0);
    }

}



function RemoveItem(id) {
    $('#' + id).remove();
    m_articleImageMainModel.EditableItem.FileCount(0);
    $('#filelistholder').html('');
    $('#filelistholder').addClass('hide');
    $('#cmdFileUpload').removeClass('hide');
}

function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
}
