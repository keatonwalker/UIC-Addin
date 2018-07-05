using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using UIC_Edit_Workflow.Models;

namespace UIC_Edit_Workflow.Views
{
    internal class WellAttributeEditorViewModel : DockPane
    {
        private readonly FacilityModel _facilityModel = Module1.GetFacilityModel();
        private readonly WellModel _wellModel = Module1.GetWellModel();
        private readonly WellInspectionModel _inspectionModel = Module1.GetWellInspectionModel();

        public const string DockPaneId = "WellAttributeEditorPane";

        protected WellAttributeEditorViewModel()
        {
            MapSelectionChangedEvent.Subscribe(OnSelectionChanged);
        }

        private bool _newWellSelected;
        public bool NewWellSelected
        {
            get => _newWellSelected;
            set
            {
                SetProperty(ref _newWellSelected, value, () => NewWellSelected);
            }
        }

        private string _newWellClass;
        public string NewWellClass
        {
            get => _newWellClass;
            set
            {
                SetProperty(ref _newWellClass, value, () => NewWellClass);
            }
        }

        private RelayCommand _addSelectedWell;
        public ICommand AddWell
        {
            get
            {
                if (_addSelectedWell == null)
                {
                    _addSelectedWell = new RelayCommand(AddSelectedWell, () => !string.IsNullOrWhiteSpace(NewWellClass));
                }

                return _addSelectedWell;
            }
        }

        private Task AddSelectedWell()
        {
            return QueuedTask.Run(() =>
            {
                long selectedId;
                var currentselection = _wellModel.FeatureLayer.GetSelection();
                using (var cursor = currentselection.Search())
                {
                    var hasrow = cursor.MoveNext();
                    using (var row = cursor.Current)
                    {
                        selectedId = Convert.ToInt64(row["OBJECTID"]);
                    }
                }

                _wellModel.AddNew(selectedId, _facilityModel.FacilityGuid, _facilityModel.CountyFips);
                NewWellSelected = false;
            });
        }


        private async void OnSelectionChanged(MapSelectionChangedEventArgs mse)
        {
            foreach (var kvp in mse.Selection)
            {
                if (!(kvp.Key is BasicFeatureLayer) || kvp.Key.Name != "UICWell")
                {
                    continue;
                }

                var selectedLayer = (BasicFeatureLayer)kvp.Key;
                //Is a feature selected? Is it an unassigned well feature?
                if (kvp.Value.Count > 0 && await IsUnassignedWell(selectedLayer))
                {
                    NewWellSelected = true;
                }
                else
                {
                    NewWellSelected = false;
                }
            }
         }

        public static Task<bool> IsUnassignedWell(BasicFeatureLayer selectedLayer)
        {
            return QueuedTask.Run(() => {
                bool noFacilityFk;
                bool noWellClass;

                var currentSelection = selectedLayer.GetSelection();
                using (var cursor = currentSelection.Search())
                {
                    var hasrow = cursor.MoveNext();
                    using (var row = cursor.Current)
                    {
                        noFacilityFk = string.IsNullOrWhiteSpace(Convert.ToString(row["Facility_FK"]));
                        noWellClass = string.IsNullOrWhiteSpace(Convert.ToString(row["WellClass"]));
                    }
                }

                return noFacilityFk && noWellClass;
            });
        }

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
        private string _heading = "My DockPane";
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
            var wellGuid = _wellModel.WellGuid;

            _inspectionModel.AddNew(wellGuid);
        }
}
}
