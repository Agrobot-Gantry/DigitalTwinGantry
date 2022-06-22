using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows you to queue actions (from other threads) to be executed on the main thread.
/// Tries to execute all queued actions during the update.
/// </summary>
public class MainThreadActionQueuer : MonoBehaviour
{
    private ConcurrentQueue<Action> m_queue;

	void Awake()
	{
        m_queue = new ConcurrentQueue<Action>();
	}

	void Update()
    {
        if (!m_queue.IsEmpty)
		{
            Action action;
            while (m_queue.TryDequeue(out action))
			{
                action.Invoke();
			}
		}
    }

    /// <summary>
    /// Queues an action which will be executed on the main thread during the next update.
    /// </summary>
    public void QueueAction(Action action)
	{
        m_queue.Enqueue(action);
	}
}
