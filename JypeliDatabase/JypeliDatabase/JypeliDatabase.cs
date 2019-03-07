//JypeliDatabase.cs 7.3.2019
//by Aki Sirviö
using Jypeli;
using System.Drawing;

//Game class included functions of game.
//Game starts when user login data is checked.
namespace JypeliDatabase
{
    public class JypeliDatabase : PhysicsGame
    {
        private Form1 form;

        // starts here
        public override void Begin()
        {
            Timer.SingleShot(1, OpenForm);

            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "End game");
        }

        // calls Form1 class that makes login window. Export JypeliDatabase class as parameter.
        private void OpenForm()
        {
            form = new Form1(this);
            form.Show();
        }

        // start game, called Form1 class
        public void Play()
        {
            Level.CreateBorders();
            Level.Background.Color = Jypeli.Color.White;
            Bitmap kuva = form.CharacterImage;
            Jypeli.Image img = Bmp2Image(kuva);

            GameObject hahmo = new GameObject(75, 75);
            hahmo.X = Screen.Left + 65;
            hahmo.Y = Screen.Top - 50;
            if (kuva != null)
            {
                hahmo.Image = img;
            }
            Add(hahmo);

            PhysicsObject pelaaja = new PhysicsObject(150, 150);
            pelaaja.Shape = Shape.Circle;
            pelaaja.Color = Jypeli.Color.LightBlue;
            if (kuva != null)
            {
                pelaaja.Image = img;
            }
            Add(pelaaja);

            pelaaja.Hit(new Vector(150, 100));

            for(int i=0; i<50; i++)
            {
                AddSquare(img);
            }
        }

        // add square
        void AddSquare(Jypeli.Image img)
        {
            PhysicsObject square = new PhysicsObject(20, 20);
            square.Color = Jypeli.Color.Aquamarine;
            square.Restitution = 1;
            square.Image = img;
            Add(square);
            Vector force = RandomGen.NextVector(100, 500);
            square.Hit(force);
        }

        // converts bitmap image to jypeli image.
        private Jypeli.Image Bmp2Image(Bitmap img)
        {
            Jypeli.Image newImg = new Jypeli.Image(img.Width, img.Height, Jypeli.Color.Azure);
            uint[,] table = new uint[img.Height, img.Width];
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    System.Drawing.Color vari = img.GetPixel(x, y);
                    uint variUint = (uint)vari.ToArgb();
                    table[y, x] = variUint;
                }
            }
            newImg.SetData(table);

            return newImg;
        }
    }
}
