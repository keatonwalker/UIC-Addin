using System.Threading.Tasks;

namespace UIC_Edit_Workflow
{
    internal interface IWorkTaskModel
    {
        Task UpdateModel(string controllingId);
        Task SaveChanges();
        void ControllingIdChangedHandler(string oldGuid, string newGuid);
    }
}
