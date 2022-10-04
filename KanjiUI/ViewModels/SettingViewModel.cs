using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

using KBE.Models;

namespace KanjiUI.ViewModels
{
    internal class SettingViewModel : MasterDetailViewModel<SettingModel>
    {

        public SettingViewModel() {
            Items.Add(new SettingModel());   
        }


        public ICommand SaveCommand => new RelayCommand<string>(SaveCommand_Executed);
        public ICommand ResetCommand => new RelayCommand<string>(ResetCommand_Executed);

        public override bool ApplyFilter(SettingModel item, string filter)
        {
            throw new NotImplementedException();
        }

        private void SaveCommand_Executed(String parm)
        {
            Items[0].Save();
        }

        private void ResetCommand_Executed(String parm)
        {
            OnPropertyChanging(nameof(Items));
            Items[0].SetDefault();
            OnPropertyChanged(nameof(Items));
        }

    }
}
