//BasicPlatform.cs for project site
//23.3.2018 by Aki Sirviö
using System;
using Jypeli;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

//Basic platform game
public class BasicPlatform : PhysicsGame
{
    const double hyppyNopeus = 750;
    double nopeus = 350;
    const int RUUDUN_KOKO = 40;
    int kenttaNro = 0;

    PlatformCharacter pelaaja1;
    PhysicsObject euro;
    PhysicsObject euro2;
    PhysicsObject euro5;
    Label otsikko;
    private Image[] kavely = LoadImages("hahmo12", "hahmo13", "hahmo14", "hahmo15", "hahmo16", "hahmo17", "hahmo1");
    private Image[] idle = LoadImages("hahmo2", "hahmo21", "hahmo22", "hahmo23", "hahmo24");
    private Image[] hyppy = LoadImages("hahmoHyppy3", "hahmoHyppy2", "hahmoHyppy1", "hahmoHyppy4");
    SoundEffect maaliAani = LoadSoundEffect("maali");
    SoundEffect kolikko1Aani = LoadSoundEffect("Pickup_Coin4");
    SoundEffect kolikko2Aani = LoadSoundEffect("Pickup_Coin2");
    SoundEffect kolikko3Aani = LoadSoundEffect("Pickup_Coin32");
    SoundEffect hyppyAani = LoadSoundEffect("Jump3");
    SoundEffect effectAani = LoadSoundEffect("Randomize22");
    IntMeter pisteLaskuri;
    Vector pelaajanPaikka;
    int alkuPisteet = 0;
    int kolikot = 0;

    //Program starts here
    public override void Begin()
    {
        SeuraavaKentta();
    }

    //Moving from one level to another 
    void SeuraavaKentta()
    {
        ClearAll();

        if (kenttaNro == 0)
        {
            LuoKentta("kentta0");
            Timer.SingleShot(2, Painovoima);
            Timer.SingleShot(4, TeeAivot);
        }
        else if (kenttaNro == 1)
        {
            MediaPlayer.Play("BasicPlatform");
            MediaPlayer.IsRepeating = true;
            LuoKentta("kentta1");
            otsikko.TextScale /= 2;
            Painovoima();
            pisteLaskuri.Value = alkuPisteet;
        }
        else if (kenttaNro == 2)
        {
            MediaPlayer.Play("BasicPlatform2");
            MediaPlayer.IsRepeating = true;
            kavely = LoadImages("hahmo3", "hahmo31", "hahmo32", "hahmo34", "hahmo35");
            hyppy = LoadImages("hahmo3", "hahmo31", "hahmo32", "hahmo34", "hahmo35");
            LuoKentta("kentta2");
            otsikko.TextScale /= 3;
            nopeus += 150;
            pelaaja1.Size *= 5;
            Timer.SingleShot(5, Painovoima);
            Timer.SingleShot(2, TeeAivot);
            pisteLaskuri.Value = alkuPisteet;

            Timer ajastin = new Timer();
            ajastin.Interval = 0.5;
            ajastin.Timeout += KolikkoEfekti;
            ajastin.Start();
        }
        else if (kenttaNro > 2)
        {
            kenttaNro = 1;
            SeuraavaKentta();
            pisteLaskuri.Value = alkuPisteet;
        }

        LisaaNappaimet();
        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 0.2;
        Camera.StayInLevel = true;
    }

    //Makes level with a map file
    void LuoKentta(string taso)
    {
        TileMap kentta = TileMap.FromLevelAsset(taso);
        kentta.SetTileMethod('O', LisaaOtsikko);
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('*', LisaaEuro);
        kentta.SetTileMethod('E', LisaaEuro2);
        kentta.SetTileMethod('$', LisaaEuro5);
        kentta.SetTileMethod('U', LisaaPelaaja);
        kentta.SetTileMethod('P', LisaaPistelaskuri);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Surfaces reunat = Level.CreateBorders();
        reunat.Bottom.Color = Color.ForestGreen;
        reunat.Bottom.Collided += TormattiinAlareunaan;
        Level.Background.CreateGradient(Color.White, Color.SkyBlue);
    }

    //Add game name to headline
    void LisaaOtsikko(Vector paikka, double leveys, double korkeus)
    {
        otsikko = new Label();
        otsikko.Position = paikka;
        otsikko.Font = Font.DefaultLargeBold;
        otsikko.Text = "Basic Platform";
        otsikko.TextColor = Color.Navy;
        otsikko.Color = Color.Transparent;
        otsikko.TextScale *= 7;
        Add(otsikko);
    }

