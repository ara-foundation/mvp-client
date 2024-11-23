using Lean.Gui;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TL;
using TMPro;
using UnityEngine;
using WTelegram;

public class Chat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI MembersLabel;
    [SerializeField] private Content Content;
    [SerializeField] private LeanWindow LoadingWindow;

    private User _owner;
    public List<string> Members = new();
    public MessageContainer Container;

    private Client _client;
    private User _user;
    private ChatBase _chat;
    private InputPeer _peer;

    /**********************************************************/
    /* Loading more chats */
    /**********************************************************/
    private int messageLimit = 20;
    private int addOffset = -1;
    private int oldestOffsetId = 0;
    private bool _loading = false;
    private bool _loaded = false;

    public async void ReceiveMessage(Message message)
    {
        var content = message.Content;
        if (string.IsNullOrEmpty(content))
        {
            return;
        }

        TL.Message msg = null;
        if (_user != null)
        {
            msg = await _client.SendMessageAsync(_user.ToInputPeer(), content);
        } else
        {
            msg = await _client.SendMessageAsync(_chat.ToInputPeer(), content);
        }
        var updatedMessage = new Message(_owner, msg.message, msg.date);
        Container.AddMessage(updatedMessage);
    }

    private void Reset() => 
        Container = FindObjectOfType<MessageContainer>();

    private void Start()
    {
        LoadingWindow.TurnOff();    
    }

    public void SetOwner(User owner)
    {
        _owner = owner;
        Content.Clear();
    }

    public bool IsOwner(Message message)
    {
        if (message.Sender != null)
        {
            return message.Sender.ID == _owner.ID;
        }
        return true;
    }
    
    public void OnMessageAdd(IPeerInfo from, MessageBase msgBase)
    {
        Debug.Log($"Peer id = {_peer.ID}, msg = {msgBase.Peer.ID}");
        if (_peer == null)
        {
            return;
        }
        if (_peer.ID != msgBase.Peer.ID)
        {
            Debug.Log("Not for this message");
            return;
        }
        Debug.Log("Add message");

        AddMessage(from, msgBase, atFront: false);
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
        _client = client;
        Content.Clear();
        _user = null;
        _chat = chat;
        MembersLabel.gameObject.SetActive(true);
        MembersLabel.text = $"loading members info...";
        Title.text = $"{_chat.Title}";
        ShowMessages(client, chat.ToInputPeer(), topMessage);
    }

    void ShowMessages(Client client, InputPeer peer, int topMessage)
    {
        _client = client;
        _peer = peer;
        oldestOffsetId = topMessage;
        LoadMessages(old: false);
    }

    /// <summary>
    /// Latest messages:
    /// https://corefork.telegram.org/api/offsets
    /// </summary>
    async void LoadMessages(bool old)
    {
        var add_offset = addOffset;
        if (old)
        {
            add_offset = 0;
        }
        var messages = await _client.Messages_GetHistory(_peer, oldestOffsetId, add_offset: add_offset, limit: messageLimit);
        
        Array.Reverse(messages.Messages);

        if (messages.Messages.Length > 0)
        {
            var firstMsg = messages.Messages[0];
            oldestOffsetId = firstMsg.ID;
        }

        if (old)
        {
            Array.Reverse(messages.Messages);
        }

        foreach (var msgBase in messages.Messages)
        {
            var from = messages.UserOrChat(msgBase.From ?? msgBase.Peer); // from can be User/Chat/Channel
            var content = "";
            if (msgBase is TL.Message msg) // msg.media includes media attachment
                content = msg.message;
            // message service to see are we mentioned, etc
            //else if (msgBase is MessageService ms)
            //    content = ms.action.GetType().Name[13..];

            var message = new Message(from, content, msgBase.Date);
            if (old)
            {
                Container.AddMessageAtFront(message);
            } else
            {
                Container.AddMessage(message);
            }
        }
    }

    void AddMessage(IPeerInfo from, MessageBase msgBase, bool atFront)
    {
        Debug.Log($"From is done? {from == null}");
        var content = "";
        if (msgBase is TL.Message msg) // msg.media includes media attachment
            content = msg.message;
        // message service to see are we mentioned, etc
        //else if (msgBase is MessageService ms)
        //    content = ms.action.GetType().Name[13..];
        Message message;
        if (from == null)
        {
            Debug.Log("From is null");
            message = new Message(_owner, content, msgBase.Date);
        } else
        {
            Debug.Log("Yes");
            message = new Message(from, content, msgBase.Date);
        }
        if (atFront)
        {
            Container.AddMessageAtFront(message);
        }
        else
        {
            Container.AddMessage(message);
        }
    }

    public void OnScrolled(Vector2 vector2)
    {
        if (vector2.y >= 1)
        {
            if (_loading)
            {
                return;
            }

            _loading = true;
            _loaded = false;

            LoadingWindow.TurnOn();

            LoadMessages(old: true);

            LoadingWindow.TurnOff();

            _loaded = true;
        } else 
        {
            if (_loading && _loaded)
            {
                _loading = false;
                _loaded = false;
            }
        } 
    } 
}