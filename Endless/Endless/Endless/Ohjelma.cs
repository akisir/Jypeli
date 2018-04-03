using System;

static class Ohjelma
{
#if WINDOWS || XBOX
    static void Main(string[] args)
    {
        using (Endless game = new Endless())
        {
#if !DEBUG
            game.IsFullScreen = true;
#endif

            game.Run();
        }
    }
#endif
}
