using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example.KendoUI.Models
{
    public class PersonModel: LogBaseModel, IPrimaryKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public ICollection<CommunicationMethodModel> CommunicationMethods { get; set; } = new Collection<CommunicationMethodModel>();
    }
}