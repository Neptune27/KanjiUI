using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiUI.ViewModels
{
    public abstract partial class MasterDetailViewModel<T> : ObservableObject
    {
        private readonly ObservableCollection<T> items = new();

        private T current;

        public virtual ObservableCollection<T> Items => filter is null
            ? items 
            : new ObservableCollection<T>(items.Where(i => ApplyFilter(i, filter)));

        public T Current { 
            get => current;
            set {
                Debug.WriteLine(value);
                SetProperty(ref current, value);
                OnPropertyChanged(nameof(HasCurrent));
            }
        }

        public bool HasCurrent => current is not null;

        public virtual T UpdateItem(T item, T original)
        {
            var hasCurrent = HasCurrent;

            var i = items.IndexOf(original);
            items[i] = item; // Raises CollectionChanged.

            if (hasCurrent && !HasCurrent)
            {
                // Restore Current.
                Current = item;
            }

            return item;
        }

        
        protected string filter;


        public string Filter
        {
            get => filter;
            set
            {
                var current = Current;

                SetProperty(ref filter, value);
                OnPropertyChanged(nameof(Items));

                if (current is not null && Items.Contains(current))
                {
                    Current = current;
                }
            }
        }

        public abstract bool ApplyFilter(T item, string filter);

    }
}
