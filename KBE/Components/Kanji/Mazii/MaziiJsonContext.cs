using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KBE.Components.Kanji.Mazii
{
    [JsonSerializable(typeof(MaziiAPI))]
    internal partial class MaziiJsonContext : JsonSerializerContext
    {
    }
}
