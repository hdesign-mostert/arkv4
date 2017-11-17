var m_articleCategoryMainModel;
var m_articleCategorySelectModel;

$(
    function () {
        m_articleCategoryMainModel = new ArticleCategoryMainModel();
        m_articleCategorySelectModel = new ArticleCategorySelectModel();

        ko.applyBindings(m_articleCategorySelectModel, document.getElementById("ReselectCategory"));
    }
);

//Main Model
function ArticleCategoryMainModel() {
    this.parent.Init.call(this, "ArticleCategory");


}

ArticleCategoryMainModel.InheritsFrom(MainModel);

ArticleCategoryMainModel.prototype.Delete = function (result) {

    if (m_articleCategoryMainModel.ListModel.GetRowModel(result).ArticleCount() > 0) {
        m_articleCategorySelectModel.Name(m_articleCategoryMainModel.ListModel.GetRowModel(result).Name());
        m_articleCategorySelectModel.ID(result);
        m_articleCategorySelectModel.Show(true);
        $("#ReselectCategory").modal();
    }
    else
        this.parent.Delete.call(this, result);

}

//Category Select Model
function ArticleCategorySelectModel() {
    this.Show = ko.observable(false);
    this.Name = ko.observable("");
    this.ID = ko.observable(0);

    this.Cancel = function () {
        $("#ReselectCategory").modal('show');
    }

    this.Save = function (id) {

        if (confirm("Are you sure you want to move articles to category")) {
            $.post("../api/articleCategory/"+m_articleCategorySelectModel.ID()+"/"+id, id, function (returnedData) {
                $("#ReselectCategory").modal('hide');
            })
        }
        else {
        }
    }
}





