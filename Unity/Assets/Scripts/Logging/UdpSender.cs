using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UdpSender : MonoBehaviour
{ 
    static public void SendToNeuroscan(string msg)
    {
        print("send=" + msg);
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        byte[] sendbuf = Encoding.ASCII.GetBytes(msg);
        IPAddress broadcast = IPAddress.Parse("127.0.0.1");
        IPEndPoint ep = new IPEndPoint(broadcast, 9999);
        s.SendTo(sendbuf, ep);
    }
}
