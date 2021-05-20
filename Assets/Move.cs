using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Move : MonoBehaviour
{
    public const int port = 9002;
    public const string server = "127.0.0.1";
    TcpClient client;
    NetworkStream stream;
    // Start is called before the first frame update
    void Start()
    {
        client = new TcpClient();
        client.Connect(server, port);
        stream = client.GetStream();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Debug.Log("x: "+x+" y: " + y);
        transform.position = transform.position + new Vector3(x/10, y/10);
    }
    
    void SendList(){
        
    }
}
