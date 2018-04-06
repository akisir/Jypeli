//CarRace.cs for hobby game projets
//3.4.2018 by Aki Sirviö
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

//car race, move car with arrow keys
public class CarRace : PhysicsGame
{
    Vector[] reitti;
    Automobile auto;
    List<Automobile> autot;
    List<Vector> koordinaatit;
    List<int> sijainti;
    RoadMap tie;
    IntMeter kierrosLaskuri;
    IntMeter moneskoLaskuri;
    Label kierrosNaytto;
    Label turboMittari;
    int leveys = 250;
    int pelaajaSijainti = 0;
    int kilpailijat = 0;
    int kierroksia = 0;
    int nopeus = 700;
    bool apuviiva = false;
    bool turbo = true;

    SoundEffect jarrutus = LoadSoundEffect("car-breaking");
    SoundEffect kaynnistys = LoadSoundEffect("StartCar");
    SoundEffect kaynnistys2 = LoadSoundEffect("car+start3");
    SoundEffect torvi = LoadSoundEffect("OOGAhorn");
    SoundEffect kaynti = LoadSoundEffect("HOT+ROD1");
    SoundEffect kaynti2 = LoadSoundEffect("car+geardown");
    SoundEffect lahto = LoadSoundEffect("CAR+Peels+Out");
    SoundEffect lahto2 = LoadSoundEffect("gt40takingOff");
    SoundEffect turboAani = LoadSoundEffect("car+running3");

    //program starts here
    public override void Begin()
    {
        kierroksia = RandomGen.NextInt(2, 5);
        LuoRata();
        LisaaPelaaja();
        TeeAutot();
        TeeKaiteet();
        TeeMaali();
        LuoKierrosLaskuri();
        Taustakuva();
        AlkuAnimaatio();

        Camera.Follow(auto);
        Window.Title = "Car race 1";
        Mouse.IsCursorVisible = true;
    }

    //enable controls and car's AI
    void AloitaKisa()
    {
        Nappaimet();
        SiirraAuto();
        foreach(Automobile a in autot)
        {
            a.Brain.Active = true;
        }

        MediaPlayer.Play("raceBack");
        MediaPlayer.IsRepeating = true;
    }

    //show random background
    void Taustakuva()
    {
        string bImage = RandomGen.SelectOne("pexels-photo", "landscapePanorama", "landscapePanorama2", "landscapePanorama3", "landscapePanorama4");
        Level.Background.Image = LoadImage(bImage);
        Level.Background.Height = 1500;
        Level.Background.Width = 4000;
        Level.Background.Size *= 3;
        Level.Background.Position = new Vector(3000, 5500);
    }

