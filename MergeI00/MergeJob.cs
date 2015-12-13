using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MergeI00
{
    public class MergeJob : INotifyPropertyChanged
    {
        public string IsoName { get; set; }
        public List<string> Parts { get; set; }

        private int count;
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                OnPropertyChanged();
            }
        }

        private int total;
        public int Total
        {
            get { return total; }
            set
            {
                total = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
