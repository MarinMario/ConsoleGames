using System;
using System.Threading;
using System.Windows.Input;

namespace Pong
{
    class Game
    {
        readonly (int, int) window = (72, 24);
        readonly ConsoleColor windowColor = ConsoleColor.Blue;

        (int, int) ballPosition;
        (int, int) ballDirection = (-2, 0);
        readonly ConsoleColor ballColor = ConsoleColor.White;

        int playerScore = 0;
        int opponentScore = 0;
        int paddleSize = 5;
        (int, int) paddlePlayer;
        (int, int) paddleOpponent;
        readonly ConsoleColor paddleColor = ConsoleColor.Red;

        public Game()
        {
            Console.CursorVisible = false;
            for (var x = 0; x < window.Item1; x++)
                for (var y = 0; y < window.Item2; y++)
                    SetColor(windowColor, (x, y));

            ballPosition = (window.Item1 / 2, window.Item2 / 2);
            paddlePlayer = (2, window.Item2 / 2 - paddleSize / 2);
            paddleOpponent = (window.Item1 - 2, window.Item2 / 2 - paddleSize / 2);
        }

        public void Update()
        {
            Ball();
            PaddlePlayer();
            PaddleOpponent();

            Console.SetCursorPosition(0, window.Item2 + 1);
            Console.Write($"{playerScore} : {opponentScore}         ");
        }

        void PaddlePlayer()
        {
            DrawPaddle(windowColor, paddlePlayer);
            if (Keyboard.IsKeyDown(Key.Up) && paddlePlayer.Item2 > 0)
                paddlePlayer.Item2 -= 1;
            if (Keyboard.IsKeyDown(Key.Down) && paddlePlayer.Item2 < window.Item2 - paddleSize)
                paddlePlayer.Item2 += 1;
            DrawPaddle(paddleColor, paddlePlayer);
        }

        int opponentTarget = 0;
        void PaddleOpponent()
        {
            var rand = new Random();
            if (rand.Next(0, 10) > 0)
                opponentTarget = ballPosition.Item2 - paddleSize / 2;

            DrawPaddle(windowColor, paddleOpponent);
            if (paddleOpponent.Item2 > opponentTarget)
                paddleOpponent.Item2 -= 1;
            if (paddleOpponent.Item2 < opponentTarget)
                paddleOpponent.Item2 += 1;

            if (paddleOpponent.Item2 < 0) paddleOpponent.Item2 = 0;
            if (paddleOpponent.Item2 > window.Item2 - paddleSize) paddleOpponent.Item2 = window.Item2 - paddleSize;
            DrawPaddle(paddleColor, paddleOpponent);
        }

        void Ball()
        {
            SetColor(windowColor, ballPosition);
            ballPosition.Item1 += ballDirection.Item1;
            ballPosition.Item2 += ballDirection.Item2;
            if (ballPosition.Item1 >= window.Item1 || ballPosition.Item1 <= -1)
            {
                Thread.Sleep(1000);
                ballPosition = (window.Item1 / 2, window.Item2 / 2);
                if (ballDirection.Item1 < 0)
                {
                    opponentScore += 1;
                    ballDirection = (2, 0);
                }
                else
                {
                    playerScore += 1;
                    ballDirection = (-2, 0);
                }
            }
            SetColor(ballColor, ballPosition);

            if (ballPosition.Item2 >= window.Item2 - 1)
                ballDirection.Item2 = -1;
            if (ballPosition.Item2 <= 0)
                ballDirection.Item2 = 1;

            paddleCollision(paddlePlayer, 1);
            paddleCollision(paddleOpponent, -1);


            void paddleCollision((int, int) paddle, int scalar)
            {
                if (ballPosition.Item1 == paddle.Item1 + scalar * 2
                && ballPosition.Item2 >= paddle.Item2
                && ballPosition.Item2 <= paddle.Item2 + paddleSize - 1)
                {
                    ballDirection.Item1 = 2 * scalar;
                    if (ballPosition.Item2 > paddlePlayer.Item2 + paddleSize / 2)
                        ballDirection.Item2 = 1 * scalar;
                    else if (ballPosition.Item2 < paddlePlayer.Item2 + paddleSize / 2)
                        ballDirection.Item2 = -1 * scalar;
                    else ballDirection.Item2 = 0;
                }
            }
        }
        void DrawPaddle(ConsoleColor color, (int, int) pos)
        {
            for (var y = 0; y < paddleSize; y++)
                SetColor(color, (pos.Item1, pos.Item2 + y));
        }

        void SetColor(ConsoleColor color, (int, int) pos)
        {
            Console.SetCursorPosition(pos.Item1, pos.Item2);
            Console.BackgroundColor = color;
            Console.Write("  ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

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
