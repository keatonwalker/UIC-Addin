namespace UIC_Edit_Workflow.Models
{
    public delegate void FacilityChangeDelegate(string oldFacId, string newFacId, string facGuid);
    public delegate void ControllingIdChangeDelegate(string oldGuid, string newGuid);
}
