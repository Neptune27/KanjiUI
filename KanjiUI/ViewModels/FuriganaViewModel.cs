using KBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiUI.ViewModels;

class FuriganaViewModel : MasterDetailViewModel<KanjiWord>
{


	public override bool ApplyFilter(KanjiWord item, string filter)
	{
		throw new NotImplementedException();
	}
}
