using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace Payola.Intranet.Models.Binders
{
    public class LocalizedDefaultModelBinder : DefaultModelBinder
    {
        public LocalizedDefaultModelBinder ()
            : base ()
        {

        }

        protected override void SetProperty (ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            base.SetProperty (controllerContext, bindingContext, propertyDescriptor, value);

            string propertyName = CreateSubPropertyName (bindingContext.ModelName, propertyDescriptor.Name);
            ModelState modelState = bindingContext.ModelState[propertyName];

            // Find all errors that were caused by FormatException during conversion and replace it with custom message.
            // Some information may be found there: http://forums.asp.net/t/1512140.aspx
            foreach (ModelError error in new List<ModelError> (modelState.Errors))
            {
                if (String.IsNullOrEmpty (error.ErrorMessage) && error.Exception != null)
                {
                    for (Exception exception = error.Exception; exception != null; exception = exception.InnerException)
                    {
                        if (exception is FormatException)
                        {
                            modelState.Errors.Remove (error);
                            modelState.Errors.Add (Resources.Resources.PropertyValueInvalid);
                            break;
                        }
                    }
                }
            }
        }
    }
}