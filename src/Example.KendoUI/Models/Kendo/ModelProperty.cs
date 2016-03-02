using Microsoft.Extensions.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Example.KendoUI.Models
{
    /// <summary>
    /// <see cref="ModelProperty"/> struct, provides a way to capture all relevant information about a model property.
    /// </summary>
    public struct ModelProperty
    {
        #region Properties
        /// <summary>
        /// get - The <see cref="PropertyInfo"/> for the property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// get - The <see cref="KeyAttribute"/> for the property.
        /// </summary>
        public KeyAttribute Key { get; }

        /// <summary>
        /// get - The <see cref="ForeignKeyAttribute"/> for the property.
        /// </summary>
        public ForeignKeyAttribute ForeignKey { get; }

        /// <summary>
        /// get - The <see cref="ColumnAttribute"/> for the property.
        /// </summary>
        public ColumnAttribute Column { get; }

        /// <summary>
        /// get - The <see cref="RequiredAttribute"/> for the property.
        /// </summary>
        public RequiredAttribute Required { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="ModelProperty"/> object.
        /// </summary>
        /// <param name="propertyInfo"><see cref="PropertyInfo"/> object.</param>
        public ModelProperty([NotNull] PropertyInfo propertyInfo)
        {
            this.Property = propertyInfo;
            this.Key = propertyInfo.GetCustomAttribute<KeyAttribute>();
            this.Column = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            this.Required = propertyInfo.GetCustomAttribute<RequiredAttribute>();
            this.ForeignKey = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get the property value through reflection.
        /// </summary>
        /// <param name="model">Model object.</param>
        /// <param name="index">Index position of the value if relevant.</param>
        /// <returns>Object value of the property.</returns>
        public object GetValue(object model, object[] index = null)
        {
            return this.Property.GetValue(model, index);
        }

        /// <summary>
        /// Set the property value through reflection.
        /// </summary>
        /// <param name="model">Model object.</param>
        /// <param name="value">New value.</param>
        /// <param name="index">Index position of the value if relevant.</param>
        public void SetValue(object model, object value, object[] index = null)
        {
            this.Property.SetValue(model, value, index);
        }

        /// <summary>
        /// Determine if the property value is equal to the specified object, through reflection.
        /// </summary>
        /// <param name="obj">Object value.</param>
        /// <returns>True if equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(ModelProperty))
                return false;

            var mp = (ModelProperty)obj;
            return this.Property.Equals(mp.Property);
        }

        /// <summary>
        /// Get the hashcode for the property.
        /// </summary>
        /// <returns>A unique hashcode for the property value.</returns>
        public override int GetHashCode()
        {
            return this.Property.GetHashCode();
        }
        #endregion
    }
}