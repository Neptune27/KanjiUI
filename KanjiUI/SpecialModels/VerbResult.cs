using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiUI.SpecialModels;

internal partial class VerbResult : ObservableObject
{
    [ObservableProperty]
    private string verbType = string.Empty;

    [ObservableProperty]
    private string correctAnswer = string.Empty;

    [ObservableProperty]
    private string answer = string.Empty;

    [ObservableProperty]
    private Brush correctColor = new SolidColorBrush(Microsoft.UI.Colors.Green);

    [ObservableProperty]
    private Brush answerColor = new SolidColorBrush(Microsoft.UI.Colors.Green);

    public void AutoColor()
    {
        AnswerColor = Answer == CorrectAnswer ? AnswerColor : new SolidColorBrush(Microsoft.UI.Colors.DarkRed);
    }

}