using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.KendoUI.Models
{
    public class KendoPropertyFormat
    {
        #region Properties
        public string FormatString { get; }

        public bool? ApplyFormatInEditMode { get; }

        public bool? ConvertEmptyStringToNull { get; }

        public bool? HtmlEncode { get; }

        public string NullDisplayText { get; }
        #endregion

        #region Constructor
        public KendoPropertyFormat(DisplayFormatAttribute format)
        {
            this.FormatString = format?.DataFormatString;
            this.ApplyFormatInEditMode = format?.ApplyFormatInEditMode;
            this.ConvertEmptyStringToNull = format?.ConvertEmptyStringToNull;
            this.HtmlEncode = format?.HtmlEncode;
            this.NullDisplayText = format?.NullDisplayText;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Converts this to a <see cref="JObject"/> object.
        /// </summary>
        /// <returns>A new instance of a <see cref="JObject"/>.</returns>
        public JObject ToJObject()
        {
            var format = new JObject();

            format.Add("formatString", new JValue(this.FormatString));
            format.Add("applyFormatInEditMode", new JValue(this.ApplyFormatInEditMode));
            format.Add("convertEmptyStringToNull", new JValue(this.ConvertEmptyStringToNull));
            format.Add("htmlEncode", new JValue(this.HtmlEncode));
            format.Add("nullDisplayText", new JValue(this.NullDisplayText));
            return format;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"formatString\": \"{this.FormatString}\",");
            sb.Append($"\"applyFormatInEditMode\": {this.ApplyFormatInEditMode.ToString().ToLower()}\",");
            sb.Append($"\"convertEmptyStringToNull\": {this.ConvertEmptyStringToNull.ToString().ToLower()}\",");
            sb.Append($"\"htmlEncode\": {this.HtmlEncode.ToString().ToLower()}\",");
            sb.Append($"\"nullDisplayText\": {this.NullDisplayText.ToString().ToLower()}");
            sb.Append("}");
            return sb.ToString();
        }
        #endregion
    }
}
