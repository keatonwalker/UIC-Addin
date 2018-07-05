using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using UIC_Edit_Workflow.Models;

namespace UIC_Edit_Workflow.Views {
    internal class WellAttributeEditorViewModel : DockPane {
        public const string DockPaneId = "WellAttributeEditorPane";
        private readonly FacilityModel _facilityModel = UicWorkflowModule.GetFacilityModel();
        private readonly WellInspectionModel _inspectionModel = UicWorkflowModule.GetWellInspectionModel();
        private readonly WellModel _wellModel = UicWorkflowModule.GetWellModel();

        private RelayCommand _addNewInspection;

        private RelayCommand _addSelectedWell;

        /// <summary>
        ///     Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "My DockPane";

        private string _newWellClass;

        private bool _newWellSelected;

        protected WellAttributeEditorViewModel() {
            MapSelectionChangedEvent.Subscribe(OnSelectionChanged);
        }

        public bool NewWellSelected {
            get => _newWellSelected;
            set => SetProperty(ref _newWellSelected, value, () => NewWellSelected);
        }

        public string NewWellClass {
            get => _newWellClass;
            set => SetProperty(ref _newWellClass, value, () => NewWellClass);
        }

        public ICommand AddWell {
            get {
                if (_addSelectedWell == null) {
                    _addSelectedWell =
                        new RelayCommand(AddSelectedWell, () => !string.IsNullOrWhiteSpace(NewWellClass));
                }

                return _addSelectedWell;
            }
        }

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

        private Task AddSelectedWell() => QueuedTask.Run(() => {
            long selectedId;
            var currentselection = _wellModel.FeatureLayer.GetSelection();
            using (var cursor = currentselection.Search()) {
                var hasrow = cursor.MoveNext();
                using (var row = cursor.Current) {
                    selectedId = Convert.ToInt64(row["OBJECTID"]);
                }
            }

            _wellModel.AddNew(selectedId, _facilityModel.FacilityGuid, _facilityModel.CountyFips);
            NewWellSelected = false;
        });


        private async void OnSelectionChanged(MapSelectionChangedEventArgs mse) {
            foreach (var kvp in mse.Selection) {
                if (!(kvp.Key is BasicFeatureLayer) || kvp.Key.Name != "UICWell") {
                    continue;
                }

                var selectedLayer = (BasicFeatureLayer)kvp.Key;
                //Is a feature selected? Is it an unassigned well feature?
                if (kvp.Value.Count > 0 && await IsUnassignedWell(selectedLayer)) {
                    NewWellSelected = true;
                } else {
                    NewWellSelected = false;
                }
            }
        }

        public static Task<bool> IsUnassignedWell(BasicFeatureLayer selectedLayer) => QueuedTask.Run(() => {
            bool noFacilityFk;
            bool noWellClass;

            var currentSelection = selectedLayer.GetSelection();
            using (var cursor = currentSelection.Search()) {
                var hasrow = cursor.MoveNext();
                using (var row = cursor.Current) {
                    noFacilityFk = string.IsNullOrWhiteSpace(Convert.ToString(row["Facility_FK"]));
                    noWellClass = string.IsNullOrWhiteSpace(Convert.ToString(row["WellClass"]));
                }
            }

            return noFacilityFk && noWellClass;
        });

        /// <summary>
        ///     Show the DockPane.
        /// </summary>
        internal static void Show() {
            var pane = FrameworkApplication.DockPaneManager.Find(DockPaneId);

            pane?.Activate();
        }

        private void AddNewInspection() {
            var wellGuid = _wellModel.WellGuid;

            _inspectionModel.AddNew(wellGuid);
        }
    }
}
