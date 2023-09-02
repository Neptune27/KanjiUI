using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using KBE.Components.Settings;
using KBE.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KBE.Models;

public partial class RandoWord : ObservableObject
{

    public static RandoWord GenerateDummy()
    {
        var a = KanjiWord.GenerateDummy();
        a.Kanji = "A";
        var b = KanjiWord.GenerateDummy();
        b.Kanji = "B";
        var c = KanjiWord.GenerateDummy();
        var d = KanjiWord.GenerateDummy();
        d.Kanji = "D";
        return new RandoWord(a,b,c,d,c);
    }

    public RandoWord(KanjiWord a, KanjiWord b, KanjiWord c, KanjiWord d, KanjiWord correct) {
        

        A = a.Clone(); B = b.Clone(); C = c.Clone(); D = d.Clone(); Correct = correct.Clone();
    }
    public RandoWord() { }

    [ObservableProperty]
    private KanjiWord? a;

    [ObservableProperty]
    private KanjiWord? b;

    [ObservableProperty]
    private KanjiWord? c;

    [ObservableProperty]
    private KanjiWord? d;

    [ObservableProperty]
    private KanjiWord? correct;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Color))]

    private int selected = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResetCommand))]
    private bool isDone;

    private string name = "Q-1";
    
    public string Name
    {
        get
        {
            if (!isDone)
            {
                return name;
            }

            if (correct is null)
            {
                return name;
            }

            return correct.Kanji;
        }
        set
        {
            SetProperty(ref name, value);
        }

    }

    private ICommand ResetCommand => new RelayCommand(Reset);

    public void Reset()
    {
        A?.ResetColor();
        B?.ResetColor();
        C?.ResetColor();
        D?.ResetColor();
        selected = 0;
    }

    public string? Color => GetColor();

    public KanjiWord GetCorrectWordWithColor()
    {
        var correct = Correct;
        var selected = IntToKanjiConverter(this.Selected);

        var clonedCorrect = correct?.Clone();
        if (selected is not null)
        {
            clonedCorrect.Color = selected.Color;
        }

        return clonedCorrect;
    }

    partial void OnSelectedChanging(int value)
    {
        if (isDone)
        {
            return;
        }

        SetWordColor(selected, EKanjiColor.DEFAULT);
    }


    partial void OnIsDoneChanged(bool value)
    {
        var correct = Correct;
        var selected = IntToKanjiConverter(this.Selected);

        if (correct is null)
        {
            Debug.WriteLine("Huh, WTF is this on OnIsDoneChanged");
            return;
        }

        if (selected is null)
        {
            return;
        }



        correct.Color = EKanjiColor.GREEN;


        if (selected.Kanji != correct.Kanji)
        {
            selected.Color = EKanjiColor.RED;
        }

        if (A.Kanji == correct.Kanji)
        {
            A.Color = EKanjiColor.GREEN;
        }
        else if (B.Kanji == correct.Kanji)
        {
            B.Color = EKanjiColor.GREEN;
        }
        else if (C.Kanji == correct.Kanji)
        {
            C.Color = EKanjiColor.GREEN;
        }
        else if (D.Kanji == correct.Kanji)
        {
            D.Color = EKanjiColor.GREEN;
        }


        OnPropertyChanged(nameof(Color));
    }

    private string? GetColor()
    {
        if (Selected == 0) {
            return null;
        }

        if (!IsDone)
        {
            return EKanjiColor.YELLOW;
        }

        var selectedWord = IntToKanjiConverter(Selected);
        return selectedWord?.Color;

    }



    partial void OnSelectedChanged(int value)
    {
        if (IsDone) {
            return;
        }

        SetWordColor(Selected, EKanjiColor.YELLOW);

    }


    private KanjiWord? IntToKanjiConverter(int selected)
    {
        return selected switch
        {
            1 => A,
            2 => B,
            3 => C,
            4 => D,
            _ => null,
        };
    }

    private void SetWordColor(int selected, string color)
    {
        var word = IntToKanjiConverter(selected);
        if (word == null)
        {
            return;
        }

        word.Color = color;
    }





}
