//FirstAndroidGame.cs 4.4.2019
//by Aki Sirviö
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

//Collect all stars and be super, fun game
namespace FirstAndroidGame
{
    public class FirstAndroidGame : PhysicsGame
    {
        const double HyppyNopeus = 350;
        const double suunta = 150;
        const int RUUDUN_KOKO = 40;
        int tahdet = 0;
        int vihut = 0;
        bool maalissa = false;
        Vector alku;
        Timer aikaLaskuri;
        Timer superLaskuri;

        PlatformCharacter pelaaja1;
        PlatformCharacter vihu;
        PhysicsObject crown;
        PlatformWandererBrain tasoAivot;
        ScoreList topLista = new ScoreList(10, true, 500);

        Image pelaajanKuva = LoadImage("norsu");
        Image tahtiKuva = LoadImage("tahti");

        SoundEffect maaliAani = LoadSoundEffect("maali");
        Sound loppuAani;
        Sound lapsyAani;

        Image[] charAnim = LoadImages("char", "char2", "char3");
        Image[] vihuAnim = LoadImages("pacman", "pacman2");
        Image[] fireBrig = LoadImages("brigwallanim31", "brigwallanim39",
            "brigwallanim32", "brigwallanim35", "brigwallanim38", "brigwallanim11",
            "brigwallanim3", "brigwallanim21", "brigwallanim22", "brigwallanim20",
            "brigwallanim8", "brigwallanim46", "brigwallanim48", "brigwallanim49",
            "brigwallanim52", "brigwallanim42", "brigwallanim44", "brigwallanim41",
            "brigwallanim51", "brigwallanim15", "brigwallanim25", "brigwallanim10",
            "brigwallanim35", "brigwallanim39");

        // game starts here
        public override void Begin()
        {
            Gravity = new Vector(0, -1000);
            SoundEffect lapsy = LoadSoundEffect("flap7");
            lapsyAani = lapsy.CreateSound();
            lapsyAani.Volume = 0.15;
            SoundEffect loppu = LoadSoundEffect("end12");
            loppuAani = loppu.CreateSound();
            loppuAani.Volume = 0.5;

            LuoKentta();
            LisaaKontrollit();
            TeeAikaLaskuri();

            Camera.Follow(pelaaja1);
            Camera.ZoomFactor = 1.2;
            Camera.StayInLevel = true;

            MediaPlayer.Play("FirstBackMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3;
            topLista = DataStorage.TryLoad<ScoreList>(topLista, "pisteet.xml");
        }

        // make game objects
        void LuoKentta()
        {
            TileMap kentta = TileMap.FromLevelAsset("kentta1");
            kentta.SetTileMethod('#', LisaaTaso);
            kentta.SetTileMethod('*', LisaaTahti);
            kentta.SetTileMethod('N', LisaaPelaaja);
            kentta.SetTileMethod('K', LisaaAlaTaso);
            kentta.SetTileMethod('T', LisaaYlaTaso);
            kentta.SetTileMethod('V', LisaaVihu);
            kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
            Level.CreateBorders();
            Level.Background.CreateGradient(Color.White, Color.SkyBlue);
        }

        // left, right and top blogs
        void LisaaTaso(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus*2);
            taso.Animation = new Animation(fireBrig);
            taso.Position = paikka;
            taso.Tag = "taso";
            Add(taso);

            double rand = RandomGen.NextDouble(0.5, 3.0);
            Timer.SingleShot(rand, taso.Animation.Start);
            taso.Animation.FPS = 5;
        }

        // bottom blogs
        void LisaaAlaTaso(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject alaTaso = PhysicsObject.CreateStaticObject(leveys*2, korkeus);
            alaTaso.Position = paikka;
            alaTaso.Image = LoadImage("brigwall");
            alaTaso.Tag = "alataso";
            Add(alaTaso);
        }

        // invisible blogs
        void LisaaYlaTaso(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject ylaTaso = PhysicsObject.CreateStaticObject(leveys*2, korkeus);
            ylaTaso.Position = paikka;
            ylaTaso.IsVisible = false;
            ylaTaso.CollisionIgnoreGroup = 1;
            Add(ylaTaso);
        }

        // stars
        void LisaaTahti(Vector paikka, double leveys, double korkeus)
        {
            PhysicsObject tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
            tahti.IgnoresCollisionResponse = true;
            tahti.Position = paikka;
            tahti.Image = tahtiKuva;
            tahti.Tag = "tahti";
            Add(tahti);
            tahdet++;
        }

        // character
        void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
        {
            pelaaja1 = new PlatformCharacter(leveys, korkeus);
            pelaaja1.Position = paikka;
            pelaaja1.Image = pelaajanKuva;
            pelaaja1.MirrorImage();
            pelaaja1.CollisionIgnoreGroup = 1;
            pelaaja1.MaintainMomentum = true;
            pelaaja1.IgnoresExplosions = true;
            AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
            AddCollisionHandler(pelaaja1, "taso", TormaaKuolettavaan);
            AddCollisionHandler(pelaaja1, "vihu", TormaaKuolettavaan);
            AddCollisionHandler(pelaaja1, "alataso", TormaaAlaTasoon);
            Add(pelaaja1);

            pelaaja1.AnimIdle = new Animation(charAnim);
            pelaaja1.AnimIdle.FPS = 10;

            tasoAivot = new PlatformWandererBrain();
            tasoAivot.Speed = 100;
            pelaaja1.Brain = tasoAivot;
            alku = paikka;
        }

