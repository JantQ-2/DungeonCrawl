using System.Numerics;

namespace DungeonCrawl
{
    internal class Monster
    {
        public string name;
        public Vector2 position;
        public int hitpoints;
        public char symbol;
        public ConsoleColor color;

        public static int PositionToTileIndex(Vector2 position, Map level)
        {
            return (int)position.X + (int)position.Y * level.width;
        }
        public static void ProcessEnemies(List<Monster> enemies, Map level, PlayerCharacter character, List<int> dirtyTiles, List<string> messages)
        {
            foreach (Monster enemy in enemies)
            {

                if (Game.GetDistanceBetween(enemy.position, character.position) < 5)
                {
                    Vector2 enemyMove = new Vector2(0, 0);

                    if (character.position.X < enemy.position.X)
                    {
                        enemyMove.X = -1;
                    }
                    else if (character.position.X > enemy.position.X)
                    {
                        enemyMove.X = 1;
                    }
                    else if (character.position.Y > enemy.position.Y)
                    {
                        enemyMove.Y = 1;
                    }
                    else if (character.position.Y < enemy.position.Y)
                    {
                        enemyMove.Y = -1;
                    }

                    int startTile = PositionToTileIndex(enemy.position, level);
                    Vector2 destinationPlace = enemy.position + enemyMove;
                    if (destinationPlace == character.position)
                    {
                        // TODO: Random change for armor to protect?
                        int damage = 1;
                        damage -= PlayerCharacter.GetCharacterDefense(character);
                        if (damage <= 0)
                        {
                            damage = 1;
                        }
                        character.hitpoints -= damage;
                        messages.Add($"{enemy.name} hits you for {damage} damage!");
                    }
                    else
                    {
                        Map.Tile destination = Map.GetTileAtMap(level, destinationPlace);
                        if (destination == Map.Tile.Floor)
                        {
                            enemy.position = destinationPlace;
                            dirtyTiles.Add(startTile);
                        }
                        else if (destination == Map.Tile.Door)
                        {
                            enemy.position = destinationPlace;
                            dirtyTiles.Add(startTile);
                        }
                        else if (destination == Map.Tile.Wall)
                        {
                            // NOP
                        }
                    }
                }
            }
        }
        public static Monster CreateMonster(string name, int hitpoints, char symbol, ConsoleColor color, Vector2 position)
        {
            Monster monster = new Monster();
            monster.name = name;
            monster.hitpoints = hitpoints;
            monster.symbol = symbol;
            monster.color = color;
            monster.position = position;
            return monster;
        }
        public static Monster CreateRandomMonster(Random random, Vector2 position)
        {
            int type = random.Next(4);
            return type switch
            {
                0 => Monster.CreateMonster("Goblin", 5, 'g', ConsoleColor.Green, position),
                1 => Monster.CreateMonster("Bat Man", 2, 'M', ConsoleColor.Magenta, position),
                2 => Monster.CreateMonster("Orc", 15, 'o', ConsoleColor.Red, position),
                3 => Monster.CreateMonster("Bunny", 1, 'B', ConsoleColor.Yellow, position)
            };
        }
        public static List<Monster> CreateEnemies(Map level, Random random)
        {
            List<Monster> monsters = new List<Monster>();

            for (int y = 0; y < level.height; y++)
            {
                for (int x = 0; x < level.width; x++)
                {
                    int ti = y * level.width + x;
                    if (level.Tiles[ti] == Map.Tile.Monster)
                    {
                        Monster m = CreateRandomMonster(random, new Vector2(x, y));
                        monsters.Add(m);
                        level.Tiles[ti] = (sbyte)Map.Tile.Floor;
                    }
                }
            }
            return monsters;
        }
        public static void DrawEnemies(List<Monster> enemies)
        {
            foreach (Monster m in enemies)
            {
                Console.SetCursorPosition((int)m.position.X, (int)m.position.Y);
                Game.Print(m.symbol, m.color);
            }
        }
    }
}