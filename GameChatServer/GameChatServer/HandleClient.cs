//HandleClient.cs 27.3.2019
//by Aki Sirviö
using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;

//Client object handels incoming and outgoing messages
class HandleClient
{
    public Action<string> ShowMessage;
    public Action<string> SendToAll;
    private Thread threadingClient;
    private bool run = true;
    private string userName = "";
    public TcpClient clientSocket { get; set; }

    // initial settings
    public void StartClient(string userName)
    {
        this.userName = userName;
        /*threadingClient = new Thread(delegate()
        {
            doChat(userName);
        });*/
        // new thread
        threadingClient = new Thread(doChat);
        threadingClient.Start();
    }

    // send text message to client
    public void Send(string message)
    {
        try
        {
            // get network stream to send message
            NetworkStream networkStream = clientSocket.GetStream();
            byte[] bTo = null;
            // convert string to byte
            bTo = Encoding.UTF8.GetBytes(message);
            // write message to networkStream
            networkStream.Write(bTo, 0, bTo.Length);
            networkStream.Flush();
        }
        catch(Exception e)
        {
            ShowMessage(e.ToString());
        }
    }

    // exit client socket
    public void Stop()
    {
        this.run = false;
        clientSocket.Close();
        threadingClient.Abort();
        ShowMessage("Exit - " + userName);
    }

    // read messages for client
    private void doChat()
    {
        NetworkStream networkStream = null;
        string dataFromClient = "";
        byte[] bFrom = new byte[10025];
        
        while (run)
        {
            try
            {
                // check connection
                if (clientSocket.Connected)
                {
                    // retrieving networkStream for reading and writing messages
                    networkStream = clientSocket.GetStream();
                    // reads message from networkStream to byte type variable
                    networkStream.Read(bFrom, 0, clientSocket.ReceiveBufferSize);
                    // clear buffers
                    networkStream.Flush();
                    // convert byte to string
                    dataFromClient = Encoding.UTF8.GetString(bFrom);
                    // cuts message at $ mark
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    dataFromClient = userName + ": " + dataFromClient;

                    SendToAll(dataFromClient);
                }
            }
            catch (Exception e)//close and exit
            {
                networkStream.Dispose();
                networkStream.Close();
                Stop();
                ShowMessage(e.ToString());
            }
        }
    }
}

