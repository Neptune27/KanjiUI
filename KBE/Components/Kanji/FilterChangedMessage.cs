using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KBE.Components.Kanji
{
    public class FilterChangedMessage : ValueChangedMessage<string>
    {
        public FilterChangedMessage(string value) : base(value)
        {
        }
    }
}
