using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace UIC_Edit_Workflow {
    public class WorkFlowBehaviors : Behavior<Button> {
        // Using a DependencyProperty as the backing store for NeedsSave.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeedsSaveProperty =
            DependencyProperty.Register("NeedsSave", typeof(bool), typeof(WorkFlowBehaviors),
                                        new PropertyMetadata(false, OnNeedsSaveChanged));

        public bool NeedsSave {
            get => (bool)GetValue(NeedsSaveProperty);
            set => SetValue(NeedsSaveProperty, value);
        }

        private static void OnNeedsSaveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var behavior = (WorkFlowBehaviors)d;
            var needsSave = (bool)e.NewValue;

            behavior.AssociatedObject.Visibility = needsSave ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
