using Ark.Controllers.Helpers;
using Ark.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Ark
{
    public enum ControlType { Textbox, Textarea, Email, Phone, RadioButtonBool, Image, RichText, Checkbox }

    public static class HtmlExtensions
    {
        public static FormContainerControl CurrentContainer
        {
            get
            {
                object item = HttpContext.Current.Items["FormContainer"];

                return item as FormContainerControl;
            }
            set
            {
                HttpContext.Current.Items["FormContainer"] = value;
            }
        }


        #region CustomForms

        public static ModelMetadata GetModelData<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
            where TModel : class
        {
            return ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, helper.ViewData);
        }

        public static string GetValueFromModelData<TModel, TValue>(ModelMetadata modelData, Expression<Func<TModel, TValue>> expression)
            where TModel : class
        {
            if (modelData.Container == null)
                return "";

            var val = expression.Compile()(modelData.Container as TModel);
            if (val == null)
                return "";

            return val.ToString();
        }

        #region GetControlFor

        #region Textbox

        public static FormTextControl GetControlFor(ModelMetadata modelData, string value, ControlType type, bool MustBind, params HtmlAttribute[] attributes)
        {
            FormTextControl control = new FormTextControl();
            control.Label = modelData.DisplayName ?? modelData.PropertyName;

            if (value != null)
                control.Value = value.ToString();
            else
                control.Value = "";

            if (type == ControlType.Textbox)
                control.Type = "text";
            else if (type == ControlType.Email)
                control.Type = "email";
            else if (type == ControlType.Phone)
                control.Type = "tel";
            else if (type == ControlType.Textarea)
                control.Type = "";

            control.Attributes = attributes.ToList();

            if (CurrentContainer != null && CurrentContainer.UseKnockout && MustBind)
            {
                control.Attributes.Add(new HtmlAttribute("data-bind", "value: " + control.VariableName));
            }

            return control;
        }

        public static FormTextControl GetControlFor(ModelMetadata modelData, string value, ControlType type, params HtmlAttribute[] attributes)
        {
            return GetControlFor(modelData, value, type, true, attributes);
        }

        #endregion

        #region Dropdown

        public static FormDropdownControl GetControlFor(ModelMetadata modelData, IEnumerable<SelectListItem> values, bool MustBind, params HtmlAttribute[] attributes)
        {
            return GetControlFor(modelData, values, MustBind, null, null, attributes);
        }

        public static FormDropdownControl GetControlFor(ModelMetadata modelData, IEnumerable<SelectListItem> values, bool MustBind, string id, string className, params HtmlAttribute[] attributes)
        {
            FormDropdownControl control = new FormDropdownControl();
            control.Label = modelData.DisplayName ?? modelData.PropertyName;
            control.Items = values;

            control.Attributes = attributes.ToList();

            if (CurrentContainer != null && CurrentContainer.UseKnockout && MustBind)
            {
                control.Attributes.Add(new HtmlAttribute("data-bind", "options: " + control.VariableName + "Items"));
                control.Attributes.Add(new HtmlAttribute("data-bind", "optionsText: '" + control.VariableName + "Name'"));
                control.Attributes.Add(new HtmlAttribute("data-bind", "optionsValue: '" + control.VariableName + "ID'"));
                control.Attributes.Add(new HtmlAttribute("data-bind", "value: " + control.VariableName + "ID"));
            }
            else if (CurrentContainer != null && CurrentContainer.UseKnockout && !MustBind)
            {
                control.Attributes.Add(new HtmlAttribute("data-bind", "value: " + control.VariableName + "ID"));
            }

            if (!string.IsNullOrEmpty(id))
            {
                control.Attributes.Add(new HtmlAttribute("id", id));
            }

            if (!string.IsNullOrEmpty(className))
            {
                control.Attributes.Add(new HtmlAttribute("class", className));
            }

            return control;
        }

        public static FormDropdownControl GetControlFor(ModelMetadata modelData, IEnumerable<SelectListItem> values, params HtmlAttribute[] attributes)
        {
            return GetControlFor(modelData, values, true, attributes);
        }

        #endregion

        #endregion

        //For custom templates
        public static MvcHtmlString FormControlFor(this HtmlHelper helper,
            string path, FormControl model,
            params HtmlAttribute[] attributes)
        {
            if (string.IsNullOrEmpty(model.Label))
                throw new Exception("Value: Label cannot be empty");

            if (CurrentContainer != null && CurrentContainer.UseKnockout)
                model.Attributes.Add(new HtmlAttribute("data-bind", "value: " + model.VariableName));

            return helper.Partial("EditorTemplates/" + path, model);
        }


        public static HtmlAttribute Attribute(this HtmlHelper helper, string Name, string Value)
        {
            return new HtmlAttribute(Name, Value);
        }


        public static MvcHtmlString StartForm(this HtmlHelper helper, FormContainerControl container)
        {
            CurrentContainer = container;
            return helper.Partial("EditorTemplates/Default/StartForm", container);
        }

        public static MvcHtmlString StopForm(this HtmlHelper helper)
        {
            CurrentContainer = null;
            return new MvcHtmlString("</form>");
        }

        //For textboxes
        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            string folder,
            ControlType type,
            bool MustBind,
            params HtmlAttribute[] attributes)
        where TModel : class
        {
            return FormControlFor(helper, expression, folder, type, MustBind, null, null, null, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            string folder,
            ControlType type,
            bool MustBind,
            string id = null,
            string cssClass = null,
            string displayName = null,
            params HtmlAttribute[] attributes)
        where TModel : class
        {
            ModelMetadata modelData = GetModelData(helper, expression);
            string value = GetValueFromModelData(modelData, expression);

            FormTextControl control = GetControlFor(modelData, value, type, MustBind, attributes);

            if (!string.IsNullOrEmpty(displayName))
                control.DisplayName = displayName;

            control.ID = id;
            control.CssClass = cssClass;

            control.Validation = helper.ValidationMessageFor<TModel, TValue>(expression);
            ModelMetadata modelMeta = helper.GetModelData<TModel, TValue>(expression);
            control.UnobtrusiveValidators = helper.GetUnobtrusiveValidationAttributes(control.VariableName, modelMeta);
            control.HtmlName = GetHtmlNameFromExpression(expression);

            if (type == ControlType.Textarea)
                return helper.Partial("EditorTemplates/" + folder + "/Textarea", control);
            else if (type == ControlType.RadioButtonBool)
                return helper.Partial("EditorTemplates/" + folder + "/RadioButtonBit", control);
            else if (type == ControlType.Image)
                return helper.Partial("EditorTemplates/" + folder + "/Image", control);
            else if (type == ControlType.RichText)
                return helper.Partial("EditorTemplates/" + folder + "/RichText", control);
            else if (type == ControlType.Checkbox)
                return helper.Partial("EditorTemplates/" + folder + "/Checkbox", control);
            else
                return helper.Partial("EditorTemplates/" + folder + "/Textbox", control);
        }

        private static string GetHtmlNameFromExpression<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            try
            {
                return expression.Body.ToString().TrimStart(expression.Parameters[0].ToString()[0], '.');
            }
            catch
            {
                return "";
            }
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, ControlType type, bool MustBind, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, "Default", type, MustBind, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, ControlType type, bool MustBind, string id = null, string cssClass = null, string displayName = null, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, "Default", type, MustBind, id, cssClass, displayName, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string folder, ControlType type, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, folder, type, true, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, ControlType type, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, type, true, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, ControlType type, string id = null, string cssClass = null, string displayName = null, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, type, true, id, cssClass, displayName, attributes);
        }

        //For dropdowns
        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            string folder,
            IEnumerable<SelectListItem> items,
            bool MustBind,
            params HtmlAttribute[] attributes)
            where TModel : class
        {
            return FormControlFor(helper, expression, folder, items, MustBind, null, null, null, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            string folder,
            IEnumerable<SelectListItem> items,
            bool MustBind,
            string id,
            string cssClass,
            string displayName,
            params HtmlAttribute[] attributes)
            where TModel : class
        {
            ModelMetadata modelData = GetModelData(helper, expression);

            FormDropdownControl control = GetControlFor(modelData, items, MustBind, id, cssClass, attributes);

            if (!string.IsNullOrEmpty(displayName))
                control.DisplayName = displayName;

            control.Validation = helper.ValidationMessageFor<TModel, TValue>(expression);
            ModelMetadata modelMeta = helper.GetModelData<TModel, TValue>(expression);
            control.HtmlName = GetHtmlNameFromExpression(expression);
            control.UnobtrusiveValidators = helper.GetUnobtrusiveValidationAttributes(control.VariableName, modelMeta);

            return helper.Partial("EditorTemplates/" + folder + "/Dropdown", control);
        }

        //Custom dropdown
        public static MvcHtmlString FormCustomControlFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expression,
            string folder,
            IEnumerable<SelectListItem> items,
            int count,
            string message = "",
            params HtmlAttribute[] attributes)
            where TModel : class
        {
            ModelMetadata modelData = GetModelData(helper, expression);

            FormDropdownControl control = GetControlFor(modelData, items, false, attributes);
            control.Count = count;
            control.Validation = helper.ValidationMessageFor<TModel, TValue>(expression);
            ModelMetadata modelMeta = helper.GetModelData<TModel, TValue>(expression);
            control.HtmlName = GetHtmlNameFromExpression(expression);
            control.UnobtrusiveValidators = helper.GetUnobtrusiveValidationAttributes(control.VariableName, modelMeta);
            control.Message = message;
            control.Value = GetValueFromModelData(modelData, expression);
            return helper.Partial("EditorTemplates/" + folder + "/Dropdown", control);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> items, bool MustBind, string id, string cssClass, string displayName, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, "Default", items, MustBind, id, cssClass, displayName, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> items, bool MustBind, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, "Default", items, MustBind, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> items, string id = null, string cssClass = null, string displayName = null, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, items, true, id, cssClass, displayName, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> items, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, items, true, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string folder, IEnumerable<SelectListItem> items, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expression, folder, items, true, attributes);
        }

        //For textbox dropdown hybrids
        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expressionText,
            Expression<Func<TModel, TValue>> expressionDropdown,
            string folder,
            ControlType type,
            IEnumerable<SelectListItem> values,
            bool MustBindText,
            bool MustBindDropdown,
            params HtmlAttribute[] attributes)
        where TModel : class
        {
            ModelMetadata modelData = GetModelData(helper, expressionText);
            string value = GetValueFromModelData(modelData, expressionText);



            FormTextControl textControl = GetControlFor(modelData, value, type, MustBindText, attributes);
            FormDropdownControl dropdownControl = GetControlFor(modelData, values, MustBindDropdown, attributes);

            textControl.Validation = helper.ValidationMessageFor<TModel, TValue>(expressionText);
            textControl.UnobtrusiveValidators = helper.GetUnobtrusiveValidationAttributes(textControl.VariableName, modelData);
            textControl.HtmlName = GetHtmlNameFromExpression(expressionText);
            dropdownControl.HtmlName = GetHtmlNameFromExpression(expressionDropdown);

            return helper.Partial("EditorTemplates/" + folder + "/TextboxAndDropdown", new FormTextAndDropdownControl() { dropdownControl = dropdownControl, textControl = textControl });
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expressionText, Expression<Func<TModel, TValue>> expressionDropdown, ControlType type, IEnumerable<SelectListItem> values, bool MustBindText, bool MustBindDropdown, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expressionText, expressionDropdown, "Default", type, values, MustBindText, MustBindDropdown, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expressionText, Expression<Func<TModel, TValue>> expressionDropdown, string folder, ControlType type, IEnumerable<SelectListItem> values, bool MustBindDropdown, params HtmlAttribute[] attributes) where TModel : class
        {
            return FormControlFor(helper, expressionText, expressionDropdown, folder, type, values, true, MustBindDropdown, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expressionText,
            Expression<Func<TModel, TValue>> expressionDropdown,
            ControlType type, IEnumerable<SelectListItem> values,
            bool MustBindDropdown,
            params HtmlAttribute[] attributes)
        where TModel : class
        {
            return FormControlFor(helper, expressionText, expressionDropdown, type, values, true, MustBindDropdown, attributes);
        }

        public static MvcHtmlString FormControlFor<TModel, TValue>(this HtmlHelper<TModel> helper,
            Expression<Func<TModel, TValue>> expressionText,
            Expression<Func<TModel, TValue>> expressionDropdown,
            ControlType type, IEnumerable<SelectListItem> values,
            params HtmlAttribute[] attributes)
        where TModel : class
        {
            return FormControlFor(helper, expressionText, expressionDropdown, type, values, true, true, attributes);
        }


        #endregion


        public static MvcHtmlString NavLink(this HtmlHelper helper, string label, string url = "", string linkClass = "", string spanClass = "", string folder = "Default")
        {
            NavLinkControl control = new NavLinkControl();

            control.Label = new MvcHtmlString(label);

            if (string.IsNullOrEmpty(url))
                control.Href = new MvcHtmlString("/" + UrlBuilder.CleanUrl(label));
            else
                control.Href = new MvcHtmlString(url);

            control.LinkClass = new MvcHtmlString(linkClass);

            control.SpanClass = new MvcHtmlString(spanClass);

            string requestedUrl = HttpContext.Current.Request.RawUrl.ToLower().Trim('/');

            if (requestedUrl.Contains(control.Href.ToString().ToLower().Trim('/')))
                control.IsActive = true;
            else
                control.IsActive = false;

            return helper.Partial("EditorTemplates/" + folder + "/NavLink", control);
        }
    }
}
