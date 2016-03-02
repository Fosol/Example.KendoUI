namespace Example.KendoUI.Models
{
    public interface IBaseModel
    {
        bool ArePrimaryKeysEqual(object obj);
        int GetPrimaryKeyHashCode();
    }
}