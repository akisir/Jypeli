//TenLevelPlatform.cs for hobby project site
//24.3.2018 by Aki Sirviö
using System.Collections.Generic;
using Jypeli;
using Jypeli.Controls;
using Jypeli.Widgets;

//game character collects kittens and cares for dogs and spikes. The cat has 9 lives.
public class TenLevelPlatform : PhysicsGame
{
    const double hyppyNopeus = 750;
    const double maxNopeus = 800;
    const int RUUDUN_KOKO = 40;
    double nopeus = 200;
    int kenttaNro = 11;
    int kissatMaara = 0;
    bool piikitetty = false;
    int superHyppy = 0;
    int kissat = 0;
    int koirat = 0;
    int elamat = 9;
    IntMeter pisteLaskuri;
    IntMeter elamaLaskuri;
    IntMeter koiraLaskuri;
    IntMeter juoksuLaskuri;
    PlatformCharacter pelaaja1;
    PhysicsObject taso;
    Label juoksuNaytto;
    Vector pelaajanPaikka;
    List<PhysicsObject> tasot = new List<PhysicsObject>();
    Image pelaajanKuva = LoadImage("norsu");
    Image koiraKuva1 = LoadImage("dog1");
    Image koiraKuva2 = LoadImage("dog2");
    Image koiraKuva3 = LoadImage("dog3");
    Image koiraKuva4 = LoadImage("dog4");
    Image koiraKuva5 = LoadImage("dog5");
    Image texture1 = LoadImage("texture1");
    Image texture2 = LoadImage("texture2");
    Image texture3 = LoadImage("texture3");
    Image texture4 = LoadImage("texture4");
    Image texture5 = LoadImage("texture5");
    Image texture6 = LoadImage("texture6");
    Image texture7 = LoadImage("texture7");
    Image texture8 = LoadImage("texture8");
    Image texture9 = LoadImage("texture9");
    Image texture10 = LoadImage("texture10");
    Image texture11 = LoadImage("texture11");
    Image texture12 = LoadImage("texture12");
    Image texture13 = LoadImage("texture13");
    Image[] kavely = LoadImages("runningcat12small", "runningcat22small", "runningcat32small", "runningcat42small", "runningcat52small", "runningcat62small", "runningcat72small", "runningcat82small");
    Image[] hyppy = LoadImages("runningcat72small", "runningcat12small", "runningcat32small");
    Image[] idle = LoadImages("0002small", "0012small", "0022small", "0032small", "0042small", "0052small", "0062small");
    SoundEffect maaliAani = LoadSoundEffect("maali");
    SoundEffect kittenAani = LoadSoundEffect("cat-meow4");
    SoundEffect kutsuaani = LoadSoundEffect("cat-meow8");
    SoundEffect kutsuaani2 = LoadSoundEffect("cat-meow7");
    SoundEffect koiraAani1 = LoadSoundEffect("dog-bark2");
    SoundEffect koiraAani2 = LoadSoundEffect("dog-bark7");
    SoundEffect koiraAani3 = LoadSoundEffect("dog-bark9");
    SoundEffect koiraAani4 = LoadSoundEffect("dog-growl2");
    SoundEffect kissaAani1 = LoadSoundEffect("cat-mad1");
    SoundEffect kissaAani2 = LoadSoundEffect("cat-mad3");
    SoundEffect kissaAani3 = LoadSoundEffect("cat-meow6");
    SoundEffect koiraAani5 = LoadSoundEffect("dog-littlebark1");
    SoundEffect ruokaAani1 = LoadSoundEffect("apple-crunch-16");
    SoundEffect ruokaAani2 = LoadSoundEffect("chewing-carrot-a");
    SoundEffect ruokaAani3 = LoadSoundEffect("eating-an-apple-03");
    SoundEffect ruokaAani4 = LoadSoundEffect("eating-chips-by-fresco");
    SoundEffect ruokaAani5 = LoadSoundEffect("eating-crisps");
    SoundEffect ruokaAani6 = LoadSoundEffect("Straw Slurp");
    SoundEffect ruokaAani7 = LoadSoundEffect("Super Burp");
    SoundEffect ruokaAani8 = LoadSoundEffect("wet-sloppy-eating");

