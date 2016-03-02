using Microsoft.Extensions.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Security.Claims;
using Example.KendoUI.Extensions;

namespace Example.KendoUI.Models {
    /// <summary>
    /// <see cref="ModelExtensions"/> static class, provides extension methods for the use of manipulating objects related to <see cref="BaseModel"/>.
    /// </summary>
    public static class ModelExtensions {
        #region Variables
        private static readonly ConcurrentDictionary<Type, ModelProperty[]> _modelKeys = new ConcurrentDictionary<Type, ModelProperty[]>();
        private static readonly ConcurrentDictionary<Type, ModelProperty[]> _modelColumns = new ConcurrentDictionary<Type, ModelProperty[]>();
        private static readonly ConcurrentDictionary<Type, ModelProperty[]> _modelPublicProperties = new ConcurrentDictionary<Type, ModelProperty[]>();
        private static readonly ConcurrentDictionary<Type, ModelProperty[]> _modelForeignKeys = new ConcurrentDictionary<Type, ModelProperty[]>();
        private static readonly ConcurrentDictionary<string, ModelProperty> _modelForeignKeyProperties = new ConcurrentDictionary<string, ModelProperty>();
        #endregion

        /// <summary>
        /// Travels the model properties (like a tree) and executes the specified action.
        /// </summary>
        /// <param name="parent">Parent object to travel.</param>
        /// <param name="action">Action to perform on each property containing child objects.</param>
        public static void TravelModelTree<T>(this T parent, Action<BaseModel, BaseModel> action) where T : BaseModel {
            parent.TravelModelTree(null, action);
        }

