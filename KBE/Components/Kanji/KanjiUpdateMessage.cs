using CommunityToolkit.Mvvm.Messaging.Messages;
using KBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Kanji
{
    public class KanjiUpdateMessage : ValueChangedMessage<bool>
    {
        public KanjiUpdateMessage(bool value) : base(value)
        {
        }
    }
}
