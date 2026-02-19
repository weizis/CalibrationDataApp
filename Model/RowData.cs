using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Space.Service
{
    public class RowData : INotifyPropertyChanged
    {
        public double Time { get; set; }
        public double Sun1X { get; set; }
        public double Sun1Y { get; set; }

        private double _sun1X_C;
        public double Sun1X_C
        {
            get => _sun1X_C;
            set { _sun1X_C = value; OnPropertyChanged(); }
        }

        private double _sun1Y_C;
        public double Sun1Y_C
        {
            get => _sun1Y_C;
            set { _sun1Y_C = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
