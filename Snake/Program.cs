using System;
using System.Threading;

namespace Snake
{
    class Program
    {
        static Timer timer;

        [STAThread]
        public static void Main()
        {
            var game = new Game();
            timer = new Timer(_ => game.Update(), null, 0, 100);

            while (true)
                game.Input();
        }
    }
}
