//Davista.cs for hobby game projects
//23.4.2018 by Aki Sirviö
using Jypeli;
using Jypeli.Controls;
using Jypeli.Widgets;

//Bible quiz try please!
public class Davista : PhysicsGame
{
    ScoreList tulosLista;
    int pisteet;
    bool vanhat = false;
    bool uusi = false;
    bool psalmit = false;

    //here program starts
    public override void Begin()
    {
        //SetWindowSize(1000, 750);
        Ohjaimet();
        Tausta();
        ParhaatPisteet();
        Painikkeet(0, Screen.Bottom/5, true);
        MediaPlayer.Play("amclassical_prelude");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = 0.5;
    }

    //controls
    void Ohjaimet()
    {
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Mouse.IsCursorVisible = true;
    }

    //load front page's color changing background image
    void Tausta()
    {
        Image[] kuvat = LoadImages("Davista", "Davista2", "Davista3");
        Image taustaKuva = RandomGen.SelectOne(kuvat[0], kuvat[1], kuvat[2]);
        Level.Background.Image = taustaKuva;
        Level.Background.Width = Window.Width;
        Level.Background.Height = Window.Height;
        Level.Background.FadeColorTo(VaihdaVaria(), 3);

        Timer taustaVari = new Timer();
        taustaVari.Interval = 5;
        taustaVari.Timeout += delegate { Level.Background.FadeColorTo(VaihdaVaria(), 3); };
        taustaVari.Start();
    }

    //change color
    Color VaihdaVaria()
    {
        Color vari = RandomGen.NextColor();
        return vari;
    }

    //continue and exit buttons
    void Painikkeet(double height1, double height2, bool both)
    {
        double leveys = Window.Width / 5;
        double korkeus = Window.Height / 10;
        if(both)
        {
            GameObject cont = new GameObject(leveys, korkeus);
            cont.Image = LoadImage("continue");
            cont.Y = height1;
            Add(cont);
            Mouse.ListenOn(cont, MouseButton.Left, ButtonState.Pressed, Alkaa, "");
        }
        GameObject ex = new GameObject(leveys, korkeus);
        ex.Y = height2;
        ex.Image = LoadImage("exit");
        Add(ex);
        Mouse.ListenOn(ex, MouseButton.Left, ButtonState.Pressed, NaytaPisteet, "");
    }

    //load menu page
    void Alkaa()
    {
        ClearAll();
        Ohjaimet();
        AlotusPainikkeet();
        Painikkeet(0, Screen.Bottom/2, false);
        Laskuri();
        Level.Background.Image = LoadImage("Davista quiz");
        Level.Background.Width = Window.Width;
        Level.Background.Height = Window.Height;
    }

    //buttons of menu page
    void AlotusPainikkeet()
    {
        double leveys = Window.Height / 6;
        double korkeus = leveys;
        GameObject OT = new GameObject(leveys, korkeus);
        OT.X = Screen.Left / 3.5;
        OT.Image = LoadImage("OT");
        Add(OT);
        GameObject NT = new GameObject(leveys, korkeus);
        NT.Image = LoadImage("NT");
        Add(NT);
        GameObject PSA = new GameObject(leveys, korkeus);
        PSA.X = Screen.Right / 3.5;
        PSA.Image = LoadImage("PSA");
        Add(PSA);
        SoundEffect not = LoadSoundEffect("NoSound");

        if(!vanhat)
            Mouse.ListenOn(OT, MouseButton.Left, ButtonState.Pressed, delegate { Vanhat(1); vanhat = true; }, "");
        else
            Mouse.ListenOn(OT, MouseButton.Left, ButtonState.Pressed, delegate { not.Play(); }, "");
        if(!uusi)
            Mouse.ListenOn(NT, MouseButton.Left, ButtonState.Pressed, delegate { Uusi(1); uusi = true; }, "");
        else
            Mouse.ListenOn(NT, MouseButton.Left, ButtonState.Pressed, delegate { not.Play(); }, "");
        if(!psalmit)
            Mouse.ListenOn(PSA, MouseButton.Left, ButtonState.Pressed, delegate { Psalmit(1); psalmit = true; }, "");
        else
            Mouse.ListenOn(PSA, MouseButton.Left, ButtonState.Pressed, delegate { not.Play(); }, "");
    }

