using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Editing.Attributes;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using UIC_Edit_Workflow.Validations;

namespace UIC_Edit_Workflow.Models
{
    internal class WellModel : ValidatableBindableBase, IWorkTaskModel
    {
        public const string IdField = "WellID";
        public const string TableName = "UICWell";

        private readonly ObservableCollection<string> _facilityWellIds = new ObservableCollection<string>();
        private readonly object _lockCollection = new object();
        public readonly FeatureLayer FeatureLayer;

        private string _comments;

        // Fields not yet used, but they will be used eventually
        private string _createdOn;

        private string _editedBy;

        private string _highPriority;

        private string _locationAccuracy;

        private string _locationMethod;
        private string _modifiedOn;

        private string _selectedWellId;
        private string _surfaceElevation;

        private string _wellClass;

        private string _wellGuid;


        private string _wellId;

        private string _wellName;

        private string _wellSubClass;

        private string _wellSwpz;

        private WellModel()
        {
            WellIds = new ReadOnlyObservableCollection<string>(_facilityWellIds);
            Utils.RunOnUiThread(() => { BindingOperations.EnableCollectionSynchronization(WellIds, _lockCollection); });
        }

        public WellModel(FeatureLayer featureLayer) : this()
        {
            if (featureLayer == null)
            {
                FrameworkApplication.AddNotification(new Notification
                {
                    Message = $"The {TableName} layer could not be found. Please add it."
                });
            }
            FeatureLayer = featureLayer;
        }

        public ReadOnlyObservableCollection<string> WellIds { get; }

        public string SelectedWellId
        {
            get => _selectedWellId;

            set
            {
                SetProperty(ref _selectedWellId, value);
                if (_selectedWellId != null)
                {
                    // TODO change to method
                    UpdateModel(_selectedWellId);
                }
            }
        }

        public long SelectedOid { get; set; }

        [Required]
        public string WellId
        {
            get => _wellId;
            set => SetProperty(ref _wellId, value);
        }

        [Required]
        [UicValidations(ErrorMessage = "{0} is not correct")]
        public string WellName
        {
            get => _wellName;
            set => SetProperty(ref _wellName, value);
        }

        [Required]
        public string WellClass
        {
            get => _wellClass;
            set => SetProperty(ref _wellClass, value);
        }

        [Required]
        public string WellSubClass
        {
            get => _wellSubClass;
            set => SetProperty(ref _wellSubClass, value);
        }

        [Required]
        public string HighPriority
        {
            get => _highPriority;
            set => SetProperty(ref _highPriority, value);
        }

        [Required]
        public string WellSwpz
        {
            get => _wellSwpz;
            set => SetProperty(ref _wellSwpz, value);
        }

        [Required]
        public string LocationMethod
        {
            get => _locationMethod;
            set => SetProperty(ref _locationMethod, value);
        }

        [Required]
        public string LocationAccuracy
        {
            get => _locationAccuracy;
            set => SetProperty(ref _locationAccuracy, value);
        }

        public string Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }

        public string WellGuid
        {
            get => _wellGuid;
            set => SetProperty(ref _wellGuid, value);
        }

        public async Task UpdateModel(string wellId)
        {
            var oldWellGuid = WellGuid;
            await QueuedTask.Run(() =>
            {
                if (string.IsNullOrEmpty(wellId))
                {
                    SelectedOid = -1;
                    WellId = "";
                    WellName = "";
                    WellClass = "";
                    WellSubClass = "";
                    HighPriority = "";
                    WellSwpz = "";
                    LocationMethod = "";
                    LocationAccuracy = "";
                    Comments = "";
                    WellGuid = "";
                }
                else
                {
                    var qf = new QueryFilter
                    {
                        WhereClause = $"WellID = '{wellId}'"
                    };
                    using (var cursor = FeatureLayer.Search(qf))
                    {
                        var hasRow = cursor.MoveNext();
                        if (!hasRow)
                        {
                            return;
                        }

                        using (var row = cursor.Current)
                        {
                            SelectedOid = Convert.ToInt64(row["OBJECTID"]);
                            WellId = Convert.ToString(row["WellID"]);
                            WellName = Convert.ToString(row["WellName"]);
                            WellClass = Convert.ToString(row["WellClass"]);
                            WellSubClass = Convert.ToString(row["WellSubClass"]);
                            HighPriority = Convert.ToString(row["HighPriority"]);
                            WellSwpz = Convert.ToString(row["WellSWPZ"]);
                            LocationMethod = Convert.ToString(row["LocationMethod"]);
                            LocationAccuracy = Convert.ToString(row["LocationAccuracy"]);
                            Comments = Convert.ToString(row["Comments"]);
                            WellGuid = Convert.ToString(row["GUID"]);
                        }
                    }
                }
            });

            LoadHash = CalculateFieldHash();
            WellChanged(oldWellGuid, WellGuid);
        }

