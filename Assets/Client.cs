using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using UnityEngine;

public class Client : MonoBehaviour
{
    public const int port = 9002;
    public const string server = "127.0.0.1";
    TcpClient client;
    NetworkStream stream;
    
    void ConnectClient()
    {
        client = new TcpClient();
        client.Connect(server, port);
        stream = client.GetStream();
    }
    
    void CloseClient()
    {
        stream.Close();
        client.Close();
    }
    
    void Start()
    {
        ConnectClient();
        
    }

    void Update()
    {
        byte[] want_to_send = BitConverter.GetBytes(true);
        stream.Write(want_to_send, 0, want_to_send.Length);
        // send true to indicate that you want to send data
        // and server won't close the connection
        SendList(new int[]{Time.frameCount});
        
    }
    
    void OnApplicationQuit()
    {
        byte[] want_to_send = BitConverter.GetBytes(false);
        stream.Write(want_to_send, 0, want_to_send.Length);
        //send false to indicate that you won't send more data
        CloseClient();
    }
    
    void SendList(int[] list){
        byte[] list_bytes = new byte[list.Length * sizeof(int)];
        byte[] list_bytes_length = BitConverter.GetBytes(list_bytes.Length);
        Buffer.BlockCopy(list, 0, list_bytes, 0, list_bytes.Length);
        stream.Write(list_bytes_length, 0, list_bytes_length.Length);
        stream.Write(list_bytes, 0, list_bytes.Length);
    }
}
