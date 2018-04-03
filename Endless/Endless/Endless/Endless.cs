//Endless.cs for hobby game projets
//2.4.2018 by Aki Sirviö
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

//character collects candies and bombs. Game level continues endless. Watch out for cats
public class Endless : PhysicsGame
{
    const double nopeus = 500;
    const double hyppyNopeus = 750;
    const int RUUDUN_KOKO = 40;

    PlatformCharacter pelaaja1;
    PlatformCharacter kissa;
    Image pelaajanKuva = LoadImage("norsu");
    Image tahtiKuva = LoadImage("tahti");
    IntMeter karkkiLaskuri;
    IntMeter pommiLaskuri;
    IntMeter kissaLaskuri;
    SoundEffect karkkiAani = LoadSoundEffect("candy");
    SoundEffect kissaAani = LoadSoundEffect("cat-mad1");
    Image[] kavely = LoadImages("runningcat12small", "runningcat22small", "runningcat32small", "runningcat42small", "runningcat52small", "runningcat62small", "runningcat72small", "runningcat82small");
    Vector pelaajanPaikka;

    double ruutuLeveys;
    double ruutuKorkeus;
    double alkuPisteX;
    double alkuPisteY;
    double loppuPisteX;
    double loppuPisteY;
    string tasoImage;

    //this will start running program
    public override void Begin()
    {
        MediaPlayer.Play("NoDrumsMix-2silent");
        Window.Title = "Endless by Aki Sirviö";
        Window.AllowUserResizing = true;
        Mouse.IsCursorVisible = true;
        ruutuLeveys = Level.Width;
        ruutuKorkeus = Level.Height;
        alkuPisteX = RUUDUN_KOKO - (ruutuLeveys / 2);
        alkuPisteY = (ruutuKorkeus / 2) - RUUDUN_KOKO * 2;
        loppuPisteX = ruutuLeveys*2;
        loppuPisteY = 0-(ruutuKorkeus / 2);

        Kentta();
        TeePelaaja();
        LisaaNappaimet();
        Otsikko();
        LisaaLaskurit();

        Gravity = new Vector(0, -1000);
        Camera.FollowX(pelaaja1);
        Level.Background.CreateStars();
    }

    //random places of blocks, candies, bombs and cats
    void Kentta()
    {
        bool first = false;
        bool second = false;
        bool third = false;

        for (double j = alkuPisteY; loppuPisteY < j; j -= RUUDUN_KOKO * 2)
        {
            for (double i = alkuPisteX; loppuPisteX > i; i += RUUDUN_KOKO * 2)
            {
                bool one = false;
                bool two = false;
                bool three = false;

                if (RandomGen.NextBool())
                {
                    one = true;
                }
                if (RandomGen.NextBool())
                {
                    two = true;
                }
                if (RandomGen.NextBool())
                {
                    three = true;
                }

                if(one && !first)
                {
                    tasoImage = RandomGen.SelectOne("tekstuuri1", "tekstuuri2", "tekstuuri3",
                        "tekstuuri4", "tekstuuri5", "tekstuuri6", "tekstuuri7", "tekstuuri8", "tekstuuri9",
                        "tekstuuri10", "tekstuuri11", "tekstuuri12", "tekstuuri13", "tekstuuri14", "tekstuuri15",
                        "tekstuuri16", "tekstuuri17", "tekstuuri18", "tekstuuri19", "tekstuuri20", "tekstuuri21");
                    LisaaTaso(i, j);
                    first = true;                    
                }
                else if (one && two && first && !second)
                {
                    LisaaTaso(i, j);
                    second = true;
                }
                else if (one && two && three && second && !third)
                {
                    LisaaTaso(i, j);
                    third = true;
                }
                else
                {
                    first = false;
                    second = false;
                    third = false;
                }

                int luku = RandomGen.NextInt(1, 101);
                if(luku <= 2)
                {
                    LisaaPommi(i, j);
                }
                else if(luku <= 7)
                {
                    LisaaKarkki(i, j);
                }
                else if (luku <= 9)
                {
                    LisaaKissa(i, j);
                }
            }
        }
        //Alareuna();
    }

