using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace SpaceInvaders
{
    class Game
    {
        readonly (int, int) window = (72, 28);
        readonly ConsoleColor windowColor = ConsoleColor.DarkMagenta;
        Random rand = new Random();

        (int, int) playerPos;
        string[] playerSprite = new string[]
        {
            "   2   ",
            " 22222 ",
            "0000000",
            " 1   1 ",
        };
        List<(int, int)> playerBullets = new List<(int, int)>();
        int playerShootTimer = 0;
        int score = 0;

        readonly ConsoleColor bulletColor = ConsoleColor.Yellow;

        List<(int, int)> enemies = new List<(int, int)>();
        string[] enemySprite = new string[]
        {
            " 00000 ",
            "0010010",
            "000 000",
            " 0   0 ",
        };
        List<(int, int)> enemyBullets = new List<(int, int)>();

        public void Init()
        {
            Console.CursorVisible = false;
            for (var x = 0; x < window.Item1; x++)
                for (var y = 0; y < window.Item2; y++)
                    SetColor(windowColor, (x, y));

            playerPos = (window.Item1 / 2, window.Item2 - playerSprite.Length - 1);
            score = 0;

            enemies.Clear();
            for (var i = 0; i < 8; i += 2)
                enemies.Add((i * enemySprite[0].Length + 10, -20));
            enemyBullets.Clear();
            playerBullets.Clear();
        }

        public void Update()
        {
            PlayerUpdate();

            if (enemies.Count > 0)
                for (var i = 0; i < enemies.Count; i++)
                    EnemyUpdate(i);

            if (enemyBullets.Count > 0)
                for (var i = 0; i < enemyBullets.Count; i++)
                    BulletUpdate(enemyBullets, i, 2);

            Console.SetCursorPosition(window.Item1 + 5, 0);
            Console.Write($"score: {score}              ");
        }

        void PlayerUpdate()
        {
            if (Keyboard.IsKeyDown(Key.Right) && playerPos.Item1 < window.Item1 - playerSprite[0].Length - 2)
            {
                PlayerDraw(false);
                playerPos.Item1 += 2;
            }
            if (Keyboard.IsKeyDown(Key.Left) && playerPos.Item1 > 0)
            {
                PlayerDraw(false);
                playerPos.Item1 -= 2;
            }
            PlayerDraw(true);

            if (playerBullets.Count > 0)
                for (var i = 0; i < playerBullets.Count; i++)
                    BulletUpdate(playerBullets, i, -2);

            playerShootTimer += 1;
            if (Keyboard.IsKeyDown(Key.Space) && playerShootTimer > 5)
            {
                playerBullets.Add((playerPos.Item1 + playerSprite[0].Length / 2, playerPos.Item2 - 1));
                playerShootTimer = 0;
            }

            // bullet collision
            for (var i = 0; i < enemyBullets.Count; i++)
                if (PointInside(enemyBullets[i], playerPos, playerSprite[0].Length, playerSprite.Length))
                {
                    PlayerDraw(false);
                    Thread.Sleep(1000);
                    Init();
                }
        }

        void PlayerDraw(bool yeah)
        {
            for (var row = 0; row < playerSprite.Length; row++)
                for (var i = 0; i < playerSprite[0].Length; i++)
                    if (yeah)
                        switch (playerSprite[row][i])
                        {
                            case '0':
                                SetColor(ConsoleColor.Blue, (playerPos.Item1 + i, playerPos.Item2 + row));
                                break;
                            case '1':
                                SetColor(ConsoleColor.Red, (playerPos.Item1 + i, playerPos.Item2 + row));
                                break;
                            case '2':
                                SetColor(ConsoleColor.DarkGray, (playerPos.Item1 + i, playerPos.Item2 + row));
                                break;
                        }
                    else SetColor(windowColor, (playerPos.Item1 + i, playerPos.Item2 + row));
        }

        void BulletUpdate(List<(int, int)> bullets, int i, int speedY)
        {
            SetColor(windowColor, bullets[i]);
            bullets[i] = (bullets[i].Item1, bullets[i].Item2 + speedY);
            if (bullets[i].Item2 < 0 || bullets[i].Item2 > window.Item2 - 1)
                bullets.RemoveAt(i);
            else
                SetColor(bulletColor, bullets[i]);
        }

        void EnemyUpdate(int index)
        {
            var pos = enemies[index];

            // bullet collision
            for (var i = 0; i < playerBullets.Count; i++)
            {
                var b = playerBullets[i];
                if (PointInside(b, pos, enemySprite[0].Length, enemySprite.Length))
                {
                    DrawEnemy(index, false);
                    enemies[index] = (pos.Item1, -10);
                    score += 1;
                }
            }

            if (pos.Item2 < 4)
            {
                DrawEnemy(index, false);
                enemies[index] = (pos.Item1, pos.Item2 + 1);
            }
            DrawEnemy(index, true);

            if (pos.Item2 > 0 && rand.Next(0, 200) > 190)
                enemyBullets.Add((pos.Item1 + enemySprite[0].Length / 2, pos.Item2 + enemySprite.Length));
        }

        bool PointInside((int, int) point, (int, int) pos, int width, int height)
        {
            return point.Item1 > pos.Item1 && point.Item1 < pos.Item1 + width
                    && point.Item2 > pos.Item2 && point.Item2 < pos.Item2 + height;
        }

    void DrawEnemy(int index, bool yeah)
        {
            if (enemies[index].Item2 > 0)
            for (var row = 0; row < enemySprite.Length; row++)
                for (var i = 0; i < enemySprite[0].Length; i++)
                    if (yeah)
                        switch (enemySprite[row][i])
                        {
                            case '0':
                                SetColor(ConsoleColor.Green, (enemies[index].Item1 + i, enemies[index].Item2 + row));
                                break;
                            case '1':
                                SetColor(ConsoleColor.Red, (enemies[index].Item1 + i, enemies[index].Item2 + row));
                                break;
                        }
                    else SetColor(windowColor, (enemies[index].Item1 + i, enemies[index].Item2 + row));
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
            game.Init();

            while (true)
            {
                Thread.Sleep(100);
                game.Update();
            }

        }
    }
}
