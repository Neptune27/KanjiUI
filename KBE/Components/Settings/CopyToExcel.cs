using CommunityToolkit.Mvvm.ComponentModel;
using KBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Settings;

public partial class CopyToExcel
{
    public CopyToExcel() { }

	public CopyToExcel(bool isEnable, string name)
	{
		IsEnable = isEnable;
		Name = name;
	}
	
	public CopyToExcel(CopyToExcelOption option)
	{
		IsEnable = option.IsEnable;
		Name = option.Name;
	}

	public bool IsEnable { get; set; } = true;

    public string Name { get; set; } = string.Empty;
}
