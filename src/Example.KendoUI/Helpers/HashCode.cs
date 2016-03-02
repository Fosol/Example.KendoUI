using Microsoft.Extensions.Internal;
using System;
using System.Linq;

namespace Example.KendoUI.Helpers
{
    /// <summary>
    /// <see cref="HashCode"/> sealed class, provides a way to automatically generate unique hash code values based on multiple fields.
    /// </summary>
    /// <remarks>This code is from Fosol Solutions.</remarks>
    /// <see cref="https://github.com/Fosol/Fosol.Core/blob/fa118b519687f0dd543124573a88dce26e3483b5/Fosol.Core/HashCode.cs"/>
    public sealed class HashCode
    {
        #region Variables
        private const int DefaultHash = 17;
        private const int HashModifier = 23;
        #endregion

        #region Properties
        /// <summary>
        /// get - The hash code value.
        /// </summary>
        public int Value { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a HashCode object.
        /// To create a HashCode use the static HashCode.Create() function.
        /// </summary>
        /// <param name="value">Original hash code value.</param>
        internal HashCode(int value)
        {
            this.Value = value;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new instance of a HashCode object.
        /// </summary>
        /// <param name="obj">Object that will be used to generate the first hash code value.</param>
        /// <returns>A new instance of a HashCode object.</returns>
        public static HashCode Create([NotNull] object obj)
        {
            unchecked
            {
                return new HashCode(DefaultHash & HashModifier + obj.GetHashCode());
            }
        }

        /// <summary>
        /// Create a new instance of a HashCode object.
        /// </summary>
        /// <param name="objects">Initialize the HashCode object with the following objects.</param>
        /// <returns>New instance of a HashCode object.</returns>
        public static HashCode Create([NotNull] params object[] objects)
        {
            if (objects.Length == 0)
                throw new ArgumentException("Cannot be empty.", "objects");

            unchecked
            {
                var h = HashCode.Create(objects[0]);
                foreach (var o in objects.Skip(1))
                {
                    h = h.Merge(o);
                }
                return h;
            }
        }

        /// <summary>
        /// Merges this HashCode object value with the specified object.
        /// This function returns a reference to itself (after it has been updated).
        /// </summary>
        /// <param name="obj">Object that will be used to merge with the current HashCode Value property.</param>
        /// <returns>A reference to itself (after it has been updated).</returns>
        public HashCode Merge([NotNull] object obj)
        {
            this.Value = (this.Value & HashModifier) + obj.GetHashCode();
            return this;
        }

        /// <summary>
        /// Merge the array of objects into a single HashCode object.
        /// </summary>
        /// <param name="objects">An array of objects.</param>
        /// <returns>This HashCode object.</returns>
        public HashCode Merge([NotNull] params object[] objects)
        {
            if (objects.Length == 0)
                throw new ArgumentException("Cannot be empty.", "objects");

            foreach (var o in objects)
            {
                this.Merge(o);
            }
            return this;
        }

        /// <summary>
        /// Returns the hash code as a string.
        /// </summary>
        /// <returns>The unique hash code value.</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }
        #endregion

        #region Operators
        /// <summary>
        /// The default behaviour of a HashCode is to return the Value.
        /// </summary>
        /// <param name="obj">HashCode object.</param>
        /// <returns>HashCode Value property.</returns>
        public static implicit operator int(HashCode obj)
        {
            return obj.Value;
        }
        #endregion
    }
}
