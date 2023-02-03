using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KBE.Components.Settings
{
    [JsonSerializable(typeof(Setting))]
    internal partial class SettingJsonContext : JsonSerializerContext
    {
    }
}
