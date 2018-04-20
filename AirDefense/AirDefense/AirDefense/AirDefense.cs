//AirDefense.cs for hobby game projects
//17.4.2018 by Aki Sirviö
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

//shoot planes with cannon
public class AirDefense : PhysicsGame
{
    List<GameObject> taustat;
    List<PhysicsObject> koneet;
    GameObject tahtain;
    PhysicsObject tykki;
    PhysicsObject lentokone;
    Flame liekki;
    Smoke savu;
    Image[] koneKuvat = LoadImages("plane", "plane2", "plane3", "plane4", "plane6");
    Image[] liekkiKuvat = LoadImages("flame", "flame2", "flame", "flame4");
    DoubleMeter osumat;
    int kenttaNro = 1;
    int nopeus = 0;
    double skaalaus = 0;
    bool turbo = false;

    //program starts here
    public override void Begin()
    {
        taustat = new List<GameObject>();
        koneet = new List<PhysicsObject>();
        osumat = new DoubleMeter(0.0);
        Window.Title = "Air Defense ©";
        SeuraavaKentta();
    }

    //load next level
    void SeuraavaKentta()
    {
        ClearAll();
        if(IsPaused)
        {
            taustat.Clear();
            osumat.Value = 0;
            Pause();
        }
        if(kenttaNro == 1)
        {
            nopeus = 10;
            skaalaus = 1.01;
            Timer.SingleShot(3, delegate { Otsikko("Air Defense ©", 2.5, 7); });
            LataaTaustakuvat("AirPanorama3", 4);
            Ohjaimet();
            ShowControlHelp();
        }
        else if(kenttaNro == 2)
        {
            nopeus = 20;
            skaalaus = 1.02;
            LataaTaustakuvat("AirPanorama8", 5);
        }
        else if(kenttaNro == 3)
        {
            nopeus = 30;
            skaalaus = 1.03;
            LataaTaustakuvat("AirPanorama1", 3);
        }
        else if(kenttaNro == 4)
        {
            nopeus = 10;
            skaalaus = 1.02;
            LataaTaustakuvat("AirPanorama11", 10);
        }
        else if (kenttaNro == 5)
        {
            nopeus = 40;
            skaalaus = 1.04;
            LataaTaustakuvat("AirPanorama10", 3);
        }
        else if (kenttaNro == 6)
        {
            nopeus = 5;
            skaalaus = 1.012;
            LataaTaustakuvat("AirPanorama12", 15);
        }
        else if (kenttaNro == 7)
        {
            nopeus = 45;
            skaalaus = 1.035;
            LataaTaustakuvat("AirPanorama7", 5);
        }
        else if (kenttaNro == 8)
        {
            nopeus = 75;
            skaalaus = 1.07;
            LataaTaustakuvat("AirPanorama5", 3);
        }
        else if (kenttaNro == 9)
        {
            nopeus = 100;
            skaalaus = 1.05;
            LataaTaustakuvat("AirPanorama2", 3);
        }

        Otsikko("Level " + kenttaNro, 0.2, 10);
        TeeTahtain();
        TeeTykki();
        Ohjaimet();
        TeeLaskuri();
        Ajastimet(1);
    }

    //make timers for planes
    void Ajastimet(int aika)
    {
        Timer lisaaKone = new Timer();
        lisaaKone.Interval = aika;
        lisaaKone.Timeout += TeeLentokone;
        lisaaKone.Start();

        Timer koneAjastin = new Timer();
        koneAjastin.Interval = 0.1;
        koneAjastin.Timeout += MuutaKokoa;
        koneAjastin.Start();
    }

    //add title
    void Otsikko(string otsikko, double aika, int koko)
    {
        Label teksti = new Label();
        teksti.Text = otsikko;
        teksti.Font = Font.DefaultLargeBold;
        teksti.Y = Screen.Top - 200;
        teksti.TextScale *= koko;
        teksti.Color = Color.Transparent;
        teksti.TextColor = Color.Ruby;
        Add(teksti);
        Timer.SingleShot(aika, delegate { TekstiAnimaatio(teksti, 255); });
    }

    //text animation
    void TekstiAnimaatio(Label teksti, int alpha)
    {
        teksti.TextColor = new Color(43, 39, 67, alpha);
        alpha -= 3;
        if(alpha>0)
        {
            Timer.SingleShot(0.01, delegate { TekstiAnimaatio(teksti, alpha); });
        }
        else
        {
            teksti.Destroy();
        }
    }

    //add background images
    void LataaTaustakuvat(string nimi, int luku)
    {
        for(int i=0; i< luku; i++)
        {
            GameObject tausta = new GameObject(1500, 1500);
            tausta.Image = LoadImage(nimi + (i + 1));
            Add(tausta);

            if (i == 0)
            {
                tausta.Left = Screen.Left - tausta.Width;
            }
            else
            {
                tausta.Left = taustat[i-1].Right;
            }
            taustat.Add(tausta);
        }
    }

