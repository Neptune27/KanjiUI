using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KBE.Components.Kanji;
using KBE.Components.Settings;
using KBE.Components.Utils;
using KBE.Enums;
using KBE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using KanjiUI.Utils;

namespace KanjiUI.ViewModels;

public partial class RandoViewModel : MasterDetailViewModel<RandoWord>
{

    private readonly List<KanjiWord> wordList = new();

    [ObservableProperty]
    private ObservableCollection<RandoWord> items = new();

    public RandoViewModel()
    {
        WeakReferenceMessenger.Default.Register<SendKanjiMessage>(this, (r, m) =>
        {
            wordList.Clear();
            wordList.AddRange(m.Value.ToList());
            Reset();
        });


        CurrentChanged -= Display;
        CurrentChanged += Display;
    }

    private void Display()
    {
        OnPropertyChanged(nameof(DisplayQuestion));
        OnPropertyChanged(nameof(DisplayQuestionFontSize));
        OnPropertyChanged(nameof(DisplayA));
        OnPropertyChanged(nameof(DisplayB));
        OnPropertyChanged(nameof(DisplayC));
        OnPropertyChanged(nameof(DisplayD));
    }


    
    private readonly Setting setting = Setting.Instance;

    [ObservableProperty]
    private bool showAnswer = false;

    public string DisplayQuestion => HasCurrent ? DisplayRando(Current.Correct, setting.QuestionType) : null;
    public int DisplayQuestionFontSize => Setting.Instance.QuestionType == EKanjiShowingType.Kanji ? 150 : 32;
    public string DisplayA => HasCurrent ? DisplayRando(Current.A, setting.AnswerType) : null;
    public string DisplayB => HasCurrent ? DisplayRando(Current.B, setting.AnswerType) : null;
    public string DisplayC => HasCurrent ? DisplayRando(Current.C, setting.AnswerType) : null;
    public string DisplayD => HasCurrent ? DisplayRando(Current.D, setting.AnswerType) : null;


    public ICommand SelectOptionCommand => new RelayCommand<string>(SelectOption);
    public ICommand SubmitCommand => new RelayCommand(Submit);
    public ICommand ResetCommand => new RelayCommand(Reset);
    public ICommand SaveCommand => new AsyncRelayCommand(SaveDialog);

    private async Task SaveDialog()
    {
        var extensionOption = Setting.Instance.RandoSave.SaveAsType;
        var extension = extensionOption switch
        {
            ESaveAsType.TEXT => ".txt",
            ESaveAsType.DOCX => ".docx",
            _ => throw new NotImplementedException(),
        };


        var storageFile = await SaveHelpers.OpenFileSaveAsync("Rando", extension);
        await Save(storageFile.Path, Setting.Instance.RandoSave);
        
    }