        public Task SaveChanges()
        {
            return QueuedTask.Run(() =>
            {
                //Create list of oids to update
                var oidSet = new List<long>
                {
                    SelectedOid
                };
                //Create edit operation and update
                var op = new EditOperation
                {
                    Name = "Update Feature"
                };
                var insp = new Inspector();
                insp.Load(FeatureLayer, oidSet);

                insp["WellName"] = WellName;
                insp["WellClass"] = WellClass;
                insp["WellSubClass"] = WellSubClass;
                insp["HighPriority"] = HighPriority;
                insp["WellSWPZ"] = WellSwpz;
                insp["LocationMethod"] = LocationMethod;
                insp["LocationAccuracy"] = LocationAccuracy;

                op.Modify(insp);
                op.Execute();
                Project.Current.SaveEditsAsync();
            });
        }

        public async void ControllingIdChangedHandler(string oldId, string facGuid)
        {
            await AddIdsForFacility(facGuid);
            SelectedWellId = WellIds.FirstOrDefault();
        }

        public event ControllingIdChangeDelegate WellChanged;

        public async Task AddIdsForFacility(string facilityId)
        {
            await QueuedTask.Run(() =>
            {
                _facilityWellIds.Clear();
                var qf = new QueryFilter
                {
                    WhereClause = $"Facility_FK = '{facilityId}'"
                };
                using (var cursor = FeatureLayer.Search(qf))
                {
                    while (cursor.MoveNext())
                    {
                        using (var row = cursor.Current)
                        {
                            _facilityWellIds.Add(Convert.ToString(row[IdField]));
                        }
                    }
                }
            });
        }

        public async void AddNew(long objectId, string facilityGuid, string countyFips)
        {
            await QueuedTask.Run(() =>
            {
                //Create list contianing the new well OID
                var oidSet = new List<long>
                {
                    objectId
                };
                //Create edit operation and update
                var op = new EditOperation
                {
                    Name = "Update date"
                };
                var insp = new Inspector();
                insp.Load(FeatureLayer, oidSet);

                long.TryParse(countyFips, out long fips);

                insp["Facility_FK"] = facilityGuid;

                var newGuid = Guid.NewGuid();
                var guidLast8 = newGuid.ToString();
                guidLast8 = guidLast8.Substring(guidLast8.Length - 8);
                insp["GUID"] = newGuid;

                var newWellId = $"UTU{countyFips.Substring(countyFips.Length - 2)}{insp["WellClass"]}{guidLast8}"
                    .ToUpper();
                insp[IdField] = newWellId;

                op.Modify(insp);
                op.Execute();
                _facilityWellIds.Add(newWellId);
                SelectedWellId = newWellId;
            });
        }

        public bool IsWellAttributesComplete()
        {
            return !string.IsNullOrEmpty(WellId) &&
                   !string.IsNullOrEmpty(WellName) &&
                   !string.IsNullOrEmpty(WellClass) &&
                   !string.IsNullOrEmpty(WellSubClass) &&
                   !string.IsNullOrEmpty(HighPriority) &&
                   !string.IsNullOrEmpty(WellSwpz);
        }

        public bool IsWellNameCorrect()
        {
            var isWellNameError = GetErrors("WellName") == null;

            return !string.IsNullOrEmpty(WellName) && isWellNameError;
        }

        protected override string FieldValueString()
        {
            var sb = new StringBuilder();
            sb.Append(Convert.ToString(WellId));
            sb.Append(Convert.ToString(WellName));
            sb.Append(Convert.ToString(WellClass));
            sb.Append(Convert.ToString(WellSubClass));
            sb.Append(Convert.ToString(HighPriority));
            sb.Append(Convert.ToString(WellSwpz));
            sb.Append(Convert.ToString(LocationMethod));
            sb.Append(Convert.ToString(LocationAccuracy));
            sb.Append(Convert.ToString(Comments));
            sb.Append(Convert.ToString(WellGuid));

            return sb.ToString();
        }
    }
}
