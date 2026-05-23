using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Conscripts.Messages
{
    internal sealed class RedirectedActivationArgumentsMessage : ValueChangedMessage<string?>
    {
        public RedirectedActivationArgumentsMessage(string value) : base(value)
        {
        }
    }
}