    private async Task Save(string fileName, RandoSaveModel model)
    {
        var itemsList = model.SaveOption switch
        {
            ERandoSaveOption.ALL => Items.Select(r => r.GetCorrectWordWithColor()).ToList(),
            ERandoSaveOption.WRONG_ONLY => Items.Where(r => r.Color == EKanjiColor.RED)
            .Select(r => r.GetCorrectWordWithColor())
            .ToList(),
            ERandoSaveOption.RIGHT_ONLY => Items.Where(r => r.Color == EKanjiColor.GREEN).Select(r => r.GetCorrectWordWithColor()).ToList(),
            _ => throw new NotImplementedException(),
        };



        if (model.SaveAsType == ESaveAsType.DOCX)
        {

            if (!fileName.EndsWith(".docx"))
            {
                fileName += ".docx";
            }
            DocxProcessor processor = new("Rando", fileName);
            var resId = await processor.CreateKanjiDocument(itemsList, Setting.Instance.SaveOption);
        }
        else
        {
            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }
            TextProcessor processor = new("Rando", fileName + ".txt");
            await processor.CreateFile(itemsList, Setting.Instance.SaveOption);
        }


    }


    private void Reset()
    {

        ShowAnswer = false;
        foreach (var item in Items)
        {
            item.IsDone = false;
        }


        Items.Clear();

        var shuffleList = wordList.Shuffle();

        var correctWord = shuffleList.ToList();

        if (setting.TotalRandomLength != 0)
        {
            var wordToTake = setting.TotalRandomLength > shuffleList.Count() ? shuffleList.Count() : setting.TotalRandomLength;
            correctWord = shuffleList.Take(wordToTake).ToList();
        }

        for (int i = 0; i < correctWord.Count; i++)
        {
            var currentWord = correctWord[i];
            var listWithoutCorrectWord = shuffleList.AsParallel().Where(i => !ReferenceEquals(i, currentWord));
            RandoWord randoWord;

            if (!listWithoutCorrectWord.Any())
            {
                randoWord = new(currentWord, currentWord, currentWord, currentWord, currentWord);
            }
            else if (listWithoutCorrectWord.Count() < 4)
            {
                List<KanjiWord> randoWords = new() { currentWord, listWithoutCorrectWord.PickRandom(), 
                listWithoutCorrectWord.PickRandom(), listWithoutCorrectWord.PickRandom()};
                randoWords = randoWords.Shuffle().ToList();
                randoWord = new(randoWords[0], randoWords[1], randoWords[2], randoWords[3], currentWord);
            }
            else
            {
                List<KanjiWord> randoWords = new()
                {
                    currentWord
                };
                randoWords.AddRange(listWithoutCorrectWord.PickRandom(3));
                randoWords = randoWords.Shuffle()
                                       .ToList();
                randoWord = new(randoWords[0], randoWords[1], randoWords[2], randoWords[3], currentWord);
            }

            randoWord.Name = $"Q{i+1}";
            Items.Add(randoWord);

        }

        OnPropertyChanged(nameof(Items));

    }


    private void Submit()
    {
        
       

        ShowAnswer = true;
        foreach (var item in Items)
        {
            item.IsDone = true;
        }
        var oldCurrent = Current;

        Items = new(Items.Where(i => true));
        var newCurrent = Items.SingleOrDefault(i => i == oldCurrent);
        Current = newCurrent;


        if (Setting.Instance.RandoAutoSave.IsEnable)
        {
            DateTime now = DateTime.Now;
            var date = now.ToString("yyyy-MM-dd HH-mm-ss");
            _ = Save($"{Setting.Instance.RandoAutoSave.SaveLocation}\\{date}", Setting.Instance.RandoAutoSave);
        }
    }

    private void SelectOption(string obj)
    {
        if (ShowAnswer)
        {
            return;
        }

        Current.Selected = int.Parse(obj);

        if (setting.MoveNextAfterSelection)
        {
            var nextIndex = Items.IndexOf(Current) + 1;
            if (nextIndex >= Items.Count)
            {
                nextIndex = 0;
            }

            Current = Items[nextIndex];
        }


    }



    private static string DisplayRando(KanjiWord word, EKanjiShowingType type)
    {
        return type switch
        {
            EKanjiShowingType.Kanji => word.Kanji,
            EKanjiShowingType.English => word.English,
            EKanjiShowingType.SinoVietnamese => word.SinoVietnamese,
            EKanjiShowingType.Onyumi => word.Onyumi,
            EKanjiShowingType.Kunyumi => word.Kunyumi,
            EKanjiShowingType.Level => word.Level,
            EKanjiShowingType.Vietnamese => word.Vietnamese,
            EKanjiShowingType.Strokes => word.Strokes,
            EKanjiShowingType.Taught => word.Taught,
            EKanjiShowingType.Radicals => word.Radicals,
            _ => word.Kanji,
        };
    }


    public override bool ApplyFilter(RandoWord item, string filter)
    {
        throw new NotImplementedException();
    }
}
