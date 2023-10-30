using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WifiVal : MonoBehaviour
{
    private WebSocket ws;
        public RotValues rs;
    void Start()
    {
        // Replace with the IP address of your NodeMCU
        string serverAddress = "ws://192.168.4.1:8080";

        ws = new WebSocket(serverAddress);
        ws.OnMessage += OnMessage;
        ws.Connect();
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        // Handle received messages here
        //Debug.Log("Received message: " + e.Data);
        string[] data =e.Data.Split(',');
        if (data.Length>3)
        {
            rs.data = data;
        }
    }
    private void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }
}
