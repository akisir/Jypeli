//HandleConnection.cs 11.3.2019
//by Aki Sirviö
using System.Text;
using System.Net;
using System.Net.Sockets;

//Open and handles client's connection to server.
class HandleConnection
{
    private SimpleClient parent;
    private TcpClient clientSocket;

    // constructor takes parent class as variable
    public HandleConnection(SimpleClient parent)
    {
        this.parent = parent;
    }

    // try to connect without IP address first
    public bool Connect(string ip)
    {
        // new TcpClient
        clientSocket = new TcpClient();
        string hostName = "";

        int attempts = 0;
        // try to connect five times
        while (!clientSocket.Connected)
        {
            try
            {
                attempts++;
                if(ip==null)
                {
                    // get host name and connects to it
                    hostName = Dns.GetHostName();
                    clientSocket.Connect(hostName, 8888);
                }
                else
                {
                    // connect to given IP address
                    clientSocket.Connect(ip, 8888);
                    hostName = ip;
                }
            }
            catch (SocketException e)
            {
                // return false after five failures
                if (attempts > 4)
                {
                    parent.MessageDisplay.Add("Cannot connect to server!!");
                    return false;
                }
            }
        }

        parent.MessageDisplay.Add("Connected to " + hostName);
        return true;
    }

    public void Send(string text)
    {
        try
        {
            // searching for NetworkStream for reading and writing messages
            NetworkStream serverStream = clientSocket.GetStream();
            // creates message to be sent
            byte[] outStream = Encoding.ASCII.GetBytes(text + "$");

            // write message to NetworkStream
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            // after send reads upcoming message
            ReadData(serverStream);
        }
        catch
        {
            parent.MessageDisplay.Add("Cannot connect to server!!");
            parent.MessageDisplay.Add("Connecting...");
            //try to establish new connection
            Jypeli.Timer.SingleShot(1, delegate { Connect(null); });
        }
    }

    // reads message from server
    private void ReadData(NetworkStream serverStream)
    {
        byte[] inStream = new byte[10025];
        try
        {
            // reading message from NetworkStream
            serverStream.Read(inStream, 0, clientSocket.ReceiveBufferSize);

            // convert message and send it to parent calss
            string returndata = Encoding.ASCII.GetString(inStream);
            returndata = returndata.Substring(0, returndata.IndexOf("\0"));
            parent.Receive(returndata);
        }
        catch{}
    }
}


