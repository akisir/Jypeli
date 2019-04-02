// GolfServer.cs 1.4.2019
// by Aki Sirviö
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Effects;
using System.Net.NetworkInformation;

// Golf game server is actually the game.
// It takes care of communication with clients
// and rans the game.
namespace GolfServer
{
    public class GolfServer : PhysicsGame
    {
        readonly string labelText = "Golf Server";
        HandleConnections handleCon;
        List<Player> playersList = new List<Player>();
        GameObject button;
        Player winner;
        Vector racketPosition;
        Vector ballPosition;
        int turn;
        int levelNro;
        int deletedPlay;
        bool go;
        bool end;
        bool gameStarted ;
        int nro;

        // game starts here
        public override void Begin()
        {
            SetWindowSize(1500, 1000);
            handleCon = new HandleConnections();
            handleCon.callBack = ManageGame; //action delegate
            Mouse.IsCursorVisible = true;
            Init();
            LevelAnim();
        }

        // initial settings
        void Init()
        {
            winner = null;
            turn = 0;
            levelNro = 0;
            deletedPlay = 0;
            go = false;
            end = false;
            gameStarted = false;
            nro = 0;
        }

        // animation at beginning of each game level
        void LevelAnim()
        {
            int a = levelNro;
            ClearAll();
            LevelBack();
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, NextLevel, "Seuraavalle tasolle");
            
            if (a == 0)
                a = 1;
            for (int i = 0; i < a; i++)
            {
                int dir = 0;
                PhysicsObject signBase = new PhysicsObject(70, 70);
                signBase.Position = new Vector(Screen.Left, 0);
                signBase.CollisionIgnoreGroup = 2;
                signBase.Shape = Shape.Circle;
                signBase.IsVisible = false;
                signBase.Restitution = 1.0;
                Add(signBase);

                Label sign = new Label();
                sign.TextColor = Color.White;
                sign.Color = Color.Transparent;
                sign.Font = Font.DefaultLargeBold;
                sign.TextScale = new Vector(1.7, 1.7);
                sign.Text = SelectTitle();
                signBase.Add(sign);
                sign.X -= 35;
                signBase.Hit(RandomGen.NextVector(50, 1000));

                if (i % 2 == 0)
                {
                    dir = -1;
                }
                else
                {
                    dir = 1;
                }

                turn = 1000;
                Timer.SingleShot(0.1, delegate { SignAnimation(signBase, sign, dir, 255); });
            }
        }

        // level animation rotation, scaling and text transparency
        void SignAnimation(PhysicsObject signBase, Label sign, int dir, double alpha)
        {
            sign.Angle += Angle.FromDegrees(1 * dir);
            signBase.Angle += Angle.FromDegrees(1 * dir);
            sign.TextScale += new Vector(0.007, 0.007);
            sign.TextColor = new Color((int)alpha, (int)alpha, (int)alpha, (int)alpha);
            alpha -= 0.7;

            if (alpha <= 0 && turn != 0)
            {
                NextLevel();
            }
            else if (alpha > 0)
            {
                Timer.SingleShot(0.02, delegate { SignAnimation(signBase, sign, dir, alpha); });
            }
        }

        // add level borders and background color
        void LevelBack()
        {
            Level.Height = Screen.Height;
            Level.Width = Screen.Width;
            Level.Background.Color = Color.DarkGreen;
            Level.CreateBorders(1.0, false);
        }

        // load next level
        void NextLevel()
        {
            turn = 0;
            ClearAll();

            if (levelNro == 0)
            {
                MakeGround("Taso00");
                go = true;
            }
            else if (levelNro == 1)
            {
                gameStarted = true;
                MakeGround("Taso1");
            }
            else if (levelNro == 2)
            {
                MakeGround("Taso2");
            }
            else if (levelNro == 3)
            {
                MakeGround("Taso3");
                end = true;
            }
            else if (levelNro > 3)
            {
                MakeGround("Loppu");
            }

            Controls();
            LevelBack();
            LevelSettings();
        }

