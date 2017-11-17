var m_articleMainModel;
var i;
var editor;
$(
    function () {
        m_articleMainModel = new ArticleMainModel();

        ko.applyBindings(m_articleMainModel.ListModel, $('#ArticleList')[0]);

        var isAdmin = $("#isAdmin").attr('value');


        var toolbar =
            [
              ['style', ['bold', 'italic', 'underline', 'clear']],
              ['font', ['strikethrough']],
              ['fontsize', ['fontsize']],
              ['color', ['color']],
              ['para', ['ul', 'ol', 'paragraph']],
            ];


        if (isAdmin) {
            toolbar.push(["misc", ["codeview"]]);
        }


       editor=  $('.editor').summernote(
            {
                toolbar:toolbar,
                minHeight: null,             // set minimum height of editor
                maxHeight: null
            });

        $('div[id*=ArticleRow]').each(function (i, o) {
            var row = m_articleMainModel.InstanceOfRowModel();
            row.ArticleID = ko.observable($(o).attr('id').replace('ArticleRow-', ''));
            row.Author = ko.observable($(o).find("#Author").html());
            row.Category = ko.observable($(o).find("#Category").html());
            row.Content = ko.observable($(o).find("#Content").html());
            row.Summary = ko.observable($(o).find("#Summary").html());
            row.IsPublished = ko.observable($(o).find("#IsPublished").html());
            row.CreateDate = ko.observable($(o).find("#CreateDate").html());
            row.PublishDate = ko.observable($(o).find("#PublishDate").html());
            row.Subject = ko.observable($(o).find("#Subject").html());
            row.ReadableDate = ko.observable($(o).find("#ReadableDate").html());
            row.Published = ko.computed(function () {
                return row.IsPublished() == 'True';
            }, this);

            var category="";
            category = $("#category").attr("value");

            if (category != undefined) {
                if (category != [] && category != "")
                    category += "/";
            }
            else {
                category = "";
            }


            $('#ArticleRow-' + row.ArticleID() + ' #TogglePublishLink').attr('href', 'http://' + document.domain + '/Article/' + category + 'TogglePublish/' + row.ArticleID());

            row.Delete = function () {

                m_articleMainModel.ServiceProxy.Delete(m_articleMainModel.DeleteUrl.replace('{0}', row.ArticleID()));


            }

            m_articleMainModel.ListModel.List().push(row);


            ko.applyBindings(row, o);
        });
    }
);

//Main Model
function ArticleMainModel() {
    var self = this;

    this.parent.Init.call(this, "Article" , 2);


    $('input[data-control="date-picker"]').pickadate({
        format: 'yyyy/mm/dd',
        selectMonths: true,
        selectYears: true,
        container: 'body',
        onStart: function ()
        {
            this.set('select', new Date());
        }

    });

}

ArticleMainModel.InheritsFrom(MainModel);

ArticleMainModel.prototype.HandleLoad = function (result) {

    this.parent.HandleLoad.call(this, result);

    $('#publishdate').val(result.FormattedDate);
    $('#publishdate').pickadate('picker').set('select',
         [result.FormattedDate.substr(0, 4), ~~(result.FormattedDate.substr(5, 2))-1, result.FormattedDate.substr(8, 2)]);

    $('.note-editor').each(function (i, e)
    {
        if($(e).hasClass('codeview'))
        {
            $(e).find('button[data-event="codeview"]').click();
        }
    });


    $('[name=Tags]').importTags(result.Tags);

    $('[id*=_tagsinput]').remove();

    $('[name=Tags]').tagsInput({
        'width': '90%',
        'height': '70px',
        'defaultText': 'Add Tag',
         'delimiter':['|']
    });


    $("#content").code(result.Content);

    m_articleImageMainModel = new ArticleImageMainModel(result.ArticleID);

    $("#ArticleImageIDVal").val(m_articleImageMainModel.EditableItem.Manager.Id);
}

ArticleMainModel.prototype.Save = function () {

    var tags = [];
    $('[name=Tags]').next('div').find('span span').each(function (i, e)
    {
        tags.push($(e).text());
    });

    this.EditableItem.Tags = tags.join('|');

    this.EditableItem.Content($("#content").code());


    var edit = this.EditableItem;


    this.parent.Save.call(this);

}



ArticleMainModel.prototype.HandleUpdate = function () {
    var tags = [];
    $('[name=Tags]').next('div').find('span span').each(function (i, e)
    {
        tags.push($(e).text());
    });

    this.EditableItem.Tags = tags.join('|');

    $("#content").code("");
    this.parent.HandleUpdate.call(this);
    m_articleImageMainModel.EditableItem.ArticleID(this.EditableItem.ArticleID());
    m_articleImageMainModel.Save();

}

ArticleMainModel.prototype.HandleAdd = function (result) {


    $("#content").code("");
    this.parent.HandleAdd.call(this, result);

    m_articleImageMainModel.EditableItem.ArticleID(result.ArticleID);
    m_articleMainModel.ListModel.GetRowModel(result.ArticleID).Content(result.Content);
    m_articleImageMainModel.Save();
}

ArticleMainModel.prototype.HandleDelete = function (result) {

    this.parent.HandleAdd.call(this, result);
    $("#ArticleRow-" + result.ArticleID).remove();
}


//Edit model
var m_articleEditModel;


$(
    function () {
        m_articleEditModel = new ArticleEditModel();
    }
);


function ArticleEditModel(main) {

    this.parent.Init.call(this, main);

}

ArticleEditModel.InheritsFrom(EditModel);

ArticleEditModel.prototype.Save = function () {

    var self = this;

    var form = $(this.GetDomElement()).find('form');
    form.validate();

    if (!form.valid() )
        return;

    var unmapped = ko.mapping.toJSON(m_articleImageMainModel.EditableItem);
    var id = m_articleImageMainModel.EditableItem.GetPrimaryKey();

    if (id == 0)
        m_articleImageMainModel.ServiceProxy.Post(m_articleImageMainModel.PostUrl, unmapped);
    else
        m_articleImageMainModel.ServiceProxy.Put(m_articleImageMainModel.PutUrl.replace('{0}',id), unmapped);

}

//Row Model
function ArticleRowModel() {

    this.parent.Init.call(this);
}

ArticleRowModel.InheritsFrom(RowModel);





