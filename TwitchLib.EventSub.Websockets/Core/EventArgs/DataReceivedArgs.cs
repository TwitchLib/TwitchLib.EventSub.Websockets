namespace TwitchLib.EventSub.Websockets.Core.EventArgs;

internal class DataReceivedArgs : System.EventArgs
{
    public byte[] Bytes { get; internal set; }
}
