using CommunityToolkit.Mvvm.ComponentModel;
using KBE.Components.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Models
{
    public class RandoSaveModel
    {
        public ESaveAsType SaveAsType { get; set; }

        public ERandoSaveOption SaveOption { get; set; }
    }
}
