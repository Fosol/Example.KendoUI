using System;

namespace Example.KendoUI.Models
{
    public interface ILogBaseModel
    {
        DateTime CreatedOn { get; set; }
        int CreatedById { get; set; }
        DateTime? UpdatedOn { get; set; }
        int? UpdatedById { get; set; }
        DateTime Stamp { get; set; }
    }
}