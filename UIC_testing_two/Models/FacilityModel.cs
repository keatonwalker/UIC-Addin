using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.Data;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;

namespace UIC_Edit_Workflow.Models
{
    internal class FacilityModel: ValidatableBindableBase, IWorkTaskModel
    {
        public readonly FeatureLayer FeatureLayer;
        public const string IdField = "FacilityID";
        public const string TableName = "UicFacility";

        public event ControllingIdChangeDelegate FacilityChanged;

        private FacilityModel()
        {
        }

        public FacilityModel(FeatureLayer facilityLayer) : this()
        {
            FeatureLayer = facilityLayer;
        }

        public long SelectedOid { get; set; }

        private string _uicFacilityId = "";
        [Required]
        public string UicFacilityId
        {
            get => _uicFacilityId;
            set => SetProperty(ref _uicFacilityId, value);
        }

        private string _facilityGuid;
        [Required]
        public string FacilityGuid
        {
            get => _facilityGuid;
            set => SetProperty(ref _facilityGuid, value);
        }

        private string _countyFips;
        [Required]
        public string CountyFips
        {
            get => _countyFips;
            set => SetProperty(ref _countyFips, value);
        }

        private string _naicsPrimary;
        [Required]
        public string NaicsPrimary
        {
            get => _naicsPrimary;
            set => SetProperty(ref _naicsPrimary, value);
        }

        private string _facilityName = "";
        [Required]
        [NameTest]
        public string FacilityName
        {
            get => _facilityName;
            set => SetProperty(ref _facilityName, value);
        }

        private string _facilityAddress;
        [Required]
        public string FacilityAddress
        {
            get => _facilityAddress;
            set => SetProperty(ref _facilityAddress, value);
        }

        private string _facilityCity;
        [Required]
        public string FacilityCity
        {
            get => _facilityCity;
            set => SetProperty(ref _facilityCity, value);
        }

        private string _facilityState;
        [Required]
        public string FacilityState
        {
            get => _facilityState;
            set => SetProperty(ref _facilityState, value);
        }

        private string _facilityZip;
        public string FacilityZip
        {
            get => _facilityZip;
            set => SetProperty(ref _facilityZip, value);
        }

        private string _facilityMilepost;
        public string FacilityMilepost
        {
            get => _facilityMilepost;
            set => SetProperty(ref _facilityMilepost, value);
        }

        private string _comments;
        public string Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }
        protected override string FieldValueString()
        {
            var sb = new StringBuilder();
            sb.Append(Convert.ToString(UicFacilityId));
            sb.Append(Convert.ToString(FacilityGuid));
            sb.Append(Convert.ToString(CountyFips));
            sb.Append(Convert.ToString(NaicsPrimary));
            sb.Append(Convert.ToString(FacilityName));
            sb.Append(Convert.ToString(FacilityAddress));
            sb.Append(Convert.ToString(FacilityCity));
            sb.Append(Convert.ToString(FacilityState));
            sb.Append(Convert.ToString(FacilityZip));
            sb.Append(Convert.ToString(FacilityMilepost));
            sb.Append(Convert.ToString(Comments));

            return sb.ToString();
        }
  
        public async Task UpdateModel(string facilityId)
        {
            if (UicFacilityId == facilityId)
            {
                return;
            }

            var oldFacId = FacilityGuid;
            await QueuedTask.Run(() =>
            {
                var qf = new QueryFilter
                {
                    WhereClause = $"FacilityID = '{facilityId}'"
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
                        UicFacilityId = Convert.ToString(row["FacilityID"]);
                        CountyFips = Convert.ToString(row["CountyFIPS"]);
                        NaicsPrimary = Convert.ToString(row["NAICSPrimary"]);
                        FacilityName = Convert.ToString(row["FacilityName"]);
                        FacilityAddress = Convert.ToString(row["FacilityAddress"]);
                        FacilityCity = Convert.ToString(row["FacilityCity"]);
                        FacilityState = Convert.ToString(row["FacilityState"]);
                        FacilityZip = Convert.ToString(row["FacilityZip"]);
                        FacilityMilepost = Convert.ToString(row["FacilityMilePost"]);
                        Comments = Convert.ToString(row["Comments"]);
                        FacilityGuid = Convert.ToString(row["GUID"]);
                    }
                }
            });

            LoadHash = CalculateFieldHash();
            FacilityChanged(oldFacId, FacilityGuid);
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
                var insp = new ArcGIS.Desktop.Editing.Attributes.Inspector();
                insp.Load(FeatureLayer, oidSet);

                insp["CountyFIPS"] = CountyFips;
                insp["NAICSPrimary"] = NaicsPrimary;
                insp["FacilityName"] = FacilityName;
                insp["FacilityAddress"] = FacilityAddress;
                insp["FacilityCity"] = FacilityCity;
                insp["FacilityState"] = FacilityState;
                insp["FacilityZip"] = FacilityZip;
                insp["FacilityMilePost"] = FacilityMilepost;
                insp["Comments"] = Comments;

                op.Modify(insp);
                op.Execute();
                Project.Current.SaveEditsAsync();
            });
        }

        //Validation
        public bool IsCountyFipsComplete()
        {
            var isFipsError = GetErrors("CountyFips") == null;

            return !string.IsNullOrEmpty(CountyFips) && CountyFips.Length == 5 && isFipsError;
        }

        public bool AreAttributesComplete()
        {
            return !HasErrors;
        }

        //Events
        public async void ControllingIdChangedHandler(string oldId, string facGuid)
        {
            await UpdateModel(facGuid);
        }
    }
}
