using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using UIC_Edit_Workflow.Models;

namespace UIC_Edit_Workflow
{
    internal class FacilityAttributeEditorViewModel : DockPane
    {
        private const string DockPaneId = "UIC_Edit_Workflow_FacilityAttributeEditor";
        private readonly FacilityModel _facilityModel = Module1.FacilityModel;
        private readonly FacilityInspectionModel _inspectionModel = Module1.FacilityInspectionModel;

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
            {
                var pane = FrameworkApplication.DockPaneManager.Find(DockPaneId);
                pane?.Activate();
            }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Facility Attributes";
        public string Heading
        {
            get => _heading;
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        private RelayCommand _addNewInspection;
        public ICommand AddInspectionRecord
        {
            get
            {
                if (_addNewInspection == null)
                {
                    _addNewInspection = new RelayCommand(() => AddNewInspection(), () => true);
                }

                return _addNewInspection;
            }
        }
        private void AddNewInspection()
        {
            var facGuid = _facilityModel.FacilityGuid;

            _inspectionModel.AddNew(facGuid);
        }
    }
}
