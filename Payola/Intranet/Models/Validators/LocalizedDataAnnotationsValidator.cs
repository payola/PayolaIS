using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Payola.Intranet.Models.Validators
{
    /// <summary>
    /// Validator that returns localized error messages.
    /// </summary>
    public class LocalizedDataAnnotationsValidator : DataAnnotationsModelValidator<ValidationAttribute>
    {
        public static void RegisterAdapters ()
        {
            Type adapterType = typeof (LocalizedDataAnnotationsValidator);
            DataAnnotationsModelValidatorProvider.RegisterAdapter (typeof (RequiredAttribute), adapterType);
            DataAnnotationsModelValidatorProvider.RegisterAdapter (typeof (DataTypeAttribute), adapterType);
            DataAnnotationsModelValidatorProvider.RegisterAdapter (typeof (MaxLengthAttribute), adapterType);
        }

        public LocalizedDataAnnotationsValidator (ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute)
            : base (metadata, context, attribute)
        {
            if (Attribute.ErrorMessageResourceType == null)
            {
                Attribute.ErrorMessageResourceType = typeof (Resources.Resources);
            }

            if (Attribute.ErrorMessageResourceName == null)
            {
                Attribute.ErrorMessageResourceName = Attribute.GetType ().Name + "ErrorMessage";
            }
        }
    }
}