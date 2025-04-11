using DoushiKatsu;
using KBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiUI.ViewModels;

internal partial class VerbPerformViewModel : MasterDetailViewModel<VerbConjure>
{

    public VerbPerformViewModel()
    {
            
    }

    public override bool ApplyFilter(VerbConjure item, string filter)
    {
        throw new NotImplementedException();
    }
}
