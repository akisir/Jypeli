//EndlessRace.cs for hobby projects
//7.4.2018 by Aki Sirviö
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

//drive by car and compete against time in endless race
public class EndlessRace : PhysicsGame
{
    Vector[] reitti;
    Automobile auto;
    RoadMap tie;
    IntMeter kakkuLaskuri;
    Timer aikaLaskuri;
    int leveys = 250;
    int nopeus = 700;

    SoundEffect jarrutus = LoadSoundEffect("car-breaking");
    SoundEffect kaynnistys = LoadSoundEffect("StartCar");
    SoundEffect torvi = LoadSoundEffect("OOGAhorn");
    SoundEffect kaynti = LoadSoundEffect("car+start3");
    SoundEffect kaide = LoadSoundEffect("kaide3");
    SoundEffect kakku = LoadSoundEffect("kakku");
    SoundEffect turboAani = LoadSoundEffect("car+running3");
    SoundEffect turboHit = LoadSoundEffect("turbo");
    SoundEffect pommi = LoadSoundEffect("pommi2");

    //program starts here
    public override void Begin()
    {
        Vector b = new Vector(0, 0);
        reitti = new Vector[] { b, b, b, b, b };
        LuoRata();
        LisaaPelaaja();
        TeeLaskuri();
        TeeAikalaskuri();
        AlkuAnimaatio();

        Camera.Follow(auto);
        Camera.FollowOffset = new Vector(0,Screen.Height*0.3);
        Window.Title = "Car race 1";
        Mouse.IsCursorVisible = true;
        Level.Background.CreateStars();
    }

