//MathDraw.cs 6.2.2019 by Aki Sirviö
//example how to use Math library
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

// this program draws figures using Math library and Jypeli picture features.
public class MathDraw : Game
{
    // program starts
    public override void Begin()
    {
        Image paper = MakePaper();
        DrawFigure(paper);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    // make canvas
    Image MakePaper()
    {
        Image paperi = new Image(Level.Right * 2, Level.Top * 2, Color.White);
        Level.Background.Image = paperi;
        Level.Background.FitToLevel();

        return paperi;
    }

    // draw figures: Epitrochoids, Hypocycloids, Hypotrochoids, Farris wheels, Lissajous curves, Rose curves
    void DrawFigure(Image paperi)
    {
        double centreX = Level.Right;
        double centreY = Level.Top;

        for (double i = 0; i < 3 * Math.PI; i += 0.01)
        {
            double x = 100 * Math.Cos(4 * i + 1.57) * Math.Cos(i) + centreX;
            double y = 100 * Math.Cos(4 * i) * Math.Sin(i) + centreY;

            paperi[(int)y, (int)x] = Color.Blue;

            for (int l = 80; l < 100; l++)
            {
                x = l * Math.Cos(4 * i) * Math.Cos(i) + centreX;
                y = l * Math.Cos(4 * i) * Math.Sin(i) + centreY;
                paperi[(int)y, (int)x] = Color.BlueGray;
            }

            for (int l = 80; l < 100; l++)
            {
                x = l * Math.Cos(4 * i + 1.57) * Math.Cos(i) + 200;
                y = l * Math.Cos(4 * i) * Math.Sin(i) + 150;
                paperi[(int)y, (int)x] = Color.ForestGreen;
            }

            for (int l = 80; l < 100; l++)
            {
                x = l * Math.Cos(3 * i + 5) * Math.Cos(i) + 500;
                y = l * Math.Cos(3 * i) * Math.Sin(i) + 150;
                paperi[(int)y, (int)x] = Color.OrangeRed;
            }

            for (int l = 80; l < 100; l++)
            {
                x = l * Math.Cos(3 * i) * Math.Cos(i) + 750;
                y = l * Math.Cos(3 * i) * Math.Sin(i) + 150;
                paperi[(int)y, (int)x] = Color.PaintDotNetBlue;
            }

            for (int l = 80; l < 100; l++)
            {
                x = l * Math.Cos(3 * i + 2) * Math.Cos(3 * i) * Math.Cos(i) + 200;
                y = l * Math.Cos(3 * i + 2) * Math.Cos(3 * i) * Math.Sin(i) + 400;
                paperi[(int)y, (int)x] = Color.Purple;
            }

            for (int l = 80; l < 100; l++)
            {
                x = l * Math.Sin(3 * i) * Math.Sin(7 * i) * Math.Sin(i * 1) + 750;
                y = l * Math.Cos(3 * i) * Math.Cos(7 * i) * Math.Cos(i * 1) + 400;
                paperi[(int)y, (int)x] = Color.Maroon;
            }

            for (int l = 10; l < 20; l++)
            {
                x = (l * 7 * Math.Cos(6 * i + 2) - (l * Math.Cos(6 * i + 1))) * Math.Cos(i) + 500;
                y = (l * 7 * Math.Sin(6 * i + 2) - (l * Math.Sin(6 * i + 1))) * Math.Sin(i) + 650;
                paperi[(int)y, (int)x] = Color.Rose;
            }

            for (int l = 10; l < 20; l++)
            {
                x = l + ((70 - 30) * Math.Cos(i)) + (70 * Math.Cos(((70 - 30) / 30) * i)) * Math.Cos(i) + 150;
                y = l + ((70 - 30) * Math.Sin(i)) - (70 * Math.Sin(((70 - 30) / 30) * i)) * Math.Sin(i) + 650;
                paperi[(int)y, (int)x] = Color.Aqua;
            }

            for (int l = 10; l < 20; l++)
            {
                x = l + (50 * Math.Cos(i)) + (24 * Math.Cos(i * (50 / 15))) + 750;
                y = l + (50 * Math.Sin(i)) - (24 * Math.Sin(i * (50 / 15))) + 650;
                paperi[(int)y, (int)x] = Color.GreenYellow;
            }
        }
    }
}
