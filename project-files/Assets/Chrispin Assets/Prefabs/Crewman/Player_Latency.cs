﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player_Latency : NetworkBehaviour 
{
	NetworkClient nClient;
	int latency;
	Text latencyText;

	public override void OnStartLocalPlayer ()
	{
		nClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
		latencyText = GameObject.Find("Latency Text").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		ShowLatency();
	}

	void ShowLatency()
	{
		if (isLocalPlayer)
		{
			latency = nClient.GetRTT();
			latencyText.text = latency.ToString();
		}
	}
}
