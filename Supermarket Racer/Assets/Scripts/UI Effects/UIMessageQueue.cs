using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMessageQueue : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private UIMessageRender messageRender;

    private readonly Queue<UIMessageData> messageQueue = new Queue<UIMessageData>();
    private Coroutine queueRoutine;
    private bool isProcessingQueue;

    public bool IsProcessingQueue => isProcessingQueue;

    private void Awake()
    {
        if (messageRender == null)
        {
            Debug.LogError("[UIMessageQueue] No UIMessageRenderer assigned.");
        }
    }

    public void EnqueueMessage(UIMessageData messageData)
    {
        if (messageData == null)
        {
            Debug.LogWarning("[UIMessageQueue] Tried to enqueue a null message.");
            return;
        }

        messageQueue.Enqueue(messageData);

        if (!isProcessingQueue)
        {
            queueRoutine = StartCoroutine(ProcessQueue());
        }
    }

    public void EnqueueMessages(List<UIMessageData> messages)
    {
        if (messages == null || messages.Count == 0)
        {
            Debug.LogWarning("[UIMessageQueue] No messages provided to enqueue.");
            return;
        }

        for (int i = 0; i < messages.Count; i++)
        {
            if (messages[i] != null)
            {
                messageQueue.Enqueue(messages[i]);
            }
        }

        if (!isProcessingQueue)
        {
            queueRoutine = StartCoroutine(ProcessQueue());
        }
    }

    public void ClearQueue()
    {
        messageQueue.Clear();

        if (queueRoutine != null)
        {
            StopCoroutine(queueRoutine);
            queueRoutine = null;
        }

        isProcessingQueue = false;

        if (messageRender != null)
        {
            messageRender.ClearImmediately();
        }
    }

    public void SkipCurrentMessage()
    {
        if (messageRender != null)
        {
            messageRender.SkipCurrentMessage();
        }
    }

    private IEnumerator ProcessQueue()
    {
        isProcessingQueue = true;

        while (messageQueue.Count > 0)
        {
            UIMessageData nextMessage = messageQueue.Dequeue();
            messageRender.PlayMessage(nextMessage);

            while (messageRender.IsPlaying)
            {
                yield return null;
            }
        }

        isProcessingQueue = false;
        queueRoutine = null;
    }

    public void PlayCheckedOut(Color colour)
    {
        UIMessageData message = messageRender.CreateDefaultMessage("CHECKED OUT!", colour);
        EnqueueMessage(message);
    }

    public void PlayGameOver(Color colour)
    {
        UIMessageData message = messageRender.CreateDefaultMessage("Game Over", colour);
        message.characterDelay = 0.08f;
        message.holdDuration = 1.5f;
        EnqueueMessage(message);
    }

    public void PlayCountdown(AudioClip tickSound = null)
    {
        UIMessageData three = messageRender.CreateDefaultMessage("3", Color.white);
        three.characterDelay = 0.02f;
        three.holdDuration = 0.6f;
        three.letterSound = tickSound;

        UIMessageData two = messageRender.CreateDefaultMessage("2", Color.white);
        two.characterDelay = 0.02f;
        two.holdDuration = 0.6f;
        two.letterSound = tickSound;

        UIMessageData one = messageRender.CreateDefaultMessage("1", Color.white);
        one.characterDelay = 0.02f;
        one.holdDuration = 0.6f;
        one.letterSound = tickSound;

        UIMessageData go = messageRender.CreateDefaultMessage("GO!", Color.green);
        go.characterDelay = 0.04f;
        go.holdDuration = 0.8f;
        go.letterSound = tickSound;

        EnqueueMessage(three);
        EnqueueMessage(two);
        EnqueueMessage(one);
        EnqueueMessage(go);
    }
}