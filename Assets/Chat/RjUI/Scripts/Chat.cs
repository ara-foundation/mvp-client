using System;
using System.Collections.Generic;
using TL;
using TMPro;
using UnityEngine;
using WTelegram;

public class Chat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI MembersLabel;
    [SerializeField] private Content Content;

    private User _owner;
    public List<string> Members = new();
    public MessageContainer Container;

    private User _user;
    private ChatBase _chat;
  
    public void ReceiveMessage(Message message) => 
        Container.AddMessage(message);

    private void Reset() => 
        Container = FindObjectOfType<MessageContainer>();
    

    public void SetOwner(User owner)
    {
        _owner = owner;
        Content.Clear();
    }

    public bool IsOwner(IPeerInfo peer)
    {
        return peer.ID == _owner.ID;
    } 

    public void Show(Client client, User user, int topMessage)
    {
        Content.Clear();
        _user = user;
        _chat = null;
        MembersLabel.gameObject.SetActive(false);
        Title.text = $"{user.first_name} {user.last_name}";
        ShowMessages(client, user.ToInputPeer(), topMessage);
    }

    public void Show(Client client, ChatBase chat, int topMessage)
    {
        Content.Clear();
        _user = null;
        _chat = chat;
        MembersLabel.gameObject.SetActive(true);
        MembersLabel.text = $"loading members info...";
        Title.text = $"{_chat.Title}";
        ShowMessages(client, chat.ToInputPeer(), topMessage);
    }

    async void ShowMessages(Client client, InputPeer peer, int topMessage)
    {
        Debug.Log("Show the messages...");
        // Latest messages:
        // https://corefork.telegram.org/api/offsets
        var messages = await client.Messages_GetHistory(peer, offset_id: topMessage, add_offset: -1, limit: 40);
        Array.Reverse(messages.Messages);
        foreach (var msgBase in messages.Messages)
        {
            var from = messages.UserOrChat(msgBase.From ?? msgBase.Peer); // from can be User/Chat/Channel
            var content = "";
            if (msgBase is TL.Message msg) // msg.media includes media attachment
                content = msg.message;
            // message service to see are we mentioned, etc
            //else if (msgBase is MessageService ms)
            //    content = ms.action.GetType().Name[13..];

            var message = new Message(from, content)
            {
                SendTime = msgBase.Date
            };
            Container.AddMessage(message);
        }

        Debug.LogWarning("When scrolled up, load the message uppercase");
    }
}