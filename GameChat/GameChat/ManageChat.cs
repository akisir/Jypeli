//ManageChat.cs 27.3.2019
//by Aki Sirviö
using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

// make and manages chat boxes and connection to server
namespace Program
{
    class ManageChat
    {
        public Action<Hashtable> AddGameObject;
        private TcpClient clientSocket = null;
        private Microsoft.Xna.Framework.GameWindow window;
        private TextBox text1;
        private TextBox text2;
        private Label label1;
        private Button but1;
        private Label label2;
        private TextBox text3;

        // constructor
        public ManageChat(Microsoft.Xna.Framework.GameWindow window)
        {
            this.window = window;
            this.window.ClientSizeChanged += new EventHandler<EventArgs>(OnResize);
        }

        // UI componentes
        public void AddComponents()
        {
            AddWinCom();
        }

        // try to connect without IP address first
        public void Connect(string ip)
        {
            // old socket will be closed
            if (clientSocket != null)
            {
                SendMessage("Disconnecting..");
                clientSocket.Close();
                clientSocket = null;
                Thread.Sleep(1000);
            }

            // new TcpClient
            clientSocket = new TcpClient();
            string hostName = "";

            int attempts = 0;
            // try to connect three times
            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    if (ip == null)
                    {
                        // get host name and connects to it, puts IP to textbox
                        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                        hostName = host.HostName;
                        text3.Text = host.AddressList[1].ToString();
                        clientSocket.ConnectAsync(hostName, 8888).Wait(2000);
                    }
                    else
                    {
                        // connect to given IP address
                        hostName = ip;
                        clientSocket.ConnectAsync(ip, 8888);
                    }
                }
                catch (Exception e)
                {
                    // return false after three failures
                    if (attempts > 2)
                    {
                        ShowMessage("Cannot connect to server!!");
                        return;
                    }
                }
            }

            ShowMessage("Connected to " + hostName);
            // starts asynchronous method
            Task.Run(() => ReadAsync());
        }

        // reading messages from server asynchronous
        private async Task ReadAsync()
        {
            // get network stream to read message
            NetworkStream networkStream = clientSocket.GetStream();
            string returndata = "";
            byte[] bFrom = new byte[10025];
            TimeSpan delay = TimeSpan.FromMilliseconds(10);
            while (true)
            {
                try
                {
                    // little dealy
                    await Task.Delay(delay);
                    Array.Clear(bFrom, 0, bFrom.Length);

                    // if message is available and readable it will be pick from stream
                    if (networkStream.DataAvailable && networkStream.CanRead)
                    {
                        // asynchronously reads bytes from stream
                        int bytesRead = await networkStream.ReadAsync(bFrom, 0, bFrom.Length);
                        if (bytesRead == 0) return; // Stream was closed
                        // clears buffers
                        networkStream.Flush();
                        // convert bytes to UTF8 string
                        returndata = Encoding.UTF8.GetString(bFrom).TrimEnd('\0');

                        // show received message
                        ShowMessage(returndata);
                    }
                }
                catch (Exception ex)
                {
                    // close connection
                    networkStream.Dispose();
                    networkStream.Close();
                    clientSocket.Close();
                    ShowMessage(ex.ToString());
                    return;
                }
            }
        }

        // adds windows UI components
        private void AddWinCom()
        {
            int width = window.ClientBounds.Width;
            int height = window.ClientBounds.Height;
            Font f = new Font("Microsoft Sans Serif", 15F, FontStyle.Regular, GraphicsUnit.Point, 0);

            text1 = new TextBox();
            text1.Font = f;
            text1.Location = new Point(10, 10);
            text1.BorderStyle = BorderStyle.None;
            text1.Multiline = true;
            text1.TabIndex = 1;
            text1.TabStop = false;
            text1.ReadOnly = true;
            text1.Text = "Chat box";
            text1.Size = new Size(width / 4, height - 150);
            Control.FromHandle(window.Handle).Controls.Add(text1);

            text2 = new TextBox();
            text2.Font = f;
            text2.Location = new Point(10, height - 100);
            text2.BorderStyle = BorderStyle.None;
            text2.Multiline = true;
            text2.TabIndex = 0;
            text2.Size = new Size(width / 4, 90);
            text2.KeyDown += new KeyEventHandler(SendKey);
            Control.FromHandle(window.Handle).Controls.Add(text2);

            label1 = new Label();
            label1.AutoSize = true;
            label1.Font = f;
            label1.Location = new Point(10, height - 130);
            label1.Text = "Player:";
            label1.BackColor = Color.Black;
            label1.ForeColor = Color.White;
            Control.FromHandle(window.Handle).Controls.Add(label1);

            but1 = new Button();
            but1.Font = f;
            but1.Location = new Point(width - 75, 30);
            but1.Text = "Connect";
            but1.Size = new Size(200, 50);
            but1.Click += new EventHandler(delegate { Connect(text3.Text); });
            Control.FromHandle(window.Handle).Controls.Add(but1);

            label2 = new Label();
            label2.AutoSize = true;
            label2.Font = f;
            label2.Location = new Point(width - 75, 90);
            label2.Text = "Server IP:";
            label2.BackColor = Color.Black;
            label2.ForeColor = Color.White;
            Control.FromHandle(window.Handle).Controls.Add(label2);

            text3 = new TextBox();
            text3.Font = f;
            text3.Location = new Point(width -75, 120);
            text3.BorderStyle = BorderStyle.None;
            text3.TabIndex = 2;
            text3.Size = new Size(200, 30);
            Control.FromHandle(window.Handle).Controls.Add(text3);
        }

        // sends message from textbox every time enter key pressed
        private void SendKey(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage(text2.Text);
                text2.Clear();
                e.Handled = true;
            }
        }

        // sending client messages to server
        private void SendMessage(string viesti)
        {
            try
            {
                if(viesti != "" && clientSocket.Connected)
                {
                    // get network stream to write message
                    NetworkStream networkStream = clientSocket.GetStream();
                    // checking if stream can be written
                    if (networkStream.CanWrite)
                    {
                        // convert string to UTF8 bytes
                        byte[] bTo = Encoding.UTF8.GetBytes(viesti + "$");
                        // write message to networkStream
                        networkStream.Write(bTo, 0, bTo.Length);
                        // clear buffers
                        networkStream.Flush();
                    }
                }
            }
            // if writing fails
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());
            }
        }

        // show message in textbox
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

        // make UI easy to resize
        void OnResize(object sender, EventArgs e)
        {
            int width = window.ClientBounds.Width;
            int height = window.ClientBounds.Height;
            text1.Size = new Size(width / 5, height - 150);
            text2.Size = new Size(width / 5, 90);
            text2.Location = new Point(10, height - 100);
            label1.Location = new Point(10, height - 130);
            but1.Location = new Point(width - 250, 30);
            label2.Location = new Point(width - 250, 90);
            text3.Location = new Point(width - 250, 120);
        }

        /*void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (threadingClient != null)
                threadingClient.Abort();
        }*/
    }
}

