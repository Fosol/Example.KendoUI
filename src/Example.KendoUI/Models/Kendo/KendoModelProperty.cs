using Example.KendoUI.Extensions;
using Microsoft.Extensions.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Example.KendoUI.Models
{
    /// <summary>
    /// <see cref="KendoModelProperty"/> class, provides a way to supply Kendo UI with field information for the given model.
    /// </summary>
    public class KendoModelProperty
    {
        #region Variables
        #endregion

        #region Properties
        /// <summary>
        /// get - The property name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// get - The property type.
        /// </summary>
        public string PropertyType { get; }

        /// <summary>
        /// get - Whether the property is editable.
        /// </summary>
        public bool IsEditable { get; }

        /// <summary>
        /// get - Whether the property is immutable after it has been set.
        /// </summary>
        public bool IsImmutable { get; }

        /// <summary>
        /// get - The default value of the property.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// get - Whether the property is nullable.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// get - Whether the DefaultValue is a function.
        /// </summary>
        public bool IsDefaultValueFunction { get; }

        /// <summary>
        /// get - The format string to apply to the value.
        /// </summary>
        public KendoPropertyFormat Format { get; }

        /// <summary>
        /// get - The validation rule.
        /// </summary>
        public KendoPropertyValidation Validation { get; }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of an <see cref="KendoModelProperty"/> object.
        /// </summary>
        /// <param name="property"><see cref="ModelProperty"/> object.</param>
        public KendoModelProperty([NotNull] ModelProperty property)
        {
            var type = property.Property.PropertyType;
            this.Name = property.Property.Name;

            if (property.Required != null)
                this.IsNullable = false;
            else
                this.IsNullable = type.IsClass || type.IsNullable() || type.IsIEnumerable();

            this.PropertyType = GetJavascriptType(property);

            this.IsEditable = property.Key == null;

            this.IsImmutable = property.Property.GetCustomAttribute<ImmutableAttribute>() != null;

            this.Validation = new KendoPropertyValidation(property);

            var format = property.Property.GetCustomAttribute<DisplayFormatAttribute>();
            this.Format = format == null ? null : new KendoPropertyFormat(format);

            var defaultValue = property.Property.GetCustomAttribute<DefaultValueAttribute>() as DefaultValueAttribute;
            if (defaultValue != null)
            {
                this.DefaultValue = defaultValue.Value;
                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    this.IsDefaultValueFunction = true;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get the javascript type associated with the C# type.
        /// </summary>
        /// <param name="property">This property.</param>
        /// <returns>Javascript type.</returns>
        private string GetJavascriptType(ModelProperty property)
        {
            var type = property.Property.PropertyType;
            if (type.IsIEnumerable())
                return "Array";
                //type = type.GetGenericIEnumerableTypes().First();
            else if (type.IsNullable())
                type = Nullable.GetUnderlyingType(type);

            var generated = property.Property.GetCustomAttribute<DatabaseGeneratedAttribute>();

            if (type == typeof(string)
                || (generated != null))
                return "string";
            else if (type == typeof(int)
                || type == typeof(int?)
                || type == typeof(long)
                || type == typeof(long?)
                || type == typeof(float)
                || type == typeof(float?)
                || type == typeof(decimal)
                || type == typeof(decimal?))
                return "number";
            else if (type == typeof(bool)
                || type == typeof(bool?))
                return "boolean";
            else if (type == typeof(DateTime)
                || type == typeof(DateTime?))
                return "Date";
            else if (type.IsEnum)
                return "number";
            else if (type.IsIEnumerable())
                return "Array";
            else if (type.IsClass)
                return "object";
            else
                return "string";
        }

        /// <summary>
        /// Converts this to a <see cref="JProperty"/> object.
        /// </summary>
        /// <returns>A new instance of a <see cref="JProperty"/>.</returns>
        public JProperty ToJProperty()
        {
            var field_details = new JObject();

            field_details.Add("type", new JValue(this.PropertyType));
            field_details.Add("editable", new JValue(this.IsEditable));
            field_details.Add("immutable", new JValue(this.IsImmutable));
            if (this.DefaultValue != null || (this.IsNullable && this.DefaultValue == null))
            {
                field_details.Add("defaultValue", new JValue(this.DefaultValue));

                if (this.IsDefaultValueFunction)
                    field_details.Add("isDefaultValueFunction", this.IsDefaultValueFunction);
            }

            field_details.Add("nullable", new JValue(this.IsNullable));

            if (this.Format != null)
            {
                field_details.Add("format", this.Format.ToJObject());
            }

            if (Validation != null)
            {
                field_details.Add("validation", this.Validation.ToJObject());
            }
            var field = new JProperty(this.Name, field_details);
            return field;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"\"{this.Name}\": {{");
            sb.Append($"\"type\": \"{this.PropertyType}\",");
            sb.Append($"\"editable\": {this.IsEditable.ToString().ToLower()}\",");
            sb.Append($"\"immutable\": {this.IsImmutable.ToString().ToLower()}\",");
            sb.Append($"\"nullable\": {this.IsNullable.ToString().ToLower()}");

            if (this.DefaultValue != null || (this.IsNullable && this.DefaultValue == null))
            {
                if (this.DefaultValue == null)
                    sb.Append($", \"defaultValue\": null");
                else
                {
                    sb.Append($", \"defaultValue\": \"{this.DefaultValue}\"");

                    if (this.IsDefaultValueFunction)
                        sb.Append($"\"isDefaultValueFunction\": {this.IsDefaultValueFunction.ToString().ToLower()}");
                }
            }

            if (this.Format != null)
            {
                sb.Append($", format: {this.Format.ToString()}");
            }

            if (this.Validation != null)
            {
                sb.Append($", validation: {this.Validation.ToString()}");
            }
            sb.Append("}");
            return sb.ToString();
        }
        #endregion
    }
}
