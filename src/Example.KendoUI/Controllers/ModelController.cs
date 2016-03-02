using Example.KendoUI.ActionResults;
using Example.KendoUI.Models;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.KendoUI.Controllers
{
    [Route("[controller]")]
    public class ModelController : Controller
    {
        #region Constructors
        #endregion

        #region Methods

        /// <summary>
        /// Returns the Kendo UI model information.
        /// </summary>
        /// <param name="model">string</param>
        /// <returns>validation type</returns>
        [HttpGet("kendo/{model}")]
        public IActionResult GetKendoModel(string model)
        {
            var type = Type.GetType(model, false, true);

            if (type == null)
            {
                type = GetModelType(model);

                if (type == null)
                    return new BadRequestResult();
            }

            return GetKendoModel(type);
        }

        /// <summary>
        /// Returns the Kendo UI model information.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IActionResult GetKendoModel(Type type)
        {
            var result = new KendoModel(type);

            return new ApiJsonResult(result.ToJObject());
        }

        /// <summary>
        /// Get the model type for the specified name.
        /// </summary>
        /// <param name="typeName">The name of the model type.</param>
        /// <returns>They <see cref="Type"/> object.</returns>
        private Type GetModelType(string typeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.InvariantCultureIgnoreCase) || t.FullName.Equals(typeName, StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion
    }
}
