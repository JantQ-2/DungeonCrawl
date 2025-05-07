using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawl
{
    internal class Item
    {
        public string name;
        public int quality; // means different things depending on the type
        public Vector2 position;
        public ItemType type;

        public static bool DoPlayerTurnVsItems(PlayerCharacter character, List<Item> items, Vector2 destinationPlace, List<string> messages)
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
                    PlayerCharacter.GiveItem(character, item);
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
