using System;
using System.Linq;
using System.Threading.Tasks;
using ArcGIS.Core.Events;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using UIC_Edit_Workflow.Models;

namespace UIC_Edit_Workflow
{
    internal class Module1 : Module
    {
        private static Module1 _this;
        private static FacilityModel _facility;
        private static WellModel _well;
        private static AuthorizationModel _authorization;
        private static FacilityInspectionModel _facilityInspection;
        private static WellInspectionModel _wellInspection;

        /// <summary>
        ///     Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current => _this ?? (_this =
                                             (Module1) FrameworkApplication.FindModule("UIC_Edit_Workflow_Module"));

        public static FacilityModel FacilityModel => _facility ?? (_facility = new FacilityModel(FindLayer(FacilityModel.TableName) as FeatureLayer));
        public static WellModel WellModel => _well ?? (_well = new WellModel(FindLayer(WellModel.TableName) as FeatureLayer));

        public static AuthorizationModel AuthorizationModel => _authorization ??
                                                               (_authorization = new AuthorizationModel(FindTable(AuthorizationModel.TableName)));

        public static FacilityInspectionModel FacilityInspectionModel => _facilityInspection ??
                                                                         (_facilityInspection =
                                                                             new FacilityInspectionModel(FindTable(FacilityInspectionModel.TableName)));

        public static WellInspectionModel WellInspectionModel => _wellInspection ??
                                                                 (_wellInspection = new WellInspectionModel(FindTable(FacilityInspectionModel.TableName)));

        public static BasicFeatureLayer FindLayer(string layerName)
        {
            return QueuedTask.Run(() =>
            {
                var layers = MapView.Active?.Map.GetLayersAsFlattenedList();

                return (BasicFeatureLayer)layers?.FirstOrDefault(x => string.Equals(SplitLast(x.Name), SplitLast(layerName),
                                                                                   StringComparison.InvariantCultureIgnoreCase));
            }).Result;
        }

        public static StandaloneTable FindTable(string tableName)
        {
            return QueuedTask.Run(() =>
            {
                var tables = MapView.Active?.Map.StandaloneTables;
                return tables?.FirstOrDefault(x => string.Equals(SplitLast(x.Name), SplitLast(tableName),
                                                                StringComparison.InvariantCultureIgnoreCase));
            }).Result;
        }

        private static string SplitLast(string x)
        {
            if (!x.Contains('.'))
            {
                return x;
            }

            return x.Split('.').Last();
        }

        public static async Task<BasicFeatureLayer> FindLayerAsync(string layerName)
        {
           return await QueuedTask.Run(() =>
            {
                var layers = MapView.Active?.Map.GetLayersAsFlattenedList();

                return (BasicFeatureLayer)layers?.FirstOrDefault(x => string.Equals(SplitLast(x.Name), SplitLast(layerName),
                                                                                   StringComparison.InvariantCultureIgnoreCase));
            });
        }
        /// <summary>
        ///     Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            return true;
        }
    }
}
