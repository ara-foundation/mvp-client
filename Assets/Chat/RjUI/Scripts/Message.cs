using System;
using TL;

[Serializable]
public class Message
{
  public IPeerInfo Sender;
  public string Content;
  public DateTime SendTime;

  public Message(IPeerInfo sender, string content)
  {
    Sender = sender;
    Content = content;
    SendTime = DateTime.Now;
  }
}