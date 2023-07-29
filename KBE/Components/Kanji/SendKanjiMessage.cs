using CommunityToolkit.Mvvm.Messaging.Messages;
using KBE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Kanji
{
    public class SendKanjiMessage : ValueChangedMessage<IEnumerable<KanjiWord>>
    {
        public SendKanjiMessage(IEnumerable<KanjiWord> value) : base(value)
        {
        }
    }
}
