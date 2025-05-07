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
        public void GiveItem(PlayerCharacter character, Item item)
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
        static int GetCharacterDamage(PlayerCharacter character)
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
        static int GetCharacterDefense(PlayerCharacter character)
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
        static bool DoPlayerTurnVsEnemies(PlayerCharacter character, List<Monster> enemies, Vector2 destinationPlace, List<string> messages)
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
        static bool DoPlayerTurnVsItems(PlayerCharacter character, List<Item> items, Vector2 destinationPlace, List<string> messages)
		{
			// Check items
			Item toRemoveItem = null;
			foreach (Item item in items)
			{
				if (item.position == destinationPlace)
				{
					string itemMessage = $"You find a ";
					switch (item.type)
					{
						case ItemType.Armor:
							itemMessage += $"{item.name}, it fits you well";
							break;
						case ItemType.Weapon:
							itemMessage += $"{item.name} to use in battle";
							break;
						case ItemType.Potion:
							itemMessage += $"potion of {item.name}";
							break;
						case ItemType.Treasure:
							itemMessage += $"valuable {item.name} and get {item.quality} gold!";
							break;
					};
					messages.Add(itemMessage);
					toRemoveItem = item;
					GiveItem(character, item);
					break;
				}
			}
			if (toRemoveItem != null)
			{
				items.Remove(toRemoveItem);
			}
			return false;
		}


    }
}
