using Example.KendoUI.Extensions;
using Example.KendoUI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Example.KendoUI.Data
{
    public static class DataSource
    {
        public static IList<UserModel> Users { get; set; } = new List<UserModel>() {
            new UserModel() { Id = 1, UserName = "admin", PersonId = 1, CreatedOn = DateTime.Now },
            new UserModel() { Id = 2, UserName = "qa", PersonId = 2, CreatedOn = DateTime.Now }
        };

        public static IList<PersonModel> Persons { get; set; } = new List<PersonModel>()
        {
            new PersonModel() { Id = 1, FirstName = "First", LastName = "Last", CreatedOn = DateTime.Now },
            new PersonModel() { Id = 2, FirstName = "John", LastName = "Doe", CreatedOn = DateTime.Now }
        };

        public static IList<RoleModel> Roles { get; set; } = new List<RoleModel>()
        {
            new RoleModel() { Id = 1, Name = "Administrators", CreatedOn = DateTime.Now },
            new RoleModel() { Id = 2, Name = "Users", CreatedOn = DateTime.Now }
        };

        public static IList<CommunicationMethodTypeModel> CommunicationMethodTypes { get; set; } = new List<CommunicationMethodTypeModel>()
        {
            new CommunicationMethodTypeModel() { Id = 1, Name = "Phone Number", CreatedOn = DateTime.Now },
            new CommunicationMethodTypeModel() { Id = 2, Name = "Email", CreatedOn = DateTime.Now }
        };

        public static IList<CommunicationMethodModel> CommunicationMethods { get; set; } = new List<CommunicationMethodModel>()
        {
            new CommunicationMethodModel() { Id = 1, PersonId = 1, CommunicationNumber = "123-123-1234", CommunicationTypeId = 1, CreatedOn = DateTime.Now },
            new CommunicationMethodModel() { Id = 2, PersonId = 1, CommunicationNumber = "your@email.com", CommunicationTypeId = 2, CreatedOn = DateTime.Now }
        };

        #region Constructors
        static DataSource()
        {
        }
        #endregion

        #region Methods
        public static int GenerateId<T>(this IList<T> dbset, DatabaseGeneratedOption option = DatabaseGeneratedOption.Identity) where T : IPrimaryKey
        {
            return dbset.Max(e => e.Id) + 1;
        }

        public static DateTime GenerateStamp<T>(this IList<T> dbset) where T : IPrimaryKey
        {
            return DateTime.Now;
        }

        public static void Create<T>(this IList<T> dbset, T entity) where T : IPrimaryKey, ILogBaseModel
        {
            entity.Id = dbset.GenerateId();
            entity.Stamp = dbset.GenerateStamp();
            entity.CreatedOn = DateTime.Now;
            entity.UpdatedOn = null;
            dbset.Add(entity);
        }

        public static void Update<T>(this IList<T> dbset, T entity) where T : IPrimaryKey, ILogBaseModel
        {
            var item = dbset.Single(i => i.Id == entity.Id);

            entity.UpdatedOn = DateTime.Now;
            dbset.Replace(item, entity);
        }

        public static bool Delete<T>(this IList<T> dbset, T entity) where T : IPrimaryKey
        {
            var item = dbset.Single(i => i.Id == entity.Id);
            return dbset.Remove(item);
        }
        #endregion
    }
}