using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KBE.Models;

namespace KanjiUI.ViewModels
{
    public class ShellViewModel : MasterDetailViewModel<Object>
    {
        public static ObservableCollection<KanjiWord> KanjiWords = new();


        public override bool ApplyFilter(object item, string filter)
        {
            throw new NotImplementedException();
        }
    }
}