    //add sight
    void TeeTahtain()
    {
        tahtain = new GameObject(100, 100);
        tahtain.Image = LoadImage("tahtain");
        Add(tahtain,1);
    }

    //add cannon
    void TeeTykki()
    {
        tykki = new PhysicsObject(100, 1400);
        tykki.Image = LoadImage("tykki");
        tykki.Y = Screen.Bottom-400;
        AddCollisionHandler(tykki, "kone", OsuiTykkiin);
        Add(tykki,2);
    }

    //add plane
    void TeeLentokone()
    {
        double y = RandomGen.NextDouble(0, Screen.Top);
        if(y>Screen.Top-100)
        {
            double x = RandomGen.NextDouble(taustat[0].Left, taustat[taustat.Count - 1].Right);
            lentokone = new PhysicsObject(30, 10);
            lentokone.Image = RandomGen.SelectOne(koneKuvat[0], koneKuvat[1], koneKuvat[2], koneKuvat[3], koneKuvat[4]);
            lentokone.Tag = "kone";
            lentokone.Y = y;
            lentokone.X = x;
            Add(lentokone);
            koneet.Add(lentokone);
            Vector paikka = new Vector(tykki.X, tykki.Y + 700);
            lentokone.MoveTo(paikka, nopeus, delegate { OsuiTykkiin(tykki,lentokone); });
        }
    }

    //make plane hit counter
    void TeeLaskuri()
    {
        Label laskuri = new Label();
        laskuri.X = Screen.Right - 250;
        laskuri.Y = Screen.Top - 100;
        laskuri.Color = Color.Transparent;
        laskuri.TextColor = Color.SpringGreen;
        laskuri.TextScale *= 2;
        laskuri.BindTo(osumat);
        Add(laskuri);

        Label laskuriKuva = new Label();
        laskuriKuva.X = Screen.Right - 100;
        laskuriKuva.Y = Screen.Top - 100;
        laskuriKuva.Image = LoadImage("planeCrash");
        laskuriKuva.Size *= 1.5;
        Add(laskuriKuva);
    }

    //move sight
    void LiikutaTahtainta(AnalogState liike)
    {
        tahtain.Position = Mouse.PositionOnScreen;
        Vector suunta = (Mouse.PositionOnWorld - tykki.AbsolutePosition).Normalize();
        double tykkiY = Mouse.PositionOnScreen.Y - Screen.Height/1.2;
        if (suunta.Angle.GetPositiveDegrees()<135 && suunta.Angle.GetPositiveDegrees() > 45)
        {
            tykki.Angle = suunta.Angle + Angle.FromDegrees(-90);
        }
        if(tykkiY>(Screen.Bottom-400) && tykkiY<Screen.Bottom-250)
        {

            tykki.Y = tykkiY;
        }
        if(liekki!=null)
        {
            liekki.Position = tykki.Position + (suunta * 700);
            savu.Position = tykki.Position + (suunta * 700);
        }
    }

    //change planes y coordinate
    void MuutaKokoa()
    {
        foreach(PhysicsObject kone in koneet)
        {
            kone.Size *= skaalaus;
        }
    }

