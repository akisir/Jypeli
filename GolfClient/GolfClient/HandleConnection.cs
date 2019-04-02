// HandleConnection.cs 1.4.2019
// by Aki Sirviö
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Jypeli;

//HandleConnection takes care of communication with server
namespace GolfClient
{
    class HandleConnection
    {
        public Action<string> CallBack;
        private Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte[] receivedBuf = new byte[1024];
        IPAddress serverIP = null;
        string text;

        // retrieves server as long as it gets connected
        public string LoopConnect(string IP)
        {
            int attempts = 0;
            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    if(IP==null)
                    {
                        // get host name and connects to it
                        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                        string hostName = host.HostName;
                        clientSocket.Connect(hostName, 100);
                    }
                    else
                    {
                        clientSocket.Connect(IP, 100);
                    }

                    text = ("Connected! " + serverIP);

                    if (clientSocket.Connected)
                    {
                        clientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
                        byte[] buffer = Encoding.UTF8.GetBytes("@@ " + text);
                        clientSocket.Send(buffer);
                        CallBack("yhdistetty");
                    }
                }
                catch (SocketException)
                {
                    // if cant connect opens window to enter new ip address
                    text = ("Not connected, attempts: " + attempts.ToString());
                    if (attempts >= 5)
                    {
                        InputWindow inputWin = new InputWindow("Enter server IP:");
                        inputWin.Message.TextScale *= 1.5;
                        inputWin.Message.SizeMode = TextSizeMode.AutoSize;
                        inputWin.Color = Color.YellowGreen;
                        inputWin.ActiveColor = new Color(120, 113, 0, 70);
                        inputWin.MaxCharacters = 15;
                        inputWin.Shape = Shape.Circle;
                        inputWin.TextureFillsShape = true;
                        inputWin.QueryWidget.Text = "192.168.1.1";
                        inputWin.QueryWidget.TextScale *= 1.5;
                        inputWin.QueryWidget.SizeMode = TextSizeMode.AutoSize;
                        inputWin.TextEntered += ProcessInput;
                        inputWin.OKButton.Shape = Shape.Circle;
                        inputWin.OKButton.Color = new Color(120, 113, 0, 120);
                        inputWin.OKButton.TextColor = Color.Black;
                        inputWin.OKButton.Text = "Connect";
                        GolfClient.Instance.Add(inputWin);
                        break;
                    }
                }
            }

            return text;
        }

        // calls LoopConnect method with new ip address
        private void ProcessInput(InputWindow ikkuna)
        {
            string serverIP = ikkuna.InputBox.Text;
            if(serverIP != "")
            {
                GolfClient.Instance.MessageDisplay.Add(LoopConnect(serverIP));
            }
            else
            {
                GolfClient.Instance.MessageDisplay.Add(LoopConnect(null));
            }
        }

        // sends message to server
        public void Send(string komento)
        {
            if (clientSocket.Connected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(komento.ToString());
                clientSocket.Send(buffer);
            }
        }

        // receive messages from server
        private void ReceiveData(IAsyncResult ar)
        {
            if(clientSocket.Connected)
            {
                try
                {
                    Socket socket = (Socket)ar.AsyncState;
                    int received = socket.EndReceive(ar);
                    byte[] dataBuf = new byte[received];
                    Array.Copy(receivedBuf, dataBuf, received);

                    text = (Encoding.UTF8.GetString(dataBuf));
                    CallBack(text);
                    clientSocket.BeginReceive(receivedBuf, 0, receivedBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
                }
                catch(Exception e)
                {
                    clientSocket.Close();
                    clientSocket.Dispose();
                    GolfClient.Instance.MessageDisplay.Add(e.ToString());
                    CallBack("restart");
                }
            }
        }
    }
}
