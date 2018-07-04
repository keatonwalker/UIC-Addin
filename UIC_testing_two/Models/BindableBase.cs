using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UIC_Edit_Workflow
{
    internal class BindableBase : INotifyPropertyChanged
    {
        public bool ModelDirty { get; set; }

        protected virtual void SetProperty<T>(ref T member, T val,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(member, val)) return;

            ModelDirty = true;
            member = val;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate {};
    }
}
