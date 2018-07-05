using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using UIC_Edit_Workflow.Models;

namespace UIC_Edit_Workflow.Views {
    internal class FacilityAttributeEditorViewModel : DockPane {
        public const string DockPaneId = "FacilityAttributeEditorPane";
        private readonly FacilityModel _facilityModel = UicWorkflowModule.GetFacilityModel();
        private readonly FacilityInspectionModel _inspectionModel = UicWorkflowModule.GetFacilityInspectionModel();

        private RelayCommand _addNewInspection;

        /// <summary>
        ///     Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Facility Attributes";

        public string Heading {
            get => _heading;
            set => SetProperty(ref _heading, value, () => Heading);
        }

        public ICommand AddInspectionRecord {
            get {
                if (_addNewInspection == null) {
                    _addNewInspection = new RelayCommand(() => AddNewInspection(), () => true);
                }

                return _addNewInspection;
            }
        }

        /// <summary>
        ///     Show the DockPane.
        /// </summary>
        internal static void Show() {
            var pane = FrameworkApplication.DockPaneManager.Find(DockPaneId);
            pane?.Activate();
        }

        private void AddNewInspection() {
            var facGuid = _facilityModel.FacilityGuid;

            _inspectionModel.AddNew(facGuid);
        }
    }
}
