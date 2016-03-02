using Microsoft.Extensions.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.KendoUI.Models
{
    /// <summary>
    /// <see cref="KendoModel"/> class, provides a way to provide Kendo UI with a dynamic models definition.
    /// </summary>
    [JsonConverter(typeof(TypeConverters.JavascriptModelConverter))]
    public class KendoModel
    {
        #region Properties
        /// <summary>
        /// get - The name of the model.  This isn't used by Kendo.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// get - The primary key of the model.  Kendo only supports single keys.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// get - A list of <see cref="ModelPropertyDefinition"/> objects.
        /// </summary>
        public List<KendoModelProperty> Properties { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="KendoModel"/> object.
        /// </summary>
        /// <param name="type">The model Type.</param>
        public KendoModel([NotNull] Type model)
        {
            this.Name = model.Name;
            var primary_keys = model.GetPrimaryKeys();

            if (primary_keys.Length == 1)
                this.Id = primary_keys.FirstOrDefault().Property.Name;

            this.Properties = model.GetPublicProperties().Select(p => new KendoModelProperty(p)).ToList();
        }

        /// <summary>
        /// Creates a new instance of a <see cref="KendoModel"/> object.
        /// </summary>
        /// <param name="model">The model.</param>
        public KendoModel([NotNull] BaseModel model) : this(model.GetType())
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns this object as a JSON string.
        /// </summary>
        /// <returns>JSON string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"name\": \"{this.Name}\",");
            sb.Append($"\"id\": \"{this.Id}\",");
            sb.Append($"\"fields\": {{{this.Properties.Select(p => p.ToString()).Aggregate((a, b) => $"{a},{b}")}}}");
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Convert this into a <see cref="JObject"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="JObject"/>.</returns>
        public JObject ToJObject()
        {
            var model = new JObject();
            model.Add("name", new JValue(this.Name));
            if (!String.IsNullOrEmpty(this.Id))
                model.Add("id", new JValue(this.Id));

            var fields = new JObject();

            foreach (var property in this.Properties)
            {
                fields.Add(property.ToJProperty());
            }
            model.Add("fields", fields);

            return model;
        }
        #endregion
    }
}
