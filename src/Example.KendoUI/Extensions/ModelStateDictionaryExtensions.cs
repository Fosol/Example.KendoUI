using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.KendoUI.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static List<string> ToErrorMessage(this ModelStateDictionary modelState)
        {
            var em = new List<string>();
            foreach (var model_state in modelState.Where(ms => ms.Value.ValidationState == ModelValidationState.Invalid))
            {
                foreach (var error in model_state.Value.Errors)
                {
                    em.Add(error.ErrorMessage);
                }
            }
            return em;
        }
    }
}
