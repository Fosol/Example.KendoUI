using Example.KendoUI.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Example.KendoUI.Models
{
    public abstract class BaseModel : IBaseModel
    {
        #region Variables
        protected const string Yes = "Y";
        protected const string No = "N";
        #endregion

        #region Properties
        #endregion

        #region Methods

        /// <summary>
        /// Determines if the two objects are equal based on their primary key values.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ArePrimaryKeysEqual(object obj)
        {
            var model = obj as BaseModel;

            if (model == null)
                return false;

            if (ReferenceEquals(this, model))
                return true;

            // If the models are new they will have default primary keys.
            // These are not equal.
            if (model.AreDefaultPrimaryKeyValues())
                return false;

            return this.GetHashCode() == model.GetPrimaryKeyHashCode();
        }

        /// <summary>
        /// Get a unique hash code comprised of the primary keys.
        /// </summary>
        /// <returns>A hash code that represents this unique model.</returns>
        public int GetPrimaryKeyHashCode()
        {
            var keys = this.GetPrimaryKeys();

            var hash = HashCode.Create(0);
            foreach (var key in keys)
            {
                var value = key.GetValue(this);
                hash.Merge(value);
            }

            return hash;
        }
        #endregion
    }
}