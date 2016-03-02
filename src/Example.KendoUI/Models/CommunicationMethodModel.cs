using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example.KendoUI.Models
{
    public class CommunicationMethodModel: LogBaseModel, IPrimaryKey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required, MaxLength(100)]
        public string CommunicationNumber { get; set; }

        [Required]
        public int CommunicationTypeId { get; set; }

        [ForeignKey(nameof(CommunicationTypeId))]
        public CommunicationMethodTypeModel CommunicationMethodType { get; set; }

        [ForeignKey(nameof(PersonId))]
        public PersonModel Person { get; set; }
    }
}