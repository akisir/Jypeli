//LabyrinthGolf.cs for hobby game projects
//10.4.2018 by Aki Sirviö
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

//multipalyer minigolf
//new player form enter key and hit form space key
//use arrow keys to rotate club
public class LabyrinthGolf : PhysicsGame
{
    List<PhysicsObject> pelaajat = new List<PhysicsObject>();
    List<Label> nimet = new List<Label>();
    List<PhysicsObject> pallot = new List<PhysicsObject>();
    List<IntMeter> pisteetLista = new List<IntMeter>();
    List<IntMeter> lyonnitLista = new List<IntMeter>();
    List<int> maalissa = new List<int>();
    List<ScoreList> parhaatPisteet = new List<ScoreList>();
    PhysicsObject maila;
    PhysicsObject pallo;
    Vector pelaajanPaikka;
    Vector pallonPaikka;
    IntMeter pisteet;
    IntMeter lyonnit;
    Angle mailanKulma;
    int vuoro = 0;
    int monesOsuma = 0;
    int kenttaNro = 1;
    int voittaja = 0;

    SoundEffect swing = LoadSoundEffect("Swing");
    SoundEffect golfball = LoadSoundEffect("golfball");
    Image rKuva1 = LoadImage("explosion");
    Image rKuva2 = LoadImage("explosion2");
    Image rKuva3 = LoadImage("explosion4");
    Image rKuva4 = LoadImage("explosion6");
    Image rKuva5 = LoadImage("explosion7");

    //game starts here
    public override void Begin()
    {
        ParhaatPisteet();
        TasoAnimaatio();
        Timer.SingleShot(1, delegate { MediaPlayer.Play("GolfMusic"); });
        MediaPlayer.IsRepeating = true;
        Window.Title = "Labyrinth Golf by Aki Sirviö";
    }

    //draw ball quidlines
    protected override void Paint(Canvas canvas)
    {
        if (pallot.Count > 0)
        {
            if (mailanKulma != pelaajat[vuoro].Angle)
            {
                Angle kulma = pelaajat[vuoro].Angle - Angle.FromDegrees(90);
                Vector keskipiste = pallot[vuoro].Position;
                Vector reunapiste = new Vector(kulma.Cos * 300, (kulma).Sin * 300);
                canvas.BrushColor = Color.DarkForestGreen;

                canvas.DrawLine(keskipiste, keskipiste + reunapiste);
            }
            mailanKulma = pelaajat[vuoro].Angle;
        }

        base.Paint(canvas);
    }

    //write text upper left coner
    void Kirjoita(string teksti)
    {
        MessageDisplay.Add(teksti);
    }