    //controls
    void Nappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Up, ButtonState.Down, Kiihdyta, "Kiihdytä", auto);
        Keyboard.Listen(Key.Down, ButtonState.Down, Jarruta, "Jarruta", auto);
        Keyboard.Listen(Key.Left, ButtonState.Down, Kaanny, "Vasemmalle", auto, 1);
        Keyboard.Listen(Key.Right, ButtonState.Down, Kaanny, "Oikealle", auto, -1);
        Keyboard.Listen(Key.Space, ButtonState.Down, Turbo, "Extra nopeus", auto);

        ShowControlHelp();
    }

    //add character
    void LisaaPelaaja()
    {
        auto = new Automobile(40, 30);
        auto.Mass = 20;
        auto.Tag = "auto";
        auto.X = 150;
        auto.Maneuverability = Angle.FromDegrees(200);
        auto.Acceleration = 450;
        auto.BrakeDeceleration = 600;
        auto.TopSpeed = 3000;
        auto.AngularDamping = 0.9;
        auto.LinearDamping = 0.97;
        Add(auto);
    }

    //add cars
    void TeeAutot()
    {
        kilpailijat = RandomGen.NextInt(5, 16);
        List<int> lNopeus = GeneroiNopeudet();
        double X = 250;
        double Y = 100;

        autot = new List<Automobile>();
        koordinaatit = new List<Vector>();
        sijainti = new List<int>();
        for (int i=0; i<kilpailijat; i++)
        {
            if(Y==-50)
            {
                Y = 100;
                X += 100;
            }
            Y -= 50;

            Automobile vAuto = new Automobile(40, 30);
            vAuto.X = X;
            vAuto.Y = Y;
            vAuto.Mass = 20;
            vAuto.Tag = "vAuto"+i;
            vAuto.Maneuverability = Angle.FromDegrees(200);
            vAuto.Acceleration = 450;
            vAuto.BrakeDeceleration = 600;
            vAuto.TopSpeed = 3000;
            vAuto.AngularDamping = 0.9;
            vAuto.LinearDamping = 0.97;
            Add(vAuto);
            autot.Add(vAuto);
            koordinaatit.Add(vAuto.Position);
            sijainti.Add(0);

            TeeAivot(vAuto, lNopeus[i]);
        }
    }

    //generate random speeds
    List<int> GeneroiNopeudet()
    {
        List<int> lNopeus = new List<int>();
        for (int i = 0; i < kilpailijat; i++)
        {
            int aNopeus = RandomGen.NextInt(150, 450);
            lNopeus.Add(aNopeus);
        }
        lNopeus.Sort();

        return lNopeus;
    }

    //cars artificial intelligence
    void TeeAivot(Automobile vAuto, int nopeus)
    {
        List<Vector> polku = new List<Vector>();
        double linja = RandomGen.NextDouble(0.95, 1.05);
        for (int j = 0; j < reitti.Length; j++)
        {
            double reittiX = (reitti[j].X * linja);
            double reittiY = (reitti[j].Y * linja);
            polku.Add(new Vector(reittiX, reittiY));
        }

        PathFollowerBrain autonAivot = new PathFollowerBrain();
        autonAivot.Path = polku;
        autonAivot.Loop = true;
        autonAivot.Speed = nopeus;
        autonAivot.TurnWhileMoving = true;
        autonAivot.Active = false;
        vAuto.Brain = autonAivot;
    }

    //make track
    void LuoRata()
    {
        reitti = new Vector[]
        {
            new Vector(700, 0),
            new Vector(1500, 0),
            new Vector(1750, 350),
            new Vector(1500, 700),
            new Vector(1300, 1500),
            new Vector(1000, 1700),
            new Vector(500, 1900),
            new Vector(100, 1700),
            new Vector(-500, 1000),
            new Vector(0, 0),
            new Vector(700, 0)
        };

        tie = new RoadMap(reitti);
        tie.DefaultWidth = leveys;
        tie.Insert();
    }

    //make handrails
    void TeeKaiteet()
    {
        Vector[] sKaide = new Vector[reitti.Length];
        for (int i = 0; i < reitti.Length; i++)
        {
            double X = (reitti[i].X * 0.8) + leveys/2;
            double Y = (reitti[i].Y * 0.8) + leveys/1.3;
            sKaide[i] = new Vector(X, Y);
        }

        RoadMap kaide1 = new RoadMap(sKaide);
        kaide1.DefaultWidth = 10;
        kaide1.CreateSegmentFunction = KaiteenOsa;
        kaide1.Insert();

        Vector[] uKaide = new Vector[reitti.Length];
        for (int i = 0; i < reitti.Length; i++)
        {
            double X = (reitti[i].X * 1.2) - leveys / 2;
            double Y = (reitti[i].Y * 1.2) - leveys / 1.3;
            uKaide[i] = new Vector(X, Y);
        }

        RoadMap kaide2 = new RoadMap(uKaide);
        kaide2.DefaultWidth = 10;
        kaide2.CreateSegmentFunction = KaiteenOsa;
        kaide2.Insert();
    }

    //part of handrail
    PhysicsObject KaiteenOsa(double width, double height, Shape shape)
    {
        PhysicsObject kaide = PhysicsObject.CreateStaticObject(width, height, shape);
        kaide.Color = Color.Green;
        Add(kaide);
        return kaide;
    }

    //make finish line
    void TeeMaali()
    {
        PhysicsObject maali = PhysicsObject.CreateStaticObject(10, leveys-35);
        maali.X = 100;
        maali.Y = 0;
        maali.Color = Color.White;
        maali.IgnoresCollisionResponse = true;
        maali.Tag = "maali";
        AddCollisionHandler<PhysicsObject, Automobile>(maali, MaaliinOsui);
        Add(maali);

        PhysicsObject apuviiva = PhysicsObject.CreateStaticObject(10, leveys+125);
        apuviiva.X = 1300;
        apuviiva.Y = 0;
        apuviiva.Color = Color.DarkGray;
        apuviiva.IgnoresCollisionResponse = true;
        apuviiva.Tag = "apuviiva";
        AddCollisionHandler<PhysicsObject, Automobile>(apuviiva, ApuviivaOsui);
        Add(apuviiva);

        PhysicsObject apuviiva2 = PhysicsObject.CreateStaticObject(10, leveys + 150);
        apuviiva2.X = 1000;
        apuviiva2.Y = 1700;
        apuviiva2.Color = Color.DarkGray;
        apuviiva2.IgnoresCollisionResponse = true;
        apuviiva2.IsVisible = false;
        apuviiva2.Tag = "apuviiva2";
        AddCollisionHandler<PhysicsObject, Automobile>(apuviiva2, ApuviivaOsui);
        Add(apuviiva2);
    }

    //make lap and other counters
    void LuoKierrosLaskuri()
    {
        kierrosLaskuri = new IntMeter(1);
        kierrosNaytto = new Label();
        kierrosNaytto.X = Screen.Left + 600;
        kierrosNaytto.Y = Screen.Top - 100;
        kierrosNaytto.TextScale *= 2;
        kierrosNaytto.TextColor = Color.DarkGreen;
        kierrosNaytto.Color = Color.Transparent;
        kierrosNaytto.Title = "Lap " + kierroksia+" /";
        kierrosNaytto.BindTo(kierrosLaskuri);
        Add(kierrosNaytto);

        moneskoLaskuri = new IntMeter(kilpailijat+1);
        Label moneskoNaytto = new Label();
        moneskoNaytto.X = Screen.Left + 1000;
        moneskoNaytto.Y = Screen.Top - 100;
        moneskoNaytto.TextScale *= 2;
        moneskoNaytto.TextColor = Color.OrangeRed;
        moneskoNaytto.Color = Color.Transparent;
        moneskoNaytto.Title = "Ranking "+(kilpailijat+1)+" /";
        moneskoNaytto.BindTo(moneskoLaskuri);
        Add(moneskoNaytto);

        turboMittari = new Label();
        turboMittari.X = Screen.Left + 1390;
        turboMittari.Y = Screen.Top - 100;
        turboMittari.TextScale *= 2;
        turboMittari.TextColor = Color.Yellow;
        turboMittari.Color = Color.Transparent;
        turboMittari.Text = "USABLE";
        Add(turboMittari);

        GameObject turboKuva = new GameObject(50, 50);
        turboKuva.Image = LoadImage("flame");
        turboKuva.X -= 115;
        turboKuva.Y += 10;
        turboMittari.Add(turboKuva);
    }

    //move car
    void SiirraAuto()
    {
        Timer autoJumissa = new Timer();
        autoJumissa.Interval = 0.2;
        autoJumissa.Timeout += TarkistaPaikka;
        autoJumissa.Start();
    }

    //chark car location
    void TarkistaPaikka()
    {
        for(int i=0; i<autot.Count; i++)
        {
            if (Math.Round(autot[i].X) == Math.Round(koordinaatit[i].X) && Math.Round(autot[i].Y) == Math.Round(koordinaatit[i].Y))
                {
                autot[i].X += 40;
                autot[i].Y -= 40;
            }
            else
            {
                koordinaatit[i] = autot[i].Position;
            }
        }
    }

    //accelerate
    void Kiihdyta(Automobile kaara)
    {
        kaara.Accelerate();
        foreach (var segment in tie.Segments)
        {
            if (segment.IsInside(auto.Position))
            {
                auto.Acceleration = nopeus;
                return;
            }
        }
        auto.Acceleration = 100;
    }

    //break
    void Jarruta(Automobile kaara)
    {
        kaara.Brake();
    }

    //turn
    void Kaanny(Automobile kaara, int suunta)
    {
        kaara.Turn(kaara.Maneuverability * suunta, Time.SinceLastUpdate.TotalSeconds);
    }

    //turbo speed
    void Turbo(Automobile kaara)
    {
        if(turbo)
        {
            nopeus = 1100;
            Timer.SingleShot(3, NormaaliNopeus);
            turboMittari.TextColor = Color.Orange;
            turboMittari.Text = "ON       ";
            turboAani.Play();
        }
    }

    //Normal speed
    void NormaaliNopeus()
    {
        turbo = false;
        nopeus = 700;
        turboMittari.Text = "OFF      ";
        turboMittari.TextColor = Color.DarkGray;
        turboAani.Stop();
    }

    //caome to finish
    void MaaliinOsui(PhysicsObject tormaaja, Automobile kohde)
    {
        if (kohde.Tag.ToString() == "auto")
        {
            if(apuviiva)
            {
                kierrosLaskuri.Value++;
                apuviiva = false;
                turbo = true;
                turboMittari.Text = "USABLE";
                turboMittari.TextColor = Color.Yellow;
                Taustakuva();
                LaskeSijainti();

                if (kierroksia == kierrosLaskuri.Value)
                {
                    kierrosNaytto.Text += " Final lap";
                }
                else if (kierroksia == kierrosLaskuri.Value-1)
                {
                    kierrosLaskuri.Value--;
                    kierrosNaytto.Text += " Finish";
                    KisanLoppu();
                }
            }
        }
        else
        {
            LisaaSijainti(kohde);
        }
    }

    //hit guide line
    void ApuviivaOsui(PhysicsObject tormaaja, Automobile kohde)
    {
        if(tormaaja.Tag.ToString() == "apuviiva")
        {
            if(kohde.Tag.ToString() == "auto")
            {
                LaskeSijainti();
            }
            else
            {
                LisaaSijainti(kohde);
            }
        }
        else if(tormaaja.Tag.ToString() == "apuviiva2")
        {
            if (kohde.Tag.ToString() == "auto")
            {
                apuviiva = true;
                LaskeSijainti();
            }
            else
            {
                LisaaSijainti(kohde);
            }
        }
    }

    //count ranking
    void LaskeSijainti()
    {
        pelaajaSijainti++;
        moneskoLaskuri.Value = 1;
        for (int i = 0; i < autot.Count; i++)
        {
            if (pelaajaSijainti <= sijainti[i])
            {
                moneskoLaskuri.Value++;
            }
        }
    }

    //add location count
    void LisaaSijainti(Automobile kohde)
    {
        for (int i = 0; i < autot.Count; i++)
        {
            if (autot[i].Tag.ToString() == kohde.Tag.ToString())
            {
                sijainti[i]++;
                return;
            }
        }
    }

    //start animation
    void AlkuAnimaatio()
    {
        Vector paikka1 = new Vector(0, 350);
        Vector paikka2 = new Vector(0, 0);
        Animaatio("Car Race", paikka1, 0.003, 255, 0.2);
        Animaatio("3", paikka2, 0.5, 200, 1.7);
        int luku = 2;
        Timer alkuAnim = new Timer();
        alkuAnim.Interval = 2;
        alkuAnim.TimesLimited = true;
        alkuAnim.Timeout += delegate { Animaatio(luku--.ToString(), paikka2, 0.5, 200, 1.7); };
        alkuAnim.Start(3);
        Timer.SingleShot(6, AloitaKisa);
        Timer.SingleShot(2, delegate { kaynnistys.Play(); kaynnistys2.Play(); });
        Timer.SingleShot(4, delegate { kaynti.Play(); kaynti2.Play(); });
        Timer.SingleShot(6, delegate { lahto.Play(); lahto2.Play(); });

    }

    //end of race
    void KisanLoppu()
    {
        Vector paikka = new Vector(0, 0);
        Animaatio("End of race", paikka, 0.01, 255, 0.5);
        Keyboard.Disable(Key.Down);
        Keyboard.Disable(Key.Left);
        Keyboard.Disable(Key.Right);
        Keyboard.Disable(Key.Up);
        //jarrutus.Play();
        //torvi.Play();
        jarrutus.CreateSound().Play();
        torvi.CreateSound().Play();
    }

    //show text animation
    void Animaatio(string teksti, Vector paikka, double koko, double alpha, double alphaMuutos)
    {
        Label kyltti = new Label();
        kyltti.TextColor = Color.DarkGray;
        kyltti.Color = Color.Transparent;
        kyltti.Text = teksti;
        kyltti.Font = Font.DefaultLargeBold;
        kyltti.TextScale *= 3;
        kyltti.Position = paikka;
        Add(kyltti);

        Timer.SingleShot(0.1, delegate { TekstiAnimaatio(kyltti, koko, alpha, alphaMuutos); });
    }

    //change text size and color transparent
    void TekstiAnimaatio(Label kyltti, double koko, double alpha, double alphaMuutos)
    {
        kyltti.TextScale += new Vector(koko, koko);
        kyltti.TextColor = new Color((int)70, (int)70, (int)alpha, (int)alpha);
        alpha -= alphaMuutos;

        if (alpha <= 0)
        {
            kyltti.Destroy();
        }
        else
        {
            Timer.SingleShot(0.01, delegate { TekstiAnimaatio(kyltti, koko, alpha, alphaMuutos); });
        }
    }
}