        // enemies
        void LisaaVihu(Vector paikka, double leveys, double korkeus)
        {
            vihu = new PlatformCharacter(leveys, korkeus);
            vihu.Position = paikka;
            vihu.CollisionIgnoreGroup = 2;
            vihu.Image = LoadImage("pacman2");
            vihu.MirrorImage();
            vihu.Tag = "vihu";
            Add(vihu);

            vihu.Animation = new Animation(vihuAnim);
            vihu.Animation.FPS = 1;
            vihu.Animation.Start();

            PlatformWandererBrain aivot = new PlatformWandererBrain();
            aivot.Speed = 200;
            vihu.Brain = aivot;
            vihut++;
        }

        // controls
        void LisaaKontrollit()
        {
            Widget vasenReuna = new Widget(Screen.Width / 3, Screen.Height);
            vasenReuna.Left = Screen.Left;
            vasenReuna.IsVisible = false;
            Add(vasenReuna);

            Widget oikeaReuna = new Widget(Screen.Width / 3, Screen.Height);
            oikeaReuna.Right = Screen.Right;
            oikeaReuna.IsVisible = false;
            Add(oikeaReuna);

            Widget ylaReuna = new Widget(Screen.Width/3, Screen.Height / 3);
            ylaReuna.Top = Screen.Top;
            ylaReuna.Left = Screen.Left + Screen.Width / 3;
            ylaReuna.IsVisible = false;
            Add(ylaReuna);

            TouchPanel.ListenOn(vasenReuna, ButtonState.Pressed, Liikuta, "Liikuta pelaajaa", pelaaja1, -suunta, HyppyNopeus);
            TouchPanel.ListenOn(oikeaReuna, ButtonState.Pressed, Liikuta, "Liikuta pelaajaa", pelaaja1, suunta, HyppyNopeus);
            TouchPanel.ListenOn(ylaReuna, ButtonState.Pressed, Hyppaa, "Hyppää", pelaaja1, HyppyNopeus);
            PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        }

        // time label
        void TeeAikaLaskuri()
        {
            aikaLaskuri = new Timer();
            aikaLaskuri.Start();

            Label aikaNaytto = new Label();
            aikaNaytto.TextColor = Color.ForestGreen;
            aikaNaytto.Y = Screen.Top - 100;
            aikaNaytto.X += 250;
            aikaNaytto.DecimalPlaces = 1;
            aikaNaytto.BindTo(aikaLaskuri.SecondCounter);
            Add(aikaNaytto);
        }

        // move character
        void Liikuta(Touch kosketus, PlatformCharacter hahmo, double liike, double hyppy)
        {
            lapsyAani.Play();
            hahmo.Walk(liike);
            hahmo.ForceJump(hyppy);

            if(tasoAivot.Active)
            {
                tasoAivot.Active = false;
            }
        }

        // character jumps
        void Hyppaa(Touch kosketus, PlatformCharacter hahmo, double hyppy)
        {
            hahmo.ForceJump(hyppy);
        }

        // character collecting star
        void TormaaTahteen(PhysicsObject hahmo, PhysicsObject tahti)
        {
            maaliAani.Play();
            tahti.Destroy();
            tahdet--;
            if(tahdet <= 0)
            {
                aikaLaskuri.Stop();
                Maalissa();
            }
        }

        // character dies and game begins
        void TormaaKuolettavaan(PhysicsObject hahmo, PhysicsObject kohde)
        {
            if (!maalissa)
            {
                pelaaja1.Position = alku;
                loppuAani.Play();
                Alusta();
            }
            else if(kohde.Tag.ToString() == "vihu")
            {
                vihut--;
                kohde.Destroy();
                if(vihut <= 0)
                {
                    Super();
                }
            }
        }

        // character hits bottom
        void TormaaAlaTasoon(PhysicsObject hahmo, PhysicsObject kohde)
        {
            tasoAivot.Active = true;
        }

        // game begins
        void Alusta()
        {
            maalissa = false;
            tahdet = 0;
            vihut = 0;
            ClearAll();
            Begin();
        }

        // character collects all stars
        void Maalissa()
        {
            maalissa = true;
            pelaaja1.Position = alku;
            pelaaja1.Stop();

            crown = PhysicsObject.CreateStaticObject(30, 30);
            crown.Image = LoadImage("crown");
            crown.CollisionIgnoreGroup = 1;
            pelaaja1.Add(crown);
            crown.Y += 40;

            Explosion rajahdys = new Explosion(5000);
            rajahdys.Force = 1;
            rajahdys.Speed = 25000;
            Add(rajahdys);

            MediaPlayer.Stop();
            aikaLaskuri.Stop();

            superLaskuri = new Timer();
            superLaskuri.Interval = 10;
            superLaskuri.Timeout += Alusta;
            superLaskuri.Start(1);
        }

        // top score window
        void ParhaatPisteet()
        {
            double aika = Math.Round(aikaLaskuri.SecondCounter, 1);

            HighScoreWindow topIkkuna = new HighScoreWindow(
                "Parhaat pisteet", "Onneksi olkoon, pääsit listalle ajalla " +
                "%p s Syötä nimesi:", topLista, aika);
            topIkkuna.Closed += TallennaPisteet;
            Add(topIkkuna);
        }

        // save score
        void TallennaPisteet(Window sender)
        {
            DataStorage.Save<ScoreList>(topLista, "pisteet.xml");
            Timer.SingleShot(10, Alusta);
        }

        // super character shows top score window
        void Super()
        {
            superLaskuri.Stop();
            pelaaja1.Size *= 3;
            crown.Size *= 3;
            crown.Y += 70;

            Label sup = new Label(500, 200);
            sup.Text = "Super";
            sup.TextScale *= 3;
            sup.TextColor = Color.OrangeRed;
            Add(sup, -3);

            Timer.SingleShot(2, ParhaatPisteet);
        }
    }
}
