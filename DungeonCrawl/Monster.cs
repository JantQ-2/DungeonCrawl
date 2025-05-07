using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
    }
}