//Hunting.cs for hobby game projects
//14.4.2018 by Aki Sirviö
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

//hunting animals, use the mouse
public class Hunting : PhysicsGame
{
    double scrollausnopeus = -0.5;
    List<GameObject> taustakuvat;
    List<PhysicsObject> elukkaLista;
    Timer taustaAjastin = new Timer();
    GameObject vikaTaustakuva;
    GameObject tahtain;
    Explosion rajahdys;
    Vector hiirenPaikka;
    Image[] elukat = LoadImages("bear", "bear2", "bear3", "bear4", "deer", "deer2", "deer3", "moose", "moose2", "moose3", "moose4", "moose5", "rabbit", "rabbit2", "rabbit3", "skeleton", "wolf", "wolf2", "wolf3", "wolf4");
    GameObject teksti;
    IntMeter osumat;
    IntMeter elukkaLaskuri;
    double y = 50;
    double zoomY = 0;
    double zoomX = 0;
    int vika = 0;
    bool suunta = true;
    bool zoomOn = false;

    //program starts from here
    public override void Begin()
    {
        LisaaTeksti();
        LuoTaustakuvat();
        Ohjaimet();
        TeeTahtain();
        LisaaLaskurit();
        Level.Background.CreateStars();
        elukkaLista = new List<PhysicsObject>();
        Timer elukkaAjastin = new Timer();
        elukkaAjastin.Interval = 3;
        elukkaAjastin.Timeout += LisaaElukka;
        elukkaAjastin.Start();
    }

    //move sight
    void Liikutatahtainta(AnalogState hiirenLiike)
    {
        Vector pos = Mouse.PositionOnScreen;
        int luku = 290;
        double left = Screen.Left - luku;
        double right = Screen.Right + luku;
        double top = Screen.Top + luku;
        double bottom = Screen.Bottom - luku;
        if(zoomOn)
        {
            if (pos.X > left && pos.X < right && pos.Y > bottom && pos.Y < top)
            {
                hiirenPaikka = Mouse.PositionOnWorld;
                tahtain.Position = Mouse.PositionOnWorld;
            }
        }
        else
        {
            hiirenPaikka = Mouse.PositionOnWorld;
            tahtain.Position = Mouse.PositionOnWorld;
        }

        TarkistaSuunta();
    }

    //change background scrolling direction
    void TarkistaSuunta()
    {
        if (Mouse.PositionOnScreen.X <= -Screen.Width / 5 && suunta)
        {
            scrollausnopeus *= -1;
            KaannaSuunta();
            suunta = false;
        }
        else if (Mouse.PositionOnScreen.X >= Screen.Width / 5 && !suunta)
        {
            scrollausnopeus *= -1;
            KaannaSuunta();
            suunta = true;
        }
    }

    //add sight
    void TeeTahtain()
    {
        tahtain = new GameObject(Screen.Width/3, Screen.Height/3);
        tahtain.Image = LoadImage("tahtain");
        Add(tahtain,3);
    }

    //starts loadin background pictures
    void LuoTaustakuvat()
    {
        taustaAjastin = new Timer();
        taustaAjastin.Interval = 0.007;
        taustaAjastin.Timeout += LiikutaTaustaa;
        taustaAjastin.Start();

        taustakuvat = new List<GameObject>();
        LisaaTaustakuva("panorama51", 1500, 1500);
        LisaaTaustakuva("panorama52", 1500, 1500);
        LisaaTaustakuva("panorama531", 1500, 1500);
        LisaaTaustakuva("panorama54", 1500, 1500);

        if (scrollausnopeus < 0)
        {
            vika = taustakuvat.Count - 1;
            vikaTaustakuva = taustakuvat[vika];
        }
        else
        {
            vika = 2;
            vikaTaustakuva = taustakuvat[vika];
            taustakuvat[3].Right = taustakuvat[0].Left;
        }
    }