    //program starts here
    public override void Begin()
    {
        SeuraavaKentta();
        MediaPlayer.Play("NoDrumsMix-2silent");
        Mouse.IsCursorVisible = true;
        SetWindowSize(700, 500);
        Window.AllowUserResizing = true;
        Window.Title = "10 level platform by Aki Sirviö";
    }

    //selecting which field information to load and set field data
    void SeuraavaKentta()
    {
        if(kenttaNro > 0 && pisteLaskuri != null)
        {
            kissat = pisteLaskuri.Value;
            koirat = koiraLaskuri.Value;
            elamat = elamaLaskuri.Value;
        }
        ClearAll();
        Gravity = new Vector(0, -1000);
        if (kenttaNro == 0)
        {
            LuoKentta("kentta0");
            Level.Background.CreateGradient(Color.Magenta, Color.White);
        }
        else if (kenttaNro == 1)
        {
            LuoKentta("kentta1");
            Level.Background.CreateGradient(Color.Magenta, Color.White);
        }
        else if (kenttaNro == 2)
        {
            LuoKentta("kentta2");
            Level.Background.CreateGradient(Color.Rose, Color.Gray);
            foreach (PhysicsObject obj in tasot)
            {
                obj.Color = Color.Olive;
            }
        }
        else if (kenttaNro == 3)
        {
            LuoKentta("kentta3");
            Level.Background.CreateGradient(Color.Violet, Color.LightGray);
            foreach (PhysicsObject obj in tasot)
            {
                obj.Color = Color.BrownGreen;
            }
        }
        else if (kenttaNro == 4)
        {
            LuoKentta("kentta4");
            Level.Background.CreateGradient(Color.HotPink, Color.Lavender);
            foreach (PhysicsObject obj in tasot)
            {
                obj.Color = Color.DarkAzure;
            }
        }
        else if (kenttaNro == 5)
        {
            LuoKentta("kentta5");
            Level.Background.CreateGradient(Color.MediumVioletRed, Color.LightGreen);
            foreach (PhysicsObject obj in tasot)
            {
                obj.Image = RandomGen.SelectOne(texture1, texture2, texture3, texture4, texture5, texture6, texture7, texture8, texture9, texture10, texture11, texture12, texture13);
            }
        }
        else if (kenttaNro == 6)
        {
            LuoKentta("kentta6");
            Level.Background.CreateGradient(Color.Purple, Color.Silver);
            foreach (PhysicsObject obj in tasot)
            {
                obj.Image = RandomGen.SelectOne(texture1, texture2, texture3, texture4, texture5, texture6, texture7, texture8, texture9, texture10, texture11, texture12, texture13);
            }
        }
        else if (kenttaNro == 7)
        {
            LuoKentta("kentta7");
            Level.Background.Image = LoadImage("landscapePanorama");
            Level.Background.FitToLevel();
            foreach (PhysicsObject obj in tasot)
            {
                obj.Image = RandomGen.SelectOne(texture1, texture2, texture3, texture4, texture5, texture6, texture7, texture8, texture9, texture10, texture11, texture12, texture13);
            }
        }
        else if (kenttaNro == 8)
        {
            LuoKentta("kentta8");
            Level.Background.Image = LoadImage("landscapePanorama2");
            Level.Background.FitToLevel();
            foreach (PhysicsObject obj in tasot)
            {
                obj.Image = LoadImage("texture4");
            }
        }
        else if (kenttaNro == 9)
        {
            LuoKentta("kentta9");
            Level.Background.Image = LoadImage("landscapePanorama3");
            Level.Background.FitToLevel();
            foreach (PhysicsObject obj in tasot)
            {
                obj.Image = LoadImage("texture13");
            }
        }
        else if (kenttaNro == 10)
        {
            LuoKentta("kentta10");
            Level.Background.Image = LoadImage("landscapePanorama4");
            Level.Background.FitToLevel();
            foreach (PhysicsObject obj in tasot)
            {
                obj.Image = LoadImage("texture6");
            }
        }
        else if (kenttaNro == 11)
        {
            LuoKentta("kentta11");
        }
        else if (kenttaNro > 11)
        {
            kenttaNro = 0;
            SeuraavaKentta();
        }

        LisaaNappaimet();
        TeeLaskurit();
        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 1.2;
        Camera.StayInLevel = true;
    }

