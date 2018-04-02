using System;

static class Ohjelma
{
#if WINDOWS || XBOX
    static void Main(string[] args)
    {
        using (TenLevelPlatform game = new TenLevelPlatform())
        {
#if !DEBUG
            game.IsFullScreen = true;
#endif

            game.Run();
        }
    }
#endif
}
