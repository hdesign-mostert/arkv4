//To ensure we dont bind beyond a certain point, incase we want to bind dom elements within dom elements to different models
ko.bindingHandlers.stopBinding = {
    init: function (element)
    {
        ko.cleanNode(element);
        return { controlsDescendantBindings: true };
    }
};


this.ServiceProxy = function (callback, error, context)
{
    var self = this;
    self.callback = callback;
    self.error = error;
    self.context = context;

    self.Ajax = function (url, method, data, methodName)
    {
        if (!methodName)
            methodName = method;

        return $.ajax(
            {
                url: url,
                type: method,
                data: data,
                datatype: "json",
                processData: false,
                contentType: "application/json; charset=utf-8",
                success: function (result)
                {
                    if (self.callback)
                        self.callback.call(context, result, methodName);
                },
                error: function (result)
                {
                    if (self.error)
                        self.error.call(context, result, methodName);
                }
            });
    }

    self.Post = function (url, data, methodName)
    {
        return self.Ajax(url, "POST", data, methodName);
    }

    self.Put = function (url, data, methodName)
    {
        return self.Ajax(url, "PUT", data, methodName);
    }

    self.Get = function (url, methodName)
    {
        return self.Ajax(url, "GET", null, methodName);
    }

    self.Delete = function (url, methodName)
    {
        return self.Ajax(url, "DELETE", null, methodName);
    }
}

//Clear validation
$.fn.resetValidation = function ()
{
    var form = this;

    //reset jQuery Validate's internals
    form.validate().resetForm();

    //reset unobtrusive field level, if it exists
    form.find("[data-valmsg-replace]")
        .removeClass("field-validation-error")
        .addClass("field-validation-valid")
        .empty();

    return form;
};

//Some OOP guidelines for JS
//http://phrogz.net/JS/classes/OOPinJS2.html
//http://phrogz.net/JS/classes/OOPinJS.html
function MainModel()
{
    this.InstanceAndInit = function (model)
    {
        var obj = new model();
        obj.Init(this);

        return obj;
    }
}

MainModel.prototype.Init = function (modelName, type, id)
{
    this.Type = (type == undefined) ? 0 : type;
    this.Id = (id == undefined) ? 0 : id;
    this.ModelName = modelName;
    this.BaseUrl = '/api/' + this.ModelName;
    this.SortUrl = '/api/' + this.ModelName + '/{0}/{1}';
    this.PostUrl = this.BaseUrl;
    this.PutUrl = this.BaseUrl + '/{0}';
    this.GetUrl = this.BaseUrl + '/{0}';
    this.DeleteUrl = this.BaseUrl + '/{0}';

    this.DomList = $('#' + this.ModelName + 'List');
    this.Template = this.ModelName + 'Template';

    this.EditableItem = this.InstanceOfEditModel();
    this.ListModel = this.InstanceOfListModel();
    this.PagingModel = this.InstanceOfPagingModel();
    this.ServiceProxy = new ServiceProxy(this.Success, this.Error, this);

    if (this.Type == 0)
        this.List();
    else if (this.Type == 1)
        this.Load(this.Id);
}

MainModel.prototype.List = function ()
{
    url = this.SortUrl.replace('{0}', this.ListModel.CurrentColumn).replace('{1}', this.PagingModel.CurrentPage);

    this.ServiceProxy.Get(url, "LIST");
}

MainModel.prototype.HandleList = function (result)
{
    var self = this;

    var mapping =
    {
        'List':
        {
            create: function (options)
            {
                return ko.mapping.fromJS(options.data, null, self.InstanceOfRowModel());
            }
        }
    }

    ko.mapping.fromJS(result, mapping, this.ListModel);
    ko.mapping.fromJS({ Total: result.Total }, null, this.PagingModel);

    var listDom = this.ListModel.GetDomElement();
    var pagingDom = this.PagingModel.GetDomElement();

    if (!ko.dataFor(listDom))
        ko.applyBindings(this.ListModel, listDom);

    if (!ko.dataFor(pagingDom))
        ko.applyBindings(this.PagingModel, pagingDom);

    var emptyItem = $(listDom).find('tr[id=' + this.ModelName + ']')[0];

    if (result.Total == 0 && emptyItem == undefined)
        $(listDom).append('<tr id="' + this.ModelName + '"><td>No items loaded</td></tr>');

    if (result.Total != 0 && emptyItem != undefined)
        $(emptyItem).remove();

    this.PagingModel.BuildPaging(0);
}

MainModel.prototype.Load = function (id)
{
    this.ServiceProxy.Get(this.GetUrl.replace('{0}', id));
}

