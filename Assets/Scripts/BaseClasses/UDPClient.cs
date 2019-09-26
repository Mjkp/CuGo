using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class UDPClient : MonoBehaviour, TimerListener
{
    private int clientCount; 
    public string IP1 ;
    public string IP2 ;
    public string IP3 ;
    public string IP4 ;

    private string[] IPS;
    private int port;
    private int Interval = 800;
    public int[] timeInterval; 
    private int initialDelay = 100;
    IPEndPoint[] remoteEndPoints;
    UdpClient[] clients;

    TimerTest[] timers;
    HingeRotation_config Cugo;
    //List<int> regenIPList;
    public string[] regenIPList;

    private static void Main(){
        UDPClient sendObj = new UDPClient();
        sendObj.init();
    }


    public void Awake()
    {
        Cugo = GetComponent<HingeRotation_config>();
        init();
    }

    public void init() 
    {
        print("UDPSend.init()");
        port = 8052;
        clientCount = 4;
        IPS = new string[] { IP1, IP2, IP3, IP4 };
        //ResetIPOrder(regenIPList);

        // two time intervals;
        timeInterval = new int[] { 800, 800, 50, 50 };

        //remoteEndpoint array
        remoteEndPoints = new IPEndPoint[clientCount];
        for ( int i = 0; i< clientCount; i++)
        {
            remoteEndPoints[i] = new IPEndPoint(IPAddress.Parse(IPS[i]), port);
        }
        //clients array
        clients = new UdpClient[clientCount];
        for (int i = 0; i < clientCount; i++)
        {
            clients[i] = new UdpClient();
        }

        // timers array
        timers = new TimerTest[clientCount];
        for (int i = 0; i < clientCount; i++)
        {
            timers[i] = new TimerTest();
        }

        foreach (TimerTest timer in timers)
        {
            timer.AddListener(this);
        }
    }

    /// <summary>
    /// the first ip is decided, and second third and fourth ip are generated
    /// </summary>
    /// <param name="IPlist">IP list.</param>
    public void ResetIPOrder(string[] IPlist)
    {
        IPS = new string[] { IP1, IP2, IP3, IP4 };
        IPlist = new string[clientCount];
        IPlist[0] = IP1;
        for (int i = 0; i<IPlist.Length;i++)
        {
            IPS[i] = IPlist[i];
        }
    }

    public void InitiateTimer(int x)
    {
        // 0 for foward and 1 for backward
        if (x == 1)
        {
            for (int i = 0; i < timers.Length; i++)
            {
                timers[i].StartTimer(initialDelay + timeInterval[3 - i], "timer-" + i.ToString());
            }
        }
        else if(x ==0)
        {
            for (int i = 0; i < timers.Length; i++)
            {
                timers[i].StartTimer(initialDelay + timeInterval[i], "timer-" + i.ToString());
            }
        }
    }

    public void sendString(string message,int clientID)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            clients[clientID].Send(data, data.Length, remoteEndPoints[clientID]);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    // 0 do not rotate, 1 rotate right, -1 rotate left
    public void OnTimerComplete(object timer)
    { 
        if (timer == timers[0])
        {
            if(Cugo.angleVal.x >= 120 -1)
            {
                sendString("r", 0);
                //Debug.Log("1st joint right");


            }
            else if (Cugo.angleVal.x <= -120 +1)
            {
                sendString("l", 0);
                //Debug.Log("1st joint left");


            }
            else if (Cugo.angleVal.x == 0)
            {
                sendString("s", 0);
                //Debug.Log("1st joint stop");


            }
            //Debug.Log("Timer1 " + "sending value: " + (Cugo.angleVal.x ).ToString());
        }
        else if (timer == timers[1])
        {
            if (Cugo.angleVal.y >= 120 - 1)
            {
                sendString("r", 1);
                //Debug.Log("2nd joint right");


            }
            else if (Cugo.angleVal.y <= -120 + 1)
            {
                sendString("l", 1);
                //Debug.Log("2nd joint left");


            }
            else if (Cugo.angleVal.y == (0))
            {
                sendString("s", 1);
                //Debug.Log("2nd joint stop");


            }
            //Debug.Log("Timer2 " + "sending value: " + (Cugo.angleVal.y ).ToString());
        }
        else if (timer == timers[2])
        {
            if (Cugo.angleVal.z >= 120 - 1)
            {
                sendString("r", 2);
                //Debug.Log("3rd joint right");

            }
            else if (Cugo.angleVal.z <= -120 + 1)
            {
                sendString("l", 2);
                //Debug.Log("3rd joint left");

            }
            else if (Cugo.angleVal.z == 0)
            {
                sendString("s", 2);
                //Debug.Log("3rd joint stop");

            }
            //Debug.Log("Timer3 " + "sending value: " + (Cugo.angleVal.z ).ToString());
        }
        else if (timer == timers[3])
        {
            if (Cugo.angleVal.w >= 120 - 1)
            {
                sendString("r", 3);
                //Debug.Log("4th joint right");

            }
            else if (Cugo.angleVal.w <= -120 + 1)
            {
                sendString("l", 3);
                //Debug.Log("4th joint left");

            }
            else if (Cugo.angleVal.w == (0))
            {
                sendString("s", 3);
                //Debug.Log("4th joint stop");

            }
            //Debug.Log("Timer4-" +"sending value: "+ (Cugo.angleVal.w ).ToString());
        }
    }
}
