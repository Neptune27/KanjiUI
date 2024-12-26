using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KBE.Components.Kanji;
using KBE.Models;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KanjiUI.ViewModels;

public partial class FuriganaViewModel : MasterDetailViewModel<KanjiWord>
{

	[ObservableProperty]
	int maziiProgress = 0;

	[ObservableProperty]
	int jishoProgress = 0;

	[ObservableProperty]
	private Visibility progressVisibility = Visibility.Collapsed;

	[ObservableProperty]
	private bool inputBox = true;


	public ICommand OpenNewWindowCommand => new RelayCommand(OpenNewWindow_CommandExecuted);

	private void OpenNewWindow_CommandExecuted()
	{

		var window = new Shell();
		window.Activate();
	}

	public async Task Translate(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return;
		}

		var maziiProgress = new Progress<int>(percent => MaziiProgress = percent);

		var jishoProgress = new Progress<int>(percent => JishoProgress = percent);

		ProgressVisibility = Visibility.Visible;
		if (await KanjiController.GetKanjiNotInDatabaseFromInternet(value, jishoProgress, maziiProgress))
		{
			WeakReferenceMessenger.Default.Send(new KanjiUpdateMessage(true));

		}
		ProgressVisibility = Visibility.Collapsed;

		MaziiProgress = 0;
		JishoProgress = 0;



	}

	public override bool ApplyFilter(KanjiWord item, string filter)
	{
		throw new NotImplementedException();
	}
}
