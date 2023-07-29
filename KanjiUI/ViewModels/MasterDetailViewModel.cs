using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Math;
using Microsoft.UI.Xaml.Input;
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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasCurrent))]
        protected T current;

        partial void OnCurrentChanged(T value)
        {
            if (CurrentChanged is null)
            {
                return;
            }
            CurrentChanged();
        }

        protected delegate void CurrentChangedEvent();

        protected event CurrentChangedEvent CurrentChanged;

        public virtual ObservableCollection<T> Items => filter is null
            ? items 
            : new ObservableCollection<T>(items.Where(i => ApplyFilter(i, filter)));




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