    //made uniform bottom edge
    void Alareuna()
    {
        for (double i = alkuPisteX; loppuPisteX > i; i += RUUDUN_KOKO)
        {
            LisaaTaso(i, loppuPisteY);
        }
    }

    //add level blocks
    void LisaaTaso(double X, double Y)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(RUUDUN_KOKO*2, RUUDUN_KOKO);
        taso.Position = new Vector(X, Y);
        taso.Color = Color.Green;
        taso.Image = LoadImage(tasoImage);
        Add(taso);
    }

    //add character
    void TeePelaaja()
    {
        pelaaja1 = new PlatformCharacter(RUUDUN_KOKO-5, RUUDUN_KOKO-5);
        pelaaja1.Position = new Vector(alkuPisteX, alkuPisteY);
        pelaaja1.Mass = 4.0;
        pelaaja1.Image = pelaajanKuva;
        pelaaja1.IgnoresExplosions = true;
        pelaaja1.Tag = "pelaaja";
        Add(pelaaja1);
        pelaajanPaikka = pelaaja1.Position;
    }

    //add candy
    void LisaaKarkki(double X, double Y)
    {
        PhysicsObject karkki = new PhysicsObject(RUUDUN_KOKO, RUUDUN_KOKO);
        karkki.Position = new Vector(X, Y+RUUDUN_KOKO);
        AddCollisionHandler(karkki, "pelaaja", SaitKarkin);
        string karkkiI = RandomGen.SelectOne("candy1", "candy2", "candy3", "candy4", "candy5", "candy6", "candy7");
        karkki.Image = LoadImage(karkkiI);
        Add(karkki);
    }

    //add bomb
    void LisaaPommi(double X, double Y)
    {
        PhysicsObject pommi = new PhysicsObject(RUUDUN_KOKO, RUUDUN_KOKO);
        pommi.Position = new Vector(X, Y + RUUDUN_KOKO);
        pommi.CanRotate = false;
        AddCollisionHandler(pommi, "pelaaja", OsuitPommiin);
        string bomI = RandomGen.SelectOne("bom1", "bom2", "bom3", "bom4", "bom5", "bom6", "bom7", "bom8", "bom9", "bom10");
        pommi.Image = LoadImage(bomI);
        Add(pommi);
    }

    //add cat
    void LisaaKissa(double X, double Y)
    {
        kissa = new PlatformCharacter(RUUDUN_KOKO, RUUDUN_KOKO);
        kissa.Position = new Vector(X, Y + RUUDUN_KOKO);
        AddCollisionHandler(kissa, "pelaaja", OsuitKissaan);
        kissa.Tag = "kissa";
        kissa.AnimWalk = new Animation(kavely);
        kissa.AnimWalk.FPS = 10;
        Add(kissa);

        int nopeus = RandomGen.NextInt(20, 200);
        PlatformWandererBrain tasoAivot = new PlatformWandererBrain();
        tasoAivot.Speed = nopeus;
        kissa.Brain = tasoAivot;
    }

    //add title
    void Otsikko()
    {
        GameObject otsikko = new GameObject(600, 200);
        otsikko.Y = Screen.Top - 100;
        otsikko.X = 0-ruutuLeveys;
        otsikko.Image = LoadImage("Endless");
        Add(otsikko);
    }

    //add counters
    void LisaaLaskurit()
    {
        karkkiLaskuri = new IntMeter(2);
        Label karkkiNaytto = new Label();
        karkkiNaytto.X = -100;
        karkkiNaytto.Y = Screen.Top - 50;
        karkkiNaytto.TextColor = Color.White;
        karkkiNaytto.Color = Color.Transparent;
        karkkiNaytto.TextScale *= 2;
        karkkiNaytto.BindTo(karkkiLaskuri);
        Add(karkkiNaytto);

        Label karkkiKuva = new Label(70, 50);
        karkkiKuva.X = -170;
        karkkiKuva.Y = Screen.Top - 50;
        karkkiKuva.Image = LoadImage("candy3");
        Add(karkkiKuva);

        pommiLaskuri = new IntMeter(0);
        Label pommiNaytto = new Label();
        pommiNaytto.X = 100;
        pommiNaytto.Y = Screen.Top - 50;
        pommiNaytto.TextColor = Color.White;
        pommiNaytto.Color = Color.Transparent;
        pommiNaytto.TextScale *= 2;
        pommiNaytto.BindTo(pommiLaskuri);
        Add(pommiNaytto);

        Label pommiKuva = new Label(50, 50);
        pommiKuva.X = 30;
        pommiKuva.Y = Screen.Top - 50;
        pommiKuva.Image = LoadImage("bom9");
        Add(pommiKuva);

        kissaLaskuri = new IntMeter(0);
        Label kissaNaytto = new Label();
        kissaNaytto.X = 300;
        kissaNaytto.Y = Screen.Top - 50;
        kissaNaytto.TextColor = Color.White;
        kissaNaytto.Color = Color.Transparent;
        kissaNaytto.TextScale *= 2;
        kissaNaytto.BindTo(kissaLaskuri);
        Add(kissaNaytto);

        Label kissaKuva = new Label(30, 50);
        kissaKuva.X = 230;
        kissaKuva.Y = Screen.Top - 50;
        kissaKuva.Image = LoadImage("0002small");
        Add(kissaKuva);
    }

    //add controls
    void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -nopeus);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, nopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);
        Keyboard.Listen(Key.Space, ButtonState.Pressed, HeitaPommi, "Heittää pommin");

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1, -nopeus);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, nopeus);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    //character moving
    void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
        TarkistaPaikka();
    }

    //character junping
    void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.ForceJump(nopeus);
    }

    //throw bomb
    void HeitaPommi()
    {
        if(pommiLaskuri.Value > 0)
        {
            pommiLaskuri.Value--;

            Grenade kranu = new Grenade(4.0);
            string bomI = RandomGen.SelectOne("bom1", "bom2", "bom3", "bom4", "bom5", "bom6", "bom7", "bom8", "bom10");
            kranu.Image = LoadImage(bomI);
            kranu.Explosion.ShockwaveReachesObject += PaineaaltoOsuu;
            kranu.Size *= 4;
            pelaaja1.Throw(kranu, Angle.FromDegrees(30), 3000);
        }
    }

    //got candy
    void SaitKarkin(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        karkkiAani.Play();
        tormaaja.Destroy();
        karkkiLaskuri.Value++;
    }

    //hit bomb
    void OsuitPommiin(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        if(karkkiLaskuri.Value > 1)
        {
            Explosion rajahdys = new Explosion(170);
            rajahdys.Position = tormaaja.Position;
            rajahdys.Speed = 70;
            rajahdys.AddShockwaveHandler("kissa", PaineaaltoOsuu);
            Add(rajahdys);
            tormaaja.Destroy();

            int pMaxMaara = 200;
            ExplosionSystem rajahdys2 = new ExplosionSystem(LoadImage("tahti"), pMaxMaara);
            rajahdys2.MinVelocity = 20;
            rajahdys2.MaxVelocity = 70;
            rajahdys2.MinScale = 1;
            rajahdys2.MaxScale = 10;
            rajahdys2.MinLifetime = 1;
            rajahdys2.MaxLifetime = 5;
            Add(rajahdys2);
            double x = tormaaja.X;
            double y = tormaaja.Y;
            int pMaara = RandomGen.NextInt(35, 200);
            rajahdys2.AddEffect(x, y, pMaara);

            pommiLaskuri.Value++;
            karkkiLaskuri.Value -= 2;
        }
    }

    //hit cat
    void OsuitKissaan(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        kissaAani.Play();
        pelaaja1.Position = pelaajanPaikka;
        tormaaja.Destroy();
        kissaLaskuri.Value++;
    }

    //shockwave hit cat
    void PaineaaltoOsuu(IPhysicsObject olio, Vector shokki)
    {
        if(olio.Tag.ToString()=="kissa")
        {
            kissaAani.Play();
            olio.Destroy();
            kissaLaskuri.Value++;
        }
    }

    //counting new coordinates for level blocks
    void TarkistaPaikka()
    {
        if (pelaaja1.X > loppuPisteX - (ruutuLeveys * 1.5))
        {
            alkuPisteX = loppuPisteX;
            loppuPisteX += ruutuLeveys * 2.5;
            Kentta();
        }
    }
}