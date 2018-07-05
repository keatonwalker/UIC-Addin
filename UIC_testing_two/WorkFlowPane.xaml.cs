namespace UIC_Edit_Workflow
{
    /// <summary>
    /// Interaction logic for WorkFlowPaneView.xaml
    /// </summary>
    public partial class WorkFlowPaneView
    {
        readonly WorkFlowPaneViewModel _taskTrackingViewModel;
        public WorkFlowPaneView()
        {
            InitializeComponent();
            DataContext = _taskTrackingViewModel;
        }
    }
}