MainModel.prototype.HandleLoad = function (result)
{
    ko.mapping.fromJS(result, {}, this.EditableItem);
    var editableDom = this.EditableItem.GetDomElement();

    if (!ko.dataFor(editableDom))
        ko.applyBindings(this.EditableItem, editableDom);

    var updatedItem = this.ListModel.GetRowModel(this.EditableItem.GetPrimaryKey());
    if (updatedItem != null)
        updatedItem.Reset();

    if (this.Type == 0 || this.Type == 2)
    {
        this.ListModel.IsLoading(false);
        this.EditableItem.Show();
    }
}

MainModel.prototype.Save = function ()
{
    var form = $(this.EditableItem.GetDomElement()).find('form').first();
    form.removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);

    form.data('validator').settings.ignore = "";
    form.validate();

    if (!form.valid())
    {
        form.data('validator').focusInvalid();
        return;
    }

    var unmapped = ko.mapping.toJSON(this.EditableItem);
    var id = this.EditableItem.GetPrimaryKey();

    if (id == 0)
        this.ServiceProxy.Post(this.PostUrl, unmapped);
    else
        this.ServiceProxy.Put(this.PutUrl.replace('{0}', id), unmapped);

    return true;
}

MainModel.prototype.HandleUpdate = function ()
{
    var updatedItem = this.ListModel.GetRowModel(this.EditableItem.GetPrimaryKey());

    ko.mapping.fromJS(ko.mapping.toJS(this.EditableItem), {}, updatedItem);

    this.EditableItem.Hide();
}

MainModel.prototype.HandleAdd = function (result)
{
    if (this.Type == 1)
    {
        ko.mapping.fromJS(result, {}, this.EditableItem);
        return;
    }

    var emptyItem = $(this.ListModel.GetDomElement()).find('tr[id=' + this.ModelName + ']')[0];

    if (emptyItem != undefined)
        $(emptyItem).remove();

    var self = this;
    var newItem = ko.mapping.fromJS(result, {}, this.InstanceOfRowModel());
    this.ListModel.List.push(newItem);

    this.EditableItem.Hide();
}

MainModel.prototype.Delete = function (id)
{
    if (confirm('Do you want to delete the ' + this.ModelName + ' ?'))
        this.ServiceProxy.Delete(this.DeleteUrl.replace('{0}', id));
}

MainModel.prototype.HandleDelete = function (result)
{
    var deletedItem = ko.mapping.fromJS(result, {}, this.InstanceOfRowModel());

    this.ListModel.List.remove(function (item) { return item.GetPrimaryKey() == deletedItem.GetPrimaryKey(); });
}

MainModel.prototype.Success = function (result, methodName)
{
    switch (methodName)
    {
        case "PUT":
            this.HandleUpdate(result);
            break;
        case "POST":
            this.HandleAdd(result);
            break;
        case "DELETE":
            this.HandleDelete(result);
            break;
        case "GET":
            this.HandleLoad(result);
            break;
        case "LIST":
            this.HandleList(result);
            break;
    }
}

MainModel.prototype.Error = function (result, methodName)
{
}

MainModel.prototype.InstanceOfListModel = function ()
{
    return this.InstanceAndInit(ListModel);
}

MainModel.prototype.InstanceOfPagingModel = function ()
{
    return this.InstanceAndInit(PagingModel);
}

MainModel.prototype.InstanceOfRowModel = function ()
{
    return this.InstanceAndInit(RowModel);
}

MainModel.prototype.InstanceOfEditModel = function ()
{
    return this.InstanceAndInit(EditModel);
}

//////////////////////////////
/////////////////////////////
ListModel = function ()
{
    this.IsLoading = ko.observable(false);
    this.List = ko.observableArray();
}

ListModel.prototype.Init = function (manager)
{
    this.Manager = manager;
    this.CurrentColumn = manager.ModelName + 'id';
}

ListModel.prototype.Add = function ()
{
    this.IsLoading(true);
    this.Manager.Load(0);
}

ListModel.prototype.Sort = function (column, page, model, evt)
{
    var dom = $(evt.target);
    var currentColumn = dom.html();

    var ascHtml = '<i class="fa fa-sort"></i>';
    var descHtml = '<i class="fa fa-sort"></i>';

    $(model.GetDomElement()).find('th a').each(function (i, o) { $(o).html($(o).html().replace(descHtml, '').replace(ascHtml, '')); });

    dom.html(column + ascHtml);
    column = column.replace(" desc", "");

    if (currentColumn.toLowerCase().indexOf("asc") > 0)
    {
        dom.html(column + descHtml);
        column += " desc";
    }

    this.CurrentColumn = column;
    this.Manager.List();
}

ListModel.prototype.GetDomID = function ()
{
    return this.Manager.ModelName + 'List';
}

ListModel.prototype.GetDomElement = function ()
{
    return $("#" + this.GetDomID())[0];
}

