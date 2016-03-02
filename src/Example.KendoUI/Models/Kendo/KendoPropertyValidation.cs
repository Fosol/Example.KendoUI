using Example.KendoUI.Extensions;
using Microsoft.Extensions.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Example.KendoUI.Models
{
    /// <summary>
    /// <see cref="KendoPropertyValidation"/> class, provides a way to define validation rules for the property.
    /// </summary>
    public class KendoPropertyValidation
    {
        #region Properties
        /// <summary>
        /// get - Whether this property is required.
        /// </summary>
        public bool Required { get; }
        
        /// <summary>
        /// get - What the minimum value can be.
        /// </summary>
        public object Minimum { get; }

        /// <summary>
        /// get - What the maximum value can be.
        /// </summary>
        public object Maximum { get; }

        /// <summary>
        /// get - Whether this property is nullable.
        /// </summary>
        public bool IsNullable { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="KendoPropertyValidation"/> class.
        /// </summary>
        /// <param name="property"><see cref="ModelProperty"/> object.</param>
        public KendoPropertyValidation([NotNull] ModelProperty property)
        {
            var type = property.Property.PropertyType;
            this.Required = property.Required != null;
            this.IsNullable = type.IsClass || type.IsNullable();
            var range = property.Property.GetCustomAttribute<RangeAttribute>();
            this.Minimum = range?.Minimum;
            this.Maximum = range?.Maximum;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts this to a <see cref="JObject"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="JObject"/>.</returns>
        public JObject ToJObject()
        {
            var val = new JObject();
            val.Add("required", new JValue(this.Required));

            if (this.Minimum != null)
                val.Add("min", new JValue(this.Minimum));
            if (this.Maximum != null)
                val.Add("max", new JValue(this.Maximum));
            return val;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("{");
            sb.Append($"\"required\": {this.Required.ToString().ToLower()}");
            if (this.Minimum != null)
            {
                if (this.Minimum.GetType().IsValueType)
                    sb.Append($", \"min\": {this.Minimum}");
                else
                    sb.Append($", \"min\": \"{this.Minimum}\"");
            }
            if (this.Maximum != null)
            {
                if (this.Maximum.GetType().IsValueType)
                    sb.Append($", \"max\": {this.Maximum}");
                else
                    sb.Append($", \"max\": \"{this.Maximum}\"");
            }
            sb.Append("}");
            return sb.ToString();
        }
        #endregion
    }
}
