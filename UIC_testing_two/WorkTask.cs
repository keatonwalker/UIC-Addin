using System.Collections.ObjectModel;
using System.Linq;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using Serilog;
using UIC_Edit_Workflow.Views;

namespace UIC_Edit_Workflow
{
    internal class WorkTask : BindableBase
    {
        public WorkTask(string activePanel, IsTaskCompelted completeCheck = null)
        {
            Items = new ObservableCollection<WorkTask>();
            ActivePanel = activePanel;

            if (completeCheck != null)
            {
                IsComplete += completeCheck;
            }
            else
            {
                IsComplete += AreChildrenComplete;
            }

            Complete = false;
            IsExpanded = true;
        }

        public IsTaskCompelted IsComplete;
        private string _title; 
        public string Title {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private bool _complete;
        public bool Complete {
            get => _complete;
            set => SetProperty(ref _complete, value);
        }
        public string ActivePanel { get; set; }
        //public IsTaskCompelted IsCompelete { get; set; }

        private bool _isExpanded;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        bool _isSelected;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                SetProperty(ref _isSelected, value);
                if (_isSelected)
                {
                    Log.Verbose("{task} pane selected", ActivePanel);

                    QueuedTask.Run(() => {
                        Utils.RunOnUiThread(() =>
                        {
                            var pane = FrameworkApplication.DockPaneManager.Find(ActivePanel);

                            if (pane == null)
                            {
                                FrameworkApplication.AddNotification(new Notification
                                {
                                    Message = $"{ActivePanel} is null"
                                });

                                return;
                            }

                            if (pane is FacilityAttributeEditorViewModel facility) {
                                facility.FacilityModel = UicWorkflowModule.GetFacilityModel();
                            }

                            if (pane is WellAttributeEditorViewModel well) {
                                well.WellModel = UicWorkflowModule.GetWellModel();
                            }

                            if (pane is AuthAttributeEditorViewModel auth) {
                                auth.AuthModel = UicWorkflowModule.GetAuthorizationModel();
                            }

                            pane.Activate();
                        });
                    });
                }
            }
        }

        public bool CheckForCompletion()
        {
            var complete = IsComplete();
            Complete = complete;

            return complete;
        }

        public bool AreChildrenComplete() => Items.All(child => child.Complete);

        public ObservableCollection<WorkTask> Items { get; set; }
    }
}