    //controls
    void Nappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Up, ButtonState.Down, Kiihdyta, "Kiihdytä", auto);
        Keyboard.Listen(Key.Up, ButtonState.Released, Jarruta, "Jarruta", auto);
        Keyboard.Listen(Key.Left, ButtonState.Down, Kaanny, "Vasemmalle", auto, 1);
        Keyboard.Listen(Key.Right, ButtonState.Down, Kaanny, "Oikealle", auto, -1);

        ShowControlHelp();
    }

    //add character
    void LisaaPelaaja()
    {
        auto = new Automobile(50, 35);
        auto.Mass = 20;
        auto.Tag = "auto";
        auto.X = 0;
        auto.Maneuverability = Angle.FromDegrees(200);
        auto.Acceleration = 450;
        auto.BrakeDeceleration = 2000;
        auto.TopSpeed = 3000;
        auto.Image = LoadImage("car");
        auto.AngularDamping = 0.2;
        auto.LinearDamping = 0.97;
        auto.Angle = Angle.FromDegrees(90);
        auto.Restitution = 1.0;
        Add(auto);
    }

    //make endless track one piece at time
    void LuoRata()
    {
        Vector a;
        double pituus = 0;
        double koordX = 0;
        double koordY = 0;
        double X = 0;
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                koordX = reitti[4].X;
                koordY = reitti[4].Y;
                a = new Vector(koordX, (koordY - 30));
            }
            else
            {
                pituus = RandomGen.NextDouble(500, 2000);
                X = RandomGen.NextInt(-100, 100);
                koordY += pituus;
                a = new Vector(X, koordY);
                
            }
            if (i == 2)
            {
                TeeApuviiva(0, koordY / 2);
            }
            reitti[i] = a;
        }

        leveys = RandomGen.NextInt(150, 350);
        tie = new RoadMap(reitti);
        tie.DefaultWidth = leveys;
        tie.Insert();

        TeeKaiteet();
        TeeEsineet(reitti[0].Y, reitti[4].Y);
    }

    //make objects to collect
    void TeeEsineet(double alku, double loppu)
    {
        Image kakku1 = LoadImage("cake");
        Image kakku2 = LoadImage("cake2");
        Image kakku3 = LoadImage("cake3");
        Image kakku4 = LoadImage("cake4");
        for (int i=0; i<10; i++)
        {
            double x = RandomGen.NextDouble(-100, 100);
            double y = RandomGen.NextDouble(alku, loppu);
            Vector piste = new Vector(x, y);
            foreach (var segment in tie.Segments)
            {
                if (segment.IsInside(piste))
                {
                    PhysicsObject kakku = PhysicsObject.CreateStaticObject(75, 60);
                    kakku.Position = piste;
                    kakku.IgnoresCollisionResponse = true;
                    kakku.Image = RandomGen.SelectOne(kakku1, kakku2, kakku3, kakku4);
                    AddCollisionHandler<PhysicsObject, Automobile>(kakku, OsuKakkuun);
                    Add(kakku);
                }
            }
        }

        for (int i = 0; i < 5; i++)
        {
            double x = RandomGen.NextDouble(-100, 100);
            double y = RandomGen.NextDouble(alku, loppu);
            Vector piste = new Vector(x, y);
            foreach (var segment in tie.Segments)
            {
                if (segment.IsInside(piste))
                {
                    PhysicsObject turbo = PhysicsObject.CreateStaticObject(40, 50);
                    turbo.Position = piste;
                    turbo.IgnoresCollisionResponse = true;
                    turbo.Image = LoadImage("flame");
                    AddCollisionHandler<PhysicsObject, Automobile>(turbo, OsuTurboon);
                    Add(turbo);
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            double x = RandomGen.NextDouble(-100, 100);
            double y = RandomGen.NextDouble(alku, loppu);
            Vector piste = new Vector(x, y);
            foreach (var segment in tie.Segments)
            {
                if (segment.IsInside(piste))
                {
                    PhysicsObject pommi = PhysicsObject.CreateStaticObject(20, 35);
                    pommi.Position = piste;
                    pommi.IgnoresCollisionResponse = true;
                    pommi.Image = LoadImage("bom6");
                    AddCollisionHandler<PhysicsObject, Automobile>(pommi, OsuPommiin);
                    Add(pommi);
                }
            }
        }
    }

    //make handrails
    void TeeKaiteet()
    {
        Vector[] sKaide = new Vector[reitti.Length];
        Vector[] uKaide = new Vector[reitti.Length];
        double X = 0;
        double Y = 0;
        for (int i = 0; i < reitti.Length; i++)
        {
            X = reitti[i].X + (leveys/2) + 25;
            Y = reitti[i].Y;
            sKaide[i] = new Vector(X, Y);
            X = reitti[i].X - (leveys/2) - 25;
            Y = reitti[i].Y;
            uKaide[i] = new Vector(X, Y);
        }

        RoadMap kaide1 = new RoadMap(sKaide);
        kaide1.DefaultWidth = 50;
        kaide1.CreateSegmentFunction = KaiteenOsa;
        kaide1.Insert();
        RoadMap kaide2 = new RoadMap(uKaide);
        kaide2.DefaultWidth = 70;
        kaide2.CreateSegmentFunction = KaiteenOsa;
        kaide2.Insert();
    }

    //part of handrail
    PhysicsObject KaiteenOsa(double width, double height, Shape shape)
    {
        PhysicsObject kaide = PhysicsObject.CreateStaticObject(width, height, shape);
        kaide.Color = Color.DarkGray;
        kaide.Restitution = 1.0;
        AddCollisionHandler<PhysicsObject, Automobile>(kaide, TormaysKaiteeseen);
        Add(kaide);
        return kaide;
    }

    //make help line
    void TeeApuviiva(double x, double y)
    {
        PhysicsObject apuviiva = PhysicsObject.CreateStaticObject(leveys+350, 10);
        apuviiva.X = x;
        apuviiva.Y = y;
        apuviiva.Color = Color.DarkGray;
        apuviiva.IgnoresCollisionResponse = true;
        apuviiva.IsVisible = false;
        apuviiva.Tag = "apuviiva";
        AddCollisionHandler<PhysicsObject, Automobile>(apuviiva, ApuviivaOsui);
        Add(apuviiva);
    }

    //make cake counter
    void TeeLaskuri()
    {
        kakkuLaskuri = new IntMeter(0);
        Label kakkuMittari = new Label();
        kakkuMittari.X = Screen.Left + Screen.Width/4;
        kakkuMittari.Y = Screen.Top - 100;
        kakkuMittari.TextScale *= 2;
        kakkuMittari.TextColor = Color.Yellow;
        kakkuMittari.Color = Color.Transparent;
        kakkuMittari.Title = "Cakes";
        kakkuMittari.BindTo(kakkuLaskuri);
        Add(kakkuMittari);

        GameObject kakkuKuva = new GameObject(50, 50);
        kakkuKuva.X -= 150;
        kakkuKuva.Y += 10;
        kakkuKuva.Image = LoadImage("cake2");
        kakkuMittari.Add(kakkuKuva);
    }

    //make time counter
    void TeeAikalaskuri()
    {
        aikaLaskuri = new Timer();
        Label aikaNaytto = new Label();
        aikaNaytto.TextColor = Color.Yellow;
        aikaNaytto.Color = Color.Transparent;
        aikaNaytto.Title = "Time";
        aikaNaytto.DecimalPlaces = 1;
        aikaNaytto.BindTo(aikaLaskuri.SecondCounter);
        aikaNaytto.X = Screen.Right - Screen.Width/4;
        aikaNaytto.Y = Screen.Top - 100;
        aikaNaytto.TextScale *= 2;
        Add(aikaNaytto);

        GameObject kelloKuva = new GameObject(50, 50);
        kelloKuva.X -= 150;
        kelloKuva.Y += 10;
        kelloKuva.Image = LoadImage("clock3");
        aikaNaytto.Add(kelloKuva);
    }

    //accelerate
    void Kiihdyta(Automobile kaara)
    {
        kaynti.Play();
        kaara.Accelerate();
        auto.Acceleration = nopeus;
    }

    //break
    void Jarruta(Automobile kaara)
    {
        kaynti.Stop();
        jarrutus.Play();
        kaara.Brake();
    }

    //turn
    void Kaanny(Automobile kaara, int suunta)
    {
        kaara.Turn(kaara.Maneuverability * suunta, Time.SinceLastUpdate.TotalSeconds);
    }

    //Normal speed
    void NormaaliNopeus()
    {
        nopeus = 700;
        turboAani.Stop();
    }

    //hit guide line
    void ApuviivaOsui(PhysicsObject tormaaja, Automobile kohde)
    {
        LuoRata();
    }

    //hit handrail
    void TormaysKaiteeseen(PhysicsObject tormaaja, Automobile kohde)
    {
        kaynti.Stop();
        kaide.Play();
    }

    //hit cake
    void OsuKakkuun(PhysicsObject tormaaja, Automobile kohde)
    {
        kakkuLaskuri.Value++;
        tormaaja.Destroy();
        kaynti.Stop();
        kakku.Play();
    }

    //hit flame
    void OsuTurboon(PhysicsObject tormaaja, Automobile kohde)
    {
        tormaaja.Destroy();
        kaynti.Stop();
        turboAani.Play();
        turboHit.Play();
        nopeus = 1700;
        Timer.SingleShot(2, NormaaliNopeus);
    }

    //hit bomb
    void OsuPommiin(PhysicsObject tormaaja, Automobile kohde)
    {
        tormaaja.Destroy();
        kaynti.Stop();
        pommi.Play();
        Explosion rajahdys = new Explosion(50);
        rajahdys.Position = tormaaja.Position;
        rajahdys.Speed = 400.0;
        rajahdys.Force = 5000;
        Add(rajahdys);
    }

    //start animation
    void AlkuAnimaatio()
    {
        Vector paikka1 = new Vector(0, 350);
        Vector paikka2 = new Vector(0, 0);
        Animaatio("Endless Race", paikka1, 0.01, 255, 0.3);
        Animaatio("3", paikka2, 0.5, 200, 1.7);
        int luku = 2;
        Timer alkuAnim = new Timer();
        alkuAnim.Interval = 2;
        alkuAnim.TimesLimited = true;
        alkuAnim.Timeout += delegate { Animaatio(luku--.ToString(), paikka2, 0.5, 200, 1.7); };
        alkuAnim.Start(3);
        Timer.SingleShot(6, Nappaimet);
        Timer.SingleShot(6, delegate { aikaLaskuri.Start(); });
        Timer.SingleShot(3.5, delegate { kaynnistys.Play(); });
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
        torvi.Play();

        Timer.SingleShot(0.1, delegate { TekstiAnimaatio(kyltti, koko, alpha, alphaMuutos); });
    }

    //change text size and color transparent
    void TekstiAnimaatio(Label kyltti, double koko, double alpha, double alphaMuutos)
    {
        kyltti.TextScale += new Vector(koko, koko);
        kyltti.TextColor = new Color((int)0, (int)70, (int)alpha, (int)alpha);
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
