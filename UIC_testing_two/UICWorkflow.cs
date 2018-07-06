using System;
using System.Linq;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using Serilog;
using UIC_Edit_Workflow.Models;

namespace UIC_Edit_Workflow {
    internal class UicWorkflowModule : Module {
        private static UicWorkflowModule _this;
        private static FacilityModel _facility;
        private static WellModel _well;
        private static AuthorizationModel _authorization;
        private static FacilityInspectionModel _facilityInspection;
        private static WellInspectionModel _wellInspection;

        /// <summary>
        ///     Retrieve the singleton instance to this module here
        /// </summary>
        public static UicWorkflowModule Current => _this ?? (_this =
                                                       (UicWorkflowModule)
                                                       FrameworkApplication.FindModule("UicForkflowModule"));

        public static FacilityModel GetFacilityModel(MapView view = null) {
            Log.Debug("getting/creating the facility model");

            var activeView = MapView.Active ?? view;
            var featureLayer = FindLayer(FacilityModel.TableName, activeView) as FeatureLayer;

            return _facility ?? (_facility = new FacilityModel(featureLayer));
        }

        public static WellModel GetWellModel(MapView view = null) {
            Log.Debug("getting/creating the well model");

            var activeView = MapView.Active ?? view;
            var featureLayer = FindLayer(WellModel.TableName, activeView) as FeatureLayer;

            return _well ?? (_well = new WellModel(featureLayer));
        }

        public static AuthorizationModel GetAuthorizationModel(MapView view = null) {
            Log.Debug("getting/creating the authorization model");

            var activeView = MapView.Active ?? view;
            var standaloneTable = FindTable(AuthorizationModel.TableName, activeView);

            return _authorization ?? (_authorization = new AuthorizationModel(standaloneTable));
        }

        public static FacilityInspectionModel GetFacilityInspectionModel(MapView view = null) {
            Log.Debug("getting/creating the facility inspection model");

            var activeView = MapView.Active ?? view;
            var standaloneTable = FindTable(FacilityInspectionModel.TableName, activeView);

            return _facilityInspection ?? (_facilityInspection = new FacilityInspectionModel(standaloneTable));
        }

        public static WellInspectionModel GetWellInspectionModel(MapView view = null) {
            Log.Debug("getting/creating the well inspection model");

            var activeView = MapView.Active ?? view;
            var standaloneTable = FindTable(WellInspectionModel.TableName, activeView);

            return _wellInspection ?? (_wellInspection = new WellInspectionModel(standaloneTable));
        }

        public static BasicFeatureLayer FindLayer(string layerName, MapView activeView) {
            Log.Verbose("finding feature layer {layer}", layerName);

            var layers = activeView.Map.GetLayersAsFlattenedList();

            return (BasicFeatureLayer)layers.FirstOrDefault(x => string.Equals(SplitLast(x.Name),
                                                                               SplitLast(layerName),
                                                                               StringComparison.InvariantCultureIgnoreCase));
        }

        public static StandaloneTable FindTable(string tableName, MapView activeView) {
            Log.Verbose("finding table {layer}", tableName);

            var tables = activeView.Map.StandaloneTables;

            return tables.FirstOrDefault(x => string.Equals(SplitLast(x.Name), SplitLast(tableName),
                                                            StringComparison.InvariantCultureIgnoreCase));
        }

        private static string SplitLast(string x) {
            if (!x.Contains('.')) {
                return x;
            }

            return x.Split('.').Last();
        }

        public static async Task<BasicFeatureLayer> FindLayerAsync(string layerName) => await QueuedTask.Run(() => {
            Log.Verbose("finding feature layer async {layer}", layerName);

            var layers = MapView.Active?.Map.GetLayersAsFlattenedList();

            var specificLayer = (BasicFeatureLayer)layers?.FirstOrDefault(x => string.Equals(SplitLast(x.Name),
                                                                                             SplitLast(layerName),
                                                                                             StringComparison
                                                                                                 .InvariantCultureIgnoreCase));

            return specificLayer;
        });

        /// <summary>
        ///     Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload() => true;
    }
}
