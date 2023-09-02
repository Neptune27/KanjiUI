using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Models
{
    public class RandoAutoSaveModel : RandoSaveModel
    {
        public bool IsEnable { get; set; }
        public string SaveLocation { get; set; } = string.Empty;
    }
}
