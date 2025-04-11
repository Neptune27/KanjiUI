using KBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiUI.ViewModels;

public partial class VerbConjunctionViewModel : MasterDetailViewModel<Object>
{
    public override bool ApplyFilter(object item, string filter)
    {
        throw new NotImplementedException();
    }
}
