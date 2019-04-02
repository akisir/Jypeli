// HandleConnections.cs 1.4.2019
// by Aki Sirviö
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

//HandleConnections class takes care of communication with clients.
namespace GolfServer
{
    class HandleConnections
    {
        public Action<int, string, string> callBack;
        private byte[] buffer = new byte[1024];
        private int portNum = 100;
        private List<Socket> ClientList;
        private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // constructor, execution starts here
        public HandleConnections()
        {
            ClientList = new List<Socket>();
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, portNum));
            serverSocket.Listen(1);
            serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
        }

        // returns list of IP addresses. Called from GolfServer class
        public List<string> GetLocalIP(NetworkInterfaceType type)
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.NetworkInterfaceType == type && x.OperationalStatus == OperationalStatus.Up)
                .SelectMany(x => x.GetIPProperties().UnicastAddresses)
                .Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork).Select(x => x.Address.ToString()).ToList();
        }

        // send message to client. Called from GolfServer class
        public void Sendata(string sSocket, string viesti)
        {
            Socket socket = null;
            byte[] data = Encoding.UTF8.GetBytes(viesti);
            for (int i = 0; i < ClientList.Count; i++)
            {
                if (ClientList[i].RemoteEndPoint.ToString().Equals(sSocket))
                {
                    socket = ClientList[i];
                }
            }
            if (socket.Connected)
            {
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
            }
        }

        // accepts connection request and makes new socket
        private void AppceptCallback(IAsyncResult ar)
        {
            Socket socket = serverSocket.EndAccept(ar);
            ClientList.Add(socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            serverSocket.BeginAccept(new AsyncCallback(AppceptCallback), null);
        }

        // receives messages of clients and passes them to GolfServer class
        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (socket.Connected)
            {
                int received;
                try
                {
                    received = socket.EndReceive(ar);

                    if (received > 0)
                    {
                        byte[] dataBuf = new byte[received];
                        Array.Copy(buffer, dataBuf, received);
                        string text = Encoding.UTF8.GetString(dataBuf);
                        string[] words = Regex.Split(text, "@,");

                        if (words[0].All(char.IsDigit))
                        {
                            callBack(Convert.ToInt32(words[0]), words[1], socket.RemoteEndPoint.ToString());
                        }

                        // does not display all messages
                        if (words[0] != "3" && words[0] != "2")
                        {
                            for (int i = 0; i < ClientList.Count; i++)
                            {
                                if (socket.RemoteEndPoint.ToString().Equals(ClientList[i].RemoteEndPoint.ToString()))
                                {
                                    GolfServer.Instance.MessageDisplay.Add(socket.RemoteEndPoint.ToString() + ": " + text);
                                }
                            }
                        } 
                    }
                    else
                    {
                        RemoveClient(socket.RemoteEndPoint.ToString());
                    }

                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                }
                catch (Exception)
                {
                    if(socket == null)
                        RemoveClient(socket.RemoteEndPoint.ToString());
                    return;
                }
            } 
        }

        // remove client
        public void RemoveClient(string clientID)
        {
            for (int i = 0; i < ClientList.Count; i++)
            {
                if (ClientList[i].RemoteEndPoint.ToString().Equals(clientID))
                {
                    ClientList[i].Close();
                    ClientList[i].Dispose();
                    ClientList.RemoveAt(i);
                    break;
                }
            }
        }

        // to do when message is sent
        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}
