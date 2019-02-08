// VideoPlay.cs 7.2.2019 by Aki Sirviö
// exsample how to play video in monogame and jypeli game environment.
using System;
using System.Windows;
using System.Windows.Forms;
using Jypeli;
using Microsoft.Xna.Framework.Input;

// Play video file in monogame project in a few different sizes.
public class VideoPlay : PhysicsGame
{
    Form form;
    System.Windows.Forms.Integration.ElementHost eh;
    System.Windows.Controls.MediaElement me;

    // progrma starts here
    public override void Begin()
    {
        VideoClipPlay();
        Keyboard.Listen(Key.Escape, Jypeli.ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Jypeli.Timer.SingleShot(10, FullScreen);
        Jypeli.Timer.SingleShot(25, SmallScreen);
    }

    // play video on windows form with full window size
    void VideoClipPlay()
    {
        form = (Form)Control.FromHandle(Window.Handle);
        form.Size = new System.Drawing.Size(1000, 700);
        form.BackColor = System.Drawing.Color.Black;

        eh = new System.Windows.Forms.Integration.ElementHost();
        eh.BackColor = System.Drawing.Color.Black;
        eh.MinimumSize = new System.Drawing.Size(1000, 650);
        me = new System.Windows.Controls.MediaElement();
        eh.Child = me;

        form.Controls.Add(eh);
        form.Show();

        me.Source = new Uri("Content/Movie.wmv", UriKind.Relative);
        me.LoadedBehavior = System.Windows.Controls.MediaState.Manual;
        me.UnloadedBehavior = System.Windows.Controls.MediaState.Manual;
        me.MediaEnded += mediaElement_OnMediaEnded;
        me.Play();
    }

    // cahnge full screen size
    void FullScreen()
    {
        form.FormBorderStyle = FormBorderStyle.None;
        form.WindowState = FormWindowState.Maximized;
        eh.MinimumSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
    }

    // change small window size
    void SmallScreen()
    {
        int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        SetWindowSize(width, height, false);
        eh.MinimumSize = new System.Drawing.Size(150, 100);
        eh.MaximumSize = new System.Drawing.Size(150, 100);
        eh.Location = new System.Drawing.Point(120, 120);

        StartGame();
    }

    // make moving physics object and timer for moving video
    void StartGame()
    {
        Level.CreateBorders(100, 100, 20, 1, Jypeli.Color.Blue);
        Level.Background.Color = Jypeli.Color.ForestGreen;
        PhysicsObject laatikko = new PhysicsObject(20, 20);
        laatikko.Restitution = 1;
        laatikko.IsVisible = false;
        Add(laatikko);
        laatikko.Hit(new Jypeli.Vector(300, 300));

        ChangeLocation(laatikko.Position);

        Jypeli.Timer loc = new Jypeli.Timer();
        loc.Interval = 0.01;
        loc.Timeout += delegate { ChangeLocation(laatikko.Position); };
        loc.Start();
    }

    // move video view around screen
    void ChangeLocation(Jypeli.Vector pos)
    {
        int width = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) / 2 + (int)pos.X-75;
        int height = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2) - (int)pos.Y-50;
        eh.Location = new System.Drawing.Point(width, height);
    }

    // repeat vidoe when media ended (event)
    private void mediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
    {
        me.Position = new TimeSpan(0, 0, 1);
        me.Play();
    }
}
