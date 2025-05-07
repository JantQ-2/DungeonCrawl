namespace DungeonCrawl
{
    internal class DrawBrickBg
    {
        public static void DrawBackground()
        {
            // Draw tiles
            Console.BackgroundColor = ConsoleColor.DarkGray;
            for (int y = 0; y < Console.WindowHeight; y++)
            {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < Console.WindowWidth; x++)
                {
                    if ((x + y) % 3 == 0)
                    {
                        Game.Print("|", ConsoleColor.Black);
                    }
                    else
                    {
                        Game.Print(" ", ConsoleColor.DarkGray);
                    }
                }
            }
        }

        public static void DrawRectangle(int x, int y, int width, int height, ConsoleColor color)
        {
            Console.BackgroundColor = color;
            for (int dy = y; dy < y + height; dy++)
            {
                Console.SetCursorPosition(x, dy);
                for (int dx = x; dx < x + width; dx++)
                {
                    Game.Print(" ");
                }
            }
        }

        public static void DrawRectangleBorders(int x, int y, int width, int height, ConsoleColor color, string symbol)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = color;
            for (int dx = x; dx < x + width; dx++)
            {
                Game.Print(symbol);
            }

            for (int dy = y; dy < y + height; dy++)
            {
                Console.SetCursorPosition(x, dy);
                Game.Print(symbol);
                Console.SetCursorPosition(x + width - 1, dy);
                Game.Print(symbol);
            }
        }

        public static void DrawEndScreen(Random random)
        {
            // Run death animation: blood flowing down the screen in columns
            // Wait until keypress
            byte[] speeds = new byte[Console.WindowWidth];
            byte[] ends = new byte[Console.WindowWidth];
            for (int i = 0; i < speeds.Length; i++)
            {
                speeds[i] = (byte)random.Next(1, 4);
                ends[i] = 0;
            }
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;


            for (int row = 0; row < Console.WindowHeight - 2; row++)
            {
                Console.SetCursorPosition(0, row);
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.Write(" ");
                }
                Thread.Sleep(100);
            }

        }
        public static void DrawTile(byte x, byte y, Map.Tile tile)
        {
            Console.SetCursorPosition(x, y);
            switch (tile)
            {
                case Map.Tile.Floor:
                    Game.Print(".", ConsoleColor.Gray); break;

                case Map.Tile.Wall:
                    Game.Print("#", ConsoleColor.DarkGray); break;

                case Map.Tile.Door:
                    Game.Print("+", ConsoleColor.Yellow); break;
                case Map.Tile.Stairs:
                    Game.Print(">", ConsoleColor.Yellow); break;

                default: break;
            }
        }
    }
}