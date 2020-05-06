using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VectronsLibrary.TextBlockLogger.Internal
{
    internal class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<T>(T previousValue, T currentValue, [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs<T>(propertyName, previousValue, currentValue));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            var oldValue = field;
            field = value;
            OnPropertyChanged(oldValue, field, propertyName);
            return true;
        }
    }
}