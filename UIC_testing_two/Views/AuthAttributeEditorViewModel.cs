using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using UIC_Edit_Workflow.Models;

namespace UIC_Edit_Workflow.Views
{
    internal class AuthAttributeEditorViewModel : DockPane
    {
        private const string DockPaneId = "UIC_Edit_Workflow_AuthAttributeEditor";
        private static readonly FacilityModel FacilityModel = Module1.FacilityModel;
        private readonly AuthorizationModel _authModel = Module1.AuthorizationModel;

        protected AuthAttributeEditorViewModel() { }

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
        private string _heading = "Authorization";
        public string Heading
        {
            get => _heading;
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        private RelayCommand _addNewRecord;
        public ICommand AddRecord
        {
            get
            {
                if (_addNewRecord == null)
                {
                    _addNewRecord = new RelayCommand(() => AddNewRecord(), () => true);
                }

                return _addNewRecord;
            }
        }
        private void AddNewRecord()
        {
            var facGuid = FacilityModel.FacilityGuid;
            var facFips = FacilityModel.CountyFips;

            _authModel.AddNew(facGuid, facFips);
        }
    }
}
