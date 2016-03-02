using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example.KendoUI.Models
{
    public class UserModel: LogBaseModel, IPrimaryKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(25)]
        public string UserName { get; set; }

        public int? PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public PersonModel Person { get; set; }

        public ICollection<RoleModel> Roles { get; set; } = new Collection<RoleModel>();
    }
}