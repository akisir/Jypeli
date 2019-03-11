//HandleClient 11.3.2019
//by Aki Sirviö
using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;

//Handles client messages
class HandleClient
{
    private TcpClient clientSocket;
    private NetworkStream networkStream;
    private Thread ctThread;
    private string clNo;
    private string ipAddr;

    // initial settings
    public void StartClient(TcpClient inClientSocket, string clineNo, string ip)
    {
        clientSocket = inClientSocket;
        clNo = clineNo;
        ipAddr = ip;
        ctThread = new Thread(doChat);
        ctThread.Start();
    }

    // read and write messages for client
    private void doChat()
    {
        string dataFromClient = "";
        string serverResponse = "";
        byte[] bFrom = new byte[10025];
        byte[] bTo = null;

        while (true)
        {
            try
            {
                // check connection
                if (clientSocket.Connected)
                {
                    int resp = 0;
                    int sum = 0;
                    string response;
                    // retrieving networkStream for reading and writing messages
                    networkStream = clientSocket.GetStream();
                    // reads message from networkStream to byte type variable
                    networkStream.Read(bFrom, 0, clientSocket.ReceiveBufferSize);
                    // convert byte to string
                    dataFromClient = Encoding.ASCII.GetString(bFrom);
                    // cuts message at $ mark
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    // convert string to int
                    resp = Convert.ToInt32(dataFromClient);
                    sum = resp + resp + 1;
                    // make response messages
                    response = dataFromClient + " + " + (resp + 1).ToString() + " = " + sum.ToString();
                    serverResponse = "How about "+ response;
                    // convert string to byte
                    bTo = Encoding.ASCII.GetBytes(serverResponse);
                    // write message to networkStream
                    networkStream.Write(bTo, 0, bTo.Length);
                    networkStream.Flush();
                    Console.WriteLine("Client<" + clNo + ">: " + dataFromClient);
                    Console.WriteLine("Server: " + serverResponse);
                }
            }
            catch (Exception e)//close and exit
            {
                networkStream.Dispose();
                networkStream.Close();
                clientSocket.Close();
                ctThread.Abort();
                Console.WriteLine("Exit");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
