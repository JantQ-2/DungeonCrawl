using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
    internal class PlayerCharacter
    {
        

        public string name;
        public int hitpoints;
        public int maxHitpoints;
        public Item weapon;
        public Item armor;
        public int gold;
        public Vector2 position;
        public List<Item> inventory;
        public static void GiveItem(PlayerCharacter character, Item item)
        {
            // Inventory order
            // Weapons
            // Armors
            // Potions
            switch (item.type)
            {
                case ItemType.Weapon:
                    if ((character.weapon != null && character.weapon.quality < item.quality)
                        || character.weapon == null)
                    {
                        character.weapon = item;
                    }
                    character.inventory.Insert(0, item);
                    break;
                case ItemType.Armor:
                    if ((character.armor != null && character.armor.quality < item.quality)
                        || character.armor == null)
                    {
                        character.armor = item;
                    }
                    int armorIndex = 0;
                    while (armorIndex < character.inventory.Count && character.inventory[armorIndex].type == ItemType.Weapon)
                    {
                        armorIndex++;
                    }
                    character.inventory.Insert(armorIndex, item);
                    break;
                case ItemType.Potion:
                    character.inventory.Add(item);
                    break;
                case ItemType.Treasure:
                    character.gold += item.quality;
                    break;
            }

        }
        public static int GetCharacterDamage(PlayerCharacter character)
        {
            if (character.weapon != null)
            {
                return character.weapon.quality;
            }
            else
            {
                return 1;
            }
        }
        public static int GetCharacterDefense(PlayerCharacter character)
        {
            if (character.armor != null)
            {
                return character.armor.quality;
            }
            else
            {
                return 0;
            }
        }
        public static bool DoPlayerTurnVsEnemies(PlayerCharacter character, List<Monster> enemies, Vector2 destinationPlace, List<string> messages)
        {
            // Check enemies
            bool hitEnemy = false;
            Monster toRemoveMonster = null;
            foreach (Monster enemy in enemies)
            {
                if (enemy.position == destinationPlace)
                {
                    int damage = GetCharacterDamage(character);
                    messages.Add($"You hit {enemy.name} for {damage}!");
                    enemy.hitpoints -= damage;
                    hitEnemy = true;
                    if (enemy.hitpoints <= 0)
                    {
                        toRemoveMonster = enemy;
                    }
                }
            }
            if (toRemoveMonster != null)
            {
                enemies.Remove(toRemoveMonster);
            }
            return hitEnemy;
        }
        public static void DrawPlayer(PlayerCharacter character)
        {
            Console.SetCursorPosition((int)character.position.X, (int)character.position.Y);
            Game.Print("@", ConsoleColor.White);
        }

        public static PlayerCharacter CreateCharacter()
        {
            PlayerCharacter character = new PlayerCharacter();
            character.name = "";
            character.hitpoints = 20;
            character.maxHitpoints = character.hitpoints;
            character.gold = 0;
            character.weapon = null;
            character.armor = null;
            character.inventory = new List<Item>();

            Console.Clear();
            DrawBrickBg.DrawBackground();

            // Draw entrance
            Console.BackgroundColor = ConsoleColor.Black;
            int doorHeight = (int)(Console.WindowHeight * (3.0f / 4.0f));
            int doorY = Console.WindowHeight - doorHeight;
            int doorWidth = (int)(Console.WindowWidth * (3.0f / 5.0f));
            int doorX = Console.WindowWidth / 2 - doorWidth / 2;

            DrawBrickBg.DrawRectangle(doorX, doorY, doorWidth, doorHeight, ConsoleColor.Black);
            DrawBrickBg.DrawRectangleBorders(doorX + 1, doorY + 1, doorWidth - 2, doorHeight - 2, ConsoleColor.Blue, "|");
            DrawBrickBg.DrawRectangleBorders(doorX + 3, doorY + 3, doorWidth - 6, doorHeight - 6, ConsoleColor.DarkBlue, "|");

            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 2);
            Game.Print("Welcome Brave Adventurer!");
            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 2 + 1);
            Game.Print("What is your name?", ConsoleColor.Yellow);
            while (string.IsNullOrEmpty(character.name))
            {
                character.name = Console.ReadLine();
            }
            Game.Print($"Welcome {character.name}!", ConsoleColor.Yellow);

            return character;
        }
    }
}