    //Add blocks
    void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = RandomGen.NextColor();
        Add(taso);
    }

    //Euro coin
    void LisaaEuro(Vector paikka, double leveys, double korkeus)
    {
        euro = new PhysicsObject(leveys, korkeus);
        euro.Position = paikka;
        euro.Image = LoadImage("euro-1");
        euro.Tag = "euro";
        Add(euro);
        kolikot++;
        euro.Oscillate(Vector.UnitX, 3, 20, 0.5 * Math.PI, 1);
        euro.OscillateAngle<Waveform.Triangle>(1, UnlimitedAngle.FromDegrees(20), 2, 1);
    }

    //2 euro coin
    void LisaaEuro2(Vector paikka, double leveys, double korkeus)
    {
        euro2 = new PhysicsObject(leveys, korkeus);
        euro2.Position = paikka;
        euro2.Image = LoadImage("euro-2");
        euro2.Tag = "euro2";
        Add(euro2);
        kolikot++;
    }

    //5 euro coin
    void LisaaEuro5(Vector paikka, double leveys, double korkeus)
    {
        euro5 = new PhysicsObject(leveys, korkeus);
        euro5.Position = paikka;
        euro5.Image = LoadImage("coin-5");
        euro5.Tag = "euro5";
        Add(euro5);
        kolikot++;
    }

    //Add game character
    void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(100, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 4.0;
        pelaaja1.Image = LoadImage("hahmo2");
        AddCollisionHandler(pelaaja1, TormaaKolikkoon);
        pelaaja1.AnimWalk = new Animation(kavely);
        pelaaja1.AnimIdle = new Animation(idle);
        pelaaja1.AnimJump = new Animation(hyppy);
        pelaaja1.AnimJump.StopOnLastFrame = true;
        pelaaja1.AnimWalk.FPS = 5;
        pelaaja1.AnimIdle.FPS = 35;
        pelaaja1.AnimJump.FPS = 5;
        Add(pelaaja1);
        pelaajanPaikka = paikka;
    }

    //Charecter brain at zero level
    void TeeAivot()
    {
        PlatformWandererBrain tasoAivot = new PlatformWandererBrain();
        tasoAivot.Speed = 500;
        tasoAivot.TriesToJump = true;
        pelaaja1.Brain = tasoAivot;
    }

    //Controls
    void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -nopeus);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, nopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");
        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1, -nopeus);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, nopeus);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    //Points calculator
    void LisaaPistelaskuri(Vector paikka, double leveys, double korkeus)
    {
        pisteLaskuri = new IntMeter(0);
        Label pisteNaytto = new Label();
        pisteNaytto.TextScale *= 3;
        pisteNaytto.Position = paikka;
        pisteNaytto.TextColor = Color.Gray;
        pisteNaytto.Color = Color.Transparent;
        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);

        Label euroTeksti = new Label();
        euroTeksti.Position = paikka;
        euroTeksti.X += 170;
        euroTeksti.TextColor = Color.Gold;
        euroTeksti.Color = Color.Transparent;
        euroTeksti.TextScale *= 3;
        euroTeksti.Text = "Euroa";
        Add(euroTeksti);
    }

    //Moving character
    void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
    }

    //Junping character
    void Hyppaa(PlatformCharacter hahmo, double hyppynopeus)
    {
        hahmo.Jump(hyppynopeus);
        hyppyAani.Play();
    }

    //Character collides with coin
    void TormaaKolikkoon(PhysicsObject hahmo, PhysicsObject kohde)
    {
        if(kohde.Tag.ToString() == "euro")
        {
            kolikko1Aani.Play();
            kohde.Destroy();
            pisteLaskuri.Value += 1;
            kolikot--;
            if (kolikot == 0)
            {
                alkuPisteet = pisteLaskuri.Value;
                kenttaNro++;
                Timer.SingleShot(2, SeuraavaKentta);
            }
        }
        else if (kohde.Tag.ToString() == "euro2")
        {
            kolikko2Aani.Play();
            kohde.Destroy();
            pisteLaskuri.Value += 2;
            kolikot--;
            if (kolikot == 0)
            {
                alkuPisteet = pisteLaskuri.Value;
                kenttaNro++;
                Timer.SingleShot(2, SeuraavaKentta);
            }
        }
        else if (kohde.Tag.ToString() == "euro5")
        {
            kolikko3Aani.Play();
            kohde.Destroy();
            pisteLaskuri.Value += 5;
            kolikot--;
            if (kolikot == 0)
            {
                alkuPisteet = pisteLaskuri.Value;
                kenttaNro++;
                Timer.SingleShot(2, SeuraavaKentta);
            }
        }
    }

    //Character crashes to bottom
    void TormattiinAlareunaan(IPhysicsObject tormaaja, IPhysicsObject kohde)
    {
        kohde.Position = pelaajanPaikka;
    }

    //Gravity on
    void Painovoima()
    {
        Gravity = new Vector(0, -1000);
    }

    //Effect is displayed when field is passed
    void KolikkoEfekti()
    {
        effectAani.Play();
        int pMaxMaara = 200;
        ExplosionSystem rajahdys = new ExplosionSystem(LoadImage("euro-1"), pMaxMaara);
        Add(rajahdys,3);
        ExplosionSystem rajahdys2 = new ExplosionSystem(LoadImage("euro-2"), pMaxMaara);
        Add(rajahdys2,0);
        ExplosionSystem rajahdys3 = new ExplosionSystem(LoadImage("coin-5"), pMaxMaara);
        Add(rajahdys3,-3);
        double x = 0;
        double y = 0;
        int pMaara = 50;
        rajahdys.AddEffect(x, y, pMaara);
        rajahdys2.AddEffect(x, y, pMaara);
        rajahdys3.AddEffect(x, y, pMaara);
    }
}
