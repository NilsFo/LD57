using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class MessageSystem : MonoBehaviour
{
    public record Message : IEquatable<string>
    {
        public readonly string Text;
        public readonly bool Important;

        public Message(string text, bool important=false)
        {
            Text = text;
            Important = important;
        }

        public virtual bool Equals(string other)
        {
            return String.Equals(Text, other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, Important);
        }
    }
    
    /// Allow duplicate messages to be shown.
    /// If false (default), messages with text already present in the queue
    /// or the currently displayed message are discarded. 
    public bool allowDuplicates = false;

    public UIPhotoMessage messagePrefab;

    private Queue<Message> _msgQueue = new();
    [CanBeNull] private Message _currentMessage;
    [CanBeNull] private Sequence _currentSeq;


    // Update is called once per frame
    void Update()
    {
        if (_msgQueue.Count == 0)
            return;

        if (_currentSeq != null)
            return;

        Message msg = _msgQueue.Dequeue();
        ShowMessage(msg.Text);
        _currentMessage = msg;
    }

    public void EnqueueMessage(string text, bool important=false){
        if (important)
        {
            ClearMessages(importantToo:false, currentMessageToo:true);
        }
        var newMsg = new Message(text, important);
        if (!allowDuplicates)
        {
            if (_msgQueue.Contains(newMsg))
                return;
            if (_currentMessage != null && _currentMessage == newMsg)
                return;
        } 
        _msgQueue.Enqueue(newMsg);
    }

    public void ClearMessages(bool currentMessageToo = false, bool importantToo = true)
    {
        if (importantToo)
        {
            // Delete non-important messages
            var msgs = _msgQueue.ToList();
            _msgQueue.Clear();
            foreach (var message in msgs.Where(msg => msg.Important))
            {
                _msgQueue.Enqueue(message);
            }
        }
    
        if(currentMessageToo && _currentSeq != null)
            // Kill current message
            _currentSeq.Kill();
    }

    private void ShowMessage(string msg)
    {
        Debug.Log("Showing Message: " + msg);
        var msgObj = Instantiate(messagePrefab, transform);
        msgObj.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        var seq = DOTween.Sequence();
        seq.Append(msgObj.transform.DOMove(transform.position + Vector3.down * 200, 0.4f).From());
        seq.AppendInterval(3f);
        seq.Append(msgObj.transform.DOMove(transform.position + Vector3.down * 200, 0.3f));
        seq.onComplete = () =>
        {
            Destroy(msgObj.gameObject);
            _currentSeq = null;
            _currentMessage = null;
        };
        seq.onKill = () =>
        {
            Destroy(msgObj.gameObject);
            _currentSeq = null;
            _currentMessage = null;
        };
        seq.Play();
        _currentSeq = seq;
    }

}