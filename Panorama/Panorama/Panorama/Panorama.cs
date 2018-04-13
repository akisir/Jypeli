//Panoroma.cs for hobby projects
//12.4.2018 by Aki Sirviö
using System.Collections.Generic;
using Jypeli;
using Jypeli.Controls;
using Jypeli.Widgets;

//Show panorama nice way. F1 for help
public class Panorama : PhysicsGame
{
    double scrollausnopeus = -0.5;
    List<GameObject> taustakuvat;
    Timer taustaAjastin = new Timer();
    GameObject vikaTaustakuva;
    GameObject teksti;
    double y = 0;
    int vika = 0;
    bool suunta = false;

    //program starts from here
    public override void Begin()
    {
        LisaaTeksti();
        LuoTaustakuvat();
        Ohjaimet();
        
        Level.Background.CreateStars();
        Camera.Zoom(0.2);
        Timer.SingleShot(0.1, delegate{ Zoomaa(1.003, true); });
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

        if(scrollausnopeus < 0)
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
        Keyboard.Listen(Key.Down, ButtonState.Down, Korkeus, "Move camera down", 1.0);
        Keyboard.Listen(Key.Space, ButtonState.Pressed, Vaihto, "Change scrolling direction");
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Show help");
        Keyboard.Listen(Key.Up, ButtonState.Down, Korkeus, "Move camera up", -1.0);
        Keyboard.Listen(Key.A, ButtonState.Down, Zoomaa, "Zoom closer", 1.001, false);
        Keyboard.Listen(Key.D, ButtonState.Down, Zoomaa, "Zoom farther", 0.999, false);
        Keyboard.Listen(Key.Left, ButtonState.Down, MuutaNopeutta, "Increase speed to left", 0.1);
        Keyboard.Listen(Key.Right, ButtonState.Down, MuutaNopeutta, "Increase speed to right", -0.1);
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
            olio.Left = Screen.Left;
        }
        taustakuvat.Add(olio);
    }

    //add text
    void LisaaTeksti()
    {
        teksti = new GameObject(500,70);
        teksti.Y = Screen.Top - 200;
        teksti.Image = LoadImage("teksti");
        Add(teksti,3);
    }

    //move background pictures
    void LiikutaTaustaa()
    {
        int i = 0;
        foreach (GameObject taustakuva in taustakuvat)
        {
            taustakuva.X += scrollausnopeus;
            taustakuva.Y = y;

            if (scrollausnopeus < 0 && taustakuva.Right < (Screen.Left))
            {
                vika = i;
                MessageDisplay.Add("" + vika);
                taustakuva.Left = vikaTaustakuva.Right;
                vikaTaustakuva = taustakuva;
                if(taustakuva.Tag.ToString() == "panorama51")
                {
                    vikaTaustakuva.X += scrollausnopeus;
                }
            }
            else if (scrollausnopeus > 0 && taustakuva.Left > (Screen.Right))
            {
                vika = i;
                MessageDisplay.Add("" + vika);
                taustakuva.Right = vikaTaustakuva.Left;
                vikaTaustakuva = taustakuva;
                vikaTaustakuva.X += scrollausnopeus;
            }
            i++;
        }

        LiikutaTekstia();
    }

    //move text
    void LiikutaTekstia()
    {
        teksti.X += scrollausnopeus / 3.0;

        if(teksti.X>3500)
        {
            teksti.X = -2500;
        }
        else if(teksti.X < -3500)
        {
            teksti.X = 2500;
        }
    }

    //change camera height
    void Korkeus(double maara)
    {
        double korkeus = y + maara;
        if(korkeus>-217 && korkeus<200)
        {
            y = korkeus;
        }
    }

    //change scrolling speed
    void MuutaNopeutta(double muutos)
    {
        scrollausnopeus += muutos;
        if(scrollausnopeus <= 0 && suunta)
        {
            KaannaSuunta();
            suunta = false;
        }
        if(scrollausnopeus >= 0 && !suunta)
        {
            KaannaSuunta();
            suunta = true;
        }
    }

    //camera zoom
    void Zoomaa(double maara, bool ohita)
    {
        double zoomi = Camera.ZoomFactor * maara;
        if((zoomi >= 1 && zoomi <= 3) || ohita)
        {
            Camera.Zoom(maara);
        }
        if(ohita)
        {
            if(zoomi < 1.2)
            {
                Timer.SingleShot(0.01, delegate{ Zoomaa(1.003, true); });
            }
            else
            {
                ShowControlHelp();
            }
        }
    }

    //change scrolling direction
    void Vaihto()
    {
        scrollausnopeus *= -1;
        KaannaSuunta();
    }

    //makes direction change possible
    void KaannaSuunta()
    {
        if(scrollausnopeus > 0)
        {
            if(vika==3)
            {
                taustakuvat[3].Right = taustakuvat[0].Left;
                taustakuvat[2].Right = taustakuvat[3].Left;
                vikaTaustakuva = taustakuvat[--vika];
            }
            else
            {
                taustakuvat[vika].Right = taustakuvat[vika+1].Left;
                if((vika-1)<0)
                {
                    vika = 3;
                    taustakuvat[3].Right = taustakuvat[0].Left;
                    vikaTaustakuva = taustakuvat[vika];
                }
                else
                {
                    vika--;
                    taustakuvat[vika].Right = taustakuvat[vika + 1].Left;
                    vikaTaustakuva = taustakuvat[vika];
                }
            }
        }
        else
        {
            if (vika == 0)
            {
                taustakuvat[0].Left = taustakuvat[3].Right;
                taustakuvat[1].Left = taustakuvat[0].Right;
                vikaTaustakuva = taustakuvat[++vika];
            }
            else
            {
                taustakuvat[vika].Left = taustakuvat[vika - 1].Right;
                if ((vika + 1) >= taustakuvat.Count)
                {
                    vika = 0;
                    taustakuvat[0].Left = taustakuvat[3].Right;
                    vikaTaustakuva = taustakuvat[vika];
                }
                else
                {
                    vika++;
                    taustakuvat[vika].Left = taustakuvat[vika - 1].Right;
                    vikaTaustakuva = taustakuvat[vika];
                }
            }
        }
    }
}

