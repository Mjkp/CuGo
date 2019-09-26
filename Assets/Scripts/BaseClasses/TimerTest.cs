using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerTest
{

    public float targetTime = 10.0f;

    private Timer aTimer;

    private string name = "defaultTimer";

    private List<TimerListener> listeners = new List<TimerListener>();

    public void AddListener(TimerListener newListener)
    {
        listeners.Add(newListener);
        //Debug.Log("AddListener" + this + " -> " + this.listeners.Count);
    }
    public void RemoveListener(TimerListener newListener)
    {
        listeners.Remove(newListener);
    }

    public void StartTimer(int durationMillis,string name)
    {
        this.name = name;
        StartTimer(durationMillis);

    }

    public void StartTimer(int durationMillis)
    {
        if(aTimer != null)
        {
            aTimer.Enabled = false;
            aTimer.Close();
            aTimer.Dispose();
            aTimer = null;
            //Debug.Log("existing timer cleared");
        }
        aTimer = new System.Timers.Timer(durationMillis);
        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e);
        aTimer.Enabled = true;
        //Debug.Log("StartTimer called");
    }

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        //Debug.Log(this + " -> " + this.listeners.Count);
        aTimer.Stop();
        for (int i = 0; i < listeners.Count; i++)
        {
            TimerListener timerListener = listeners[i];
            timerListener.OnTimerComplete(this);
        }
    }

    public void StopTimer()
    {
        aTimer.Stop();
    }

    public override string ToString()
    {
        return "[TestTimer name="+name+"]";
    }


}
