using CommunityToolkit.Mvvm.ComponentModel;
using KBE.Components.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Models;

public partial class CopyToExcelOption : ObservableObject
{
	public string Name { get; set; } = string.Empty;

	[ObservableProperty]
	private bool isEnable = false;

	public CopyToExcelOption(string name, bool isEnable)
	{
		Name = name;
		IsEnable = isEnable;
	}

	public CopyToExcelOption(CopyToExcel model)
	{
		this.Name = model.Name;
		this.IsEnable = model.IsEnable;
	}
}
