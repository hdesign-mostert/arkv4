using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ark.Models
{
    public class RequiredIf: ValidationAttribute, IClientValidatable
    {
        private string dependentProperty;
        private object[] targetValue;

        public RequiredIf(string dependentProperty, params object[] targetValue)
        {
            this.dependentProperty = dependentProperty;
            this.targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Type containerType = validationContext.ObjectInstance.GetType();
            PropertyInfo field = containerType.GetProperty(this.dependentProperty);

            if(value!=null &&!string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            if (field == null)
                return new ValidationResult(this.ErrorMessage);

            object dependentValue = field.GetValue(validationContext.ObjectInstance);

            string dVal = null;

            if(dependentValue!=null)
                 dVal = dependentValue.ToString();

            if (string.IsNullOrEmpty(dVal))
                return new ValidationResult(this.ErrorMessage);

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "requiredif",
            };

            string depProp = BuildDependentPropertyId(metadata, context as ViewContext);

            // find the value on the control we depend on;
            // if it's a bool, format it javascript style
            // (the default is True or False!)

            StringBuilder sb = new StringBuilder();

            foreach (var obj in this.targetValue)
            {
                string targetValue = (obj ?? "").ToString();

                if (obj.GetType() == typeof(bool))
                    targetValue = targetValue.ToLower();

                sb.AppendFormat("|{0}", targetValue);
            }

            rule.ValidationParameters.Add("dependentproperty", depProp);
            rule.ValidationParameters.Add("targetvalue", sb.ToString().TrimStart('|'));

            yield return rule;
        }

        private string BuildDependentPropertyId(ModelMetadata metadata, ViewContext viewContext)
        {
            // build the ID of the property
            string depProp = viewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(this.dependentProperty);
            // unfortunately this will have the name of the current field appended to the beginning,
            // because the TemplateInfo's context has had this fieldname appended to it. Instead, we
            // want to get the context as though it was one level higher (i.e. outside the current property,
            // which is the containing object (our Person), and hence the same level as the dependent property.
            var thisField = metadata.PropertyName + "_";
            if (depProp.StartsWith(thisField))
                // strip it off again
                depProp = depProp.Substring(thisField.Length);
            return depProp;
        }
    }
}
