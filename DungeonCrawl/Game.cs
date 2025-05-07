using System;
using System.Numerics;
using System.Xml.Serialization;
using DungeonCrawl;

namespace DungeonCrawl
{

	enum GameState
	{
		CharacterCreation,
		GameLoop,
		Inventory,
		DeathScreen,
		Quit
	}
	enum PlayerTurnResult
	{
		TurnOver,
		NewTurn,
		OpenInventory,
		NextLevel,
		BackToGame
	}
	internal enum ItemType
	{
		Weapon,
		Armor,
		Potion,
		Treasure
	}

	


	internal class Game
	{
		

		


		static void Main(string[] args)
		{

            PlayerCharacter playerCharacters = new PlayerCharacter();
            List<Monster> monsters = null;
			List<Item> items = null;
			PlayerCharacter player = null;
			Map currentLevel = null;
			Random random = new Random();

			List<int> dirtyTiles = new List<int>();
			List<string> messages = new List<string>();

			// Main loop
			GameState state = GameState.CharacterCreation;
			while (state != GameState.Quit)
			{
				switch (state)
				{
					case GameState.CharacterCreation:
						// Character creation screen
						player = PlayerCharacter.CreateCharacter();
						Console.CursorVisible = false;
						Console.Clear();

						// Map Creation 
						currentLevel = Map.CreateMap(random);

						// Enemy init
						monsters = Monster.CreateEnemies(currentLevel, random);
						// Item init
						items = Item.CreateItems(currentLevel, random);
						// Player init
						Map.PlacePlayerToMap(player, currentLevel);
						Map.PlaceStairsToMap(currentLevel);
						state = GameState.GameLoop;
						break;
					case GameState.GameLoop:
						Map.DrawMap(currentLevel, dirtyTiles);
						dirtyTiles.Clear();
						Monster.DrawEnemies(monsters);
						Item.DrawItems(items);

						PlayerCharacter.DrawPlayer(player);
						Map.DrawCommands();
						Map.DrawInfo(player, monsters, items, messages);
						// Draw map
						// Draw information
						// Wait for player command
						// Process player command
						while (true)
						{
							messages.Clear();
							PlayerTurnResult result = DoPlayerTurn(currentLevel, player, monsters, items, dirtyTiles, messages);
							Map.DrawInfo(player, monsters, items, messages);
							if (result == PlayerTurnResult.TurnOver)
							{
								break;
							}
							else if (result == PlayerTurnResult.OpenInventory)
							{
								Console.Clear();
								state = GameState.Inventory;
								break;
							}
							else if (result == PlayerTurnResult.NextLevel)
							{
								currentLevel = Map.CreateMap(random);
								monsters = Monster.CreateEnemies(currentLevel, random);
								items = Item.CreateItems(currentLevel, random);
								Map.PlacePlayerToMap(player, currentLevel);
								Map.PlaceStairsToMap(currentLevel);
								Console.Clear();
								break;
							}
						}
						// Either do computer turn or wait command again
						// Do computer turn
						// Process enemies
						Monster.ProcessEnemies(monsters, currentLevel, player, dirtyTiles, messages);

						Map.DrawInfo(player, monsters, items, messages);

						// Is player dead?
						if (player.hitpoints < 0)
						{
							state = GameState.DeathScreen;
						}

						break;
					case GameState.Inventory:
						// Draw inventory 
						PlayerTurnResult inventoryResult = Item.DrawInventory(player, messages);
						if (inventoryResult == PlayerTurnResult.BackToGame)
						{
							state = GameState.GameLoop;
							Map.DrawMapAll(currentLevel);
							Map.DrawInfo(player, monsters, items, messages);
						}
						// Read player command
						// Change back to game loop
						break;
					case GameState.DeathScreen:
						DrawBrickBg.DrawEndScreen(random);
						// Animation is over
						Console.SetCursorPosition(Console.WindowWidth/2 - 4, Console.WindowHeight / 2);
						Print("YOU DIED", ConsoleColor.Yellow);
						Console.SetCursorPosition(Console.WindowWidth/2 - 4, Console.WindowHeight / 2 + 1);
						while(true)
						{ 
							Print("Play again (y/n)", ConsoleColor.Gray);
							ConsoleKeyInfo answer = Console.ReadKey();
							if (answer.Key == ConsoleKey.Y)
							{
								state = GameState.CharacterCreation;
								break;
							}
							else if (answer.Key == ConsoleKey.N)
							{
								state = GameState.Quit;
								break;
							}
						}
						break;
				};
			}
			Console.ResetColor();
			Console.Clear();
			Console.CursorVisible = true;
		}

		public static void PrintLine(string text, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text);
		}
		public static void Print(string text, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.Write(text);
		}
		public static void PrintLine(string text)
		{
			Console.WriteLine(text);
		}
		public static void Print(string text)
		{
			Console.Write(text);
		}

		public static void Print(char symbol, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.Write(symbol);
		}

		

		
		

        /*
		 * Character functions
		 */
        

        

		

		

		

		

		

		
		
		

		
		


		
		

		
		

		
		

		

		

		

		

		
		static PlayerTurnResult DoPlayerTurn(Map level, PlayerCharacter character, List<Monster> enemies, List<Item> items, List<int> dirtyTiles, List<string> messages)
		{
			Vector2 playerMove = new Vector2(0, 0);
			while (true)
			{
				ConsoleKeyInfo key = Console.ReadKey();
				if (key.Key == ConsoleKey.W || key.Key == ConsoleKey.UpArrow)
				{
					playerMove.Y = -1;
					break;
				}
				else if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.DownArrow)
				{
					playerMove.Y = 1;
					break;
				}
				else if (key.Key == ConsoleKey.A || key.Key == ConsoleKey.LeftArrow)
				{
					playerMove.X = -1;
					break;
				}
				else if (key.Key == ConsoleKey.D || key.Key == ConsoleKey.RightArrow)
				{
					playerMove.X = 1;
					break;
				}
				// Other commands
				else if (key.Key == ConsoleKey.I)
				{
					return PlayerTurnResult.OpenInventory;
				}
			}

			int startTile = Monster.PositionToTileIndex(character.position, level);
			Vector2 destinationPlace = character.position + playerMove;

			if (PlayerCharacter.DoPlayerTurnVsEnemies(character, enemies, destinationPlace, messages))
			{
				return PlayerTurnResult.TurnOver;
			}

			if (Item.DoPlayerTurnVsItems(character, items, destinationPlace, messages))
			{
				return PlayerTurnResult.TurnOver;
			}

			// Check movement
			Map.Tile destination = Map.GetTileAtMap(level, destinationPlace);
			if (destination == Map.Tile.Floor)
			{
				character.position = destinationPlace;
				dirtyTiles.Add(startTile);
			}
			else if (destination == Map.Tile.Door)
			{
				messages.Add("You open a door");
				character.position = destinationPlace;
				dirtyTiles.Add(startTile);
			}
			else if (destination == Map.Tile.Wall)
			{
				messages.Add("You hit a wall");
			}
			else if (destination == Map.Tile.Stairs)
			{	
				messages.Add("You find stairs leading down");
				return PlayerTurnResult.NextLevel;
			}

			return PlayerTurnResult.TurnOver;
		}

		

		public static int GetDistanceBetween(Vector2 A, Vector2 B)
		{
			return (int)Vector2.Distance(A, B);
		}
	}
}
