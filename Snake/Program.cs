using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;

namespace Snake
{
    class Program
    {
        class Game
        {
            readonly (int, int) window = (48, 24);

            enum Dir { Up, Down, Left, Right }
            (int, int) snakeHead = (0, 0);
            Dir snakeDir = Dir.Right;
            List<(int, int)> snakeBody = new List<(int, int)> { (0, 0) };

            (int, int) fruit;

            public Game()
            {
                Console.CursorVisible = false;
                for (var x = 0; x < window.Item1 + 2; x++)
                    for (var y = 0; y < window.Item2 + 1; y++)
                        SetColor(ConsoleColor.Blue, x, y);
                fruit = GenerateFruitPos();
            }

            public void Update()
            {
                DrawSnake(ConsoleColor.Blue);
                for (var i = snakeBody.Count - 1; i > 0; i--)
                    snakeBody[i] = snakeBody[i - 1];
                snakeBody[0] = snakeHead;

                switch (snakeDir)
                {
                    case Dir.Left:
                        snakeHead.Item1 -= 2;
                        break;
                    case Dir.Right:
                        snakeHead.Item1 += 2;
                        break;
                    case Dir.Up:
                        snakeHead.Item2 -= 1;
                        break;
                    case Dir.Down:
                        snakeHead.Item2 += 1;
                        break;
                }

                SnakeDieCheck();

                if (snakeHead.Item1 < 0)
                    snakeHead.Item1 = window.Item1;
                if (snakeHead.Item1 > window.Item1)
                    snakeHead.Item1 = 0;
                if (snakeHead.Item2 < 0)
                    snakeHead.Item2 = window.Item2;
                if (snakeHead.Item2 > window.Item2)
                    snakeHead.Item2 = 0;

                DrawSnake(ConsoleColor.Green);

                SetColor(ConsoleColor.Blue, fruit.Item1, fruit.Item2);
                if (snakeHead == fruit)
                {
                    fruit = GenerateFruitPos();
                    snakeBody.Add(snakeHead);
                }
                SetColor(ConsoleColor.Red, fruit.Item1, fruit.Item2);

                Console.SetCursorPosition(0, window.Item2 + 2);
                Console.Write($"Score: {snakeBody.Count - 1}            ");
            }

            public void Input()
            {
                if (Keyboard.IsKeyDown(Key.Left) && snakeDir != Dir.Right)
                    snakeDir = Dir.Left;
                if (Keyboard.IsKeyDown(Key.Right) && snakeDir != Dir.Left)
                    snakeDir = Dir.Right;
                if (Keyboard.IsKeyDown(Key.Up) && snakeDir != Dir.Down)
                    snakeDir = Dir.Up;
                if (Keyboard.IsKeyDown(Key.Down) && snakeDir != Dir.Up)
                    snakeDir = Dir.Down;
            }

            void DrawSnake(ConsoleColor color)
            {
                SetColor(color, snakeHead.Item1, snakeHead.Item2);
                for (var i = 0; i < snakeBody.Count; i++)
                    SetColor(color, snakeBody[i].Item1, snakeBody[i].Item2);
            }

            (int, int) GenerateFruitPos()
            {
                var rand = new Random();
                (int, int) gen()
                {
                    return (rand.Next(0, window.Item1 - 1), rand.Next(0, window.Item2 - 1));
                }
                var toReturn = gen();
                while (toReturn.Item1 % 2 != 0)
                    toReturn = gen();

                return toReturn;
            }

            void SnakeDieCheck()
            {
                for (var i = 0; i < snakeBody.Count; i++)
                    if (snakeBody[i] == snakeHead)
                    {
                        snakeBody.Clear();
                        snakeBody.Add(snakeHead);
                        break;
                    }
            }

            void SetColor(ConsoleColor color, int x, int y)
            {
                Console.SetCursorPosition(x, y);
                Console.BackgroundColor = color;
                Console.Write("  ");
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }

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
