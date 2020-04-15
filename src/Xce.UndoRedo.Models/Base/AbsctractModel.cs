using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xce.UndoRedo.Models.Interfaces;

namespace Xce.UndoRedo.Models.Base
{
    public abstract class AbsctractModel : INotifyPropertyChanged, IAbsctractModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Initialize() { }

        protected virtual bool SetProperty<TObject, TValue>(TObject item, ref TValue field, TValue value, [CallerMemberName] string callerName = null)
        {
            if (field == null && value == null || (field?.Equals(value) ?? false))
                return false;

            OnBeforeSetProperty(item, field, value, callerName);

            field = value;

            OnAfterSetProperty(item, field, value, callerName);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));

            return true;
        }

        protected virtual void OnBeforeSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName) { }

        protected virtual void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName) { }
    }
}
