//ImageProcessing.cs by Aki Sirviö
//Image processing with Jypeli library
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

// image prosessing program that edits image colors.
public class ImageProcessing : Game
{
    private int choose = 0;
    private Image backImage;

    // start program
    public override void Begin()
    {
        Controls();
        ChooseImage();
        ProcTimers();
        BackgroundImage(backImage);
        Camera.ZoomFactor = 1.2;
    }

    // load background image
    void BackgroundImage(Image img)
    {
        Level.Background.Image = img;
        Level.Background.FitToLevel();
    }

    // load keyboard controls
    void Controls()
    {
        PhoneBackButton.Listen(ConfirmExit, "End program");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "End program");
        Keyboard.Listen(Key.Space, ButtonState.Pressed, ChooseImage, "Change background image");
    }

    // choose random image for background
    void ChooseImage()
    {
        string sImage = RandomGen.SelectOne("Banaue", "Bayombong2", "BigPlane", "KuvaL", "KuvaP");
        backImage = LoadImage(sImage);
    }

    // setup timer for different image processing
    void ProcTimers()
    {
        Timer process = new Timer();
        process.Interval = 1;
        process.Timeout += ChooseProcess;
        process.Start();
    }

    // choose image processing
    void ChooseProcess()
    {
        choose++;
        switch(choose)
        {
            case 1:
                Grayscale(backImage);
                break;
            case 2:
                Redscale(backImage);
                break;
            case 3:
                Greenscale(backImage);
                break;
            case 4:
                Bluescale(backImage);
                break;
            case 5:
                ColorChange(backImage);
                break;
            case 6:
                RestlessImage(backImage);
                break;
            case 7:
                Timer.SingleShot(2, delegate { BackgroundImage(backImage); });
                choose = -2;
                break;
        }
    }

    // make grayscale image
    void Grayscale(Image kuva)
    {
        Image grayImage = kuva.Clone();
        Color[,] bmp = grayImage.GetData();
        int ny = grayImage.Height;
        int nx = grayImage.Width;

        for (int iy = 0; iy < ny; iy++)
        {
            for (int ix = 0; ix < nx; ix++)
            {
                Color c = bmp[iy, ix];
                byte b = (byte)((c.RedComponent +
                                 c.GreenComponent +
                                 c.BlueComponent) / 3);
                bmp[iy, ix] = new Color(b, b, b);
            }
        }
        grayImage.SetData(bmp);
        BackgroundImage(grayImage);
    }

    // make red shaded image
    void Redscale(Image kuva)
    {
        Image redImage = kuva.Clone();
        uint[,] bmp = redImage.GetDataUInt();
        int ny = redImage.Height;
        int nx = redImage.Width;

        for (int iy = 0; iy < ny; iy++)
        {
            for (int ix = 0; ix < nx; ix++)
            {
                uint c = bmp[iy, ix];
                byte r = Color.GetRed(c);
                byte g = 50;
                byte b = 50;
                bmp[iy, ix] = Color.PackRGB(r, g, b);
            }
        }
        redImage.SetData(bmp);
        BackgroundImage(redImage);
    }

    // make green shaded image
    void Greenscale(Image kuva)
    {
        Image greenImage = kuva.Clone();
        uint[,] bmp = greenImage.GetDataUInt();
        int ny = greenImage.Height;
        int nx = greenImage.Width;

        for (int iy = 0; iy < ny; iy++)
        {
            for (int ix = 0; ix < nx; ix++)
            {
                uint c = bmp[iy, ix];
                byte r = 0;
                byte g = Color.GetGreen(c);
                byte b = 0;
                bmp[iy, ix] = Color.PackRGB(r, g, b);
            }
        }
        greenImage.SetData(bmp);
        BackgroundImage(greenImage);
    }

    // make blue shaded image
    void Bluescale(Image kuva)
    {
        Image blueImage = kuva.Clone();
        uint[,] bmp = blueImage.GetDataUInt();
        int ny = blueImage.Height;
        int nx = blueImage.Width;

        for (int iy = 0; iy < ny; iy++)
        {
            for (int ix = 0; ix < nx; ix++)
            {
                uint c = bmp[iy, ix];
                byte r = Color.GetRed(c / 5);
                byte g = 70;
                byte b = Color.GetBlue(c);
                bmp[iy, ix] = Color.PackRGB(r, g, b);
            }
        }
        blueImage.SetData(bmp);
        BackgroundImage(blueImage);
    }

    // make image with white light colored
    void ColorChange(Image kuva)
    {
        Image whiteImage = kuva.Clone();
        Color[,] bmp = whiteImage.GetData();

        for (int x = 1; x < whiteImage.Width; x++)
        {
            for (int y = 1; y < whiteImage.Height; y++)
            {
                Color col = bmp[y, x];
                byte r = col.RedComponent;
                byte g = col.GreenComponent;
                byte b = col.BlueComponent;
                byte red = (byte)(255 - (0.393 * r + 0.269 * g + 0.189 * b));
                byte green = (byte)(255 - (0.349 * r + 0.486 * g + 0.168 * b));
                byte blue = (byte)(255 - (0.272 * r + 0.134 * g + 0.131 * b));
                bmp[y, x] = new Color(red, green, blue);
            }
        }
        whiteImage.SetData(bmp);
        BackgroundImage(whiteImage);
    }

    // make restless image
    void RestlessImage(Image kuva)
    {
        Image redImage = kuva.Clone();
        uint[,] bmp = redImage.GetDataUInt();
        int ny = redImage.Height;
        int nx = redImage.Width;

        for (int iy = 0; iy < ny; iy++)
        {
            for (int ix = 0; ix < nx; ix++)
            {
                uint c = bmp[iy, ix];
                byte r = Color.GetRed(c);
                byte g = Color.GetGreen(c / 2);
                byte b = Color.GetBlue(c / 2);
                bmp[iy, ix] = Color.PackRGB(r, g, b);
            }
        }
        redImage.SetData(bmp);
        BackgroundImage(redImage);
    }
}
