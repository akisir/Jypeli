// Player.cs 1.4.2019
// by Aki Sirviö
using Jypeli;

//Stores palyer information
namespace GolfServer
{
    class Player
    {
        public PhysicsObject racket { get; set; }
        public Label name { get; set; }
        public PhysicsObject ball { get; set; }
        public IntMeter score { get; set; }
        public IntMeter shots { get; set; }
        public string ID { get; set; }
        public Angle prevAngle { get; set; }
        public bool inHole { get; set; }

        public Player()
        {
            inHole = false;
        }
    }
}
