using System.Numerics;

namespace DungeonCrawl
{
    internal class Map
    {
        const int INFO_HEIGHT = 6;
        const int COMMANDS_WIDTH = 12;
        const int ENEMY_CHANCE = 3;
        const int ITEM_CHANCE = 4;

        // Room generation 
        const int ROOM_AMOUNT = 12;
        const int ROOM_MIN_W = 4;
        const int ROOM_MAX_W = 12;
        const int ROOM_MIN_H = 4;
        const int ROOM_MAX_H = 8;

        public enum Tile : sbyte
        {
            Floor,
            Wall,
            Door,
            Monster,
            Item,
            Player,
            Stairs
        }
        public int width;
        public int height;
        public Tile[] Tiles;

        public static Map.Tile GetTileAtMap(Map level, Vector2 position)
        {
            if (position.X >= 0 && position.X < level.width)
            {
                if (position.Y >= 0 && position.Y < level.height)
                {
                    int ti = (int)position.Y * level.width + (int)position.X;
                    return (Map.Tile)level.Tiles[ti];
                }
            }
            return Map.Tile.Wall;
        }
        public static Map CreateMap(Random random)
        {
            Map level = new Map();

            level.width = Console.WindowWidth - COMMANDS_WIDTH;
            level.height = Console.WindowHeight - INFO_HEIGHT;
            level.Tiles = new Map.Tile[level.width * level.height];

            // Create perimeter wall
            for (int y = 0; y < level.height; y++)
            {
                for (int x = 0; x < level.width; x++)
                {
                    int ti = y * level.width + x;
                    if (y == 0 || x == 0 || y == level.height - 1 || x == level.width - 1)
                    {
                        level.Tiles[ti] = Map.Tile.Wall;
                    }
                    else
                    {
                        level.Tiles[ti] = Map.Tile.Floor;
                    }
                }
            }

            int roomRows = 3;
            int roomsPerRow = 6;
            int boxWidth = (Console.WindowWidth - COMMANDS_WIDTH - 2) / roomsPerRow;
            int boxHeight = (Console.WindowHeight - INFO_HEIGHT - 2) / roomRows;
            for (int roomRow = 0; roomRow < roomRows; roomRow++)
            {
                for (int roomColumn = 0; roomColumn < roomsPerRow; roomColumn++)
                {
                    AddRoom(level, roomColumn * boxWidth + 1, roomRow * boxHeight + 1, boxWidth, boxHeight, random);
                }
            }

            // Add enemies and items
            for (int y = 0; y < level.height; y++)
            {
                for (int x = 0; x < level.width; x++)
                {
                    int ti = y * level.width + x;
                    if (level.Tiles[ti] == Map.Tile.Floor)
                    {
                        int chance = random.Next(100);
                        if (chance < Map.ENEMY_CHANCE)
                        {
                            level.Tiles[ti] = Map.Tile.Monster;
                            continue;
                        }

                        chance = random.Next(100);
                        if (chance < ITEM_CHANCE)
                        {
                            level.Tiles[ti] = Map.Tile.Item;
                        }
                    }
                }
            }

            // Find starting place for player
            for (int i = 0; i < level.Tiles.Length; i++)
            {
                if (level.Tiles[i] == Map.Tile.Floor)
                {
                    level.Tiles[i] = Map.Tile.Player;
                    break;
                }
            }

            return level;
        }

        public static void DrawInfo(PlayerCharacter player, List<Monster> enemies, List<Item> items, List<string> messages)
        {
            int infoLine1 = Console.WindowHeight - INFO_HEIGHT;
            Console.SetCursorPosition(0, infoLine1);
            Game.Print($"{player.name}: hp ({player.hitpoints}/{player.maxHitpoints}) gold ({player.gold}) ", ConsoleColor.White);
            int damage = 1;
            if (player.weapon != null)
            {
                damage = player.weapon.quality;
            }
            Game.Print($"Weapon damage: {damage} ");
            int armor = 0;
            if (player.armor != null)
            {
                armor = player.armor.quality;
            }
            Game.Print($"Armor: {armor} ");



            // Print last INFO_HEIGHT -1 messages
            DrawBrickBg.DrawRectangle(0, infoLine1 + 1, Console.WindowWidth, INFO_HEIGHT - 2, ConsoleColor.Black);
            Console.SetCursorPosition(0, infoLine1 + 1);
            int firstMessage = 0;
            if (messages.Count > (INFO_HEIGHT - 1))
            {
                firstMessage = messages.Count - (INFO_HEIGHT - 1);
            }
            for (int i = firstMessage; i < messages.Count; i++)
            {
                Game.Print(messages[i], ConsoleColor.Yellow);
            }
        }
        public static void DrawCommands()
        {
            int cx = Console.WindowWidth - COMMANDS_WIDTH + 1;
            int ln = 1;
            Console.SetCursorPosition(cx, ln); ln++;
            Game.Print(":Commands:", ConsoleColor.Yellow);
            Console.SetCursorPosition(cx, ln); ln++;
            Game.Print("I", ConsoleColor.Cyan); Game.Print("nventory", ConsoleColor.White);
        }

        public static void AddRoom(Map level, int boxX, int boxY, int boxWidth, int boxHeight, Random random)
        {
            int width = random.Next(ROOM_MIN_W, boxWidth);
            int height = random.Next(ROOM_MIN_H, boxHeight);
            int sx = boxX + random.Next(0, boxWidth - width);
            int sy = boxY + random.Next(0, boxHeight - height);
            int doorX = random.Next(1, width - 1);
            int doorY = random.Next(1, height - 1);

            // Create perimeter wall
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int ti = (sy + y) * level.width + (sx + x);
                    if (y == 0 || x == 0 || y == height - 1 || x == width - 1)
                    {

                        if (y == doorY || x == doorX)
                        {
                            level.Tiles[ti] = Map.Tile.Door;
                        }
                        else
                        {
                            level.Tiles[ti] = Map.Tile.Wall;
                        }
                    }
                }
            }
        }
        public static void PlacePlayerToMap(PlayerCharacter character, Map level)
        {
            for (int i = 0; i < level.Tiles.Length; i++)
            {
                if (level.Tiles[i] == Map.Tile.Player)
                {
                    level.Tiles[i] = Map.Tile.Floor;
                    int px = i % level.width;
                    int py = i / level.width;

                    character.position = new Vector2(px, py);
                    break;
                }
            }
        }
        public static void PlaceStairsToMap(Map level)
        {
            for (int i = level.Tiles.Length - 1; i >= 0; i--)
            {
                if (level.Tiles[i] == Map.Tile.Floor)
                {
                    level.Tiles[i] = Map.Tile.Stairs;
                    break;
                }
            }
        }

        public static void DrawMapAll(Map level)
        {
            for (byte y = 0; y < level.height; y++)
            {
                for (byte x = 0; x < level.width; x++)
                {
                    int ti = y * level.width + x;
                    Map.Tile tile = (Map.Tile)level.Tiles[ti];
                    DrawBrickBg.DrawTile(x, y, tile);
                }
            }
        }

        public static void DrawMap(Map level, List<int> dirtyTiles)
        {
            if (dirtyTiles.Count == 0)
            {
                DrawMapAll(level);
            }
            else
            {
                foreach (int dt in dirtyTiles)
                {
                    byte x = (byte)(dt % level.width);
                    byte y = (byte)(dt / level.width);
                    Map.Tile tile = (Map.Tile)level.Tiles[dt];
                    DrawBrickBg.DrawTile(x, y, tile);
                }
            }
        }
    }
}