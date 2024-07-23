using CommunityToolkit.Mvvm.ComponentModel;
using KBE.Components.Kanji;
using KBE.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiUI.ViewModels;

public partial class FuriganaViewModel : MasterDetailViewModel<KanjiWord>
{

	[ObservableProperty]
	int maziiProgress = 0;

	[ObservableProperty]
	int jishoProgress = 0;

	[ObservableProperty]
	private Visibility progressVisibility = Visibility.Collapsed;

	public async Task Translate(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return;
		}

		var maziiProgress = new Progress<int>(percent => MaziiProgress = percent);

		var jishoProgress = new Progress<int>(percent => JishoProgress = percent);

		ProgressVisibility = Visibility.Visible;
		await KanjiController.GetKanjiNotInDatabaseFromInternet(value, jishoProgress, maziiProgress);

		ProgressVisibility = Visibility.Collapsed;

		MaziiProgress = 0;
		JishoProgress = 0;



	}

	public override bool ApplyFilter(KanjiWord item, string filter)
	{
		throw new NotImplementedException();
	}
}
