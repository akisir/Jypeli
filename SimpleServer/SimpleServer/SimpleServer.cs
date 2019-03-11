//SimpleServer.cs 11.3.2019
//by Aki Sirviö
using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

//Console program accepts connect raquests and receive and send messages for client
public class SimpleServer
{
    // starts here
    public static void Main()
    {
        string ipAddr = "";

        // if network available searching host IP address
        if (NetworkInterface.GetIsNetworkAvailable())
        {
            // get host name
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                // get IP address
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddr = ip.ToString();
                }
            }
        }

        Console.WriteLine("ServerIP: " + ipAddr);
        StartSocket(ipAddr);
    }

    //
    private static void StartSocket(string ipAddr)
    {
        // parse server IP
        IPAddress localAddr = IPAddress.Parse(ipAddr);
        // define which address and port to listen to
        TcpListener serverSocket = new TcpListener(localAddr, 8888);
        // TcpClient variable by default
        TcpClient clientSocket = default(TcpClient);
        // let's start listening to client requests
        serverSocket.Start();
        Console.WriteLine("Server started!");

        int counter = 0;
        // accept client request and call HandleClient class for chat
        while (true)
        {
            counter++;
            // accept client
            clientSocket = serverSocket.AcceptTcpClient();
            // HandleClient class handles client messages
            HandleClient client = new HandleClient();
            client.StartClient(clientSocket, Convert.ToString(counter), ipAddr);
            Console.WriteLine("Client no: " + counter + " started!");
        }
    }
}
