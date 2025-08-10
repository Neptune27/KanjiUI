using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KBE.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KanjiUI.ViewModels;

public partial class ShellViewModel : MasterDetailViewModel<Object>
{
    public static ObservableCollection<KanjiWord> KanjiWords = new();

    public ICommand GoBackCommand => new RelayCommand(GoBack);

    private void GoBack()
    {
        if (Frame.CanGoBack)
            Frame.GoBack();
        TriggerFrameCheck();
    }

    [ObservableProperty]
    public partial Frame? Frame { get; set; }

    partial void OnFrameChanged(Frame value)
    {
        TriggerFrameCheck();
    }

    [ObservableProperty]
    public partial bool CanGoBack { get; set; } = false;

    public void TriggerFrameCheck()
    {
        if (Frame is null)
        {
            CanGoBack = false;
        }

        CanGoBack = Frame.CanGoBack;
    }



    public override bool ApplyFilter(object item, string filter)
    {
        throw new NotImplementedException();
    }
}
