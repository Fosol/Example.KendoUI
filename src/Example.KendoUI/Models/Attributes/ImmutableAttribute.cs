using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.KendoUI.Models
{
    /// <summary>
    /// <see cref="ImmutableAttribute"/> class, provides a way to define a property as immutable after it has been set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ImmutableAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="ImmutableAttribute"/> object.
        /// </summary>
        public ImmutableAttribute()
        {

        }
        #endregion
    }
}
