using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class SendUDP
{

	int localPort;
	int port;
	IPEndPoint IP;
	public int revIndex;
	public UdpClient client;
	byte[] bytes;

	public SendUDP()
	{
		localPort = 55555;
		port = 44444;
		client = new UdpClient(localPort);
	}

	public void send(byte[] data, String ip)
	{
		IP = new IPEndPoint(IPAddress.Parse(ip), port);
		client.Send(data, data.Length, IP);
	}

	public String receive()
	{
		return "";
	}
}
