//GameChat.cs 27.3.2019
//by Aki Sirviö
using System;
using System.Collections;
using Jypeli;

//Socket based multiuser game chat
namespace Program
{
    public class GameChat : PhysicsGame
    {
        const double Nopeus = 200;
        const double HyppyNopeus = 750;
        const int RUUDUN_KOKO = 40;

        PlatformCharacter pelaaja1;

        Image pelaajanKuva = LoadImage("norsu");
        Image tahtiKuva = LoadImage("tahti");

        SoundEffect maaliAani = LoadSoundEffect("maali");

        // game starts here, makes new chat class object
        public override void Begin()
        {
            ManageChat chat = new ManageChat(Window);
            chat.AddGameObject = this.AddGameObject;
            chat.AddComponents();
            chat.Connect(null);
            StartGame();
        }

        // enables to make jypeli objects in ManageChat class
        private void AddGameObject(Hashtable obj)
        {
            var o = (InputWindow)obj[1];
            Add(o);
        }

        // game initial settings
        void StartGame()
        {
            Gravity = new Vector(0, -1000);

            LuoKentta();
            LisaaNappaimet();

            Mouse.IsCursorVisible = true;
            Window.AllowUserResizing = true;
            SetWindowSize(1200, 768, false);
            Screen.Width = 650;
            Screen.Height = 750;
            Window.Title = "Game chat by Jypeli & Aki Sirviö";
            Camera.Follow(pelaaja1);
            Camera.ZoomToLevel();
        }

        // show messages in top left game area
        void ShowMessage(string text)
        {
            MessageDisplay.Add(text);
        }

        // makes game map
        void LuoKentta()
        {
            TileMap kentta = TileMap.FromLevelAsset("kentta1");
            kentta.SetTileMethod('#', LisaaTaso);
            kentta.SetTileMethod('*', LisaaTahti);
            kentta.SetTileMethod('N', LisaaPelaaja);
            kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
            Level.CreateBorders(false);
            Level.Background.CreateGradient(Color.White, Color.SkyBlue);
        }

        // add game blogs
        void LisaaTaso(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
            taso.Position = paikka;
            taso.Color = Color.Green;
            Add(taso);
        }

        // add collect stars
        void LisaaTahti(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
            tahti.IgnoresCollisionResponse = true;
            tahti.Position = paikka;
            tahti.Image = tahtiKuva;
            tahti.Tag = "tahti";
            Add(tahti);
        }

        // add game character
        void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
        {
            pelaaja1 = new PlatformCharacter(leveys, korkeus);
            pelaaja1.Position = paikka;
            pelaaja1.Mass = 4.0;
            pelaaja1.Image = pelaajanKuva;
            AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
            Add(pelaaja1);
        }

        // add controls
        void LisaaNappaimet()
        {
            Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

            Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -Nopeus);
            Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, Nopeus);
            Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HyppyNopeus);

            ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

            ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1, -Nopeus);
            ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, Nopeus);
            ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HyppyNopeus);

            PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        }

        // puts character in motion
        void Liikuta(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Walk(nopeus);
        }

        // puts character to jump
        void Hyppaa(PlatformCharacter hahmo, double nopeus)
        {
            hahmo.Jump(nopeus);
        }

        // collision handler collect stars
        void TormaaTahteen(PhysicsObject hahmo, PhysicsObject tahti)
        {
            maaliAani.Play();
            MessageDisplay.Add("You collected a star!");
            tahti.Destroy();
        }
    }
}