    //add controls
    void Ohjaimet()
    {
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Exit");
        Keyboard.Listen(Key.A, ButtonState.Down, KaannaTornia, "Turn cannon to left", 1);
        Keyboard.Listen(Key.D, ButtonState.Down, KaannaTornia, "Turn cannon to right", -1);
        Keyboard.Listen(Key.W, ButtonState.Pressed, Turbo, "Turbo on", true);
        Keyboard.Listen(Key.W, ButtonState.Released, Turbo, "Turbo off", false);
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Help");
        Mouse.ListenMovement(0.1, LiikutaTahtainta, "Aim");
        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Ammu, "Shoot");
    }

    //turn turret
    void KaannaTornia(int suunta)
    {
        if (taustat[0].Left+30 < Screen.Left && suunta==1 || taustat[taustat.Count-1].Right-30>Screen.Right && suunta==-1)
        {
            foreach (GameObject tausta in taustat)
            {
                if (turbo)
                {
                    tausta.X += 30 * suunta;
                }
                else
                {
                    tausta.X += 10 * suunta;
                }
            }
            foreach(PhysicsObject lk in koneet)
            {
                if (turbo)
                {
                    lk.X += 30 * suunta;
                }
                else
                {
                    lk.X += 10 * suunta;
                }
            }
        }
    }

    //put turbo on or off
    void Turbo(bool virta)
    {
        if(virta)
        {
            turbo = true;
        }
        else
        {
            turbo = false;
        }
    }

    //shoot
    void Ammu()
    {
        Vector suunta = (Mouse.PositionOnWorld - tykki.AbsolutePosition).Normalize();
        if (suunta.Angle.GetPositiveDegrees() < 135 && suunta.Angle.GetPositiveDegrees() > 45)
        {
            Vector paikka = tykki.Position + (suunta * 600);
            GameObject ammus = new GameObject(25, 100);
            ammus.Image = LoadImage("ammus4");
            ammus.Position = tykki.Position;
            ammus.Position = paikka;
            ammus.Angle = tykki.Angle;
            Add(ammus, 1);
            ammus.MoveTo(tahtain.Position, 15000, delegate { rajahdys(ammus); });

            Image liekkiKuva = RandomGen.SelectOne(liekkiKuvat[0],liekkiKuvat[1], liekkiKuvat[2], liekkiKuvat[3]);
            liekki = new Flame(liekkiKuva);
            liekki.Position = paikka;
            liekki.Angle = tykki.Angle+Angle.FromDegrees(90);
            liekki.MaximumLifetime = TimeSpan.FromSeconds(0.1);
            liekki.MaxLifetime = 0.1;
            liekki.MaxVelocity = 1000;
            liekki.MinVelocity = 500;
            Add(liekki);
            savu = new Smoke();
            savu.Position = paikka+(suunta*100);
            savu.Angle = tykki.Angle + Angle.FromDegrees(90);
            savu.MaximumLifetime = TimeSpan.FromSeconds(0.3);
            savu.MaxLifetime = 0.5;
            savu.MaxVelocity = 1000;
            savu.MinVelocity = 50;
            Add(savu);
        }
    }

    //explosion destroys cannon
    void rajahdys(GameObject ammus)
    {
        Vector paikka = ammus.Position;
        ammus.Destroy();
        Explosion rajahdys = new Explosion(10.0);
        rajahdys.Position = paikka;
        rajahdys.Image = LoadImage("explosion2");
        rajahdys.AddShockwaveHandler("kone", Osu);
        Add(rajahdys);
    }

    //ammo hit the plane
    void Osu(IPhysicsObject olio, Vector shokki)
    {
        olio.Destroy();
        Explosion rajahdys = new Explosion(100);
        rajahdys.Position = olio.Position;
        rajahdys.Image = LoadImage("explosion2");
        rajahdys.Speed = 1500;
        Add(rajahdys);
        osumat.Value += 0.5; 
        if(osumat.Value >= kenttaNro*10)
        {
            kenttaNro++;
            taustat.Clear();
            koneet.Clear();
            SeuraavaKentta();
        }
    }

    //plane hit the cannon
    void OsuiTykkiin(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        Vector paikka = new Vector(tykki.X, tykki.Y + 700);
        Explosion rajahdys = new Explosion(150);
        rajahdys.Position = paikka;
        rajahdys.Image = LoadImage("explosion2");
        rajahdys.Speed = 500;
        Add(rajahdys);
        tykki.Destroy();
        tahtain.Destroy();
        EndOfGame();
    }

    //some nice to end of game
    void EndOfGame()
    {
        Mouse.DisableAll();
        foreach(PhysicsObject kone in koneet)
        {
            kone.Destroy();
        }
        koneet.Clear();
        nopeus = 50;
        skaalaus = 1.05;
        Timer lisaaKone = new Timer();
        lisaaKone.Interval = 0.01;
        lisaaKone.Timeout += TeeLentokone;
        lisaaKone.Start();

        Timer.SingleShot(0.1, delegate { Otsikko("End of game", 1, 5); });
        Timer.SingleShot(2.6, delegate { Otsikko("End of game", 1, 5); });
        Timer.SingleShot(5.1, delegate { Otsikko("End of game", 1, 5); });
        Timer.SingleShot(7, delegate { lisaaKone.Stop(); Pause(); Painikkeet(); });
    }

    //add start and end buttons
    void Painikkeet()
    {
        Mouse.IsCursorVisible = true;
        GameObject startPainike = new GameObject(300,100);
        startPainike.Y = Screen.Top / 7;
        startPainike.Image = LoadImage("start");
        Add(startPainike,2);
        kenttaNro = 1;
        Mouse.ListenOn(startPainike, MouseButton.Left, ButtonState.Pressed, SeuraavaKentta, "");

        GameObject endPainike = new GameObject(300,100);
        endPainike.Y = Screen.Bottom / 7;
        endPainike.Image = LoadImage("end");
        Add(endPainike,2);
        Mouse.ListenOn(endPainike, MouseButton.Left, ButtonState.Pressed, Exit, "");
    }
}
