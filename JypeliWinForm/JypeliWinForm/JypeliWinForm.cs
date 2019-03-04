//JypeliWinForm.cs 4.3.2019
//by Aki Sirviö
using System;
using System.Collections.Generic;
using System.Threading;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace JypeliWinForm
{
    public class JypeliWinForm : PhysicsGame
    {
        private List<PhysicsObject> pallot = new List<PhysicsObject>();

        // open form and sets initial settings
        public override void Begin()
        {
            Form1 lomake = new Form1(this);
            Jypeli.Timer.SingleShot(1, delegate { lomake.Show(); });
            Gravity = new Vector(0, 500);
            Level.CreateBorders();

            PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
            Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        }

        // adds balloons
        public void LisaaPallo()
        {
            PhysicsObject pallo = new PhysicsObject(185, 200);
            int xCoord = RandomGen.NextInt((int)Screen.Left+110, (int)Screen.Right-110);
            pallo.Image = LoadImage("balloon");
            pallo.X = xCoord;
            pallo.Y = Screen.Bottom;
            pallo.CanRotate = false;
            pallo.CollisionIgnoreGroup = 1;
            Add(pallo);

            pallot.Add(pallo);
        }

        // destroy balloons
        public void TuhoaPallot()
        {
            SoundEffect poksahdus = LoadSoundEffect("Explosion147");
            foreach (PhysicsObject x in pallot)
            {
                x.Destroy();
                poksahdus.Play();
            }
        }

        // close program
        public void Sulje()
        {
            Exit();
        }
    }
}
