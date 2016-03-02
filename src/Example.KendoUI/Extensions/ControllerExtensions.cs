using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.KendoUI.Extensions
{
    /// <summary>
    /// ControllerExtensions static class, provides extension methods for Controller objects.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Generates a fully qualified or absolute URL for an action method by using the specified action name, and controller name;
        /// </summary>
        /// <typeparam name="T">Type of Controller.</typeparam>
        /// <param name="controller">Controller object.</param>
        /// <param name="action">Name of the action.</param>
        /// <returns>A fully qualified or absolute URL.</returns>
        public static string Action<T>(this T controller, string action) where T : Controller
        {
            return controller.Action(action, null);
        }

        /// <summary>
        /// Generates a fully qualified or absolute URL for an action method by using the specified action name, controller name, and route values.
        /// </summary>
        /// <typeparam name="T">Type of Controller.</typeparam>
        /// <param name="controller">Controller object.</param>
        /// <param name="action">Name of the action.</param>
        /// <param name="values"></param>
        /// <returns>A fully qualified or absolute URL.</returns>
        public static string Action<T>(this T controller, string action, object values) where T : Controller
        {
            return controller.Url.Action(action, controller.GetName(), values);
        }

        /// <summary>
        /// Get the name of the controller for routing purposes.
        /// </summary>
        /// <typeparam name="T">Type of Controller.</typeparam>
        /// <param name="controller">Controller object.</param>
        /// <returns>The name of the Controller.</returns>
        public static string GetName<T>(this T controller) where T : Controller
        {
            return typeof(T).GetName();
        }

        /// <summary>
        /// Get the name of the controller for routing purposes.
        /// </summary>
        /// <param name="type">Controller Type.</param>
        /// <returns>The name of the Controller.</returns>
        public static string GetName(this Type type)
        {
            if (!typeof(Controller).IsAssignableFrom(type))
                throw new ArgumentException($"Parameter '{nameof(type)}' must be of Type Controller.");

            var name = type.Name;
            return name.EndsWith("Controller") ? name.Substring(0, name.Length - 10) : name;
        }
    }
}
