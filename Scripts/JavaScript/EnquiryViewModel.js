var m_enquiryViewModel;

$(
    function () {
        m_enquiryViewModel = new EnquiryViewModel();

    }
);

//Main Model
function EnquiryViewModel() {
    this.parent.Init.call(this, "Enquiry", 1, 0);


}

function Clear() {
    m_enquiryViewModel.EditableItem.Name('');
    m_enquiryViewModel.EditableItem.Email('');
    m_enquiryViewModel.EditableItem.TelephoneNumber('');
}

EnquiryViewModel.InheritsFrom(MainModel);

EnquiryViewModel.prototype.HandleAdd = function (result) {
    this.parent.HandleAdd.call(this, result);
    this.EditableItem[this.ModelName + "ID"](0);
    this.EditableItem.IsSaving(false);
    if (result) {
        TrackVirtualPageview("Enquiry Success", "/virtual/success");
        this.EditableItem.Email('')
        this.EditableItem.Name('');
        this.EditableItem.TelephoneNumber('');
        this.EditableItem.Subject('');
        this.EditableItem.Message('');
        this.EditableItem.ShowPopup(true);
        this.EditableItem.BranchID('');
    }
    else {
        this.EditableItem.ShowErrorPopup(true);
        TrackVirtualPageview("Enquiry Failed", "/virtual/failed");
    }

}

EnquiryViewModel.prototype.HandleLoad = function (result) {
    result.Subscribetonewsletter = result.Subscribe;
    result.OwnerCategoryID = result.EnquiryOwnerCategoryID;
    //get BranchID from HTML with jquery
    var tmpBranchID = parseInt($("#BranchID").val());

    this.parent.HandleLoad.call(this, result);

    //Reasign stored value
    this.EditableItem.BranchID(tmpBranchID);
}

EnquiryViewModel.prototype.Save = function () {
    TrackVirtualPageview("Enquiry Attempt", "/virtual/attempt");

    var form = $(this.EditableItem.GetDomElement()).find('form').first();
    form.removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);

    form.data('validator').settings.ignore = "";
    form.validate();

    if (!form.valid()) {
        form.data('validator').focusInvalid();
        return;
    }

    this.EditableItem.Subscribe = this.EditableItem.Subscribetonewsletter;
    this.EditableItem.EnquiryOwnerCategoryID = this.EditableItem.OwnerCategoryID;

    var unmapped = ko.mapping.toJSON(this.EditableItem);

    var id = this.EditableItem.GetPrimaryKey();
    this.EditableItem.IsSaving(true);
    if (id == 0)
        this.ServiceProxy.Post(this.PostUrl, unmapped);
    else
        this.ServiceProxy.Put(this.PutUrl.replace('{0}', id), unmapped);




}




EnquiryViewModel.prototype.InstanceOfEditModel = function () {
    return new EnquiryEditModel(this);
}

//Edit Model
function EnquiryEditModel(main) {
    this.parent.Init.call(this, main);

    this.ShowPopup = ko.observable(false);
    this.ShowErrorPopup = ko.observable(false);
}

function TrackVirtualPageview(event, url) {

    var branch = $("#BranchTitle").val();

    if (branch)
        url += "/" + branch;

    ga('send', {
        'hitType': 'pageview',
        'page': window.location.href + url,
        'title': event
    });

}

EnquiryEditModel.InheritsFrom(EditModel);
