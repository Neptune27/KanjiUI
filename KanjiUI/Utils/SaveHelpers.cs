using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

namespace KanjiUI.Utils;

public class SaveHelpers
{

	public static async Task<StorageFile> OpenFileSaveAsync(string name, string extension)
	{
		var savePicker = new FileSavePicker();

		// Get the current window's HWND by passing in the Window object
		var hwnd = WindowNative.GetWindowHandle(App.Window);

		// Associate the HWND with the file picker
		InitializeWithWindow.Initialize(savePicker, hwnd);

		// Use file picker like normal!
		//folderPicker.FileTypeFilter.Add("*");
		savePicker.FileTypeChoices.Add($"{name}", [$"{extension}"]);
		savePicker.SuggestedFileName = "New Document";

		return await savePicker.PickSaveFileAsync();
	}
}
