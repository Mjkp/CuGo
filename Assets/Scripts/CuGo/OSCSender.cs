using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class OSCSender : MonoBehaviour {

	public string outIP = "127.0.0.1";
	public int outPort = 9999;
	// Use this for initialization
	void Start () {
		// init OSC
		OSCHandler.Instance.Init(); 
		// client
		OSCHandler.Instance.CreateClient("myClient", IPAddress.Parse(outIP), outPort);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.S)) {
			List<float> arguments = new List<float> (){ 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f };
			Debug.Log ("sending: " + arguments.ToString());
			OSCHandler.Instance.SendMessageToClient("myClient", "/markers", arguments);
		}	
	}
}
