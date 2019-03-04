//JypeliWinComponent 4.3.2019
//by Aki Sirviö
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

using System.Windows.Forms;
using System.Drawing;

// on this class Windows components:
// two buttons two labels, textbox and picturebox are added
// to Jypeli game.
public class JypeliWinComponent : PhysicsGame
{
    PictureBox pictureBox1;
    TextBox text1;
    Jypeli.Label tekstikentta;
    List<PhysicsObject> neliot = new List<PhysicsObject>();

    // starts here with a few method calls
    public override void Begin()
    {
        AlkuAsetukset();
        LisaaKomponentit();
        LisaaTekstikentta();

        for(int i=0; i<50; i++)
        {
            LisaaNelio();
        }
    }

    // initial settings
    void AlkuAsetukset()
    {
        Mouse.IsCursorVisible = true;
        Window.AllowUserResizing = true;
        Screen.Width = 500;
        Screen.Height = 500;
        Window.Title = "GAME Copyright© Aki Sirviö";
        Camera.ZoomToLevel();
        Level.CreateBorders(1, true);
        Keyboard.Listen(Key.Escape, Jypeli.ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    // add windows components
    void LisaaKomponentit()
    {
        System.Windows.Forms.Button nappi1 = new System.Windows.Forms.Button();
        nappi1.Location = new Point(4, 10);
        nappi1.Size = new Size(240, 51);
        nappi1.Text = "Avaa kuvahaku!";
        nappi1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
        nappi1.UseVisualStyleBackColor = true;
        nappi1.Click += new EventHandler(KuvaHaku);
        Control.FromHandle(Window.Handle).Controls.Add(nappi1);

        System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
        label2.AutoSize = true;
        label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
        label2.Location = new Point(5, 70);
        label2.Size = new Size(76, 31);
        label2.Text = "PictureBox:";
        label2.BackColor = System.Drawing.Color.Black;
        label2.ForeColor = System.Drawing.Color.White;
        Control.FromHandle(Window.Handle).Controls.Add(label2);

        pictureBox1 = new PictureBox();
        pictureBox1.BorderStyle = BorderStyle.FixedSingle;
        pictureBox1.Location = new Point(5, 90);
        pictureBox1.Size = new Size(240, 200);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        Control.FromHandle(Window.Handle).Controls.Add(pictureBox1);

        System.Windows.Forms.Label label1 = new System.Windows.Forms.Label();
        label1.AutoSize = true;
        label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, FontStyle.Regular, GraphicsUnit.Point, 0);
        label1.Location = new Point(310, 10);
        label1.Size = new Size(76, 31);
        label1.Text = "Kirjoita tähän:";
        label1.BackColor = System.Drawing.Color.Black;
        label1.ForeColor = System.Drawing.Color.White;
        Control.FromHandle(Window.Handle).Controls.Add(label1);

        text1 = new TextBox();
        text1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, FontStyle.Regular, GraphicsUnit.Point, 0);
        text1.Location = new Point(310, 40);
        text1.BorderStyle = BorderStyle.None;
        text1.Multiline = true;
        text1.Size = new Size(250, 70);
        Control.FromHandle(Window.Handle).Controls.Add(text1);

        System.Windows.Forms.Button nappi2 = new System.Windows.Forms.Button();
        nappi2.Location = new Point(590, 10);
        nappi2.Size = new Size(350, 51);
        nappi2.Text = "Lisää teksti pelikentälle..";
        nappi2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, FontStyle.Regular, GraphicsUnit.Point, 0);
        nappi2.UseVisualStyleBackColor = true;
        nappi2.Click += new EventHandler(LisaaTeksti);
        Control.FromHandle(Window.Handle).Controls.Add(nappi2);
    }

    // add squares
    void LisaaNelio()
    {
        PhysicsObject nelio = new PhysicsObject(20, 20);
        nelio.Color = Jypeli.Color.Aquamarine;
        nelio.Restitution = 1;
        Add(nelio);
        Vector voima = RandomGen.NextVector(100, 500);
        nelio.Hit(voima);

        neliot.Add(nelio);
    }

    // add jypeli label to game layer
    void LisaaTekstikentta()
    {
        tekstikentta = new Jypeli.Label(Screen.Right, Screen.Top);
        tekstikentta.Size *= 10;
        Add(tekstikentta);
    }

    // add text to jypeli label from windows textbox
    void LisaaTeksti(object sender, EventArgs e)
    {
        tekstikentta.Text = text1.Text;
    }

    // open file search and load image to picturebox and game objects
    void KuvaHaku(object sender, EventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Title = "Open Image";
        dlg.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            Bitmap img = (Bitmap)System.Drawing.Image.FromFile(dlg.FileName);
            pictureBox1.Image = img;
            Jypeli.Image kuva = Bmp2Image(img);
            Level.Background.Image = kuva;
            foreach (PhysicsObject x in neliot)
            {
                x.Image = kuva;
            }
        }
        dlg.Dispose();
    }

    // converts bitmap image to jypeli image
    Jypeli.Image Bmp2Image(Bitmap img)
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
