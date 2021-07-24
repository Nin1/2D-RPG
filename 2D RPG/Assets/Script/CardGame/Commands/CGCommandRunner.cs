using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGCommandRunner {

    Queue<CGCommand> m_commandQueue = new Queue<CGCommand>();
    public bool m_runningQueue { get; private set; }
    CGVisualManager m_visualManager;

    public void Update()
    {
        if (!m_runningQueue && m_commandQueue.Count > 0)
        {
            RunQueue();
        }
    }


    public CGCommandRunner(CGVisualManager visualManager)
    {
        m_visualManager = visualManager;
    }

    public void AddToQueue(CGCommand command)
    {
        if(m_visualManager == null)
        {
            m_visualManager = GameObject.FindObjectOfType<CGVisualManager>();
            if(m_visualManager == null)
            {
                Debug.LogError("No CGVisualManager found");
            }
        }

        command.m_visualManager = m_visualManager;
        command.SetCommandRunner(this);
        m_commandQueue.Enqueue(command);
        //if(!m_runningQueue)
        //{
        //    RunQueue();
        //}
    }

    void RunQueue()
    {
        m_runningQueue = true;
        m_commandQueue.Dequeue().ExecuteCommand();
    }

    public void CommandFinished()
    {
        // Stop running the queue to exit any callbacks
        // The queue starts running again next Update()
        m_runningQueue = false;
    }
}
