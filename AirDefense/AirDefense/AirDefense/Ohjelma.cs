using System;

static class Ohjelma
{
#if WINDOWS || XBOX
    static void Main(string[] args)
    {
        using (AirDefense game = new AirDefense())
        {
#if !DEBUG
            game.IsFullScreen = true;
#endif
            game.Run();
        }
    }
#endif
}
