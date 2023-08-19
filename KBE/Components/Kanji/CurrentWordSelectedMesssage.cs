using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Kanji;

public class CurrentWordSelectedMesssage : ValueChangedMessage<char>
{

    public CurrentWordSelectedMesssage(char value) : base(value)
    {
    }
}
