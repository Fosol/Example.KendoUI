using System;
using System.ComponentModel.DataAnnotations;

namespace Example.KendoUI.Models
{
    public abstract class LogBaseModel: BaseModel, ILogBaseModel
    {

        [Required]
        public DateTime CreatedOn { get; set; }

        public int CreatedById { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedById { get; set; }

        public DateTime Stamp { get; set; }

        public ModelStates State { get; set; }
    }
}