        // controls
        void Controls()
        {
            Keyboard.Listen(Key.L, ButtonState.Pressed, DebugString, "Add player", 1, "player", "2019");
            Keyboard.Listen(Key.Space, ButtonState.Pressed, DebugString, "Swing racket", 2, "300,0", "2019");
            Keyboard.Listen(Key.P, ButtonState.Down, Swing, "Swing racket hard", 3000.0);
            Keyboard.Listen(Key.A, ButtonState.Down, ManageGame, "Roll racket right", 3, "1,0", "20191");
            Keyboard.Listen(Key.D, ButtonState.Down, ManageGame, "Roll racket left", 3, "-1,0", "20191");
            Keyboard.Listen(Key.Left, ButtonState.Down, ManageGame, "Roll racket right", 3, "1,0", "20190");
            Keyboard.Listen(Key.Right, ButtonState.Down, ManageGame, "Roll racket left", 3, "-1,0", "20190");
            Keyboard.Listen(Key.O, ButtonState.Pressed, GoBegin, "Moves ball to begin");
            Keyboard.Listen(Key.I, ButtonState.Pressed, ShowServerIP, "Shows server IP");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "End game");
        }

        // add level objects
        void MakeGround(string taso)
        {
            ColorTileMap ruudut = ColorTileMap.FromLevelAsset(taso);
            ruudut.SetTileMethod(Color.FromHexCode("4CFF00"), RacketPosition);
            ruudut.SetTileMethod(Color.FromHexCode("808080"), BallPosition);
            ruudut.SetTileMethod(Color.FromHexCode("0026FF"), AddHole);
            ruudut.SetTileMethod(Color.FromHexCode("FF1420"), AddLabel1);
            ruudut.SetTileMethod(Color.FromHexCode("FF933A"), AddLabel2);
            ruudut.SetTileMethod(Color.FromHexCode("E9FF47"), AddButton);
            ruudut.SetTileMethod(Color.Black, AddBorder);
            ruudut.Execute(17, 17);
        }

        // position of first racket
        void RacketPosition(Vector position, double width, double height)
        {
            racketPosition = position;
        }

        // position of all golf balls
        void BallPosition(Vector position, double width, double height)
        {
            ballPosition = position;
        }

        // add hole and its frame
        void AddHole(Vector position, double width, double height)
        {
            PhysicsObject frame = PhysicsObject.CreateStaticObject(70, 70);
            frame.Position = position;
            frame.Shape = Shape.Circle;
            frame.Color = Color.Green;
            frame.CollisionIgnoreGroup = 1;
            Add(frame);

            PhysicsObject hole = PhysicsObject.CreateStaticObject(30, 30);
            hole.Position = position;
            hole.Shape = Shape.Circle;
            hole.Color = Color.Black;
            hole.Restitution = 0.0;
            Add(hole);
            AddCollisionHandler(hole, ToHole);
        }

        // add label with game name and level number
        void AddLabel1(Vector position, double width, double height)
        {
            Label label = new Label();
            label.Position = position;
            label.TextColor = Color.White;
            label.Color = Color.Transparent;
            label.TextScale = new Vector(2, 2);
            label.Font = Font.DefaultLargeBold;
            label.Text = SelectTitle();
            Add(label);
        }

        // choose text of title
        string SelectTitle()
        {
            string title = "";
            if (levelNro != 0)
            {
                title = labelText + " level " + levelNro;
            }
            else
            {
                title = labelText;
            }

            return title;
        }

        // add label with score and numbers of shots
        void AddLabel2(Vector position, double width, double height)
        {
            Label label = new Label();
            label.Position = position + new Vector(-50, 0);
            label.TextColor = Color.LightGray;
            label.Color = Color.Transparent;
            label.Text = "Score / Shots";
            label.TextScale = new Vector(1, 1);
            label.Font = Font.DefaultLargeBold;
            Add(label);
        }

        // add start button
        void AddButton(Vector position, double width, double height)
        {
            button = new GameObject(400, 150);
            button.Position = position;
            button.Image = LoadImage("start0");
            Add(button);
        }

        // borders of golf path
        void AddBorder(Vector position, double width, double height)
        {
            PhysicsObject border = PhysicsObject.CreateStaticObject(width, height);
            border.Position = position;
            border.Color = Color.Gray;
            border.Restitution = 0.2;
            border.CollisionIgnoreGroup = 2;
            Add(border);

            Shape randShape = RandomGen.SelectOne(Shape.Rectangle, Shape.Circle, Shape.Diamond, Shape.Hexagon, Shape.Triangle);
            border.Shape = Shape.Circle;
        }

        // place rackets, balls and names for new level
        void LevelSettings()
        {
            turn = 0;

            if (playersList.Count > 0)
            {
                if (winner != null)
                {
                    double scale = 35 / winner.name.Height;
                    winner.name.TextScale *= scale;
                }

                foreach (Player play in playersList)
                {
                    play.racket.Angle = Angle.FromDegrees(0);
                    play.racket.Size = new Vector(100, 100);
                    Add(play.racket);
                    racketPosition = play.racket.Position;
                    
                    play.ball.Position = ballPosition;
                    play.ball.Stop();
                    play.ball.IsVisible = true;
                    Add(play.ball);
                    Add(play.name);
                    PlayerSign(play.ball.Color);
                    AddScores(play, play.score);

                    play.shots.Value = 0;
                    play.inHole = false;
                }
            }
        }

        // controls action of game, called from GolfServer class
        void ManageGame(int command, string text, string playerNro)
        {
            SelectPlayer(playerNro);

            switch (command)
            {
               case 1:
                   AddPlayer(text, playerNro);
                   break;
               case 2:
                   {
                       double luku = 0.0;
                       if (double.TryParse(text, out luku))
                           Swing(luku);
                       break;
                   }
               case 3:
                   {
                       double luku = 0.0;
                       if (double.TryParse(text, out luku))
                           Roll(luku);
                       break;
                   }
               case 4:
                   {
                       if (go)
                       {
                           handleCon.Sendata(playerNro, "valmista");
                       }
                       else
                       {
                           handleCon.Sendata(playerNro, "ei valmista");
                       }
                       break;
                   }
               case 5:
                   {
                       DeletePlayer(playerNro);
                       break;
                   }
            }
        }

        // retrieves number of player who sent command
        void SelectPlayer(string playerNro)
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                if (playerNro == playersList[i].ID)
                {
                    turn = i;
                }
            }
        }

        // add racket, ball and scoreboard
        void AddPlayer(string name, string playerNro)
        {
            if (!gameStarted)
            {
                Player player = new Player();
                PhysicsObject racket = new PhysicsObject(100, 100);
                racket.Position = racketPosition;
                racket.Image = LoadImage("Maila");
                racket.CanRotate = false;
                racket.CollisionIgnoreGroup = 1;
                racket.Restitution = 0.0;
                racket.IgnoresExplosions = true;
                Add(racket);
                player.racket = racket;
                player.ID = playerNro;

                AddNamePlate(player, name);
                Color color = AddBall(player);
                PlayerSign(color);
                AddScores(player, 0);
                racketPosition += new Vector(0, -120);
                playersList.Add(player);

                if (playersList.Count == 2)
                {
                    button.Image = LoadImage("start");
                    levelNro++;
                    Mouse.ListenOn(button, MouseButton.Left, ButtonState.Pressed, NextLevel, "Peli alkaa");
                }
            }
        }

        // add label showing name of player
        void AddNamePlate(Player player, string name)
        {
            Label namePlate = new Label();
            namePlate.Position = racketPosition + new Vector(-170, +25);
            namePlate.TextColor = Color.Black;
            namePlate.Color = Color.Transparent;
            namePlate.Text = name;
            namePlate.Font = Font.DefaultLarge;
            Add(namePlate);
            player.name = namePlate;
        }

        // players ball in random color
        Color AddBall(Player player)
        {
            Color color = Color.Black;
            while (color.BlueComponent < 50 || color.GreenComponent < 70 || color.RedComponent < 50)
            {
                color = RandomGen.NextColor();
            }
            PhysicsObject ball = new PhysicsObject(15, 15);
            ball.Position = ballPosition;
            ball.Shape = Shape.Circle;
            ball.CollisionIgnoreGroup = 1;
            ball.Color = color;
            ball.Restitution = 0.2;
            ball.CanRotate = false;
            ball.Tag = player.ID.ToString();
            Add(ball);
            player.ball = ball;
            
            return color;
        }

        // add sign next to racket that tells player color
        void PlayerSign(Color color)
        {
            GameObject playerSign = new GameObject(15, 25);
            playerSign.Position = racketPosition + new Vector(20, 15);
            playerSign.Color = color;
            playerSign.Shape = Shape.Diamond;
            Add(playerSign);
        }

        // add labels of score and shots
        void AddScores(Player player, int startScore)
        {
            player.score = new IntMeter(startScore);
            player.shots = new IntMeter(0);
            Label scoreLabel = new Label();
            scoreLabel.Position = racketPosition + new Vector(-170, -5);
            scoreLabel.Font = Font.DefaultBold;
            scoreLabel.BindTo(player.score);
            Add(scoreLabel);

            Label shotLabel = new Label();
            shotLabel.Position = racketPosition + new Vector(-90, -5);
            shotLabel.Font = Font.DefaultBold;
            shotLabel.BindTo(player.shots);
            Add(shotLabel);
        }

        // hit the ball
        void Swing(double force)
        {
            if (playersList.Count > 0)
            {
                Angle dir = playersList[turn].racket.Angle + Angle.FromDegrees(-90);
                playersList[turn].ball.Hit(dir.GetVector() * force);
                playersList[turn].shots.Value++;
                LoadSoundEffect("swing").Play();
            }
        }

        // roll the racket
        void Roll(double dir)
        {
            if (playersList.Count > 0)
            {
                playersList[turn].racket.Angle += Angle.FromDegrees(0.5 * dir);
            }
        }

        // moves ball to starting point
        void GoBegin()
        {
            playersList[turn].ball.Position = ballPosition;
            playersList[turn].ball.Stop();
        }

        // ball hits hole
        void ToHole(PhysicsObject hole, PhysicsObject target)
        {
            Player player = null;
            int inFinish = 0;
            foreach(Player play in playersList)
            {
                if(play.ID.ToString() == target.Tag.ToString())
                {
                    player = play;
                    player.inHole = true;
                }

                if (play.inHole)
                    inFinish++;
            }

            target.Position = player.racket.Position;
            target.Stop();
            CountScore(player);
            LoadSoundEffect("golf_ball_putt_in").Play();

            if (player.shots.Value == 1 && winner == null)
            {
                winner = player;
            }

            // all players in goal
            if (inFinish == (playersList.Count - deletedPlay))
            {
                FindWinner();
                levelNro++;
                if (!end)
                {
                    Jypeli.Timer.SingleShot(10, LevelAnim);
                }
                else
                {
                    NextLevel();
                }
            }
        }

        // searching player with fewest shots
        void FindWinner()
        {
            int shots = 10000;
            for (int i = 0; i < playersList.Count; i++)
            {
                if (playersList[i].shots.Value < shots)
                {
                    shots = playersList[i].shots.Value;
                    winner = playersList[i];
                }
            }

            ShowWinner(2);
        }

        // count hit score
        void CountScore(Player player)
        {
            if (playersList.Count > 0)
            {
                int score = 100 / player.shots.Value;
                if (score < 0)
                {
                    score = 0;
                }
                player.score.Value += score;
            }
        }

        // animation of winners racket and name
        void ShowWinner(int luku)
        {
            luku++;
            if ((luku % 2) != 0)
            {
                winner.racket.Size *= 1.35;
                winner.name.TextScale *= 1.35;
            }
            else if ((luku % 2) == 0)
            {
                winner.racket.Size *= 0.75;
                winner.name.TextScale *= 0.75;
            }

            if (luku < 20)
            {
                Exp();
            }

            Timer.SingleShot(0.2, delegate { ShowWinner(luku); });
        }

        // huge explosion before next level
        void Exp()
        {
            Explosion explo = new Explosion(1000);
            explo.Position = new Vector(0, 0);
            Add(explo);
            int pMax = 200;
            ExplosionSystem explo2 = new ExplosionSystem(LoadImage("Maila"), pMax);
            Add(explo2);
            double x = 0;
            double y = 0;
            int pVolume = 50;
            explo2.AddEffect(x, y, pVolume);
        }

        // if client is closed, player is removed from race
        void DeletePlayer(string playerNro)
        {
            for (int i = 0; i < playersList.Count; i++)
            {
                if (playerNro == playersList[i].ID)
                {
                    deletedPlay++;
                    playersList[i].racket.Image = LoadImage("maila0");
                    playersList[i].ball.IsVisible = false;
                    playersList[i].shots.Value = 1000;

                    if ((playersList.Count - deletedPlay) == 1)
                    {
                        button.Image = LoadImage("start0");
                        Mouse.Clear();
                    }
                    else if((playersList.Count - deletedPlay) < 1)
                    {
                        deletedPlay = 0;
                        foreach(Player play in playersList)
                        {
                            handleCon.RemoveClient(play.ID);
                        }
                        playersList.Clear();
                        Init();
                        levelNro = 0;
                        NextLevel();
                        break;
                    }
                }
            }
        }

        // show ip address of server in label
        void ShowServerIP()
        {
            Label label = new Label();
            label.Position = new Vector(250, 200);
            label.TextColor = Color.White;
            label.Color = Color.Transparent;
            label.TextScale = new Vector(2, 2);
            label.Font = Font.DefaultLargeBold;
            label.LifetimeLeft = new TimeSpan(0, 0, 5);
            Add(label);

            List<string> stringList = new List<string>();
            stringList = handleCon.GetLocalIP(NetworkInterfaceType.Ethernet);
            foreach (string ip in stringList)
            {
                label.Text = "ServerIP: " + ip;
            }
        }

        // draw guideline of ball
        protected override void Paint(Canvas canvas)
        {
            if (playersList.Count > 0 && playersList.Count > turn)
            {
                if (playersList[turn].ball.Position != playersList[turn].racket.Position)
                {
                    if (playersList[turn].prevAngle != playersList[turn].racket.Angle)
                    {
                        Angle angle = playersList[turn].racket.Angle - Angle.FromDegrees(90);
                        Vector line = new Vector(angle.Cos, (angle).Sin);
                        Vector center = playersList[turn].ball.Position + line * 10;
                        Vector edge = line * 300;
                        canvas.BrushColor = Color.DarkForestGreen;

                        canvas.DrawLine(center, center + edge);
                    }
                    playersList[turn].prevAngle = playersList[turn].racket.Angle;
                }
            }

            base.Paint(canvas);
        }

        // helps to test game without clients
        void DebugString(int command, string text, string playerNro)
        {
            if(command == 1)
            {
                playerNro += nro++;
                ManageGame(command, text, playerNro);
            }
            else if(command == 2)
            {
                if (nro <= 0)
                    nro = playersList.Count;
                 playerNro += --nro;
                ManageGame(command, text, playerNro);
            }
        }
    }
}
