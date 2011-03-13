using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace TerseControllerTesting.Utils
{
    public class EmailAttribute : ValidationAttribute, IClientValidatable
    {

        private static readonly Regex Validator = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);
        private const string EmailErrorMessage = @"The {0} field should be a valid email address.";

        public override bool IsValid(object value)
        {
            var strValue = value as string;
            if (!string.IsNullOrEmpty(strValue) && !Validator.IsMatch(strValue))
            {
                return false;
            }
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(EmailErrorMessage, name);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = string.Format(EmailErrorMessage, metadata.DisplayName ?? metadata.PropertyName),
                ValidationType = "email"
            };
        }
    }
}