    //questions of Old Testament
    void Vanhat(int kysymys)
    {
        ClearAll();
        Ohjaimet();
        Laskuri();
        Level.Background.Image = LoadImage("Davista OT");
        Level.Background.Color = Color.Lavender;
        Level.Background.Width = Window.Width;
        Level.Background.Height = Window.Height;
        Label kys = KysymysKentta();
        Label vas1 = VastausKentta(Screen.Left + Screen.Width/5, Screen.Bottom + Screen.Height/3.5);
        Label vas2 = VastausKentta(0, Screen.Bottom + Screen.Height/3.5);
        Label vas3 = VastausKentta(Screen.Right - Screen.Width/5, Screen.Bottom + Screen.Height/3.5);
        kys.TextColor = Color.DarkBlue;
        vas1.BorderColor = Color.DarkViolet;
        vas2.BorderColor = Color.DarkViolet;
        vas3.BorderColor = Color.DarkViolet;
        vas1.TextColor = Color.DarkForestGreen;
        vas2.TextColor = Color.DarkForestGreen;
        vas3.TextColor = Color.DarkForestGreen;

        switch (kysymys)
        {
            case 1:
                {
                    kys.Text = "1. How many books are in the Old Testament?";
                    vas1.Text = "7";
                    vas2.Text = "39";
                    vas3.Text = "66";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys,1); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 2:
                {
                    kys.Text = "2. In how many days did God create the world?";
                    vas1.Text = "6";
                    vas2.Text = "7";
                    vas3.Text = "8";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 1); }, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 3:
                {
                    kys.Text = "3. What was the name of the first human?";
                    vas1.Text = "Eva";
                    vas2.Text = "Adam";
                    vas3.Text = "Noah";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 1); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 4:
                {
                    kys.Text = "4. How old did Methuselah live?";
                    vas1.Text = "152";
                    vas2.Text = "677";
                    vas3.Text = "969";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 1); }, "");
                    break;
                }
            case 5:
                {
                    kys.Text = "5. How many sons did Noah have?";
                    vas1.Text = "1";
                    vas2.Text = "2";
                    vas3.Text = "3";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 1); }, "");
                    break;
                }
            case 6:
                {
                    kys.Text = "6. Which of these are not the descendants of Abraham?";
                    vas1.Text = "Syrians";
                    vas2.Text = "Jewry";
                    vas3.Text = "Asshurim";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 1); }, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 7:
                {
                    kys.Text = "7. How many brothers did David have?";
                    vas1.Text = "7";
                    vas2.Text = "11";
                    vas3.Text = "24";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 1); }, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 8:
                {
                    kys.Text = "8. At what age did David become king?";
                    vas1.Text = "20";
                    vas2.Text = "30";
                    vas3.Text = "40";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 1); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 9:
                {
                    kys.Text = "9. Who prophet annoited Daavid king?";
                    vas1.Text = "Samuel";
                    vas2.Text = "Jeremiah";
                    vas3.Text = "Elijah";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 1); }, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 10:
                {
                    kys.Text = "10. Which is the last book of the Old Testament?";
                    vas1.Text = "Ezekiel";
                    vas2.Text = "2 Chronicles";
                    vas3.Text = "Malachi";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 1); }, "");
                    break;
                }
            case 11:
                {
                    Alkaa();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    //questions of New Testament
    void Uusi(int kysymys)
    {
        ClearAll();
        Ohjaimet();
        Laskuri();
        Level.Background.Image = LoadImage("Davista NT");
        Level.Background.Color = Color.LightGreen;
        Level.Background.Width = Window.Width;
        Level.Background.Height = Window.Height;
        Label kys = KysymysKentta();
        Label vas1 = VastausKentta(Screen.Left + Screen.Width / 5, Screen.Bottom + Screen.Height / 3.5);
        Label vas2 = VastausKentta(0, Screen.Bottom + Screen.Height / 3.5);
        Label vas3 = VastausKentta(Screen.Right - Screen.Width / 5, Screen.Bottom + Screen.Height / 3.5);
        kys.TextColor = Color.Olive;
        vas1.BorderColor = Color.Green;
        vas2.BorderColor = Color.Green;
        vas3.BorderColor = Color.Green;
        vas1.TextColor = Color.DarkBlue;
        vas2.TextColor = Color.DarkBlue;
        vas3.TextColor = Color.DarkBlue;

        switch (kysymys)
        {
            case 1:
                {
                    kys.Text = "1. How many books are in the New Testament?";
                    vas1.Text = "4";
                    vas2.Text = "12";
                    vas3.Text = "27";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    break;
                }
            case 2:
                {
                    kys.Text = "2. How many Paul's letters are in the New Testament?";
                    vas1.Text = "4";
                    vas2.Text = "13";
                    vas3.Text = "21";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 3:
                {
                    kys.Text = "3. In what place was Jesus born?";
                    vas1.Text = "Nazareth";
                    vas2.Text = "Jerusalem";
                    vas3.Text = "Bethlehem";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    break;
                }
            case 4:
                {
                    kys.Text = "4. At what age did Jesus begin to preach about the kingdom of God?";
                    vas1.Text = "12";
                    vas2.Text = "30";
                    vas3.Text = "45";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    kys.TextScale *= 0.85;
                    break;
                }
            case 5:
                {
                    kys.Text = "5. How many disciples did Jesus have?";
                    vas1.Text = "12";
                    vas2.Text = "25";
                    vas3.Text = "5000";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 6:
                {
                    kys.Text = "6. At what age did Jesus die?";
                    vas1.Text = "33";
                    vas2.Text = "58";
                    vas3.Text = "170";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 7:
                {
                    kys.Text = "7. In how many days did Jeesus rise from the dead?";
                    vas1.Text = "1";
                    vas2.Text = "3";
                    vas3.Text = "7";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 8:
                {
                    kys.Text = "8. How many missions did Paul do?";
                    vas1.Text = "1";
                    vas2.Text = "3";
                    vas3.Text = "5";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 9:
                {
                    kys.Text = "9. What is the most famous verse of the New Testament?";
                    vas1.Text = "Matt 3:16";
                    vas2.Text = "John 3:16";
                    vas3.Text = "Acts 3:16";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 10:
                {
                    kys.Text = "10. Which is the last book of the New Testament?";
                    vas1.Text = "John";
                    vas2.Text = "Acts";
                    vas3.Text = "Revelation";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 2); }, "");
                    break;
                }
            case 11:
                {
                    Alkaa();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    //questions of Psalms
    void Psalmit(int kysymys)
    {
        ClearAll();
        Ohjaimet();
        Laskuri();
        Level.Background.Image = LoadImage("Davista PSA");
        Level.Background.Color = Color.LightBlue;
        Level.Background.Width = Window.Width;
        Level.Background.Height = Window.Height;
        Label kys = KysymysKentta();
        Label vas1 = VastausKentta(Screen.Left + Screen.Width / 5, Screen.Bottom + Screen.Height / 3.5);
        Label vas2 = VastausKentta(0, Screen.Bottom + Screen.Height / 3.5);
        Label vas3 = VastausKentta(Screen.Right - Screen.Width / 5, Screen.Bottom + Screen.Height / 3.5);
        kys.TextColor = Color.DarkViolet;
        vas1.BorderColor = Color.DarkCyan;
        vas2.BorderColor = Color.DarkCyan;
        vas3.BorderColor = Color.DarkCyan;
        vas1.TextColor = Color.DarkOrange;
        vas2.TextColor = Color.DarkOrange;
        vas3.TextColor = Color.DarkOrange;

        switch (kysymys)
        {
            case 1:
                {
                    kys.Text = "1. How many books are the Psalms in the Bible?";
                    vas1.Text = "5";
                    vas2.Text = "19";
                    vas3.Text = "24";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 2:
                {
                    kys.Text = "2. How many verses are in the Psalmes book?";
                    vas1.Text = "150";
                    vas2.Text = "1724";
                    vas3.Text = "2526";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    break;
                }
            case 3:
                {
                    kys.Text = "3. Who has written the most Psalms?";
                    vas1.Text = "Jesus";
                    vas2.Text = "Moses";
                    vas3.Text = "David";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    break;
                }
            case 4:
                {
                    kys.Text = "4. How does Psalm 23 begin?";
                    vas1.Text = "The Lord is my shepherd";
                    vas2.Text = "The Mighty One, God, the Lord, speaks";
                    vas3.Text = "The Lord reigns, let the earth be glad";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    vas1.Y += 100;
                    vas3.Y -= 100;
                    vas1.X = 0;
                    vas3.X = 0;
                    break;
                }
            case 5:
                {
                    kys.Text = "5. Which is the most famous Psalm?";
                    vas1.Text = "Psalm 119";
                    vas2.Text = "Psalm 50";
                    vas3.Text = "Psalm 23";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    break;
                }
            case 6:
                {
                    kys.Text = "6. Which Psalm is the shortest chapter of Bible?";
                    vas1.Text = "Psalm 7";
                    vas2.Text = "Psalm 71";
                    vas3.Text = "Psalm 117";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    break;
                }
            case 7:
                {
                    kys.Text = "7. Which Psalm is the longest chapter of Bible?";
                    vas1.Text = "Psalm 9";
                    vas2.Text = "Psalm 90";
                    vas3.Text = "Psalm 119";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    break;
                }
            case 8:
                {
                    kys.Text = "8. What Psalm does Jesus quote on the cross?";
                    vas1.Text = "Psalm 1";
                    vas2.Text = "Psalm 22";
                    vas3.Text = "Psalm 150";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    break;
                }
            case 9:
                {
                    kys.Text = "9. What did Jesus say on the cross?";
                    vas1.Text = "My God, my God, why have you forsaken me?";
                    vas2.Text = "Do not be far from me";
                    vas3.Text = "I can count all my bones";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    vas1.Y += 100;
                    vas3.Y -= 100;
                    vas1.X = 0;
                    vas3.X = 0;
                    break;
                }
            case 10:
                {
                    kys.Text = "10. How does the last Psalm begin?";
                    vas1.Text = "Praise the Lord";
                    vas2.Text = "He who dwells in the shelter";
                    vas3.Text = "I love you, O Lord";
                    Mouse.ListenOn(vas1, MouseButton.Left, ButtonState.Pressed, delegate { Oikein(kysymys, 3); }, "");
                    Mouse.ListenOn(vas2, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    Mouse.ListenOn(vas3, MouseButton.Left, ButtonState.Pressed, Vaarin, "");
                    vas1.Y += 100;
                    vas3.Y -= 100;
                    vas1.X = 0;
                    vas3.X = 0;
                    break;
                }
            case 11:
                {
                    Alkaa();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    //load question label
    Label KysymysKentta()
    {
        double koko = Window.Width / 800;
        Label kys = new Label();
        kys.Font = Font.DefaultLargeBold;
        kys.TextScale *= koko;
        Add(kys);

        return kys;
    }

    //load answer labels
    Label VastausKentta(double leveys, double korkeus)
    {
        double koko = Window.Width / 800;
        Label vas = new Label();
        vas.Font = Font.DefaultLarge;
        vas.XMargin = 10;
        vas.YMargin = 10;
        vas.TextScale *= koko;
        vas.X = leveys;
        vas.Y = korkeus;
        Add(vas);

        return vas;
    }

    //wrong answer
    void Vaarin()
    {
        ClearAll();
        Painikkeet(Screen.Bottom/1.7, Screen.Bottom/1.3,true);
        SoundEffect wrong = LoadSoundEffect("wrongSound");
        wrong.Play();
        Level.Background.Image = LoadImage("Davista quiz");
        Level.Background.Color = Color.RosePink;
        Level.Background.Width = Window.Width;
        Level.Background.Height = Window.Height;
        GameObject vaarin = new GameObject(Window.Height / 2, Window.Height / 2);
        vaarin.Image = LoadImage("Wrong");
        Add(vaarin, 3);
    }

    //right answer
    void Oikein(int kysymys, int sarja)
    {
        ClearAll();
        pisteet++;
        Level.Background.Image = LoadImage("Davista quiz");
        Level.Background.Color = Color.YellowGreen;
        Level.Background.Width = Window.Width;
        Level.Background.Height = Window.Height;
        GameObject oikein = new GameObject(Window.Height/2, Window.Height/2);
        oikein.Image = LoadImage("Right");
        oikein.Y -= Window.Height / 20;
        Add(oikein,3);
        Timer.SingleShot(1.5, delegate { SeuraavaKysymys(kysymys, sarja); });
    }

    //next question
    void SeuraavaKysymys(int kysymys, int sarja)
    {
        kysymys++;
        if (sarja == 1)
        {
            Vanhat(kysymys);
        }
        else if (sarja == 2)
        {
            Uusi(kysymys);
        }
        else if (sarja == 3)
        {
            Psalmit(kysymys);
        }
    }

    //counter
    void Laskuri()
    {
        double koko = Window.Width / 800;
        IntMeter pisteLaskuri = new IntMeter(pisteet);
        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + Window.Width/7;
        pisteNaytto.Y = Screen.Top - Window.Height/2.7;
        pisteNaytto.TextColor = Color.DarkViolet;
        pisteNaytto.Color = Color.Transparent;
        pisteNaytto.Font = Font.DefaultLarge;
        pisteNaytto.TextScale *= koko;
        pisteNaytto.Title = "Score";
        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }

    //add top ten score list
    void ParhaatPisteet()
    {
        string tiedosto = "pisteet.xml";
        tulosLista = new ScoreList(10, false, 1);
        tulosLista = DataStorage.TryLoad<ScoreList>(tulosLista, tiedosto);
    }

    //show top ten score window
    void NaytaPisteet()
    {
        HighScoreWindow topIkkuna = new HighScoreWindow(
                              "",
                              "",
                              tulosLista, pisteet);
        topIkkuna.Closed += TallennaPisteet;
        topIkkuna.Image = LoadImage("Davista score");
        topIkkuna.Layout.RightPadding = 50;
        topIkkuna.Layout.LeftPadding = 50;
        topIkkuna.Height = 500;
        topIkkuna.Width = 350;
        topIkkuna.SizingByLayout = false;
        topIkkuna.OKButton.Image = LoadImage("OK");
        topIkkuna.OKButton.Text = "";
        topIkkuna.NameInputWindow.Image = LoadImage("Davista name");
        topIkkuna.NameInputWindow.Layout.RightPadding = 70;
        topIkkuna.NameInputWindow.Layout.LeftPadding = 50;
        topIkkuna.NameInputWindow.Layout.BottomPadding = 70;
        topIkkuna.NameInputWindow.Layout.TopPadding = 70;
        topIkkuna.NameInputWindow.OKButton.Image = LoadImage("OK");
        topIkkuna.NameInputWindow.OKButton.Text = "";
        Add(topIkkuna);
    }

    //save score lists
    void TallennaPisteet(Window sender)
    {
        string tiedosto = "pisteet.xml";
        DataStorage.Save<ScoreList>(tulosLista, tiedosto);
        Exit();
    }
}
