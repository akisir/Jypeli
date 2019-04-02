// GolfClient.cs 1.4.2019
// by Aki Sirviö
using System;
using Jypeli;

//GolfClient send messages to GolfServer.
//It makes possible to play game with client
//that acts as controller.
namespace GolfClient
{
    public class GolfClient : Game
    {
        HandleConnection gameSocket;
        GameObject button;

        // starts here
        public override void Begin()
        {
            gameSocket = new HandleConnection();
            gameSocket.CallBack = Manage;
            MessageDisplay.Add(gameSocket.LoopConnect(null));
            Level.Background.Color = Color.BlueGray;
            Mouse.IsCursorVisible = true;
            AddButton();
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "End game");
        }

        // receive messages from HandleConnection class
        private void Manage(string reply)
        {
            if (reply == "yhdistetty")
            {
                IsReady();
            }
            else if (reply == "valmista")
            {
                button.Image = LoadImage("go");
                Mouse.ListenOn(button, MouseButton.Left, ButtonState.Pressed, Go, "Kisaan mukaan");
            }
            else if (reply == "ei valmista")
            {
                IsReady();
            }
            else if(reply == "restart")
            {
                ClearAll();
                Begin();
            }
        }

        // add go button
        private void AddButton()
        {
            button = new GameObject(300, 300);
            button.Image = LoadImage("go0");
            Add(button);
        }

        // send 'ready?' message to server via HandleConnoction class
        private void IsReady()
        {
            Timer.SingleShot(1, delegate { gameSocket.Send("4@,valmista?"); });
        }

        // opens window where palyer enters nickname
        private void Go()
        {
            InputWindow name = new InputWindow("");
            name.TextEntered += Recover;
            name.Color = Color.YellowGreen;
            name.ActiveColor = Color.BrownGreen;
            name.Image = LoadImage("pelaaja2");
            name.MaxCharacters = 15;
            name.Shape = Shape.Circle;
            name.TextureFillsShape = true;
            name.OKButton.Image = LoadImage("go0");
            name.OKButton.ImageHover = LoadImage("go");
            name.OKButton.Shape = Shape.Circle;
            name.OKButton.Text = "";
            name.OKButton.TextureFillsShape = true;
            name.QueryWidget.TextScale *= 2;
            name.QueryWidget.SizeMode = TextSizeMode.AutoSize;
            Add(name);
        }

        // sends nickname to server via HandleConnection class
        private void Recover(InputWindow name)
        {
            if (name.InputBox.Text.Length > 0)
            {
                string playerName = name.InputBox.Text;
                gameSocket.Send("1@," + playerName);
                button.Destroy();
                Controls();
            }
        }

        // makes buttons of swing, turn right and turn left
        private void Controls()
        {
            GameObject swing = new GameObject(300, 120);
            swing.Image = LoadImage("heilauta");
            swing.Y += 70;
            Add(swing);
            Mouse.ListenOn(swing, MouseButton.Left, ButtonState.Pressed, Swing, "Swing racket");
            Keyboard.Listen(Key.Space, ButtonState.Pressed, Swing, "Swing racket");
            GameObject Right = new GameObject(120, 120);
            Right.Image = LoadImage("kaannaOikea2");
            Right.X += 70;
            Right.Y -= 70;
            Add(Right);
            Mouse.ListenOn(Right, MouseButton.Left, ButtonState.Down, Roll, "Roll racket to right", 1);
            Keyboard.Listen(Key.D, ButtonState.Down, Roll, "Roll racket to right", 1);
            GameObject Left = new GameObject(120, 120);
            Left.Image = LoadImage("kaannaVasen2");
            Left.X -= 70;
            Left.Y -= 70;
            Add(Left);
            Mouse.ListenOn(Left, MouseButton.Left, ButtonState.Down, Roll, "Roll racket to left", -1);
            Keyboard.Listen(Key.A, ButtonState.Down, Roll, "Roll racket to left", -1);
        }

        // sends message to server to hit the ball
        private void Swing()
        {
            gameSocket.Send("2@,200");
        }

        // sends message to server to roll the racket
        private void Roll(int dir)
        {
            gameSocket.Send("3@," + dir);
        }

        // ensure that player is removed when program is closed
        protected override void OnExiting(object sender, EventArgs e)
        {
            gameSocket.Send("5@,bye");
        }
    }
}
