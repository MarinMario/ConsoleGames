using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pong
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var game = new Game();

            while (true)
            {
                Thread.Sleep(100);
                game.Update();
            }
        }
    }
}