    //field is uploaded using a field file
    void LuoKentta(string taso)
    {
        TileMap kentta = TileMap.FromLevelAsset(taso);
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('*', LisaaKissa);
        kentta.SetTileMethod('N', LisaaPelaaja);
        kentta.SetTileMethod('O', LisaaOtsikko);
        kentta.SetTileMethod('K', LisaaKoira);
        kentta.SetTileMethod('R', LisaaRuoka);
        kentta.SetTileMethod('P', LisaaPiikki);
        kentta.SetTileMethod('L', LoppuKommentti);
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
    }

    //add level blocks
    void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.DarkCyan;
        Add(taso);
        tasot.Add(taso);
    }

    //add cats to be collocted
    void LisaaKissa(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject kissa = new PhysicsObject(leveys/3, korkeus/3);
        kissa.Position = paikka;
        kissa.Image = LoadImage("kitten2");
        kissa.Tag = "kitten";
        kissa.CanRotate = false;
        Add(kissa);

        FollowerBrain seuraajanAivot = new FollowerBrain("pelaaja");
        seuraajanAivot.Speed = 0;
        kissa.Brain = seuraajanAivot;
        seuraajanAivot.DistanceToTarget.AddTrigger(200, TriggerDirection.Down, Naukasu);
        seuraajanAivot.DistanceToTarget.AddTrigger(100, TriggerDirection.Up, Naukasu2);

        kissatMaara++;
    }

    //add food bowl
    void LisaaRuoka(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject ruoka = new PhysicsObject(leveys*2, korkeus*2);
        ruoka.Position = paikka;
        ruoka.Image = LoadImage("dry-cat-food");
        ruoka.Tag = "ruoka";
        ruoka.CanRotate = false;
        AddCollisionHandler(ruoka, "pelaaja", SaitRuokaa);
        Add(ruoka);
    }

    //add dog
    void LisaaKoira(Vector paikka, double leveys, double korkeus)
    {
        PlatformCharacter koira = new PlatformCharacter(75, 75);
        koira.Position = paikka;
        koira.Image = RandomGen.SelectOne(koiraKuva1,koiraKuva2,koiraKuva3,koiraKuva4, koiraKuva5);
        koira.Color = Color.Green;
        koira.Tag = "koira";
        koira.CanRotate = false;
        AddCollisionHandler<PlatformCharacter, PhysicsObject>(koira, "pelaaja", OsuitKoiraan);
        Add(koira);

        int nopeus = RandomGen.NextInt(50, 300);
        PlatformWandererBrain tasoAivot = new PlatformWandererBrain();
        tasoAivot.Speed = nopeus;
        koira.Brain = tasoAivot;

        GameObject apuPalikka = new GameObject(leveys, korkeus);
        apuPalikka.IsVisible = false;
        koira.Add(apuPalikka);
        FollowerBrain seuraajanAivot = new FollowerBrain("pelaaja");
        seuraajanAivot.Speed = 0;
        apuPalikka.Brain = seuraajanAivot;
        seuraajanAivot.DistanceToTarget.AddTrigger(100, TriggerDirection.Irrelevant, Haukkasu);
    }

    //add spike
    void LisaaPiikki(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject piikki = PhysicsObject.CreateStaticObject(leveys, korkeus);
        piikki.Position = paikka;
        piikki.Image = LoadImage("piikki");
        AddCollisionHandler(piikki, OsuiPiikkiin);
        Add(piikki);
    }

    //add counters
    void TeeLaskurit()
    {
        pisteLaskuri = new IntMeter(kissat);

        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.Transparent;
        pisteNaytto.TextScale *= 1.5;
        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);

        Label kissaNaytto = new Label(40,40);
        kissaNaytto.X = Screen.Left + 50;
        kissaNaytto.Y = Screen.Top - 100;
        kissaNaytto.Image = LoadImage("kitten2");
        Add(kissaNaytto);

        elamaLaskuri = new IntMeter(elamat);

        Label elamaNaytto = new Label();
        elamaNaytto.X = Screen.Left + 320;
        elamaNaytto.Y = Screen.Top - 100;
        elamaNaytto.TextColor = Color.Black;
        elamaNaytto.Color = Color.Transparent;
        elamaNaytto.TextScale *= 1.5;
        elamaNaytto.BindTo(elamaLaskuri);
        Add(elamaNaytto);

        Label pelaajaNaytto = new Label(50, 50);
        pelaajaNaytto.X = Screen.Left + 300;
        pelaajaNaytto.Y = Screen.Top - 100;
        pelaajaNaytto.Image = LoadImage("0002small");
        Add(pelaajaNaytto);

        koiraLaskuri = new IntMeter(koirat, -999,999);

        Label koiraNaytto = new Label();
        koiraNaytto.X = Screen.Left + 220;
        koiraNaytto.Y = Screen.Top - 100;
        koiraNaytto.TextColor = Color.Black;
        koiraNaytto.Color = Color.Transparent;
        koiraNaytto.TextScale *= 1.5;
        koiraNaytto.BindTo(koiraLaskuri);
        Add(koiraNaytto);

        Label haukkuNaytto = new Label(50, 50);
        haukkuNaytto.X = Screen.Left + 170;
        haukkuNaytto.Y = Screen.Top - 100;
        haukkuNaytto.Image = LoadImage("dog3");
        Add(haukkuNaytto);

        juoksuLaskuri = new IntMeter(200);

        juoksuNaytto = new Label();
        juoksuNaytto.X = Screen.Left + 450;
        juoksuNaytto.Y = Screen.Top - 100;
        juoksuNaytto.TextColor = Color.Black;
        juoksuNaytto.Color = Color.Transparent;
        juoksuNaytto.TextScale *= 1.5;
        juoksuNaytto.BindTo(juoksuLaskuri);
        Add(juoksuNaytto);

        Label juoksijaNaytto = new Label(50, 50);
        juoksijaNaytto.X = Screen.Left + 380;
        juoksijaNaytto.Y = Screen.Top - 100;
        juoksijaNaytto.Image = LoadImage("runningcat82small");
        Add(juoksijaNaytto);

        Label tasoNaytto = new Label();
        tasoNaytto.X = Screen.Left + 600;
        tasoNaytto.Y = Screen.Top - 100;
        tasoNaytto.TextColor = Color.DarkGray;
        tasoNaytto.Color = Color.Transparent;
        tasoNaytto.Font = Font.DefaultBold;
        tasoNaytto.TextScale *= 1.5;
        tasoNaytto.Text = "Level " + kenttaNro;
        Add(tasoNaytto);
    }

    //meow sound
    void Naukasu()
    {
        kutsuaani.Play();
    }

    //meow sound
    void Naukasu2()
    {
        kutsuaani2.Play();
    }

    //woof sound
    void Haukkasu()
    {
        SoundEffect kAani = RandomGen.SelectOne(koiraAani1, koiraAani2, koiraAani3, koiraAani4);
        kAani.Play();
    }

    //add game character
    void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 4.0;
        pelaaja1.Image = pelaajanKuva;
        pelaaja1.Tag = "pelaaja";
        pelaaja1.AnimWalk = new Animation(kavely);
        pelaaja1.AnimJump = new Animation(hyppy);
        pelaaja1.AnimIdle = new Animation(idle);
        pelaaja1.AnimJump.StopOnLastFrame = true;
        pelaaja1.AnimWalk.FPS = 10;
        pelaaja1.AnimJump.FPS = 5;
        pelaaja1.AnimIdle.FPS = 3;
        AddCollisionHandler(pelaaja1, "kitten", KerasitKissan);
        pelaajanPaikka = paikka;
        Add(pelaaja1);
    }

    //add title
    void LisaaOtsikko(Vector paikka, double leveys, double korkeus)
    {
        GameObject otsikko = new GameObject(50, 300);
        otsikko.Position = paikka;
        otsikko.Image = LoadImage("title");
        Add(otsikko,-3);
    }

    //add controls
    void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -1);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, 1);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);
    }

    //character moving
    void Liikuta(PlatformCharacter hahmo, int suunta)
    {
        if(!piikitetty)
        {
            hahmo.Walk(nopeus * suunta);
        }
    }

    //character jumping
    void Hyppaa(PlatformCharacter hahmo, double hyppy)
    {
        if (!piikitetty)
        {
            if(superHyppy == 0)
            {
                hahmo.Jump(hyppy);
            }
            else
            {
                hahmo.ForceJump(hyppy);
            }
        }
    }

    //player got the cat
    void KerasitKissan(PhysicsObject hahmo, PhysicsObject kohde)
    {
        kittenAani.Play();
        kohde.Destroy();
        pisteLaskuri.Value++;
        kissatMaara--;
        if(kissatMaara == 0)
        {
            kenttaNro++;
            SeuraavaKentta();
        }
    }

    //player got the food
    void SaitRuokaa(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        if(nopeus < maxNopeus)
        {
            nopeus += 200;
            juoksuLaskuri.Value += 200;
        }
        superHyppy++;
        tormaaja.Destroy();
        Timer.SingleShot(10, PudotaNopeus);
        SoundEffect rAani = RandomGen.SelectOne(ruokaAani1, ruokaAani2, ruokaAani3, ruokaAani4, ruokaAani5, ruokaAani6, ruokaAani7);
        Sound ruokaAani = rAani.CreateSound();
        ruokaAani.Volume = 1.0;
        ruokaAani.Play();
        juoksuNaytto.TextColor = Color.Darker(Color.Ruby,20);
    }

    //player hit the dog
    void OsuitKoiraan(PlatformCharacter tormaaja, PhysicsObject kohde)
    {
        pelaaja1.Position = pelaajanPaikka;
        tormaaja.Brain.Active = false;
        Timer.SingleShot(3, delegate { AivotKayntiin(tormaaja); });
        SoundEffect k2Aani = RandomGen.SelectOne(kissaAani1, kissaAani2);
        koiraAani5.Play();
        k2Aani.Play();
        koiraLaskuri.Value--;
    }

    //player or some other object hit the spike
    void OsuiPiikkiin(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        if (kohde.Tag.ToString() == "pelaaja")
        {
            pelaaja1.Position = pelaajanPaikka;
            pelaaja1.AnimIdle = new Animation(LoadImage("dedcatsmall"));
            pelaaja1.AnimWalk = null;
            pelaaja1.AnimJump = null;
            kissaAani2.Play();
            piikitetty = true;
            Timer.SingleShot(2, UusiElama);
            elamaLaskuri.Value--;
        }
        else if (kohde.Tag.ToString() == "kitten")
        {
            kissaAani3.Play();
            kohde.Destroy();
            kissatMaara--;
            if (kissatMaara == 0)
            {
                kenttaNro++;
                SeuraavaKentta();
            }
        }
        else if (kohde.Tag.ToString() == "koira")
        {
            koiraAani5.Play();
            kohde.Destroy();
        }
        else if (kohde.Tag.ToString() == "ruoka")
        {
            ruokaAani2.Play();
            kohde.Destroy();
        }
    }

    //dropping speed
    void PudotaNopeus()
    {
        if(nopeus > 200)
        {
            nopeus -= 200;
            juoksuLaskuri.Value -= 200;
        }
        superHyppy--;
        if(superHyppy == 0)
        {
            juoksuNaytto.TextColor = Color.Black;
        }
    }

    //activate dog's brain
    void AivotKayntiin(PlatformCharacter aivot)
    {
        aivot.Brain.Active = true;
    }

    //new life for character
    void UusiElama()
    {
        pelaaja1.AnimWalk = new Animation(kavely);
        pelaaja1.AnimJump = new Animation(hyppy);
        pelaaja1.AnimIdle = new Animation(idle);
        piikitetty = false;
        if (elamaLaskuri.Value == 0)
        {
            PeliLoppu();
        }
    }

    //end of game
    void PeliLoppu()
    {
        kissatMaara = 0;
        kenttaNro = 11;
        SeuraavaKentta();
    }

    //game end remarks
    void LoppuKommentti(Vector paikka, double leveys, double korkeus)
    {
        GameObject gameOver = new GameObject(300, 100);
        gameOver.Position = paikka;
        gameOver.Image = LoadImage("gameOver");
        Add(gameOver);
    }
}