using System.Numerics;

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
        public static void UseItem(PlayerCharacter character, Item item, List<string> messages)
        {
            switch (item.type)
            {
                case ItemType.Weapon:
                    character.weapon = item;
                    messages.Add($"You are now wielding a {item.name}");
                    break;
                case ItemType.Armor:
                    character.armor = item;
                    messages.Add($"You equip {item.name} on yourself.");
                    break;
                case ItemType.Potion:
                    character.hitpoints += item.quality;
                    if (character.hitpoints > character.maxHitpoints)
                    {
                        character.maxHitpoints = character.hitpoints;
                    }
                    messages.Add($"You drink a potion and gain {item.quality} hitpoints");
                    character.inventory.Remove(item);
                    break;
            }
        }
        public static Item CreateRandomItem(Random random, Vector2 position)
        {
            ItemType type = Enum.GetValues<ItemType>()[random.Next(4)];
            Item i = type switch
            {
                ItemType.Treasure => CreateItem("Book", type, 2, position),
                ItemType.Weapon => CreateItem("Sword", type, 3, position),
                ItemType.Armor => CreateItem("Helmet", type, 1, position),
                ItemType.Potion => CreateItem("Apple Juice", type, 1, position)
            };
            return i;
        }
        public static Item CreateItem(string name, ItemType type, int quality, Vector2 position)
        {
            Item i = new Item();
            i.name = name;
            i.type = type;
            i.quality = quality;
            i.position = position;
            return i;
        }
        public static List<Item> CreateItems(Map level, Random random)
        {
            List<Item> items = new List<Item>();

            for (int y = 0; y < level.height; y++)
            {
                for (int x = 0; x < level.width; x++)
                {
                    int ti = y * level.width + x;
                    if (level.Tiles[ti] == Map.Tile.Item)
                    {
                        Item m = CreateRandomItem(random, new Vector2(x, y));
                        items.Add(m);
                        level.Tiles[ti] = (sbyte)Map.Tile.Floor;
                    }
                }
            }
            return items;
        }

        public static void DrawItems(List<Item> items)
        {
            foreach (Item m in items)
            {
                Console.SetCursorPosition((int)m.position.X, (int)m.position.Y);
                char symbol = '$';
                ConsoleColor color = ConsoleColor.Yellow;
                switch (m.type)
                {
                    case ItemType.Armor:
                        symbol = '[';
                        color = ConsoleColor.White;
                        break;
                    case ItemType.Weapon:
                        symbol = '}';
                        color = ConsoleColor.Cyan;
                        break;
                    case ItemType.Treasure:
                        symbol = '$';
                        color = ConsoleColor.Yellow;
                        break;
                    case ItemType.Potion:
                        symbol = '!';
                        color = ConsoleColor.Red;
                        break;
                }
                Game.Print(symbol, color);
            }
        }
        public static PlayerTurnResult DrawInventory(PlayerCharacter character, List<string> messages)
        {
            Console.SetCursorPosition(1, 1);
            Game.PrintLine("Inventory. Select item by inputting the number next to it. Invalid input closes inventory");
            ItemType currentType = ItemType.Weapon;
            Game.PrintLine("Weapons", ConsoleColor.DarkCyan);
            for (int i = 0; i < character.inventory.Count; i++)
            {
                Item it = character.inventory[i];
                if (currentType == ItemType.Weapon && it.type == ItemType.Armor)
                {
                    currentType = ItemType.Armor;
                    Game.PrintLine("Armors", ConsoleColor.DarkRed);
                }
                else if (currentType == ItemType.Armor && it.type == ItemType.Potion)
                {
                    currentType = ItemType.Potion;
                    Game.PrintLine("Potions", ConsoleColor.DarkMagenta);
                }
                Game.Print($"{i} ", ConsoleColor.Cyan);
                Game.PrintLine($"{it.name} ({it.quality})", ConsoleColor.White);
            }
            while (true)
            {
                Game.Print("Choose item: ", ConsoleColor.Yellow);
                string choiceStr = Console.ReadLine();
                int selectionindex = 0;
                if (int.TryParse(choiceStr, out selectionindex))
                {
                    if (selectionindex >= 0 && selectionindex < character.inventory.Count)
                    {
                        Item.UseItem(character, character.inventory[selectionindex], messages);
                        break;
                    }
                }
                else
                {
                    messages.Add("No such item");
                }
            };
            return PlayerTurnResult.BackToGame;
        }
    }
}
