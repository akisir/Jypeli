//SimpleClient.cs 11.3.2019
//by Aki Sirviö
using Jypeli;

//Connects to server and sends and receives messages from server.
public class SimpleClient : Game
{
    private HandleConnection client;
    private Label title;
    private Label ask;
    private Label answer;
    private GameObject connBut;
    private GameObject sendBut;
    private int anNum = 1;

    // starts here
    public override void Begin()
    {
        // new client object for server connection
        client = new HandleConnection(this);
        Establish();
        MessageDisplay.Add("Connecting...");
        Timer.SingleShot(1, delegate { OpenClient(client.Connect(null)); });

        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    // make Jypeli objects like buttons
    private void Establish()
    {
        Level.Background.Color = Color.Mint;
        title = new Label(200, 50);
        title.Text = "Simple Client";
        title.TextScale *= 2;
        title.Y = Screen.Top - 100;
        Add(title);

        connBut = new GameObject(300, 100);
        connBut.Image = LoadImage("tryconnectGray");
        connBut.Y = Screen.Top - 200;
        Add(connBut);

        Label hint = new Label(300, 50);
        hint.Text = "ask and answer";
        hint.Y = Screen.Top - 300;
        Add(hint);

        ask = new Label(300, 50);
        ask.Text = anNum.ToString();
        ask.TextScale *= 2;
        ask.BorderColor = Color.Black;
        ask.Y = Screen.Top - 350;
        Add(ask);

        answer = new Label(300, 50);
        answer.BorderColor = Color.Black;
        answer.Y = Screen.Top - 420;
        Add(answer);

        sendBut = new GameObject(300, 100);
        sendBut.Image = LoadImage("sendGray");
        sendBut.Y = Screen.Top - 550;
        Add(sendBut);
    }

    // show title and enble or disable buttons
    private void OpenClient(bool connected)
    {
        if (connected)
        {
            title.Text = "Connected!";
            connBut.Image = LoadImage("tryconnectGray");
            sendBut.Image = LoadImage("send");
            Mouse.ListenOn(sendBut, MouseButton.Left, ButtonState.Pressed, Send, "Send message to server");
        }
        else
        {
            title.Text = "Not connected!";
            connBut.Image = LoadImage("tryconnect");
            sendBut.Image = LoadImage("sendGray");
            Mouse.ListenOn(connBut, MouseButton.Left, ButtonState.Pressed, Connect, "Try connect to server");
        }
    }

    // prepare message to be sent
    private void Send()
    {
        anNum++;
        client.Send(ask.Text.ToString());
        ask.Text = anNum.ToString();
    }

    // ask server IP
    private void Connect()
    {
        InputWindow iWindow = new InputWindow("Server IP:");
        iWindow.QueryWidget.Text = "192.168.1.1";
        iWindow.TextEntered += ProcessInput;
        Add(iWindow);
    }

    // prepare to try to connecting to server
    private void ProcessInput(InputWindow window)
    {
        ClearAll();
        Establish();
        string resp = window.InputBox.Text;
        MessageDisplay.Add("Connecting...");
        Timer.SingleShot(1, delegate { OpenClient(client.Connect(resp)); });
    }

    // set message from server to label
    public void Receive(string text)
    {
        answer.Text = text.ToString();
    }
}
