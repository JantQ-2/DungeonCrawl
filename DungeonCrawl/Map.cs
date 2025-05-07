using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
    internal class Map
    {
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
    }
}