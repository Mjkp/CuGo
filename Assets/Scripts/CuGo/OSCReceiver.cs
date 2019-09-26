using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityOSC;

public class OSCReceiver : MonoBehaviour, OSCListener {

	private OSCServer myServer;
    UDPClient UdpClient;
    ResetPhysics SetRotation;
	public int inPort = 9052;
	// Buffer size of the application (stores 100 messages from different servers)
	public int bufferSize = 100;
    public int ipListCounter=1;
    private string message;



    public int[] UnitsRotationPattern;

    // Script initialization
    void Start() {
        UdpClient = transform.GetComponent<UDPClient>();
        SetRotation = transform.GetComponent<ResetPhysics>();
        UnitsRotationPattern = new int[transform.childCount];
		// init OSC
		OSCHandler.Instance.Init(); 

		myServer = OSCHandler.Instance.CreateServer(this.name, inPort);

		OSCHandler.Instance.AddCallback (this);

		// Set buffer size (bytes) of the server (default 1024)
		myServer.ReceiveBufferSize = 1024;
		// Set the sleeping time of the thread (default 10)
		myServer.SleepMilliseconds = 10;

	}
    public void OnOSC(OSCPacket pckt)
    {
        message = "";
        for(int i =0; i< pckt.Data.Count; i ++)
        {
            message += pckt.Data[i];
        }
        //Debug.Log (message);


        if (pckt.Address.Equals ("/UnitPatterns")) 
        {
            string[] ipNrotData = message.Split(',');
            string ipData = ipNrotData[0];
            string rotationData = ipNrotData[1];
            Debug.Log ("ip Number:"+ipData+", "+"rotation value: "+ rotationData);


            UdpClient.regenIPList[ipListCounter] = ipData;
            UnitsRotationPattern[ipListCounter] = int.Parse(rotationData);
            ipListCounter++;

            if(ipListCounter == transform.childCount)
            {
                UdpClient.init();
                // for now 0 is default rotation
                SetRotation.AssignYRotation(0,UnitsRotationPattern);
                ipListCounter = 0;
            }

        }
    }
		
}