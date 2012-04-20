using System;

namespace NurfWars
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (NurfGame game = new NurfGame())
            {
                game.Run();
            }
        }
    }
}

