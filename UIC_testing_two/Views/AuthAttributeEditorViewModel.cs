using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using UIC_Edit_Workflow.Models;

namespace UIC_Edit_Workflow.Views {
    internal class AuthAttributeEditorViewModel : DockPane {
        public const string DockPaneId = "AuthAttributeEditorPane";

        private RelayCommand _addNewRecord;

        /// <summary>
        ///     Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "Authorization";

        private AuthorizationModel _attributeEditorViewModel;

        protected AuthAttributeEditorViewModel() {
        }

        public string Heading {
            get => _heading;
            set => SetProperty(ref _heading, value, () => Heading);
        }

        public AuthorizationModel AuthModel {
            get => _attributeEditorViewModel;
            set => SetProperty(ref _attributeEditorViewModel, value, () => AuthModel);
        }

        public ICommand AddRecord {
            get {
                if (_addNewRecord == null) {
                    _addNewRecord = new RelayCommand(() => AddNewRecord(), () => true);
                }

                return _addNewRecord;
            }
        }

        /// <summary>
        ///     Show the DockPane.
        /// </summary>
        internal static void Show() {
            var pane = FrameworkApplication.DockPaneManager.Find(DockPaneId);

            pane?.Activate();
        }

        private void AddNewRecord() {
//            var facGuid = FacilityModel.FacilityGuid;
//            var facFips = FacilityModel.CountyFips;
//
//            _authModel.AddNew(facGuid, facFips);
        }
    }
}
