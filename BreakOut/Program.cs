using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;

namespace BreakOut
{
    class Game
    {
        readonly (int, int) window = (72, 28);
        readonly ConsoleColor windowColor = ConsoleColor.Blue;

        (int, int) ball;
        (int, int) ballDir;
        readonly ConsoleColor ballColor = ConsoleColor.Green;

        (int, int) paddle;
        readonly int paddleSize = 10;
        readonly ConsoleColor paddleColor = ConsoleColor.Red;

        List<(int, int)> pieces = new List<(int, int)> { (10, 2) };
        readonly (int, int) pieceSize = (10, 3);
        readonly ConsoleColor pieceColor = ConsoleColor.Red;

        public void Init()
        {
            Console.CursorVisible = false;
            for (var x = 0; x < window.Item1; x++)
                for (var y = 0; y < window.Item2; y++)
                    SetColor(windowColor, (x, y));

            ball = (window.Item1 / 2, window.Item2 / 2);
            ballDir = (0, 1);
            paddle = (window.Item1 / 2 - paddleSize / 2, window.Item2 - 2);


            pieces.Clear();
            for (var x = pieceSize.Item1; x < window.Item1 - pieceSize.Item1; x += pieceSize.Item1 + 4)
                for (var y = pieceSize.Item2; y < window.Item2 / 2; y += pieceSize.Item2 + 1)
                    pieces.Add((x, y));

            for (var i = 0; i < pieces.Count; i++)
                DrawPiece(pieceColor, pieces[i]);

        }
        public void Update()
        {
            Ball();
            Paddle();
            if (pieces.Count > 0)
                for (var i = 0; i < pieces.Count; i++)
                    Piece(pieces[i]);
        }

        void Ball()
        {
            SetColor(windowColor, ball);
            ball.Item1 += ballDir.Item1;
            ball.Item2 += ballDir.Item2;

            if (ball.Item1 < 2)
                ballDir.Item1 = 2;
            if (ball.Item1 >= window.Item1 - 2)
                ballDir.Item1 = -2;
            if (ball.Item2 < 1)
                ballDir.Item2 = 1;
            if (ball.Item1 >= paddle.Item1 && ball.Item1 < paddle.Item1 + paddleSize && ball.Item2 == paddle.Item2 - 1)
                if (ball.Item1 > paddle.Item1 + paddleSize / 2)
                    ballDir = (2, -1);
                else if (ball.Item1 < paddle.Item1 + paddleSize / 2)
                    ballDir = (-2, -1);
                else ballDir = (0, -1);
                    

            if (ball.Item2 > window.Item2 - 1)
            {
                Thread.Sleep(1000);
                Init();
            }

            SetColor(ballColor, ball);
        }

        void Paddle()
        {
            DrawPaddle(windowColor);
            if (Keyboard.IsKeyDown(Key.Right) && paddle.Item1 < window.Item1 - paddleSize - 1)
                paddle.Item1 += 2;
            if (Keyboard.IsKeyDown(Key.Left) && paddle.Item1 > 1)
                paddle.Item1 -= 2;
            DrawPaddle(paddleColor);
        }


        void DrawPaddle(ConsoleColor color)
        {
            for (var i = 1; i < paddleSize; i++)
                SetColor(color, (paddle.Item1 + i, paddle.Item2)) ;
        }

        void Piece((int, int) piece)
        {
            if (ball.Item1 >= piece.Item1 
                && ball.Item1 <= piece.Item1 + pieceSize.Item1
                && ball.Item2 >= piece.Item2 
                && ball.Item2 <= piece.Item2 + pieceSize.Item2)
            {
                pieces.Remove(piece);
                DrawPiece(windowColor, piece);
                if (ball.Item1 == piece.Item1 || ball.Item1 == piece.Item1 + pieceSize.Item1)
                    ballDir.Item1 = -ballDir.Item1;
                if (ball.Item2 == piece.Item2 || ball.Item2 == piece.Item2 + pieceSize.Item2)
                    ballDir.Item2 = -ballDir.Item2;
            }
        }

        void DrawPiece(ConsoleColor color, (int, int) piece)
        {
            for (var x = 0; x < pieceSize.Item1; x++)
                for (var y = 0; y < pieceSize.Item2; y++)
                    SetColor(color, (piece.Item1 + x, piece.Item2 + y));
        }

        public void SetColor(ConsoleColor color, (int, int) pos)
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
            game.Init();

            while (true)
            {
                Thread.Sleep(100);
                game.Update();
            }

        }
    }
}