    //controls
    void Ohjaimet()
    {
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Exit");
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Show help");
        Mouse.ListenMovement(0.1, Liikutatahtainta, "Aim wiht gun");
        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Rajahdys, "Shoot with gun.");
        Mouse.Listen(MouseButton.Right, ButtonState.Pressed, Zoom, "Sight on");
        Mouse.Listen(MouseButton.Right, ButtonState.Released, PaastaZoomi, "Sight off");
    }

    //add background picture as GameObject
    void LisaaTaustakuva(string nimi, double leveys, double korkeus)
    {
        GameObject olio = new GameObject(leveys, korkeus);
        olio.Image = LoadImage(nimi);
        olio.Tag = nimi;
        olio.Y = y;
        Add(olio);

        if (taustakuvat.Count > 0)
        {
            olio.Left = taustakuvat[taustakuvat.Count - 1].Right;
        }
        else
        {
            olio.Left = Screen.Left-olio.Width;
        }
        taustakuvat.Add(olio);
    }

    //add animals to random positions
    void LisaaElukka()
    {
        Image elukkaKuva = RandomGen.SelectOne(elukat[0], elukat[1], elukat[2], elukat[3], elukat[4], elukat[5], elukat[6], elukat[7], elukat[8], elukat[9], elukat[10], elukat[12], elukat[13], elukat[14], elukat[15], elukat[16], elukat[17], elukat[18], elukat[19]);
        double koko = RandomGen.NextDouble(1, 10);
        double koordx = RandomGen.NextDouble(Screen.Left, Screen.Right);
        double koordy = RandomGen.NextDouble(Screen.Bottom, -20);
        bool peili = RandomGen.NextBool();
        PhysicsObject elukka = new PhysicsObject(6,3);
        elukka.Size *= koko;
        elukka.X = koordx;
        elukka.Y = koordy;
        elukka.Tag = "elukka";
        elukka.Image = elukkaKuva;
        if(peili)
        {
            elukka.MirrorImage();
        }
        Add(elukka,2);
        elukkaLista.Add(elukka);

        elukkaLaskuri.Value++;
    }

    //add text
    void LisaaTeksti()
    {
        teksti = new GameObject(500, 70);
        teksti.Y = Screen.Top - 100;
        teksti.Image = LoadImage("hunting");
        Add(teksti, 3);
    }

    //move background pictures
    void LiikutaTaustaa()
    {
        int i = 0;
        foreach (GameObject taustakuva in taustakuvat)
        {
            taustakuva.X += scrollausnopeus;
            if(!zoomOn)
            {
                taustakuva.Y = y;
            }
            
            if (scrollausnopeus < 0 && taustakuva.Right < (Screen.Left-1500))
            {
                vika = i;
                taustakuva.Left = vikaTaustakuva.Right;
                vikaTaustakuva = taustakuva;
                if (taustakuva.Tag.ToString() == "panorama51")
                {
                    vikaTaustakuva.X += scrollausnopeus;
                }
            }
            else if (scrollausnopeus > 0 && taustakuva.Left > (Screen.Right+1500))
            {
                vika = i;
                taustakuva.Right = vikaTaustakuva.Left;
                vikaTaustakuva = taustakuva;
                vikaTaustakuva.X += scrollausnopeus;
            }
            i++;
        }

        foreach(GameObject obj in elukkaLista)
        {
            obj.X += scrollausnopeus;
        }
        LiikutaTekstia();
    }

    //makes explosion to mouse's coordinates
    void Rajahdys()
    {
        rajahdys = new Explosion(10);
        rajahdys.Position = hiirenPaikka;
        rajahdys.ShockwaveReachesObject += Osu;
        if(zoomOn)
        {
            rajahdys.Speed = 2000;
        }
        Add(rajahdys,2);
    }

    //move text
    void LiikutaTekstia()
    {
        teksti.X += scrollausnopeus / 2.0;

        if (teksti.X > 2500)
        {
            teksti.X = -1500;
        }
        else if (teksti.X < -2500)
        {
            teksti.X = 1500;
        }
    }

    //camera zoom
    void Zoom()
    {
        MuutaPaikka(-1);
        Camera.ZoomFactor = 3.0;
        zoomOn = true;
        Mouse.PositionOnWorld = new Vector(0, 0);
        hiirenPaikka = Mouse.PositionOnWorld;
        tahtain.Position = Mouse.PositionOnWorld;
        tahtain.Image = LoadImage("tahtainZoom3");
        tahtain.Size *= 2.5;
    }

    //release camera zoon
    void PaastaZoomi()
    {
        MuutaPaikka(1);
        Camera.ZoomFactor = 1.0;
        Mouse.PositionOnWorld = new Vector((zoomX/2), (zoomY/2));
        zoomOn = false;
        tahtain.Image = LoadImage("tahtain");
        tahtain.Size /= 2.5;
    }

    //move position if zoom is on
    void MuutaPaikka(int luku)
    {
        if(luku == -1)
        {
            zoomY = Mouse.PositionOnWorld.Y;
            zoomX = Mouse.PositionOnWorld.X;
        }
        foreach (GameObject taustakuva in taustakuvat)
        {
            taustakuva.Y += zoomY * luku;
            taustakuva.X += zoomX * luku;
        }
        foreach(GameObject obj in elukkaLista)
        {
            obj.Y += zoomY * luku;
            obj.X += zoomX * luku;
        }
    }

    //makes direction change possible
    void KaannaSuunta()
    {
        if (scrollausnopeus > 0)
        {
            if (vika == 3)
            {
                taustakuvat[3].Right = taustakuvat[0].Left;
                vikaTaustakuva = taustakuvat[vika];
            }
            else
            {
                taustakuvat[vika].Right = taustakuvat[vika + 1].Left;
            }
        }
        else
        {
            if (vika == 0)
            {
                taustakuvat[0].Left = taustakuvat[3].Right;
                vikaTaustakuva = taustakuvat[vika];
            }
            else
            {
                taustakuvat[vika].Left = taustakuvat[vika - 1].Right;
            }
        }
    }

    //ammo hit the animal
    void Osu(IPhysicsObject olio, Vector shokki)
    {
        if(!olio.IsDestroyed)
        {
            olio.Destroy();
            osumat.Value++;
        }
    }

    //add counters
    void LisaaLaskurit()
    {
        osumat = new IntMeter(0);
        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + Screen.Width/4;
        pisteNaytto.Y = Screen.Top - 170;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.Transparent;
        pisteNaytto.TextScale *= 2;
        pisteNaytto.BindTo(osumat);
        Add(pisteNaytto);

        Label Naytto = new Label(100,50);
        Naytto.X = Screen.Left + Screen.Width / 5;
        Naytto.Y = Screen.Top - 170;
        Naytto.TextColor = Color.Black;
        Naytto.Color = Color.Transparent;
        Naytto.TextScale *= 2;
        Naytto.Image = elukat[15];
        Add(Naytto);

        elukkaLaskuri = new IntMeter(0);
        Label pisteNaytto2 = new Label();
        pisteNaytto2.X = Screen.Right - Screen.Width / 4;
        pisteNaytto2.Y = Screen.Top - 170;
        pisteNaytto2.TextColor = Color.Black;
        pisteNaytto2.Color = Color.Transparent;
        pisteNaytto2.TextScale *= 2;
        pisteNaytto2.BindTo(elukkaLaskuri);
        Add(pisteNaytto2);

        Label Naytto2 = new Label(100,50);
        Naytto2.X = Screen.Right - Screen.Width / 5;
        Naytto2.Y = Screen.Top - 170;
        Naytto2.TextColor = Color.Black;
        Naytto2.Color = Color.Transparent;
        Naytto2.TextScale *= 2;
        Naytto2.Image = elukat[10];
        Add(Naytto2);
    }
}
