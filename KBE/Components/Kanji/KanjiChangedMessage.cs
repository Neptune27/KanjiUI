using CommunityToolkit.Mvvm.Messaging.Messages;
using KBE.Models;

namespace KBE.Components.Kanji
{
    public class KanjiChangedMessage : ValueChangedMessage<KanjiWord>
    {
        public KanjiChangedMessage(KanjiWord value) : base(value)
        {
        }
    }
}