ListModel.prototype.GetRowModel = function (id)
{
    if (this.List == undefined)
        return null;

    var lst = this.List();
    for (var idx in lst)
    {

        if (lst[idx].GetPrimaryKey() == id)
            return lst[idx];

    }
}
//////////////////////////////
/////////////////////////////
PagingModel = function (manager)
{
    this.Init(manager);
}

PagingModel.prototype.Init = function (manager)
{
    this.Manager = manager;
    this.Rpp = 10;
    this.Paging = ko.observableArray();
    this.CurrentPage = 1;
}

PagingModel.prototype.Page = function (model, evt)
{
    var dom = $(evt.target);
    this.CurrentPage = parseInt(dom.html());

    this.Manager.List();
}

PagingModel.prototype.BuildPaging = function ()
{
    var offset = (this.CurrentPage * this.Rpp) - this.Rpp;
    var sb = [];
    var m_rpp = this.Rpp;

    if (this.Total() == 0 || this.Total() < m_rpp)
    {
        this.Paging('');
        return;
    }

    var start = offset;

    for (var a = 0; a < 2; a++)
        if ((start - m_rpp) >= 0)
            start -= m_rpp;

    var total = start + m_rpp * 5;

    if (total > this.Total())
    {
        start -= (total - (Math.ceil(this.Total() / 10) * 10));
        if (start < 0)
            start = 0;

        total = this.Total();
    }

    for (var a = start; a < total; a += m_rpp)
    {
        var page = { pageNumber: ((a / m_rpp) + 1), active: false };

        if (a == offset)
            page.active = true;

        sb.push(page);
    }

    var pageEnd = offset + m_rpp;
    if (pageEnd > this.Total())
        pageEnd = this.Total();

    this.Paging(sb);
}

PagingModel.prototype.GetDomID = function ()
{
    return this.Manager.ModelName + 'Paging';
}

PagingModel.prototype.GetDomElement = function ()
{
    return $("#" + this.GetDomID())[0];
}
//////////////////////////////
//////////////////////////////

RowModel = function ()
{
    this.Loading = ko.observable(false);
}

RowModel.prototype.Init = function (manager)
{
    this.Manager = manager;
}

RowModel.prototype.GetPrimaryKey = function ()
{
    return this[this.Manager.ModelName + "ID"]();
}

RowModel.prototype.GetDomID = function ()
{
    return this.Manager.ModelName + 'Row-' + this.GetPrimaryKey();
}

RowModel.prototype.GetDomElementType = function ()
{
    return 'tr';
}

RowModel.prototype.GetDomElement = function ()
{
    return $("#" + this.GetDomID())[0];
}

RowModel.prototype.Load = function ()
{
    this.Loading(true);

    this.Manager.Load(this.GetPrimaryKey());
}

RowModel.prototype.Reset = function (model, evt)
{
    this.Loading(false);
}

RowModel.prototype.View = function ()
{
    this.Manager.View(this.GetPrimaryKey());
}

RowModel.prototype.Delete = function ()
{
    this.Manager.Delete(this.GetPrimaryKey());
}

/////////////////////////////////
/////////////////////////////////

function EditModel()
{
    this.IsSaving = ko.observable(false);
}

EditModel.prototype.Init = function (manager)
{
    this.Manager = manager;
}

EditModel.prototype.GetPrimaryKey = function ()
{
    return this[this.Manager.ModelName + "ID"]();
}

EditModel.prototype.GetDomID = function ()
{
    return this.Manager.ModelName + 'Edit';
}

EditModel.prototype.GetDomElement = function ()
{
    return $('#' + this.GetDomID())[0];
}

EditModel.prototype.Save = function ()
{
    var state = this.Manager.Save(this.GetPrimaryKey());
    if (state)
        this.IsSaving(true);
    return state;
}

EditModel.prototype.Cancel = function ()
{
    this.Hide();
}

EditModel.prototype.Hide = function ()
{
    $('#' + this.GetDomID()).modal('hide');
    this.IsSaving(false);
}

EditModel.prototype.Show = function ()
{
    $('#' + this.GetDomID()).modal();
}
/////////////////////////////////
/////////////////////////////////

var root = domain = location.protocol + '//' + location.host;

function BuildApiUrl(url)
{
    return root + url;
}

function BuildApiUrl(url, param)
{
    return root + url + '/' + param;
}

Function.prototype.InheritsFrom = function (parentClassOrObject, param)
{
    this.prototype = new parentClassOrObject(param);
    this.prototype.constructor = this;
    this.prototype.parent = parentClassOrObject.prototype;

    return this;
}

//Example of extending the current functionality
/*
function EmployeeMainModel()
{
    this.ModelName = "Employee";
    this.parent.Init.call(this);
}

EmployeeMainModel.InheritsFrom(MainModel);

EmployeeMainModel.prototype.InstanceOfRowModel = function ()
{
    return new EmployeeRowModel(this);
}

*/