        /// <summary>
        /// Travels the model properties (like a tree) and executes the specified action.
        /// </summary>
        /// <param name="parent">Parent object to travel.</param>
        /// <param name="circularCheck">An object that originated the travel which will be used to stop circular references.</param>
        /// <param name="action">Action to perform on each property containing child objects.</param>
        private static void TravelModelTree<T>(this T parent, BaseModel circularCheck, Action<BaseModel, BaseModel> action) where T : BaseModel
        {
            var parent_properties = parent.GetForeignKeys();

            // Call the action on each child object property.
            foreach (var parent_property in parent_properties)
            {
                var child = parent_property.GetValue(parent, null);

                // Child property is null, exit.
                if (child == null)
                    continue;

                var model_child = child as BaseModel;
                if (model_child != null && model_child != circularCheck)
                {
                    model_child.TravelModelTree(parent, action);

                    action(parent, model_child);
                }
                else {
                    var model_collection = (IList)parent_property.GetValue(parent, null);
                    if (model_collection != null)
                    {
                        for (var i = 0; i < model_collection.Count; i++)
                        {
                            model_child = model_collection[i] as BaseModel;

                            if (model_child != null && model_child != circularCheck)
                            {
                                model_child.TravelModelTree(parent, action);

                                action(parent, model_child);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs the specified action on each child of the given parent model.
        /// </summary>
        /// <typeparam name="T">Type of parent model</typeparam>
        /// <param name="parent">Parent model who's children are going to be acted upon.</param>
        /// <param name="action">The action to perform on each child.</param>
        /// <param name="childProperties">The child properties to perform the action upon.</param>
        public static void ForEachChild<T>(this T parent, [NotNull] Action<BaseModel, BaseModel> action, ModelProperty[] childProperties = null) where T : BaseModel
        {
            if (childProperties == null)
            {
                // Get all child properties.
                childProperties = parent.GetForeignKeys();
            }

            // Call the action on each child object property.
            foreach (var parent_property in childProperties)
            {
                var child = parent_property.GetValue(parent, null);

                // Child property is null, exit.
                if (child == null)
                    continue;

                var model_child = child as BaseModel;
                if (model_child != null)
                {
                    action(parent, model_child);
                }
                else {
                    var model_collection = (IList)parent_property.GetValue(parent, null);
                    if (model_collection != null)
                    {
                        for (var i = 0; i < model_collection.Count; i++)
                        {
                            model_child = model_collection[i] as BaseModel;

                            if (model_child != null)
                            {
                                action(parent, model_child);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Travels the parent model properties and updates the children's foreign key that points to the parent.
        /// Set the parent key value in all child objects.
        /// </summary>
        /// <param name="parent">Parent object to travel.</param>
        public static void UpdateChildrenWithParentKeys<T>(this T parent) where T : BaseModel
        {
            parent.ForEachChild((p, c) => { c.SetParentKeys(p); });
        }

        /// <summary>
        /// The parent primary key(s) must have the same name as the child foreign key relationship.
        /// </summary>
        /// <param name="child">Child object to update.</param>
        /// <param name="parent">Parent object with primary key(s).</param>
        public static void SetParentKeys(this BaseModel child, [NotNull] BaseModel parent) {
            if (child == null)
                return;

            var parent_type = parent.GetType();
            var child_type = child.GetType();

            // Get all the primary key properties.
            var parent_keys = parent.GetPrimaryKeys();

            foreach (var parent_key in parent_keys) {
                if (parent_key.Column == null)
                    continue;

                // Get the matching foreign key property in the child object.
                var key = (from p in child.GetColumns()
                           where p.Column?.Name?.Equals(parent_key.Column.Name) ?? false
                           select p).Cast<ModelProperty?>().FirstOrDefault();

                if (key.HasValue) {
                    key.Value.SetValue(child, parent_key.GetValue(parent));
                }
            }
        }

        /// <summary>
        /// Due to serialization creating new instances of every object we need to reassign references.
        /// Travel the model tree foreign keys, if they contain the same primary key information then they should be reassigned to a common reference.
        /// </summary>
        /// <typeparam name="T">Type of BaseModel.</typeparam>
        /// <param name="model">Model to reassign references for.</param>
        /// <param name="action">An action to perform on each child.</param>
        public static void ReassignReferences<T>(this T model, Action<BaseModel, BaseModel> action = null) where T : BaseModel {
            var references = new Dictionary<string, BaseModel>();
            model.ReassignReferences(references, action);
        }

        /// <summary>
        /// Due to serialization creating new instances of every object we need to reassign references.
        /// Travel the model tree foreign keys, if they contain the same primary key information then they should be reassigned to a common reference.
        /// Ensures the whole model tree (all children) are reassigned using a common reference list.
        /// </summary>
        /// <typeparam name="T">Type of BaseModel.</typeparam>
        /// <param name="model">Model to reassign references for.</param>
        /// <param name="references">Dictionary of references.</param>
        /// <param name="action">An action to perform on each child.</param>
        private static void ReassignReferences<T>(this T model, Dictionary<string, BaseModel> references, Action<BaseModel, BaseModel> action = null) where T : BaseModel {
            var model_type = model.GetType();
            var child_properties = model.GetForeignKeys();

            foreach (var child_property in child_properties) {
                var child = child_property.GetValue(model) as BaseModel;

                if (child != null) {
                    var child_type = child.GetType();
                    var key = $"{child_type.Name}_{model.GetPrimaryKeyHashCode()}";

                    // If a child has the same primarykey as a prior reference it should be reassigned.
                    if (references.ContainsKey(key))
                        child_property.SetValue(model, references[key]);
                    else
                        references.Add(key, child);

                    if (action != null)
                        action(model, child);
                }
                else {
                    var collection = child_property.GetValue(model) as IList;

                    if (collection != null) {
                        foreach (var collection_child in collection) {
                            child = collection_child as BaseModel;

                            if (child != null)
                            {
                                child.ReassignReferences(references);

                                if (action != null)
                                    action(model, child);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update the model with default values.
        /// Set the CreatedById and CreatedOn for a POST.
        /// Set the UpdatedById and UpdatedOn for a PUT.
        /// </summary>
        /// <typeparam name="T">Type of model.</typeparam>
        /// <param name="model"><see cref="LogBaseModel"/> model to update.</param>
        /// <param name="context"><see cref="OperationBindingContext"/> object.</param>
        public static void AssignDefaultValues<T>(this T model, OperationBindingContext context) where T : LogBaseModel
        {
            int user_id = 0;
            int.TryParse(context.HttpContext.User.GetUserId(), out user_id);
            var date = DateTime.UtcNow;
            var http_method = context.HttpContext.Request.Method;

            /// This part is for the original parent object.
            /// It's possible to have a collection of child objects that are not being added even though they belong to a parent that is being posted.
            switch (http_method)
            {
                case ("POST"):
                    model.CreatedById = user_id;
                    model.CreatedOn = date;
                    break;
                case ("PUT"):
                case ("DELETE"):
                    model.UpdatedById = user_id;
                    model.UpdatedOn = date;
                    break;
            }

            // This part is relevant for all child objects of the parent object.
            switch (model.State)
            {
                case (ModelStates.Added):
                    model.CreatedById = user_id;
                    model.CreatedOn = date;
                    break;
                case (ModelStates.Modified):
                default:
                    model.UpdatedById = user_id;
                    model.UpdatedOn = date;
                    break;
            }
        }

        /// <summary>
        /// Get all the primary key properties for the specified model.
        /// </summary>
        /// <exception cref="System.ArgumentException">Parameter 'type' must inherit from BaseModel.</exception>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModelProperty[] GetPrimaryKeys(this Type type) {
            if (_modelKeys.ContainsKey(type))
                return _modelKeys[type];

            var key_properties = (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                  where p.GetCustomAttribute<KeyAttribute>() != null
                                  select new ModelProperty(p)).OrderBy(k => k.Column?.Order).ToArray();

            _modelKeys.TryAdd(type, key_properties);
            return key_properties;
        }

        /// <summary>
        /// Get all the primary key properties for the specified model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ModelProperty[] GetPrimaryKeys<T>(this T model) where T : BaseModel {
            return model.GetType().GetPrimaryKeys();
        }

        /// <summary>
        /// Get all the column properties for the specified model.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModelProperty[] GetColumns(this Type type) {
            if (_modelColumns.ContainsKey(type))
                return _modelColumns[type];

            var column_properties = (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                     where p.GetCustomAttribute<ColumnAttribute>() != null
                                     select new ModelProperty(p)).ToArray();
            _modelColumns.TryAdd(type, column_properties);
            return column_properties;
        }

        /// <summary>
        /// Get the public properties for the specified type.
        /// </summary>
        /// <param name="type">Type object.</param>
        /// <returns>An array of <see cref="ModelProperty"/>.</returns>
        public static ModelProperty[] GetPublicProperties(this Type type)
        {
            if (_modelPublicProperties.ContainsKey(type))
                return _modelPublicProperties[type];

            var column_properties = (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                     select new ModelProperty(p)).ToArray();
            _modelPublicProperties.TryAdd(type, column_properties);
            return column_properties;
        }

        /// <summary>
        /// Get all the column properties for the specified model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns>An array of <see cref="ModelProperty"/>.</returns>
        public static ModelProperty[] GetColumns<T>(this T model) where T : BaseModel {
            return model.GetType().GetColumns();
        }

        /// <summary>
        /// Get all the foreignkey properties for the specified type.
        /// </summary>
        /// <exception cref="System.ArgumentException">Parameter 'type' must inherit from BaseModel.</exception>
        /// <param name="type"></param>
        /// <returns>An array of <see cref="ModelProperty"/>.</returns>
        public static ModelProperty[] GetForeignKeys(this Type type) {
            if (_modelForeignKeys.ContainsKey(type))
                return _modelForeignKeys[type];

            var column_properties = (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                     where p.GetCustomAttribute<ForeignKeyAttribute>() != null // Anything marked with a ForeignKeyAttribute is a reference.
                                        || (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)) // Collections are foreignkey references.
                                     select new ModelProperty(p)).ToArray();
            _modelForeignKeys.TryAdd(type, column_properties);
            return column_properties;
        }

        /// <summary>
        /// Get all the foreignkey properties for the specified model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns>An array of <see cref="ModelProperty"/>.</returns>
        public static ModelProperty[] GetForeignKeys<T>(this T model) where T : BaseModel {
            return model.GetType().GetForeignKeys();
        }

        /// <summary>
        /// Determines if all the primary key values are default values.
        /// This indicates that the model is (or should be) in a Added state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns>True if all the model primary keys are default values.</returns>
        public static bool AreDefaultPrimaryKeyValues<T>(this T model) where T : BaseModel {
            var keys = model.GetPrimaryKeys();

            foreach (var key in keys) {
                // A primary key that is also a foreignkey must be ignored.
                if (key.ForeignKey == null) {
                    var value = key.GetValue(model);
                    var default_value = key.Property.PropertyType.GetDefaultValue();

                    if (value != default_value)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the property for the foreignkey.
        /// </summary>
        /// <exception cref="System.ArgumentException">Parameter 'type' must inherit from BaseModel.</exception>
        /// <param name="type"></param>
        /// <param name="foreignKey"></param>
        /// <returns>A <see cref="ModelProperty"/> if one exists, or null.</returns>
        public static ModelProperty? GetForeignKeyProperty(this Type type, [NotNull] ForeignKeyAttribute foreignKey) {
            if (!typeof(BaseModel).IsAssignableFrom(type))
                throw new ArgumentException($"Parameter '{nameof(type)}' must inherit from BaseModel.", nameof(type));

            var key = $"{type.Name}_{foreignKey.Name}";

            if (_modelForeignKeyProperties.ContainsKey(key))
                return _modelForeignKeyProperties[key];

            var foreign_key_property = (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                        where p.Name.Equals(foreignKey.Name)
                                        select p).FirstOrDefault();

            if (foreign_key_property == null)
                return null;

            var foreign_key = new ModelProperty(foreign_key_property);

            _modelForeignKeyProperties.TryAdd(key, foreign_key);
            return foreign_key;
        }

        /// <summary>
        /// Get the property for the foreignkey.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="foreignKey"></param>
        /// <returns>A <see cref="ModelProperty"/> if one exists, or null.</returns>
        public static ModelProperty? GetForeignKeyProperty<T>(this T model, [NotNull] ForeignKeyAttribute foreignKey) where T : BaseModel {
            return model.GetType().GetForeignKeyProperty(foreignKey);
        }

        /// <summary>
        /// Get the foreignkey property in the parent for the specified primaykey in the child.
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="childType"></param>
        /// <param name="childPrimaryKey"></param>
        /// <returns>A <see cref="ModelProperty"/> if one exists, or null.</returns>
        public static ModelProperty? GetForeignKeyProperty(this Type parentType, [NotNull] Type childType, ModelProperty childPrimaryKey) {
            if (!typeof(BaseModel).IsAssignableFrom(parentType))
                throw new ArgumentException($"Parameter '{nameof(parentType)}' must inherit from BaseModel.", nameof(parentType));

            if (!typeof(BaseModel).IsAssignableFrom(childType))
                throw new ArgumentException($"Parameter '{nameof(childType)}' must inherit from BaseModel.", nameof(childType));

            if (childPrimaryKey.Column == null
                || childPrimaryKey.Key == null)
                throw new ArgumentException($"Parameter '{nameof(childPrimaryKey)}' must contain a Column and Key value.", nameof(childPrimaryKey));

            var key = $"{parentType.Name}_{childPrimaryKey.Column.Name}";

            if (_modelForeignKeyProperties.ContainsKey(key))
                return _modelForeignKeyProperties[key];

            // Find a ForeignKeyAttribute for the child's PrimaryKey then map it to the parent foreignkey property.
            var possible_foreign_key_properties = (from p in parentType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                   join f in (from fp in parentType.GetForeignKeys()
                                                              where fp.Property.PropertyType == childType
                                                              select fp) on p.Name.ToLower() equals f.ForeignKey?.Name.ToLower()
                                                   select new ModelProperty(p)).ToArray();

            // There was no ForeignKeyAttribute map found, we'll need to look for the foreignkey property by inferring the type.
            if (possible_foreign_key_properties == null || possible_foreign_key_properties.Count() == 0) {
                // The foreignkey property in the parent may be nullable.
                possible_foreign_key_properties = (from p in parentType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                   where p.PropertyType == childPrimaryKey.Property.PropertyType
                                                    || (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                                    && p.PropertyType.GetGenericTypeDefinition() == childPrimaryKey.Property.PropertyType)
                                                   select new ModelProperty(p)).ToArray();
            }

            // The parent model does not contain a foreignkey for the child.
            if (possible_foreign_key_properties == null || possible_foreign_key_properties.Count() == 0)
                return null;

            if (possible_foreign_key_properties.Count() == 1) {
                _modelForeignKeyProperties.TryAdd(key, possible_foreign_key_properties[0]);
                return possible_foreign_key_properties[0];
            }

            // Find a property on the parent that matches the child's primary key, based on the column name.
            var foreign_key_properties = (from p in possible_foreign_key_properties
                                          where p.Column.Name.Equals(childPrimaryKey.Column.Name, StringComparison.InvariantCultureIgnoreCase)
                                          select p).ToArray();

            if (foreign_key_properties != null && foreign_key_properties.Length == 1) {
                _modelForeignKeyProperties.TryAdd(key, foreign_key_properties[0]);
                return foreign_key_properties[0];
            }

            return null;
        }

        /// <summary>
        /// Get the foreignkey property in the parent for the specified primaykey in the child.
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="child"></param>
        /// <param name="childPrimaryKey"></param>
        /// <returns>A <see cref="ModelProperty"/> if one exists, or null.</returns>
        public static ModelProperty? GetForeignKeyProperty(this Type parentType, [NotNull] BaseModel child, ModelProperty childPrimaryKey) {
            if (!typeof(BaseModel).IsAssignableFrom(parentType))
                throw new ArgumentException($"Parameter '{nameof(parentType)}' must inherit from BaseModel.", nameof(parentType));

            if (childPrimaryKey.Column == null
                || childPrimaryKey.Key == null)
                throw new ArgumentException($"Parameter '{nameof(childPrimaryKey)}' must contain a Column and Key value.", nameof(childPrimaryKey));

            return parentType.GetForeignKeyProperty(child.GetType(), childPrimaryKey);
        }

        /// <summary>
        /// Get the foreignkey property in the parent for the specified primaykey in the child.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="childPrimaryKey"></param>
        /// <returns>A <see cref="ModelProperty"/> if one exists, or null.</returns>
        public static ModelProperty? GetForeignKeyProperty<T>(this T parent, [NotNull] BaseModel child, ModelProperty childPrimaryKey) where T : BaseModel {
            if (childPrimaryKey.Column == null
                || childPrimaryKey.Key == null)
                throw new ArgumentException($"Parameter '{nameof(childPrimaryKey)}' must contain a Column and Key value.", nameof(childPrimaryKey));

            return parent.GetType().GetForeignKeyProperty(child, childPrimaryKey);
        }

        /// <summary>
        /// Get all the foreignkey properties in the parent model type for the specified child.
        /// </summary>
        /// <param name="parent">Parent model type.</param>
        /// <param name="child">Child model.</param>
        /// <returns>An array of <see cref="ModelProperty"/>.</returns>
        public static ModelProperty[] GetForeignKeysFor(this Type parentType, Type childType) {
            if (!typeof(BaseModel).IsAssignableFrom(parentType))
                throw new ArgumentException($"Parameter '{nameof(parentType)}' must inherit from BaseModel.", nameof(parentType));

            if (!typeof(BaseModel).IsAssignableFrom(childType))
                throw new ArgumentException($"Parameter '{nameof(childType)}' must inherit from BaseModel.", nameof(childType));

            var primary_keys = childType.GetPrimaryKeys();

            var foreign_keys = new List<ModelProperty>();

            foreach (var primary_key in primary_keys) {
                var foreign_key = parentType.GetForeignKeyProperty(childType, primary_key);

                if (foreign_key.HasValue) {
                    foreign_keys.Add(foreign_key.Value);
                }
            }
            return foreign_keys.ToArray();
        }

        /// <summary>
        /// Get all the foreignkey properties in the parent model type for the specified child.
        /// </summary>
        /// <param name="parentType">Parent model type.</param>
        /// <param name="child">Child model.</param>
        /// <returns>An array of <see cref="ModelProperty"/>.</returns>
        public static ModelProperty[] GetForeignKeysFor(this Type parentType, BaseModel child) {
            if (!typeof(BaseModel).IsAssignableFrom(parentType))
                throw new ArgumentException($"Parameter '{nameof(parentType)}' must inherit from BaseModel.", nameof(parentType));

            return parentType.GetForeignKeysFor(child.GetType());
        }

        /// <summary>
        /// Get all the foreignkey properties in the parent model for the specified child.
        /// </summary>
        /// <typeparam name="T">Type of parent model.</typeparam>
        /// <param name="parent">Parent model.</param>
        /// <param name="child">Child model.</param>
        /// <returns>An array of <see cref="ModelProperty"/>.</returns>
        public static ModelProperty[] GetForeignKeysFor<T>(this T parent, BaseModel child) where T : BaseModel {
            return parent.GetType().GetForeignKeysFor(child);
        }

        /// <summary>
        /// When an model contains child entities it may need to update it's own foreign key property values.
        /// This method ensures the parent uses the child id values.
        /// </summary>
        /// <param name="model">BaseModel object to update.</param>
        public static void UpdateForeignKeys<T>(this T model) where T : BaseModel {
            var properties = model.GetForeignKeys();

            foreach (var property in properties) {
                var child = property.GetValue(model) as BaseModel;
                if (child != null) {
                    // Get the foreign key information.
                    ModelProperty[] foreign_keys;

                    if (property.ForeignKey != null)
                        foreign_keys = new[] { model.GetForeignKeyProperty(property.ForeignKey).Value };
                    else
                        foreign_keys = model.GetForeignKeysFor(child);

                    if (foreign_keys != null && foreign_keys.Length > 0) {
                        var child_keys = child.GetPrimaryKeys();

                        foreach (var child_key in child_keys) {
                            var child_key_value = child_key.GetValue(child);
                            var default_value = child_key.Property.PropertyType.GetDefaultValue();
                            var foreign_key = model.GetForeignKeyProperty(child, child_key);

                            // All parent foreignkey values will be overrided by the child object's id (if it is provided).
                            if (foreign_key.HasValue && child_key_value != default_value)
                                foreign_key.Value.SetValue(model, child_key_value);
                        }
                    }
                }
                else {
                    // Travel through the model tree and update foreignkeys.
                    var child_collection = property.GetValue(model) as IList;
                    if (child_collection != null) {
                        for (var i = 0; i < child_collection.Count; i++) {
                            child = child_collection[i] as BaseModel;

                            if (child != null) {
                                child.UpdateForeignKeys();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the parent objects with the child's primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <param name="possibleParents"></param>
        public static void UpdateParentForeignKeys<T>(this T child, IEnumerable possibleParents) where T : BaseModel {
            var keyValue = child.GetKey().GetValue(child);
            foreach (var possibleParent in possibleParents) {
                var foreignKeysForType = possibleParent.GetType().GetProperties().Where(property => property.PropertyType == child.GetType());
                var foreignKeysForChild = foreignKeysForType.Where(key => key.GetValue(possibleParent) == child && key.GetCustomAttribute<ForeignKeyAttribute>() != null);
                foreach (var foreignKeyToChild in foreignKeysForChild) {
                    var foreignKeyName = foreignKeyToChild.GetCustomAttribute<ForeignKeyAttribute>().Name;
                    var foreignKeyInfo = possibleParent.GetType().GetProperty(foreignKeyName);
                    foreignKeyInfo.SetValue(possibleParent, keyValue);
                }
            }
        }

        /// <summary>
        /// Get the <see cref="PropertyInfo"/> for the primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns>A new instance of a <see cref="PropertyInfo"/>.</returns>
        private static PropertyInfo GetKey<T>(this T model) where T : BaseModel {
            return model.GetType().GetProperties().Single(property => property.GetCustomAttribute<KeyAttribute>() != null);
        }

        /// <summary>
        /// Creates a new instance of type 'RT' and populates it with the property values from the entity.
        /// This requires that type 'RT' has a base empty constructor.
        /// Only sets the public properties.
        /// </summary>
        /// <typeparam name="RT"></typeparam>
        /// <param name="entity"></param>
        /// <returns>A new instance of type 'RT'.</returns>
        public static RT CopyTo<RT>(this object entity)
        {
            var properties = GetPublicProperties(entity.GetType());
            var result = Activator.CreateInstance<RT>();

            var r_type = typeof(RT);
            foreach (var property in properties)
            {
                var rp = r_type.GetProperty(property.Property.Name);
                if (rp != null)
                {
                    var value = property.GetValue(entity);
                    rp.SetValue(result, value);
                }
            }

            return result;
        }
    }
}