    //animation at begining of level
    void TasoAnimaatio()
    {
        ClearAll();
        MediaPlayer.Volume = 0.15;
        Tausta();
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, SeuraavaTaso, "Seuraavalle tasolle");
        for (int i = 0; i < kenttaNro; i++)
        {
            int suunta = 0;
            PhysicsObject kylttiPohja = new PhysicsObject(50, 50);
            kylttiPohja.Position = new Vector(Screen.Left, 0);
            kylttiPohja.CollisionIgnoreGroup = 2;
            kylttiPohja.Shape = Shape.Circle;
            kylttiPohja.IsVisible = false;
            kylttiPohja.Restitution = 1.0;
            AddCollisionHandler(kylttiPohja, OsuKylttiin);
            Add(kylttiPohja);

            Label kyltti = new Label();
            kyltti.TextColor = Color.White;
            kyltti.Color = Color.Transparent;
            kyltti.Text = "Labyrinth Golf level " + kenttaNro;
            kyltti.Font = Font.DefaultLargeBold;
            kyltti.TextScale = new Vector(1.7, 1.7);
            kylttiPohja.Add(kyltti);
            kylttiPohja.Hit(RandomGen.NextVector(50, 1000));

            if (i % 2 == 0)
            {
                suunta = -1;
            }
            else
            {
                suunta = 1;
            }

            Timer.SingleShot(0.1, delegate { TasoAnim(kylttiPohja, kyltti, suunta, 255); });
        }
    }

    //change text scale and transparet 
    void TasoAnim(PhysicsObject kylttiPohja, Label kyltti, int suunta, double alpha)
    {
        kyltti.Angle += Angle.FromDegrees(5 * suunta);
        kylttiPohja.Angle += Angle.FromDegrees(5 * suunta);
        kyltti.TextScale += new Vector(0.007, 0.007);
        kyltti.TextColor = new Color((int)alpha, (int)alpha, (int)alpha, (int)alpha);
        alpha -= 0.5;
        Timer.SingleShot(0.01, delegate { TasoAnim(kylttiPohja, kyltti, suunta, alpha); });

        if (alpha <= 0)
        {
            SeuraavaTaso();
        }
    }

    //borders and background
    void Tausta()
    {
        Level.Height = Screen.Height;
        Level.Width = Screen.Width;
        Level.Background.CreateGradient(Color.DarkGreen, Color.LightGreen);
        Level.CreateBorders(1.0, false);
    }

    //choose next level
    void SeuraavaTaso()
    {
        ClearAll();
        MediaPlayer.Volume = 0.3;

        if (kenttaNro == 1)
        {
            LuoKentta("Taso1");
        }
        else if (kenttaNro == 2)
        {
            LuoKentta("Taso2");
        }
        else if (kenttaNro == 3)
        {
            LuoKentta("Taso3");
        }
        else if (kenttaNro == 4)
        {
            LuoKentta("Taso4");
        }
        else if (kenttaNro == 5)
        {
            LuoKentta("Taso5");
        }
        else if (kenttaNro == 6)
        {
            LuoKentta("Taso6");
        }
        else if (kenttaNro == 7)
        {
            LuoKentta("Taso7");
        }
        else if (kenttaNro == 8)
        {
            LuoKentta("Taso8");
            kenttaNro++;
            PudotaPallo();
            Loppu();
        }
        else if (kenttaNro > 8)
        {
            Exit();
        }

        Ohjaimet();
        Tausta();
        NollaaAsetukset();
        Camera.Zoom(0.9);
    }

    //make level with picture
    void LuoKentta(string taso)
    {
        ColorTileMap ruudut = ColorTileMap.FromLevelAsset(taso);
        ruudut.SetTileMethod(Color.FromHexCode("4CFF00"), PelaajanPaikka);
        ruudut.SetTileMethod(Color.FromHexCode("808080"), PallonPaikka);
        ruudut.SetTileMethod(Color.FromHexCode("0026FF"), LisaaMaali);
        ruudut.SetTileMethod(Color.FromHexCode("FF1420"), LisaaKyltti1);
        ruudut.SetTileMethod(Color.FromHexCode("FF933A"), LisaaKyltti2);
        ruudut.SetTileMethod(Color.Black, LisaaReuna);
        ruudut.Execute(20, 20);
    }

    //position of first character
    void PelaajanPaikka(Vector paikka, double leveys, double korkeus)
    {
        pelaajanPaikka = paikka;
    }

    //position of all balls
    void PallonPaikka(Vector paikka, double leveys, double korkeus)
    {
        pallonPaikka = paikka;
    }

    //add hole and its ground
    void LisaaMaali(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject tausta = PhysicsObject.CreateStaticObject(70, 70);
        tausta.Position = paikka;
        tausta.Shape = Shape.Circle;
        tausta.Color = Color.Green;
        tausta.CollisionIgnoreGroup = 1;
        Add(tausta);

        PhysicsObject reika = PhysicsObject.CreateStaticObject(30, 30);
        reika.Position = paikka;
        reika.Shape = Shape.Circle;
        reika.Color = Color.Black;
        reika.Restitution = 0.0;
        Add(reika);
        AddCollisionHandler(reika, Maali);
    }

    //label of game name and level
    void LisaaKyltti1(Vector paikka, double leveys, double korkeus)
    {
        Label kyltti = new Label();
        kyltti.Position = paikka;
        kyltti.TextColor = Color.White;
        kyltti.Color = Color.Transparent;
        kyltti.Text = "Labyrinth Golf level " + kenttaNro;
        kyltti.TextScale = new Vector(2, 2);
        kyltti.Font = Font.DefaultLargeBold;
        Add(kyltti);
    }

    //title label "score / shots"
    void LisaaKyltti2(Vector paikka, double leveys, double korkeus)
    {
        Label kyltti = new Label();
        kyltti.Position = paikka + new Vector(-50, 0);
        kyltti.TextColor = Color.DarkGreen;
        kyltti.Color = Color.Transparent;
        kyltti.Text = "Score / Shots";
        kyltti.TextScale = new Vector(1, 1);
        kyltti.Font = Font.DefaultLargeBold;
        Add(kyltti);
    }

    //line edges
    void LisaaReuna(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject reuna = PhysicsObject.CreateStaticObject(leveys, korkeus);
        reuna.Position = paikka;
        reuna.Color = Color.Gray;
        reuna.Restitution = 0.2;
        reuna.Tag = "reuna";
        reuna.CollisionIgnoreGroup = 2;
        Add(reuna);

        Shape muoto = RandomGen.SelectOne(Shape.Rectangle, Shape.Circle, Shape.Diamond, Shape.Hexagon, Shape.Triangle);
        reuna.Shape = muoto;
    }

    //controls
    void Ohjaimet()
    {
        Keyboard.Listen(Key.Enter, ButtonState.Pressed, Pelaaja, "Lisätään pelaaja peliin");
        Keyboard.Listen(Key.Space, ButtonState.Pressed, Heilauta, "Isketään mailalla palloa", 300);
        Keyboard.Listen(Key.D, ButtonState.Down, Kaanna, "Kääntää mailaa", 1);
        Keyboard.Listen(Key.A, ButtonState.Down, Kaanna, "Kääntää mailaa", -1);
        Keyboard.Listen(Key.Right, ButtonState.Down, Kaanna, "Kääntää mailaa", 1);
        Keyboard.Listen(Key.Left, ButtonState.Down, Kaanna, "Kääntää mailaa", -1);
        Keyboard.Listen(Key.F1, ButtonState.Pressed, Alusta, "Siirtää pallon alkuun");

        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    //asking player's name
    void Pelaaja()
    {
        if(kenttaNro <= 1)
        {
            InputWindow nimi = new InputWindow("Name: ");
            nimi.MaxCharacters = 15;
            nimi.Image = LoadImage("golf3");
            nimi.OKButton.Image = LoadImage("go1");
            nimi.OKButton.Text = "";
            nimi.Layout.RightPadding = 50;
            nimi.Layout.TopPadding = 50;
            nimi.Layout.BottomPadding = 50;
            nimi.TextEntered += TallennaNimi;
            Add(nimi);
        }
    }

    //label of player name
    void TallennaNimi(InputWindow nimi)
    {
        if (nimi.InputBox.Text.Length > 0)
        {
            string pelaajanNimi = nimi.InputBox.Text;
            Label nimiKyltti = new Label();
            nimiKyltti.Position = pelaajanPaikka + new Vector(-330, -5);
            nimiKyltti.TextColor = Color.DarkGray;
            nimiKyltti.Color = Color.Transparent;
            nimiKyltti.Text = pelaajanNimi;
            nimiKyltti.Font = Font.DefaultLargeBold;
            Add(nimiKyltti);
            nimet.Add(nimiKyltti);

            LisaaPelaaja();
        }
    }

    //add golf club
    void LisaaPelaaja()
    {
        maila = new PhysicsObject(50, 50);
        maila.Position = pelaajanPaikka;
        maila.Image = LoadImage("Maila");
        maila.CanRotate = false;
        maila.CollisionIgnoreGroup = 1;
        maila.Restitution = 0.0;
        maila.IgnoresExplosions = true;
        Add(maila);
        pelaajat.Add(maila);

        Color vari = LisaaPallo();
        PelaajanVari(vari, pelaajanPaikka);
        LisaaTulokset(0, -1, pelaajanPaikka);
        pelaajanPaikka += new Vector(0, -70);
    }

    //Add golf ball
    Color LisaaPallo()
    {
        Color vari = Color.Black;
        while (vari.BlueComponent < 50 || vari.GreenComponent < 70 || vari.RedComponent < 50)
        {
            vari = RandomGen.NextColor();
        }
        pallo = new PhysicsObject(15, 15);
        pallo.Position = pallonPaikka;
        pallo.Shape = Shape.Circle;
        pallo.CollisionIgnoreGroup = 1;
        pallo.Color = vari;
        pallo.Restitution = 0.2;
        pallo.CanRotate = false;
        pallo.Tag = (pelaajat.Count - 1).ToString();
        Add(pallo);
        pallot.Add(pallo);

        return vari;
    }

    //sign next to palyer club that tels player about ball color
    void PelaajanVari(Color vari, Vector paikka)
    {
        GameObject pelaajanVari = new GameObject(15, 25);
        pelaajanVari.Position = paikka + new Vector(45, -20);
        pelaajanVari.Color = vari;
        pelaajanVari.Shape = Shape.Diamond;
        Add(pelaajanVari);
    }

    //add label of player scores and shots
    void LisaaTulokset(int alkuPisteet, int paikkaListalla, Vector paikka)
    {
        pisteet = new IntMeter(alkuPisteet);
        lyonnit = new IntMeter(0);
        lyonnitLista.Add(lyonnit);

        if (paikkaListalla >= 0)
        {
            pisteetLista.RemoveAt(paikkaListalla);
            pisteetLista.Insert(paikkaListalla, pisteet);
        }
        else
        {
            pisteetLista.Add(pisteet);
        }

        Label pisteNaytto = new Label();
        pisteNaytto.Position = paikka + new Vector(-110, -5);

        pisteNaytto.BindTo(pisteet);
        Add(pisteNaytto);

        Label lyonnitNaytto = new Label();
        lyonnitNaytto.Position = paikka + new Vector(-35, -5);
        lyonnitNaytto.BindTo(lyonnit);
        Add(lyonnitNaytto);
    }

    //hit the ball
    void Heilauta(int voima)
    {
        if (pelaajat.Count > 0)
        {
            Angle suunta = pelaajat[vuoro].Angle + Angle.FromDegrees(-90);
            pallot[vuoro].Hit(suunta.GetVector() * voima);
            lyonnitLista[vuoro].Value++;
            LoadSoundEffect("golfball").Play();

            SeuraavaVuoro();
        }
    }

    //next palyer's turn
    void SeuraavaVuoro()
    {
        do
        {
            vuoro++;
            if (vuoro > pelaajat.Count - 1)
            {
                vuoro = 0;
            }
        } while (maalissa.Contains(vuoro) && maalissa.Count != pelaajat.Count);
    }

    //rotate club
    void Kaanna(int suunta)
    {
        if (pelaajat.Count > 0)
        {
            pelaajat[vuoro].Angle += Angle.FromDegrees(0.5 * suunta);
        }
    }

    //move ball to starting point
    void Alusta()
    {
        pallot[vuoro].Position = pallonPaikka;
        pallot[vuoro].Stop();
    }

    //animation sound effects
    void OsuKylttiin(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        golfball.Play();
        swing.Play();
    }

    //ball hits the hole
    void Maali(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        string tag = kohde.Tag.ToString();
        int pelaaja = Int32.Parse(tag);
        monesOsuma++;
        kohde.IsVisible = false;
        kohde.Position = pelaajat[pelaaja].Position;
        LaskePisteet(pelaaja);
        maalissa.Add(pelaaja);
        LoadSoundEffect("golfball2").Play();

        if (vuoro == pelaaja)
        {
            SeuraavaVuoro();
        }

        if (monesOsuma == 1)
        {
            voittaja = pelaaja;
        }

        if (maalissa.Count == pelaajat.Count)
        {
            Image rajahdysKuva = RandomGen.SelectOne(rKuva1, rKuva2, rKuva3, rKuva4, rKuva5);
            Voittaja(2, rajahdysKuva);
            int luku = kenttaNro;
            Timer.SingleShot(7, delegate { NaytaPisteet(luku); });
            kenttaNro++;
            foreach(GameObject obj in GetObjects(x => x.Tag.ToString()=="reuna"))
            {
                obj.Destroy();
            }
        }
    }

    //count score
    void LaskePisteet(int pelaaja)
    {
        if (lyonnitLista.Count > 0)
        {
            int pisteet = 100 / monesOsuma - lyonnitLista[pelaaja].Value;
            if (pisteet < 0)
            {
                pisteet = 0;
            }
            pisteetLista[pelaaja].Value += pisteet;
        }
    }

    //reset settings for next level
    void NollaaAsetukset()
    {
        monesOsuma = 0;
        vuoro = 0;
        maalissa.Clear();
        lyonnitLista.Clear();
        if (nimet.Count > 0)
        {
            double kerroin = 35 / nimet[voittaja].Height;
            nimet[voittaja].TextScale *= kerroin;
        }

        if (pelaajat.Count > 0)
        {
            for (int i = 0; i < pelaajat.Count; i++)
            {
                pelaajat[i].Angle = Angle.FromDegrees(0);
                pelaajat[i].Size = new Vector(50, 50);
                Add(pelaajat[i]);
                pelaajanPaikka = pelaajat[i].Position;
                LisaaTulokset(pisteetLista[i].Value, i, pelaajanPaikka);
                PelaajanVari(pallot[i].Color, pelaajanPaikka);
            }
            foreach (PhysicsObject pallo in pallot)
            {
                pallo.Position = pallonPaikka;
                pallo.Stop();
                pallo.IsVisible = true;
                Add(pallo);
            }
            foreach (Label nimi in nimet)
            {
                Add(nimi);
            }
        }
    }

    //level winner animation
    void Voittaja(int luku, Image rajahdysKuva)
    {
        luku++;
        if ((luku % 2) != 0)
        {
            pelaajat[voittaja].Size *= 1.35;
            nimet[voittaja].TextScale *= 1.35;
        }
        else if ((luku % 2) == 0)
        {
            pelaajat[voittaja].Size *= 0.75;
            nimet[voittaja].TextScale *= 0.75;
        }

        if (luku < 20)
        {
            Rajahdys(rajahdysKuva);
        }

        Timer.SingleShot(0.2, delegate { Voittaja(luku, rajahdysKuva); });
    }

    //huge explosion before next level
    void Rajahdys(Image rajahdysKuva)
    {
        Explosion rajahdys = new Explosion(1000);
        rajahdys.Position = new Vector(0, 0);
        rajahdys.Sound = LoadSoundEffect("golf_swing");
        rajahdys.Image = rajahdysKuva;
        Add(rajahdys);
        int pMaxMaara = 200;
        ExplosionSystem rajahdys2 = new ExplosionSystem(LoadImage("Maila"), pMaxMaara);
        Add(rajahdys2);
        double x = 0;
        double y = 0;
        int pMaara = 50;
        rajahdys2.AddEffect(x, y, pMaara);
    }

    //add top ten score lists
    void ParhaatPisteet()
    {
        for(int i=1; i<=7; i++)
        {
            string tiedosto = "pisteet" + i + ".xml";
            ScoreList topLista = new ScoreList(10, true, 35);
            topLista = DataStorage.TryLoad<ScoreList>(topLista, tiedosto);
            parhaatPisteet.Add(topLista);
        }
    }

    //show top ten score windows
    void NaytaPisteet(int luku)
    {
        HighScoreWindow topIkkuna = new HighScoreWindow(
                              "",
                              "",
                              parhaatPisteet[luku-1], lyonnit);
        topIkkuna.Closed += delegate { TallennaPisteet(luku); };
        topIkkuna.Image = LoadImage("top10");
        topIkkuna.Layout.RightPadding = 50;
        topIkkuna.Layout.LeftPadding = 50;
        topIkkuna.Height = 500;
        topIkkuna.Width = 350;
        topIkkuna.SizingByLayout = false;
        topIkkuna.OKButton.IsVisible = false;
        topIkkuna.NameInputWindow.Image = LoadImage("top10nimi");
        topIkkuna.NameInputWindow.Layout.RightPadding = 70;
        topIkkuna.NameInputWindow.Layout.LeftPadding = 50;
        topIkkuna.NameInputWindow.Layout.BottomPadding = 70;
        topIkkuna.NameInputWindow.Layout.TopPadding = 70;
        topIkkuna.NameInputWindow.OKButton.Image = LoadImage("go0");
        topIkkuna.NameInputWindow.OKButton.Text = "";
        Add(topIkkuna);
    }

    //save score lists
    void TallennaPisteet(int luku)
    {
        string tiedosto = "pisteet" + luku + ".xml";
        DataStorage.Save<ScoreList>(parhaatPisteet[luku-1], tiedosto);
        if(kenttaNro<=7)
        {
            TasoAnimaatio();
        }
        else
        {
            SeuraavaTaso();
        }
    }

    //drop balls
    void PudotaPallo()
    {
        double leveys = RandomGen.NextDouble(10, 100);
        double korkeus = leveys;
        double x = RandomGen.NextDouble(-300, 300);
        golfball.Play();
        Gravity = new Vector(0, -1000);
        PhysicsObject pallo = new PhysicsObject(leveys, korkeus);
        pallo.Image = LoadImage("golfball5");
        pallo.Shape = Shape.Circle;
        pallo.X = x;
        pallo.Y = Screen.Top - 50;
        Add(pallo);
        double sec = RandomGen.NextDouble(0.1, 0.5);
        Timer.SingleShot(sec, PudotaPallo);
    }

    //end of game button
    void Loppu()
    {
        GameObject loppu = new GameObject(200, 100);
        loppu.Image = LoadImage("loppu");
        Add(loppu);
        Mouse.ListenOn(loppu, MouseButton.Left, ButtonState.Pressed, Exit, "peli loppuu");
        Mouse.IsCursorVisible = true;
    }
}
