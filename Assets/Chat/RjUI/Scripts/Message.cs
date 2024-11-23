using System;
using TL;

[Serializable]
public class Message
{
    public User Owner;
    public IPeerInfo Sender;
    public string Content;
    public DateTime SendTime;

    public Message(IPeerInfo sender, string content)
    {
        Owner = null;
        Sender = sender;
        Content = content;
        SendTime = DateTime.Now;
    }

    public Message(IPeerInfo sender, string content,  DateTime sendTime)
    {
        Owner = null;
        Sender = sender;
        Content = content;
        SendTime = sendTime;
    }

    public Message(string content)
    {
        Owner = null;
        Sender = null;
        Content = content;
    }

    public Message(User owner, string content)
    {
        Owner = owner;
        Sender = null;
        Content = content;
    }

    public Message(User owner, string content, DateTime sendTime)
    {
        Owner = owner;
        Sender = null;
        Content = content;
        SendTime = sendTime;
    }
}