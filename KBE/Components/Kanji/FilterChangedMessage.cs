using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Kanji
{
    public class FilterChangedMessage : ValueChangedMessage<string>
    {
        public FilterChangedMessage(string value) : base(value)
        {
        }
    }
}
