using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using System.Text;

public class Client : MonoBehaviour
{
    public const int port = 9003;
    public const string server = "127.0.0.1";
    TcpClient client;
    NetworkStream stream;

    List<int> types;
    List<int> lengths;
    MemoryStream data;
    int totalLength;
    enum Types
    {
        NOTSUPPORTED = 0,
        INT = 1,
        FLOAT = 2,
        BOOL = 3,
        STRING = 4
    }

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
        totalLength = 0;
        types = new List<int>();
        lengths = new List<int>();
        data = new MemoryStream();
        ConnectClient();
    }

    void Update()
    {
        byte[] want_to_send = BitConverter.GetBytes(true);
        stream.Write(want_to_send, 0, want_to_send.Length);
        // send true to indicate that you want to send data
        // and server won't close the connection
        Add(Time.frameCount);
        Add(1.0f * Time.frameCount / 10);
        Add(Time.frameCount % 2 == 0);
        Add(Time.frameCount + 2);
        Add(Time.frameCount.ToString());
        SendFrameData();
    }
    
    void OnApplicationQuit()
    {
        byte[] want_to_send = BitConverter.GetBytes(false);
        stream.Write(want_to_send, 0, want_to_send.Length);
        //send false to indicate that you won't send more data
        CloseClient();
    }
    
    void SendArray(int[] list){
        byte[] list_bytes = new byte[list.Length * sizeof(int)];
        byte[] list_bytes_length = BitConverter.GetBytes(list_bytes.Length);
        Buffer.BlockCopy(list, 0, list_bytes, 0, list_bytes.Length);
        stream.Write(list_bytes_length, 0, list_bytes_length.Length);
        stream.Write(list_bytes, 0, list_bytes.Length);
    }


    void Add(string n){
        byte[] nBytes = Encoding.ASCII.GetBytes(n);
        data.Write(nBytes, 0, nBytes.Length);

        totalLength += nBytes.Length;
        types.Add((int)Types.STRING);
        lengths.Add(nBytes.Length);
    }
    void Add(bool n)
    {
        byte[] nBytes = BitConverter.GetBytes(n);
        data.Write(nBytes, 0, nBytes.Length);

        totalLength += nBytes.Length;
        types.Add((int)Types.BOOL);
        lengths.Add(nBytes.Length);
    }
    void Add(int n) {
        byte[] nBytes = BitConverter.GetBytes(n);
        data.Write(nBytes, 0, nBytes.Length);

        totalLength += nBytes.Length;
        types.Add((int)Types.INT);
        lengths.Add(nBytes.Length);
    }
    void Add(float n)
    {
        byte[] nBytes = BitConverter.GetBytes(n);
        data.Write(nBytes, 0, nBytes.Length);

        totalLength += nBytes.Length;
        types.Add((int)Types.FLOAT);
        lengths.Add(nBytes.Length);
    }

    void SendFrameData()
    {
        byte[] a = data.ToArray();
        for(int i = 0; i < a.Length; i++)
        {
            Debug.Log(Time.frameCount.ToString() + "   " + i.ToString() + "th byte " + a[i]);
        }
        SendArray(types.ToArray()); // send all the types
        SendArray(lengths.ToArray()); // send corrensponding lengths;
        byte[] data_bytes = data.ToArray();
        stream.Write(data_bytes, 0, data_bytes.Length); // send the data itself
        types.Clear();
        lengths.Clear();
        data.Seek(0, SeekOrigin.Begin);
    }
}
