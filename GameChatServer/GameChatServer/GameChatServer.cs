//GameChatServer.cs 27.3.2019
//by Aki Sirviö
using System;
using System.Collections.Generic;
using Jypeli;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using Font = System.Drawing.Font;

//Multiuser chat server
public class GameChatServer : Game
{
    private Thread threadingServer = null;
    private TcpListener serverSocket;
    private List<HandleClient> clientList = new List<HandleClient>();
    private TextBox text1;
    private TextBox text2;
    private bool run = true;

    // program starts here
    public override void Begin()
    {
        AddWinComponents();
        AddJypeliBut();
        GetIP();

        Keyboard.Listen(Key.Escape, Jypeli.ButtonState.Pressed, Exit, "Lopeta peli");
    }

    // search host IP
    private void GetIP()
    {
        run = true;
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

        ShowMessage("ServerIP: " + ipAddr);

        // starts server in new thread
        threadingServer = new Thread(StartSocket);
        threadingServer.IsBackground = true;
        threadingServer.Start(ipAddr);
    }

    // starts listening client requests and accept clients
    private void StartSocket(object data)
    {
        string ipAddr = data.ToString();
        // parse server IP
        IPAddress localAddr = IPAddress.Parse(ipAddr);
        // define which address and port to listen to
        serverSocket = new TcpListener(localAddr, 8888);
        // TcpClient variable by default
        TcpClient clientSocket = default(TcpClient);
        // let's start listening to client requests
        serverSocket.Start();
        ShowMessage("Waiting connections..");

        int counter = 0;
        // accept client request and call HandleClient class for chat
        while (run)
        {
            counter++;
            try
            {
                // accept client
                clientSocket = serverSocket.AcceptTcpClient();
                // HandleClient class manages client messages
                HandleClient client = new HandleClient();
                client.clientSocket = clientSocket;
                clientList.Add(client);
                // sets action methods
                client.ShowMessage = this.ShowMessage;
                client.SendToAll = this.SendToAll;
                string userName = "Player " + counter;
                client.StartClient(userName);
                this.SendToAll(userName + " connected!");
            }
            catch(ThreadAbortException e) { }
            catch (SocketException e) { }
            catch (Exception e)
            {
                ShowMessage(e.ToString());
            }
            
        }
        // if while loop ends try restart server
        ShowMessage("Restart!");
        GetIP();
    }

    // add windows UI componens
    private void AddWinComponents()
    {
        int width = (int)Screen.Width;
        int height = (int)Screen.Height;
        Font f = new Font("Microsoft Sans Serif", 15F, FontStyle.Regular, GraphicsUnit.Point, 0);

        text1 = new TextBox();
        text1.Font = f;
        text1.Location = new Point(150, 10);
        text1.BorderStyle = BorderStyle.FixedSingle;
        text1.Multiline = true;
        text1.TabStop = false;
        text1.ReadOnly = true;
        text1.Text = "Chat Server:";
        text1.Size = new Size(width / 2 + 10, height - 20);
        Control.FromHandle(Window.Handle).Controls.Add(text1);

        text2 = new TextBox();
        text2.Font = f;
        text2.Location = new Point(182 + width/2, 85);
        text2.BorderStyle = BorderStyle.None;
        text2.Multiline = true;
        text2.TabIndex = 0;
        text2.Size = new Size(200, 50);
        Control.FromHandle(Window.Handle).Controls.Add(text2);
    }

    // add jypeli buttons
    private void AddJypeliBut()
    {
        GameObject resetBut = new GameObject(200, 50);
        resetBut.Position = new Vector(280, Screen.Top-182);
        resetBut.Image = LoadImage("resetBut");
        Mouse.ListenOn(resetBut, MouseButton.Left, Jypeli.ButtonState.Pressed, Restart, "Restart server");
        Add(resetBut);

        GameObject sendAllBut = new GameObject(200, 50);
        sendAllBut.Position = new Vector(280, Screen.Top - 50);
        sendAllBut.Image = LoadImage("sendAllBut");
        Mouse.ListenOn(sendAllBut, MouseButton.Left, Jypeli.ButtonState.Pressed, delegate { SendToAll("Server: " + text2.Text); }, "Send message to all clients");
        Add(sendAllBut);
    }

    // restarts server, called by keystroke
    private void Restart()
    {
        run = false;
        if(serverSocket != null)
            serverSocket.Stop();
        serverSocket = null;
        foreach(HandleClient hc in clientList)
        {
            hc.Send("Server Closing..");
            hc.Stop();
        }
        clientList.Clear();
        Thread.Sleep(1000);
    }

    // shows message on textbox
    private void ShowMessage(string text)
    {
        // check if invoke required for different thread
        if (text1.InvokeRequired)
        {
            text1.Invoke(new MethodInvoker(delegate { text1.AppendText("\r\n" + text); }));
        }
        else
            text1.AppendText("\r\n" + text);
    }

    // sends text message to all clients
    private void SendToAll(string text)
    {
        int sentMes = 0;
        if (text != "")
        {
            for (int i = clientList.Count - 1; i >= 0; i--)
            {
                // if connected send message otherwise remove client from list
                if (clientList[i].clientSocket.Connected)
                {
                    sentMes++;
                    clientList[i].Send(text);
                }
                else
                {
                    clientList.Remove(clientList[i]);
                }
            }
        }
        ShowMessage(text + " (" + sentMes + "pcs)");
    }

    // when closing stops socket and abort thread
    protected override void OnExiting(object sender, EventArgs args)
    {
        base.OnExiting(sender, args);
        serverSocket.Stop();
        threadingServer.Abort();
    }
}
