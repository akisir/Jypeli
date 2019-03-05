//DrawLines.cs 5.3.2019
//by Aki Sirviö
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

// here are a few different ways to draw line.
// selection are made from 1, 2 and 3 keys.
// the line is drawn with mouse. Essence of drawing is found on Viiva()-method.
public class DrawLines : Game
{
    private Image paperi;
    private double xPoint;
    private double yPoint;

    // starts from here
    public override void Begin()
    {
        SetWindowSize(1300, 1000);
        PiirtoAlusta();
        Ohjaimet();
        ListenDown();

        xPoint = paperi.Width / 2;
        yPoint = paperi.Height / 2;
    }

    //canvas
    Image PiirtoAlusta()
    {
        paperi = new Image(Level.Right * 2, Level.Top * 2, Color.White);
        Level.Background.Image = paperi;
        Level.Background.FitToLevel();

        return paperi;
    }

    // controls
    void Ohjaimet()
    {
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.D1, ButtonState.Pressed, ListenPress, "Piirtää kun hiiren nappia klikataan");
        Keyboard.Listen(Key.D2, ButtonState.Pressed, ListenDown, "Piirtää kun hiiren nappi on pohjassa");
        Keyboard.Listen(Key.D3, ButtonState.Pressed, ListenMove, "Piirtää kun liikutetaan hiirtä");
        IsMouseVisible = true;
    }

    // draw line with every click (D1)
    void ListenPress()
    {
        Mouse.DisableAll();
        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, PiirraViiva, "Piirrä viiva");
        Mouse.Listen(MouseButton.Right, ButtonState.Pressed, MerkkaaPiste, "Merkkaa nollapisteen");
    }

    // draw line when mouse button is down (D2)
    void ListenDown()
    {
        Mouse.DisableAll();
        Mouse.Listen(MouseButton.Left, ButtonState.Down, PiirraViiva, "Piirrä viiva");
        Mouse.Listen(MouseButton.Right, ButtonState.Pressed, MerkkaaPiste, "Merkkaa nollapisteen");
    }

    // draw line as mouse moves (D3)
    void ListenMove()
    {
        Mouse.DisableAll();
        Mouse.ListenMovement(0.15, PiirraViiva, "Piirra viiva");
        Mouse.Listen(MouseButton.Right, ButtonState.Pressed, MerkkaaPiste, "Merkkaa nollapisteen");
    }

    // start point of line
    void MerkkaaPiste()
    {
        xPoint = Mouse.PositionOnScreen.X + Level.Right;
        yPoint = Level.Top - Mouse.PositionOnScreen.Y;
        TarkistaPiste();
    }

    // if Ctrl key is down draws continuous line
    void PiirraViiva()
    {
        Viiva();

        if(Keyboard.IsCtrlDown())
        {
            xPoint = Mouse.PositionOnScreen.X + Level.Right;
            yPoint = Level.Top - Mouse.PositionOnScreen.Y;
        }

        TarkistaPiste();
    }

    // check if start poins are outside canvas
    void TarkistaPiste()
    {
        if (xPoint < 10)
            xPoint = 10;
        if (yPoint < 10)
            yPoint = 10;
        if (xPoint > paperi.Width - 10)
            xPoint = paperi.Width - 10;
        if (yPoint > paperi.Height - 10)
            yPoint = paperi.Height - 10;
    }

    // core of drawing
    void Viiva()
    {
        double centreX = Level.Right;
        double centreY = Level.Top;
        int wd = 3;
        int x0 = (int)xPoint;
        int y0 = (int)yPoint;
        int x1 = (int)(Mouse.PositionOnScreen.X + centreX);
        int y1 = (int)(centreY - Mouse.PositionOnScreen.Y);
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx - dy, e2, x2, y2;
        double ed = dx + dy == 0 ? 1 : Math.Sqrt(dx * dx + dy * dy);
        Color pc;
        int alfa = 0;

        // check if end points are outside of canvas
        if (x1 < 10)
            x1 = 10;
        if (y1 < 10)
            y1 = 10;
        if (x1 > paperi.Width - 10)
            x1 = paperi.Width - 10;
        if (y1 > paperi.Height - 10)
            y1 = paperi.Height - 10;

        for (wd = (wd + 1) / 2; ;)
        {
            alfa = (int)(255 - (Math.Max(0, 255 * (Math.Abs(err - dx + dy) / ed - wd + 1))));
            pc = new Color(43, 39, 67, alfa);
            paperi[y0, x0] = pc;
            e2 = err; x2 = x0;
            if (2 * e2 >= -dx)
            {
                for (e2 += dy, y2 = y0; e2 < ed * wd && (y1 != y2 || dx > dy); e2 += dx)
                {
                    alfa = (int)(255 - (Math.Max(0, 255 * (Math.Abs(e2) / ed - wd + 1))));
                    pc = new Color(43, 39, 67, alfa);
                    paperi[y2 += sy, x0] = pc;
                }

                if (x0 == x1) break;
                e2 = err; err -= dy; x0 += sx;
            }
            if (2 * e2 <= dy)
            {
                for (e2 = dx - e2; e2 < ed * wd && (x1 != x2 || dx < dy); e2 += dy)
                {
                    alfa = (int)(255 - (Math.Max(0, 255 * (Math.Abs(e2) / ed - wd + 1))));
                    pc = new Color(43, 39, 67, alfa);
                    paperi[y0, x2 += sx] = pc;
                }

                if (y0 == y1) break;
                err += dx; y0 += sy;
            }
        }
    }
}
