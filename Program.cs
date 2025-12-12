// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.IO;
// using System.Text.Json; 
// using System.Text.Json.Serialization; 

// namespace ShadowsOfEldoria
// {
//     // ENUMS
//     public enum CharacterClass { Warrior, Mage, Rogue }
//     public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }
//     public enum TrapType { Spike, PoisonGas, MagicRune, Arrow }

//     // ABILITY CLASS
//     public class Ability
//     {
//         public string Name { get; set; }
//         public int ManaCost { get; set; }
//         public int BaseDamage { get; set; }
//         public CharacterClass RequiredClass { get; set; } 
//         public float CritChanceBonus { get; set; }
//         public bool IsHealing { get; set; }

//         public Ability(string name, int manaCost, int baseDamage, CharacterClass requiredClass, float critChanceBonus = 0f, bool isHealing = false)
//         {
//             Name = name;
//             ManaCost = manaCost;
//             BaseDamage = baseDamage;
//             RequiredClass = requiredClass;
//             CritChanceBonus = critChanceBonus;
//             IsHealing = isHealing;
//         }
//     }

//     // BASE CHARACTER CLASS
//     public class Character
//     {
//         public string Name { get; set; }
//         public int Level { get; set; }
//         public int Health { get; set; }
//         public int MaxHealth { get; set; }
//         public int Mana { get; set; }
//         public int MaxMana { get; set; }
//         public int AttackPower { get; set; }
//         public int Defense { get; set; }
//         public int Agility { get; set; }
//         public int Experience { get; set; }
//         public CharacterClass Class { get; set; }
//         public List<Item> Inventory { get; set; }
//         public Item EquippedWeapon { get; set; }
//         public Item EquippedArmor { get; set; }
//         public List<Ability> Abilities { get; set; } = new List<Ability>();

//         private Random rand = new Random();

//         // FIX 1: Add a parameterless constructor for JSON deserialization to work reliably.
//         public Character() { } 

//         // Main constructor (used for *new* game creation)
//         public Character(string name, CharacterClass charClass)
//         {
//             Name = name;
//             Class = charClass;
//             Level = 1;
//             Experience = 0;
//             Inventory = new List<Item>();
            
//             InitializeStats(charClass);
            
//             Health = MaxHealth; 
//             Mana = MaxMana;
//         }

//         // Helper method for setting base stats
//         private void InitializeStats(CharacterClass charClass)
//         {
//             switch (charClass)
//             {
//                 case CharacterClass.Warrior:
//                     MaxHealth = 120; MaxMana = 30; AttackPower = 15; Defense = 12; Agility = 5;
//                     break;
//                 case CharacterClass.Mage:
//                     MaxHealth = 70; MaxMana = 100; AttackPower = 8; Defense = 5; Agility = 7;
//                     break;
//                 case CharacterClass.Rogue:
//                     MaxHealth = 90; MaxMana = 50; AttackPower = 12; Defense = 7; Agility = 15;
//                     break;
//             }
//         }
        
//         // NEW: Recalculates stats based on current level (used to reverse GM cheat)
//         public void RecalculateStats()
//         {
//             // 1. Reset to base level 1 stats
//             InitializeStats(this.Class);
            
//             // 2. Apply level-up bonuses for levels 2 through current Level
//             for (int i = 2; i <= this.Level; i++)
//             {
//                 MaxHealth += 10;
//                 MaxMana += 10;
//                 AttackPower += 3;
//                 Defense += 2;
//                 Agility += 1;
//             }

//             // 3. Re-apply equipped item bonuses (keeping your loot buffs)
//             if (EquippedArmor != null)
//             {
//                 ApplyEquipStats(EquippedArmor);
//             }

//             // 4. Restore current HP/Mana, ensuring they don't exceed the new max
//             Health = Math.Min(Health, MaxHealth);
//             Mana = Math.Min(Mana, MaxMana);
            
//             Console.WriteLine("\n✨ Your core stats have been reset and recalculated based on your current level!");
//         }

//         public void ApplyEquipStats(Item item) 
//         {
//             AttackPower += item.AttackBonus;
//             Defense += item.DefenseBonus;
//             MaxHealth += item.HealthBonus;
//             Health += item.HealthBonus;
//         }

//         public void RemoveEquipStats(Item item) 
//         {
//             AttackPower -= item.AttackBonus;
//             Defense -= item.DefenseBonus;
//             MaxHealth -= item.HealthBonus;
//             Health = Math.Min(Health, MaxHealth); 
//         }
        
//         public void EquipItem(Item item)
//         {
//             if (!Inventory.Contains(item)) return;

//             if (item.IsWeapon)
//             {
//                 if (EquippedWeapon != null) UnequipItem(EquippedWeapon);
//                 EquippedWeapon = item;
//                 Console.WriteLine($"Equipped: {item.Name}. Attack Power increased by {item.AttackBonus}.");
//             }
//             else if (item.IsArmor)
//             {
//                 if (EquippedArmor != null) UnequipItem(EquippedArmor);
//                 EquippedArmor = item;
//                 Console.WriteLine($"Equipped: {item.Name}. Defense increased by {item.DefenseBonus}.");
//                 ApplyEquipStats(item);
//             }
//             else
//             {
//                 Console.WriteLine($"{item.Name} cannot be equipped.");
//             }
//         }
        
//         public void UnequipItem(Item item)
//         {
//             if (item == EquippedWeapon)
//             {
//                 EquippedWeapon = null;
//                 Console.WriteLine($"Unequipped: {item.Name}.");
//             }
//             else if (item == EquippedArmor)
//             {
//                 EquippedArmor = null;
//                 RemoveEquipStats(item);
//                 Console.WriteLine($"Unequipped: {item.Name}.");
//             }
//             else
//             {
//                 Console.WriteLine($"{item.Name} is not equipped.");
//             }
//         }
        
//         public void LearnAbility(Ability ability)
//         {
//             if (!Abilities.Any(a => a.Name == ability.Name))
//             {
//                 Abilities.Add(ability);
//                 Console.WriteLine($"✨ {Name} learned a new ability: {ability.Name}!");
//             }
//         }

//         public bool AttackEnemy(Enemy enemy)
//         {
//             int totalAttack = AttackPower + (EquippedWeapon?.AttackBonus ?? 0);
            
//             bool isCritical = rand.Next(100) < Agility;
//             int damage = isCritical ? totalAttack * 2 : totalAttack;
            
//             if (isCritical)
//                 Console.WriteLine("CRITICAL HIT!");
            
//             enemy.TakeDamage(damage);
//             return true;
//         }

//         public void UseAbility(Enemy enemy)
//         {
//             if (!Abilities.Any())
//             {
//                 Console.WriteLine("You have no special abilities to use.");
//                 return;
//             }

//             Console.WriteLine("\n--- Abilities ---");
//             for (int i = 0; i < Abilities.Count; i++)
//             {
//                 Console.WriteLine($"{i + 1}. {Abilities[i].Name} (Cost: {Abilities[i].ManaCost} MP)");
//             }
//             Console.Write("Choose an ability (or 0 to cancel): ");

//             if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= Abilities.Count)
//             {
//                 Ability selectedAbility = Abilities[choice - 1];
//                 if (Mana >= selectedAbility.ManaCost)
//                 {
//                     Mana -= selectedAbility.ManaCost;
                    
//                     if (selectedAbility.IsHealing) 
//                     {
//                         int healAmount = Math.Abs(selectedAbility.BaseDamage) + (MaxMana / 10);
//                         Health = Math.Min(MaxHealth, Health + healAmount);
//                         Console.WriteLine($"✨ {Name} casts {selectedAbility.Name} and restores {healAmount} HP!");
//                     }
//                     else 
//                     {
//                         Console.WriteLine($"{Name} uses {selectedAbility.Name}!");
//                         int totalDamage = selectedAbility.BaseDamage + AttackPower;
                        
//                         bool isCritical = false;
//                         if (selectedAbility.CritChanceBonus > 0f)
//                         {
//                             isCritical = rand.Next(100) < (Agility + (int)selectedAbility.CritChanceBonus);
//                         }
                        
//                         if (isCritical)
//                         {
//                             Console.WriteLine("✨ ASSASSIN'S CRIT!");
//                             totalDamage *= 3; 
//                         }

//                         enemy.TakeDamage(totalDamage);
//                     }
//                 }
//                 else
//                 {
//                     Console.WriteLine("Not enough mana for this ability!");
//                 }
//             }
//             else if (choice != 0)
//             {
//                 Console.WriteLine("Invalid choice.");
//             }
//         }

//         public void TakeDamage(int damage)
//         {
//             int actualDamage = Math.Max(1, damage - Defense - (EquippedArmor?.DefenseBonus ?? 0));
//             Health -= actualDamage;
//             Console.WriteLine($"{Name} takes {actualDamage} damage! HP: {Health}/{MaxHealth}");
//         }

//         public void GainExperience(int exp)
//         {
//             Experience += exp;
//             Console.WriteLine($"{Name} gains {exp} experience!");
            
//             int expNeeded = Level * 100;
//             if (Experience >= expNeeded)
//             {
//                 LevelUp();
//             }
//         }

//         private void LevelUp()
//         {
//             Level++;
//             Experience = 0;
            
//             MaxHealth += 10;
//             MaxMana += 10;
//             AttackPower += 3;
//             Defense += 2;
//             Agility += 1;
            
//             Health = MaxHealth;
//             Mana = MaxMana;
            
//             Console.WriteLine($"\n*** LEVEL UP! {Name} is now level {Level}! ***");
//             Console.WriteLine($"Stats increased! HP: {MaxHealth}, Mana: {MaxMana}, ATK: {AttackPower}");
//         }

//         public void AddItem(Item item)
//         {
//             Inventory.Add(item);
//             Console.WriteLine($"Obtained: {item.Name} ({item.Rarity})");
//         }
//     }

//     // ENEMY CLASS
//     public class Enemy
//     {
//         public string Name { get; set; }
//         public int Health { get; set; }
//         public int MaxHealth { get; set; }
//         public int Attack { get; set; }
//         public int Defense { get; set; }
//         public int ExpReward { get; set; }
//         public bool IsBoss { get; set; }

//         public Enemy(string name, int health, int attack, int defense, int expReward, bool isBoss = false)
//         {
//             Name = name;
//             MaxHealth = health;
//             Health = health;
//             Attack = attack;
//             Defense = defense;
//             ExpReward = expReward;
//             IsBoss = isBoss;
//         }

//         public void TakeDamage(int damage)
//         {
//             int actualDamage = Math.Max(1, damage - Defense);
//             Health -= actualDamage;
//             Console.WriteLine($"{Name} takes {actualDamage} damage! HP: {Health}/{MaxHealth}");
//         }

//         public void AttackPlayer(Character player)
//         {
//             player.TakeDamage(Attack);
//         }

//         public bool IsAlive()
//         {
//             return Health > 0;
//         }
//     }

//     // ITEM CLASS
//     public class Item
//     {
//         public string Name { get; set; }
//         public ItemRarity Rarity { get; set; }
//         public int AttackBonus { get; set; }
//         public int DefenseBonus { get; set; }
//         public int HealthBonus { get; set; }
//         public bool IsWeapon { get; set; }
//         public bool IsArmor { get; set; }

//         public Item(string name, ItemRarity rarity, int attackBonus = 0, int defenseBonus = 0, int healthBonus = 0, bool isWeapon = false, bool isArmor = false)
//         {
//             Name = name;
//             Rarity = rarity;
//             AttackBonus = attackBonus;
//             DefenseBonus = defenseBonus;
//             HealthBonus = healthBonus;
//             IsWeapon = isWeapon;
//             IsArmor = isArmor;
//         }
//     }

//     // TRAP CLASS
//     public class Trap
//     {
//         public TrapType Type { get; set; }
//         public int Damage { get; set; }
//         public bool IsDisarmed { get; set; }

//         public Trap(TrapType type)
//         {
//             Type = type;
//             IsDisarmed = false;
            
//             switch (type)
//             {
//                 case TrapType.Spike: Damage = 20; break;
//                 case TrapType.PoisonGas: Damage = 15; break;
//                 case TrapType.MagicRune: Damage = 25; break;
//                 case TrapType.Arrow: Damage = 18; break;
//             }
//         }

//         public void Trigger(Character player)
//         {
//             if (IsDisarmed)
//             {
//                 Console.WriteLine("The trap has already been disarmed.");
//                 return;
//             }

//             Random rand = new Random();
            
//             switch (Type)
//             {
//                 case TrapType.Spike:
//                     if (player.Class == CharacterClass.Warrior)
//                     {
//                         Console.WriteLine("Your warrior's armor absorbs most of the spike trap damage!");
//                         player.TakeDamage(Damage / 2);
//                     }
//                     else
//                     {
//                         Console.WriteLine("You trigger a spike trap!");
//                         player.TakeDamage(Damage);
//                     }
//                     break;
                    
//                 case TrapType.PoisonGas:
//                     Console.WriteLine("Poison gas fills the room!");
//                     player.TakeDamage(Damage);
//                     break;
                    
//                 case TrapType.MagicRune:
//                     if (player.Class == CharacterClass.Mage)
//                     {
//                         Console.WriteLine("You sense the magical trap and avoid it!");
//                     }
//                     else
//                     {
//                         Console.WriteLine("A magic rune explodes!");
//                         player.TakeDamage(Damage);
//                     }
//                     break;
                    
//                 case TrapType.Arrow:
//                     if (rand.Next(100) < player.Agility * 3)
//                     {
//                         Console.WriteLine("You dodge the arrow trap!");
//                     }
//                     else
//                     {
//                         Console.WriteLine("An arrow shoots from the wall!");
//                         player.TakeDamage(Damage);
//                     }
//                     break;
//             }
//         }

//         public bool AttemptDisarm(Character player)
//         {
//             if (IsDisarmed) return true;

//             Random rand = new Random();
//             int chance = player.Class == CharacterClass.Rogue ? 80 : 30;
            
//             if (rand.Next(100) < chance)
//             {
//                 Console.WriteLine($"Successfully disarmed the {Type} trap!");
//                 IsDisarmed = true;
//                 return true;
//             }
//             else
//             {
//                 Console.WriteLine("Failed to disarm the trap!");
//                 Trigger(player);
//                 return false;
//             }
//         }
//     }

//     // STORY PROGRESSION CLASS (STATIC)
//     public static class StoryProgression
//     {
//         private static Dictionary<int, string> Chapters = new Dictionary<int, string>()
//         {
//             { 1, "The King's Summons: You arrive in the Capital of Eldoria. Your first task is to investigate the old guard barracks for clues. (Tip: Use 'Explore' to gain levels before advancing the plot.)" },
//             { 2, "The Dark Barracks: You've found a secret passage beneath the barracks. Defeat the powerful 'Chapter Guardian' to proceed to the next chapter." },
//             { 3, "The Cultist Hideout: Deeper underground, you find a note mentioning a 'Shadow Nexus' beneath the Iron Peaks. You must travel North." },
//             { 4, "Iron Peaks Ascent: The path is treacherous and guarded by mountain beasts. The 'Chapter Guardian' here is particularly tough." },
//             { 5, "The Shadow Nexus: You have reached the final layer. The High Priest of the Shadow Cult awaits. Prepare for the final battle!" }
//         };

//         public static string GetChapterDescription(int chapter)
//         {
//             return Chapters.GetValueOrDefault(chapter, "The End: The fate of Eldoria is decided.");
//         }

//         public static bool IsLastChapter(int chapter) => chapter >= Chapters.Keys.Max();
//     }

//     // SAVE DATA CLASS
//     public class SaveData
//     {
//         public Character Player { get; set; }
//         public int CurrentStoryChapter { get; set; }
//         public string LastCheckpointName { get; set; }
//     }

//     // GAME CLASS
//     public class Game
//     {
//         private Character player;
//         private Random rand = new Random();
//         private int currentStoryChapter = 1;
//         private const string SAVE_FILE = "eldoria_save.json";

//         // GM CHEAT CONSTANTS
//         private const string CHEAT_WORD = "EldoriaGM"; 
//         private const string CHEAT_OPTION = "9";
//         private const string RESET_CHEAT_OPTION = "0";

//         public void Start()
//         {
//             Console.WriteLine("╔════════════════════════════════════╗");
//             Console.WriteLine("║    SHADOWS OF ELDORIA: AWAKENING   ║");
//             Console.WriteLine("╚════════════════════════════════════╝\n");
            
//             if (File.Exists(SAVE_FILE))
//             {
//                 Console.Write("Save file found. Load game? (y/n): ");
//                 if (Console.ReadLine()?.ToLower() == "y")
//                 {
//                     LoadGame();
//                 }
//                 else
//                 {
//                     CreateCharacter();
//                 }
//             }
//             else
//             {
//                 CreateCharacter();
//             }

//             GiveStartingAbilities();

//             Console.WriteLine($"\nWelcome, {player.Name} the {player.Class}!");
//             Console.WriteLine("--- Current Plot ---");
//             Console.WriteLine(StoryProgression.GetChapterDescription(currentStoryChapter));
            
//             MainGameLoop();
//         }

//         private void CreateCharacter()
//         {
//             Console.Write("Enter your character's name: ");
//             string name = Console.ReadLine() ?? "Hero";
            
//             Console.WriteLine("\nChoose your class:");
//             Console.WriteLine("1. Warrior - High HP, strong defense, reliable damage.");
//             Console.WriteLine("2. Mage - Powerful magic, healing, high mana.");
//             Console.WriteLine("3. Rogue - Critical hits, high agility, trap expert.");
//             Console.Write("\nChoice (1-3): ");
            
//             int choice = 0;
//             if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
//             {
//                 Console.WriteLine("Invalid choice, defaulting to Warrior.");
//                 choice = 1;
//             }
            
//             CharacterClass charClass = (CharacterClass)(choice - 1);
//             player = new Character(name, charClass); 
            
//             // Give starting weapon
//             Item startWeapon = charClass switch
//             {
//                 CharacterClass.Warrior => new Item("Iron Sword", ItemRarity.Common, 5, 0, 0, true, false),
//                 CharacterClass.Mage => new Item("Wooden Staff", ItemRarity.Common, 3, 0, 0, true, false),
//                 CharacterClass.Rogue => new Item("Rusty Dagger", ItemRarity.Common, 4, 0, 0, true, false),
//                 _ => new Item("Stick", ItemRarity.Common, 2, 0, 0, true, false)
//             };
            
//             player.EquippedWeapon = startWeapon;
//             player.AddItem(startWeapon);
//         }

//         private void GiveStartingAbilities()
//         {
//             player.Abilities.Clear(); 
            
//             switch (player.Class)
//             {
//                 case CharacterClass.Warrior:
//                     player.LearnAbility(new Ability("Power Strike", 15, 20, CharacterClass.Warrior));
//                     break;
//                 case CharacterClass.Mage:
//                     player.LearnAbility(new Ability("Fireball", 25, 30, CharacterClass.Mage));
//                     player.LearnAbility(new Ability("Heal", 10, -15, CharacterClass.Mage, isHealing: true)); 
//                     break;
//                 case CharacterClass.Rogue:
//                     player.LearnAbility(new Ability("Backstab", 20, 25, CharacterClass.Rogue, critChanceBonus: 40f)); 
//                     break;
//             }
//         }

//         private void MainGameLoop()
//         {
//             bool playing = true;
            
//             while (playing && player.Health > 0)
//             {
//                 Console.WriteLine($"\n--- Chapter {currentStoryChapter} Menu ---");
//                 Console.WriteLine($"Goal: {StoryProgression.GetChapterDescription(currentStoryChapter).Split(':')[1].Trim()}");
//                 Console.WriteLine("1. Explore (Random Encounter/Treasure)");
//                 Console.WriteLine("2. Story Progression (Advance Plot)");
//                 Console.WriteLine("3. Inventory & Equipment");
//                 Console.WriteLine("4. View Status");
//                 Console.WriteLine("5. Rest & Checkpoint");
//                 Console.WriteLine("6. Exit Game");
//                 // Option 9 and 0 are hidden
//                 Console.Write("\nChoice: ");
                
//                 string choice = Console.ReadLine();
                
//                 switch (choice)
//                 {
//                     case "1":
//                         ExploreDungeon(currentStoryChapter);
//                         break;
//                     case "2":
//                         AdvanceStory();
//                         break;
//                     case "3":
//                         DisplayInventoryOptions();
//                         break;
//                     case "4":
//                         ShowStatus();
//                         break;
//                     case "5":
//                         Rest();
//                         SaveGame(); 
//                         break;
//                     case "6":
//                         playing = false;
//                         Console.WriteLine("Thanks for playing Shadows of Eldoria!");
//                         break;
//                     case CHEAT_OPTION: // GM Buff Cheat
//                         ProcessCheat();
//                         break;
//                     case RESET_CHEAT_OPTION: // GM Reset Cheat
//                         ProcessResetCheat();
//                         break;
//                     default:
//                         Console.WriteLine("Invalid choice!");
//                         break;
//                 }
//             }
            
//             if (player.Health <= 0)
//             {
//                 Console.WriteLine("\n*** GAME OVER ***");
//                 Console.WriteLine($"{player.Name} has fallen in battle...");
//             }
//         }

//         // GM Cheat Method (Buff Only)
//         private void ProcessCheat()
//         {
//             Console.WriteLine("\n[GM CONSOLE ACCESS]: Enter secret code to engage Buff Protocol.");
//             Console.Write("Secret Code: ");
//             string code = Console.ReadLine();

//             if (code == CHEAT_WORD)
//             {
//                 Console.WriteLine("\n*** CHEAT ACTIVATED: ELDORIA GM PROTOCOL ENGAGED ***");

//                 // Apply massive stats
//                 player.Level = 50; 
//                 player.MaxHealth = 5000;
//                 player.MaxMana = 5000;
//                 player.AttackPower = 500;
//                 player.Defense = 500;
//                 player.Agility = 100;
//                 player.Health = player.MaxHealth;
//                 player.Mana = player.MaxMana;

//                 Console.WriteLine($"\n{player.Name} is now Level {player.Level}! You feel GODLIKE.");
//                 Console.WriteLine($"You remain on Chapter {currentStoryChapter}, but your stats are maxed.");
                
//                 SaveGame(); 
//             }
//             else
//             {
//                 Console.WriteLine("[ACCESS DENIED] That is not the correct protocol.");
//             }
//         }

//         // GM Reset Method
//         private void ProcessResetCheat()
//         {
//             Console.WriteLine("\n[GM CONSOLE ACCESS]: Enter secret code to engage Stat Reset Protocol.");
//             Console.Write("Secret Code: ");
//             string code = Console.ReadLine();

//             if (code == CHEAT_WORD)
//             {
//                 Console.WriteLine("\n*** CHEAT ACTIVATED: STAT RESET PROTOCOL ENGAGED ***");
//                 player.RecalculateStats();
//                 SaveGame(); 
//             }
//             else
//             {
//                 Console.WriteLine("[ACCESS DENIED] That is not the correct protocol.");
//             }
//         }

//         private void AdvanceStory()
//         {
//             if (StoryProgression.IsLastChapter(currentStoryChapter))
//             {
//                 Console.WriteLine("\nThere are no more main story events. You are at the final stage.");
//                 return;
//             }

//             Console.WriteLine("\nAttempting to advance the main plot...");
            
//             if (player.Level < currentStoryChapter + 1)
//             {
//                 Console.WriteLine($"You feel unprepared. You need to be at least Level {currentStoryChapter + 1} to proceed.");
//                 return;
//             }

//             Console.WriteLine("\nThe way is guarded by a powerful Chapter Guardian!");
//             Enemy boss = new Enemy("Chapter Guardian", 100 + currentStoryChapter * 50, 20 + currentStoryChapter * 5, 10 + currentStoryChapter * 3, 300 + currentStoryChapter * 100, true);
//             Battle(boss);
            
//             if (player.Health > 0 && !boss.IsAlive())
//             {
//                 currentStoryChapter++;
//                 Console.WriteLine($"\n*** You overcome the challenge and advance to Chapter {currentStoryChapter}! ***");
//                 Console.WriteLine(StoryProgression.GetChapterDescription(currentStoryChapter));
//                 SaveGame(); 
//             }
//             else if (player.Health > 0)
//             {
//                 Console.WriteLine("The path is clear, but you must still prepare before facing the next Chapter Guardian.");
//             }
//         }

//         private void DisplayInventoryOptions()
//         {
//             Console.WriteLine("\n--- Inventory & Equipment Menu ---");
//             Console.WriteLine("1. View Items");
//             Console.WriteLine("2. Equip/Unequip Item");
//             Console.Write("\nChoice: ");

//             switch (Console.ReadLine())
//             {
//                 case "1":
//                     ShowInventory();
//                     break;
//                 case "2":
//                     ManageEquipment();
//                     break;
//                 default:
//                     Console.WriteLine("Returning to main menu.");
//                     break;
//             }
//         }

//         private void ManageEquipment()
//         {
//             ShowInventory();
//             Console.Write("Enter the number of the item to Equip/Unequip (0 to cancel): ");
//             if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= player.Inventory.Count)
//             {
//                 Item selectedItem = player.Inventory[index - 1];
                
//                 if (selectedItem.IsWeapon)
//                 {
//                     if (selectedItem == player.EquippedWeapon)
//                         player.UnequipItem(selectedItem);
//                     else
//                         player.EquipItem(selectedItem);
//                 }
//                 else if (selectedItem.IsArmor)
//                 {
//                     if (selectedItem == player.EquippedArmor)
//                         player.UnequipItem(selectedItem);
//                     else
//                         player.EquipItem(selectedItem);
//                 }
//                 else
//                 {
//                     Console.WriteLine("This item is not a weapon or armor and cannot be equipped.");
//                 }
//             }
//             else
//             {
//                 Console.WriteLine("Invalid selection or canceled.");
//             }
//         }

//         private void SaveGame()
//         {
//             try
//             {
//                 var saveData = new SaveData
//                 {
//                     Player = this.player,
//                     CurrentStoryChapter = this.currentStoryChapter,
//                     LastCheckpointName = $"Chapter {this.currentStoryChapter} Checkpoint"
//                 };
                
//                 string jsonString = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
//                 File.WriteAllText(SAVE_FILE, jsonString);
//                 Console.WriteLine("\n✔ Checkpoint saved successfully! Game state secured.");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"\n✘ Error saving game: {ex.Message}");
//             }
//         }

//         private void LoadGame()
//         {
//             try
//             {
//                 string jsonString = File.ReadAllText(SAVE_FILE);
//                 var saveData = JsonSerializer.Deserialize<SaveData>(jsonString); 
                
//                 this.player = saveData.Player;
//                 this.currentStoryChapter = saveData.CurrentStoryChapter;
                
//                 Console.WriteLine($"\n✔ Game loaded from {saveData.LastCheckpointName}.");
                
//                 if (player.EquippedArmor != null)
//                 {
//                      player.ApplyEquipStats(player.EquippedArmor);
//                      Console.WriteLine($"Applied bonus from {player.EquippedArmor.Name}.");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"\n✘ Error loading game. Starting a new game. ({ex.Message})");
//                 CreateCharacter(); 
//             }
//         }

//         private void ExploreDungeon(int level)
//         {
//             Console.WriteLine("\nYou venture deeper into the dungeon...");
            
//             int encounter = rand.Next(100);
            
//             if (encounter < 30)
//             {
//                 EncounterTrap();
//             }
//             else if (encounter < 80)
//             {
//                 EncounterEnemy(level);
//             }
//             else
//             {
//                 FindTreasure(level);
//             }
//         }

//         private void EncounterTrap()
//         {
//             TrapType trapType = (TrapType)rand.Next(4);
//             Trap trap = new Trap(trapType);
            
//             Console.WriteLine($"\n⚠ You encounter a {trapType} trap!");
//             Console.WriteLine("1. Try to disarm it");
//             Console.WriteLine("2. Trigger it and take the hit");
//             Console.WriteLine("3. Try to avoid it");
//             Console.Write("\nChoice: ");
            
//             string choice = Console.ReadLine();
            
//             switch (choice)
//             {
//                 case "1": trap.AttemptDisarm(player); break;
//                 case "2": trap.Trigger(player); break;
//                 case "3":
//                     if (rand.Next(100) < player.Agility * 2)
//                     {
//                         Console.WriteLine("You successfully avoid the trap!");
//                     }
//                     else
//                     {
//                         trap.Trigger(player);
//                     }
//                     break;
//                 default: trap.Trigger(player); break;
//             }
//         }

//         private void EncounterEnemy(int level)
//         {
//             Enemy enemy = GenerateEnemy(level);
            
//             Console.WriteLine($"\n⚔ A {enemy.Name} appears!");
            
//             Battle(enemy);
//         }

//         private Enemy GenerateEnemy(int level)
//         {
//             string[] normalEnemies = { "Goblin", "Skeleton", "Wolf", "Bandit", "Spider" };
//             string[] bossEnemies = { "Goblin King", "Lich Lord", "Dragon", "Dark Knight" };
            
//             bool isBoss = level % 5 == 0 && rand.Next(100) < 40;
            
//             if (isBoss)
//             {
//                 string name = bossEnemies[rand.Next(bossEnemies.Length)];
//                 return new Enemy(name, 100 + level * 30, 15 + level * 3, 8 + level * 2, 200 + level * 50, true);
//             }
//             else
//             {
//                 string name = normalEnemies[rand.Next(normalEnemies.Length)];
//                 return new Enemy(name, 30 + level * 10, 8 + level * 2, 3 + level, 50 + level * 10);
//             }
//         }

//         private void Battle(Enemy enemy)
//         {
//             Console.WriteLine($"\nBattle Start! {player.Name} vs {enemy.Name}");
            
//             while (player.Health > 0 && enemy.IsAlive())
//             {
//                 Console.WriteLine($"\n{player.Name}: {player.Health}/{player.MaxHealth} HP | {player.Mana}/{player.MaxMana} MP");
//                 Console.WriteLine($"{enemy.Name}: {enemy.Health}/{enemy.MaxHealth} HP");
//                 Console.WriteLine("\n1. Attack (Basic)");
//                 Console.WriteLine("2. Use Skill/Spell"); 
//                 Console.WriteLine("3. Run");
//                 Console.Write("\nChoice: ");
                
//                 string choice = Console.ReadLine();
                
//                 switch (choice)
//                 {
//                     case "1":
//                         player.AttackEnemy(enemy);
//                         break;
//                     case "2":
//                         player.UseAbility(enemy); 
//                         break;
//                     case "3":
//                         if (rand.Next(100) < 50)
//                         {
//                             Console.WriteLine("You successfully escaped!");
//                             return;
//                         }
//                         else
//                         {
//                             Console.WriteLine("Failed to escape!");
//                         }
//                         break;
//                     default:
//                         Console.WriteLine("Invalid choice! Lost turn!");
//                         break;
//                 }
                
//                 if (enemy.IsAlive() && player.Health > 0)
//                 {
//                     enemy.AttackPlayer(player);
                    
//                     if (enemy.IsBoss && enemy.Health < enemy.MaxHealth / 2 && rand.Next(100) < 30)
//                     {
//                         Console.WriteLine($"\n{enemy.Name} unleashes a devastating special attack!");
//                         player.TakeDamage(enemy.Attack * 2);
//                     }
//                 }
//             }
            
//             if (!enemy.IsAlive())
//             {
//                 Console.WriteLine($"\n*** Victory! {enemy.Name} defeated! ***");
//                 player.GainExperience(enemy.ExpReward);
                
//                 if (enemy.IsBoss)
//                 {
//                     Console.WriteLine("\n🏆 BOSS DEFEATED! Legendary loot obtained!");
//                     DropLoot(ItemRarity.Legendary);
//                 }
//                 else if (rand.Next(100) < 40)
//                 {
//                     DropLoot(ItemRarity.Uncommon);
//                 }
//             }
//         }

//         private void FindTreasure(int level)
//         {
//             Console.WriteLine("\n💎 You found a treasure chest!");
            
//             ItemRarity rarity = rand.Next(100) switch
//             {
//                 < 50 => ItemRarity.Common,
//                 < 80 => ItemRarity.Uncommon,
//                 < 95 => ItemRarity.Rare,
//                 < 99 => ItemRarity.Epic,
//                 _ => ItemRarity.Legendary
//             };
            
//             DropLoot(rarity);
//         }

//         private void DropLoot(ItemRarity rarity)
//         {
//             string[] weaponNames = { "Sword", "Staff", "Dagger", "Axe", "Bow" };
//             string[] armorNames = { "Helmet", "Chestplate", "Boots", "Gloves" };
            
//             bool isWeapon = rand.Next(2) == 0;
//             string itemName = isWeapon ? weaponNames[rand.Next(weaponNames.Length)] : armorNames[rand.Next(armorNames.Length)];
            
//             int multiplier = rarity switch
//             {
//                 ItemRarity.Common => 1, ItemRarity.Uncommon => 2, ItemRarity.Rare => 3, 
//                 ItemRarity.Epic => 5, ItemRarity.Legendary => 8, _ => 1
//             };
            
//             string prefix = rarity switch
//             {
//                 ItemRarity.Epic => "Enchanted ", ItemRarity.Legendary => "Mythical ", _ => ""
//             };
            
//             Item item = new Item(
//                 prefix + itemName, rarity,
//                 isWeapon ? 5 * multiplier : 0, !isWeapon ? 3 * multiplier : 0, 0,
//                 isWeapon, !isWeapon
//             );
            
//             player.AddItem(item);
//         }

//         private void ShowStatus()
//         {
//             int totalAttack = player.AttackPower + (player.EquippedWeapon?.AttackBonus ?? 0);
//             int totalDefense = player.Defense + (player.EquippedArmor?.DefenseBonus ?? 0);

//             Console.WriteLine($"\n=== {player.Name} the {player.Class} ===");
//             Console.WriteLine($"Level: {player.Level} (EXP: {player.Experience}/{player.Level * 100})");
//             Console.WriteLine($"Health: {player.Health}/{player.MaxHealth} | Mana: {player.Mana}/{player.MaxMana}");
//             Console.WriteLine($"Total Attack: {totalAttack} (Base: {player.AttackPower})");
//             Console.WriteLine($"Total Defense: {totalDefense} (Base: {player.Defense})");
//             Console.WriteLine($"Agility: {player.Agility}");
//             Console.WriteLine($"Equipped Weapon: {player.EquippedWeapon?.Name ?? "None"}");
//             Console.WriteLine($"Equipped Armor: {player.EquippedArmor?.Name ?? "None"}");
//         }

//         private void ShowInventory()
//         {
//             Console.WriteLine("\n=== Inventory ===");
//             if (player.Inventory.Count == 0)
//             {
//                 Console.WriteLine("Empty");
//                 return;
//             }
            
//             for (int i = 0; i < player.Inventory.Count; i++)
//             {
//                 Item item = player.Inventory[i];
//                 string equipStatus = "";
//                 if (item == player.EquippedWeapon) equipStatus = "[WEAPON]";
//                 else if (item == player.EquippedArmor) equipStatus = "[ARMOR]";

//                 string stats = $"ATK+{item.AttackBonus} DEF+{item.DefenseBonus} HP+{item.HealthBonus}";

//                 Console.WriteLine($"{i + 1}. {item.Name} ({item.Rarity}) {equipStatus} - {stats}");
//             }
//         }

//         private void Rest()
//         {
//             Console.WriteLine("\nYou rest and recover...");
//             player.Health = player.MaxHealth;
//             player.Mana = player.MaxMana;
//             Console.WriteLine("HP and Mana fully restored! Saving checkpoint now...");
//         }
//     }

//     // MAIN PROGRAM
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             Game game = new Game();
//             game.Start();
//         }
//     }
// }

// Program.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json; 
using System.Text.Json.Serialization; 

namespace ShadowsOfEldoria
{
    // GAME CLASS
    public class Game
    {
        private Character player;
        private Random rand = new Random();
        private int currentStoryChapter = 1;
        private const string SAVE_FILE = "eldoria_save.json";

        // GM CHEAT CONSTANTS
        private const string CHEAT_WORD = "EldoriaGM"; 
        private const string CHEAT_OPTION = "9";
        private const string RESET_CHEAT_OPTION = "0";

        public void Start()
        {
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║    SHADOWS OF ELDORIA: AWAKENING   ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");
            
            if (File.Exists(SAVE_FILE))
            {
                Console.Write("Save file found. Load game? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    LoadGame();
                }
                else
                {
                    CreateCharacter();
                }
            }
            else
            {
                CreateCharacter();
            }

            GiveStartingAbilities();

            Console.WriteLine($"\nWelcome, {player.Name} the {player.Class}!");
            Console.WriteLine(StoryProgression.GetChapterPlot(currentStoryChapter));
            
            MainGameLoop();
        }

        private void CreateCharacter()
        {
            Console.Write("Enter your character's name: ");
            string name = Console.ReadLine() ?? "Hero";
            
            Console.WriteLine("\nChoose your class:");
            Console.WriteLine("1. Warrior - High HP, strong defense, reliable damage.");
            Console.WriteLine("2. Mage - Powerful magic, healing, high mana.");
            Console.WriteLine("3. Rogue - Critical hits, high agility, trap expert.");
            Console.Write("\nChoice (1-3): ");
            
            int choice = 0;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
            {
                Console.WriteLine("Invalid choice, defaulting to Warrior.");
                choice = 1;
            }
            
            CharacterClass charClass = (CharacterClass)(choice - 1);
            player = new Character(name, charClass); 
            
            // Give starting weapon (Now uses EquipSlot.Weapon)
            Item startWeapon = charClass switch
            {
                CharacterClass.Warrior => new Item("Iron Sword", ItemRarity.Common, 5, 0, 0, EquipSlot.Weapon),
                CharacterClass.Mage => new Item("Wooden Staff", ItemRarity.Common, 3, 0, 0, EquipSlot.Weapon),
                CharacterClass.Rogue => new Item("Rusty Dagger", ItemRarity.Common, 4, 0, 0, EquipSlot.Weapon),
                _ => new Item("Stick", ItemRarity.Common, 2, 0, 0, EquipSlot.Weapon)
            };
            
            player.AddItem(startWeapon);
            player.EquipItem(startWeapon);
        }

        private void GiveStartingAbilities()
        {
            player.Abilities.Clear(); 
            
            foreach (var ability in Ability.MasterAbilityList)
            {
                if (ability.RequiredClass == player.Class && ability.RequiredLevel == 1)
                {
                    player.LearnAbility(ability);
                }
            }
        }

        private void MainGameLoop()
        {
            bool playing = true;
            
            while (playing && player.Health > 0)
            {
                if (StoryProgression.IsChapterComplete(currentStoryChapter))
                {
                    currentStoryChapter++;
                    Console.WriteLine($"\n*** You have advanced to Chapter {currentStoryChapter}! ***");
                    Console.WriteLine(StoryProgression.GetChapterPlot(currentStoryChapter));
                    SaveGame();
                }

                Console.WriteLine($"\n--- CHAPTER {currentStoryChapter}/{StoryProgression.TOTAL_CHAPTERS} MENU ---");
                Console.WriteLine("1. Explore (Random Encounter/Treasure/Trap)");
                Console.WriteLine("2. Start Chapter Objective");
                Console.WriteLine("3. Inventory & Equipment");
                Console.WriteLine("4. View Status");
                Console.WriteLine("5. Rest & Checkpoint");
                Console.WriteLine("6. Exit Game");
                Console.Write("\nChoice: ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        ExploreDungeon();
                        break;
                    case "2":
                        StartChapterObjective();
                        break;
                    case "3":
                        DisplayInventoryOptions();
                        break;
                    case "4":
                        ShowStatus();
                        break;
                    case "5":
                        Rest();
                        SaveGame(); 
                        break;
                    case "6":
                        playing = false;
                        Console.WriteLine("Thanks for playing Shadows of Eldoria!");
                        break;
                    case CHEAT_OPTION: 
                        ProcessCheat();
                        break;
                    case RESET_CHEAT_OPTION: 
                        ProcessResetCheat();
                        break;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }
            
            if (player.Health <= 0)
            {
                Console.WriteLine("\n*** GAME OVER ***");
                Console.WriteLine($"{player.Name} has fallen in battle...");
            }
        }

        // GM Cheat Method (Buff Only)
        private void ProcessCheat()
        {
            Console.WriteLine("\n[GM CONSOLE ACCESS]: Enter secret code to engage Buff Protocol.");
            Console.Write("Secret Code: ");
            string code = Console.ReadLine();

            if (code == CHEAT_WORD)
            {
                Console.WriteLine("\n*** CHEAT ACTIVATED: ELDORIA GM PROTOCOL ENGAGED ***");

                player.Level = 50; 
                player.MaxHealth = 5000;
                player.MaxMana = 5000;
                player.AttackPower = 500;
                player.Defense = 500;
                player.Agility = 100;
                player.Health = player.MaxHealth;
                player.Mana = player.MaxMana;

                Console.WriteLine($"\n{player.Name} is now Level {player.Level}! You feel GODLIKE.");
                Console.WriteLine($"You remain on Chapter {currentStoryChapter}, but your stats are maxed.");
                
                SaveGame(); 
            }
            else
            {
                Console.WriteLine("[ACCESS DENIED] That is not the correct protocol.");
            }
        }

        // GM Reset Method
        private void ProcessResetCheat()
        {
            Console.WriteLine("\n[GM CONSOLE ACCESS]: Enter secret code to engage Stat Reset Protocol.");
            Console.Write("Secret Code: ");
            string code = Console.ReadLine();

            if (code == CHEAT_WORD)
            {
                Console.WriteLine("\n*** CHEAT ACTIVATED: STAT RESET PROTOCOL ENGAGED ***");
                player.RecalculateStats();
                SaveGame(); 
            }
            else
            {
                Console.WriteLine("[ACCESS DENIED] That is not the correct protocol.");
            }
        }

        // NEW: Story-based progression logic
        private void StartChapterObjective()
        {
            if (StoryProgression.IsLastChapter(currentStoryChapter))
            {
                Console.WriteLine("\nYou have reached the final stage! Use explore to find the end.");
                return;
            }

            Location location = StoryProgression.GetLocationDetails(currentStoryChapter);
            List<Choice> choices = StoryProgression.GetChoices(this, currentStoryChapter);

            Console.WriteLine("\nAttempting to start the chapter objective...");
            
            if (player.Level < location.RecommendedLevel)
            {
                Console.WriteLine($"You feel unprepared. You need to be at least Level {location.RecommendedLevel} to proceed safely.");
                return;
            }

            if (StoryProgression.IsChapterComplete(currentStoryChapter))
            {
                Console.WriteLine("You have already completed this chapter's objective.");
                return;
            }

            if (choices.Any())
            {
                Console.WriteLine("You have a choice to make:");
                for (int i = 0; i < choices.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {choices[i].Text}");
                }
                Console.Write("\nChoice: ");

                if (int.TryParse(Console.ReadLine(), out int choiceIndex) && choiceIndex > 0 && choiceIndex <= choices.Count)
                {
                    choices[choiceIndex - 1].OnChoose(this);
                }
                else
                {
                    Console.WriteLine("Invalid choice. The moment passes...");
                }
            }
            else
            {
                if (StoryProgression.IsMajorBossChapter(currentStoryChapter))
                {
                        Console.WriteLine($"\nThe critical path is blocked by the {location.Name} Guardian!");
                        Enemy boss = GenerateEnemy(location.RecommendedLevel, isBoss: true);
                        Battle(boss);
                }
                else
                {
                        // Small random encounter or NPC interaction on non-boss chapters
                        ExploreDungeon(isStoryAdvance: true);
                }
            }
        }

        // MODIFIED: Explore method
        private void ExploreDungeon(bool isStoryAdvance = false)
        {
            Console.WriteLine("\nYou venture deeper into the current area...");
            
            int encounter = rand.Next(100);
            
            if (encounter < 20)
            {
                 EncounterNPC();
            }
            else if (encounter < 40)
            {
                EncounterTrap();
            }
            else if (encounter < 85)
            {
                EncounterEnemy(currentStoryChapter);
            }
            else
            {
                FindTreasure(currentStoryChapter);
            }

            if (isStoryAdvance)
            {
                StoryProgression.CompleteChapter(currentStoryChapter);
            }
        }
        
        // NEW: NPC Interaction
        private void EncounterNPC()
        {
            NPC npc = StoryProgression.GetRandomNPC(currentStoryChapter);
            if (npc == null)
            {
                Console.WriteLine("You find an abandoned campsite, nothing of note.");
                return;
            }
            
            Console.WriteLine($"\n? You meet {npc.Name}, a {npc.Role}.");
            Console.WriteLine($"\"{npc.Dialogue}\"");

            if (npc.GivesQuest && npc.RewardItem != null)
            {
                Console.WriteLine($"She offers you a small reward for listening.");
                player.AddItem(npc.RewardItem);
            }
            else
            {
                Console.WriteLine("She nods solemnly and departs.");
            }
        }

        private void EncounterTrap()
        {
            TrapType trapType = (TrapType)rand.Next(4);
            Trap trap = new Trap(trapType);
            
            Console.WriteLine($"\n⚠ You discover a concealed {trapType} trap!");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("1. Try to **DISARM** it (Rogue's specialty)");
            Console.WriteLine("2. Use **AGILITY** to avoid the mechanism");
            Console.WriteLine("3. Use **MANA** to block or disrupt it (Magic-based)");
            Console.WriteLine("4. **TRIGGER** it and take the hit");
            Console.Write("\nChoice: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1": 
                    trap.AttemptDisarm(player); 
                    break;
                case "2":
                    if (rand.Next(100) < player.Agility * 3)
                    {
                        Console.WriteLine("🏃💨 You expertly dodge the activation sequence!");
                    }
                    else
                    {
                        Console.WriteLine("❌ You trip! The trap catches you.");
                        trap.Trigger(player);
                    }
                    break;
                case "3":
                    if (player.Mana >= 10)
                    {
                        player.Mana -= 10;
                        Console.WriteLine("✨ You expend 10 mana to weave a momentary shield.");
                        player.TakeDamage(trap.Damage / 3); // Reduced damage
                    }
                    else
                    {
                        Console.WriteLine("🔋 Mana is too low! You resort to triggering the trap.");
                        trap.Trigger(player);
                    }
                    break;
                case "4": 
                    trap.Trigger(player); 
                    break;
                default: 
                    Console.WriteLine("Uncertain, you cautiously move, triggering the trap.");
                    trap.Trigger(player); 
                    break;
            }
        }

        public void EncounterEnemy(int level)
        {
            Enemy enemy = GenerateEnemy(level);
            
            Console.WriteLine($"\n⚔ A {enemy.Name} appears!");
            
            Battle(enemy);
        }

        private Enemy GenerateEnemy(int level, bool isBoss = false)
        {
            string[] normalEnemies = { "Goblin", "Skeleton", "Wolf", "Bandit", "Spider", "Cultist" };
            string[] bossEnemies = { "Goblin King", "Lich Lord", "Ancient Dragon", "Dark Knight", "Shadow Priest" };
            
            if (isBoss || StoryProgression.IsMajorBossChapter(currentStoryChapter))
            {
                string name = bossEnemies[rand.Next(bossEnemies.Length)];
                return new Enemy(name, 100 + level * 30, 15 + level * 3, 8 + level * 2, 200 + level * 50, true);
            }
            else
            {
                string name = normalEnemies[rand.Next(normalEnemies.Length)];
                return new Enemy(name, 30 + level * 10, 8 + level * 2, 3 + level, 50 + level * 10);
            }
        }

        public void Battle(Enemy enemy)
        {
            Console.WriteLine($"\nBattle Start! {player.Name} vs {enemy.Name}");
            
            while (player.Health > 0 && enemy.IsAlive())
            {
                Console.WriteLine($"\n{player.Name}: {player.Health}/{player.MaxHealth} HP | {player.Mana}/{player.MaxMana} MP");
                Console.WriteLine($"{enemy.Name}: {enemy.Health}/{enemy.MaxHealth} HP");
                Console.WriteLine("\n1. Attack (Basic)");
                Console.WriteLine("2. Use Skill/Spell"); 
                Console.WriteLine("3. Run");
                Console.Write("\nChoice: ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        player.AttackEnemy(enemy);
                        break;
                    case "2":
                        player.UseAbility(enemy); 
                        break;
                    case "3":
                        if (rand.Next(100) < 50)
                        {
                            Console.WriteLine("You successfully escaped!");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Failed to escape!");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Lost turn!");
                        break;
                }
                
                if (enemy.IsAlive() && player.Health > 0)
                {
                    enemy.AttackPlayer(player);
                    
                    if (enemy.IsBoss && enemy.Health < enemy.MaxHealth / 2 && rand.Next(100) < 30)
                    {
                        Console.WriteLine($"\n{enemy.Name} unleashes a devastating special attack!");
                        player.TakeDamage(enemy.Attack * 2);
                    }
                }
            }
            
            if (!enemy.IsAlive())
            {
                Console.WriteLine($"\n*** Victory! {enemy.Name} defeated! ***");
                player.GainExperience(enemy.ExpReward);
                
                if (enemy.IsBoss)
                {
                    Console.WriteLine("\n🏆 BOSS DEFEATED! Legendary loot obtained!");
                    StoryProgression.CompleteChapter(currentStoryChapter);
                    DropLoot(ItemRarity.Legendary);
                }
                else if (rand.Next(100) < 40)
                {
                    DropLoot(ItemRarity.Uncommon);
                }
            }
        }

        public void FindTreasure(int level)
        {
            Console.WriteLine("\n💎 You found a treasure chest!");
            
            ItemRarity rarity = rand.Next(100) switch
            {
                < 50 => ItemRarity.Common,
                < 80 => ItemRarity.Uncommon,
                < 95 => ItemRarity.Rare,
                < 99 => ItemRarity.Epic,
                _ => ItemRarity.Legendary
            };
            
            DropLoot(rarity);
        }

        private void DropLoot(ItemRarity rarity)
        {
            string[] prefixes = { "Heavy", "Arcane", "Stealthy", "Vicious", "Mythic" };
            string prefix = prefixes[rand.Next(prefixes.Length)] + " ";

            EquipSlot slot = (EquipSlot)rand.Next(1, 6); // Random slot from Head to Feet (0 is Weapon, 6 is None)

            string itemName = slot switch
            {
                EquipSlot.Weapon => "Greatsword",
                EquipSlot.Head => "Helmet",
                EquipSlot.Chest => "Chestplate",
                EquipSlot.Hands => "Gauntlets",
                EquipSlot.Legs => "Leggings",
                EquipSlot.Feet => "Boots",
                _ => "Strange Orb"
            };

            int multiplier = rarity switch
            {
                ItemRarity.Common => 1, ItemRarity.Uncommon => 2, ItemRarity.Rare => 3, 
                ItemRarity.Epic => 5, ItemRarity.Legendary => 8, _ => 1
            };
            
            int attack = (slot == EquipSlot.Weapon) ? 5 * multiplier : 0;
            int defense = (slot != EquipSlot.Weapon && slot != EquipSlot.None) ? 3 * multiplier : 0;
            
            Item item = new Item(
                prefix + itemName, rarity,
                attack, defense, 0,
                slot
            );
            
            player.AddItem(item);
        }

        private void DisplayInventoryOptions()
        {
            Console.WriteLine("\n--- Inventory & Equipment Menu ---");
            Console.WriteLine("1. View Items");
            Console.WriteLine("2. Equip/Unequip Item");
            Console.Write("\nChoice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    ShowInventory();
                    break;
                case "2":
                    ManageEquipment();
                    break;
                default:
                    Console.WriteLine("Returning to main menu.");
                    break;
            }
        }

        private void ManageEquipment()
        {
            ShowInventory();
            Console.Write("Enter the number of the item to Equip/Unequip (0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= player.Inventory.Count)
            {
                Item selectedItem = player.Inventory[index - 1];
                
                // Check if the item is currently equipped
                if (player.Equipment.ContainsKey(selectedItem.Slot) && player.Equipment[selectedItem.Slot] == selectedItem)
                {
                    player.UnequipItem(selectedItem);
                }
                else
                {
                    player.EquipItem(selectedItem);
                }
            }
            else
            {
                Console.WriteLine("Invalid selection or canceled.");
            }
        }

        private void ShowStatus()
        {
            // The logic inside Character.TakeDamage/AttackEnemy already sums the stats, 
            // so we'll simulate the display here using a separate calculation for clarity.
            int weaponBonus = player.Equipment.GetValueOrDefault(EquipSlot.Weapon)?.AttackBonus ?? 0;
            int armorDefense = player.Equipment.Where(kvp => kvp.Key != EquipSlot.Weapon && kvp.Value != null)
                                        .Sum(kvp => kvp.Value.DefenseBonus);

            int totalAttack = player.AttackPower + weaponBonus;
            int totalDefense = player.Defense + armorDefense;

            Console.WriteLine($"\n=== {player.Name} the {player.Class} ===");
            Console.WriteLine($"Level: {player.Level} (EXP: {player.Experience}/{player.Level * 100})");
            Console.WriteLine($"Health: {player.Health}/{player.MaxHealth} | Mana: {player.Mana}/{player.MaxMana}");
            Console.WriteLine($"Total Attack: {totalAttack} (Base: {player.AttackPower})");
            Console.WriteLine($"Total Defense: {totalDefense} (Base: {player.Defense})");
            Console.WriteLine($"Agility: {player.Agility}");
            
            Console.WriteLine("\n--- EQUIPMENT SLOTS ---");
            foreach(var kvp in player.Equipment)
            {
                 Console.WriteLine($"- {kvp.Key}: {kvp.Value?.Name ?? "None"}");
            }
        }

        private void ShowInventory()
        {
            Console.WriteLine("\n=== Inventory ===");
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("Empty");
                return;
            }
            
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Item item = player.Inventory[i];
                string equipStatus = "";
                if (player.Equipment.ContainsValue(item)) equipStatus = "[EQUIPPED]";

                string stats = $"ATK+{item.AttackBonus} DEF+{item.DefenseBonus} Slot:{item.Slot}";

                Console.WriteLine($"{i + 1}. {item.Name} ({item.Rarity}) {equipStatus} - {stats}");
            }
        }

        private void Rest()
        {
            Console.WriteLine("\nYou rest and recover...");
            player.Health = player.MaxHealth;
            player.Mana = player.MaxMana;
            Console.WriteLine("HP and Mana fully restored! Saving checkpoint now...");
        }
        
        // Save/Load Logic
        private void SaveGame()
        {
            try
            {
                var saveData = new SaveData
                {
                    Player = this.player,
                    CurrentStoryChapter = this.currentStoryChapter,
                    LastCheckpointName = $"Chapter {this.currentStoryChapter} Checkpoint",
                    ChapterCompletionStatus = StoryProgression.ChapterCompletionStatus
                };
                
                string jsonString = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SAVE_FILE, jsonString);
                Console.WriteLine("\n✔ Checkpoint saved successfully! Game state secured.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✘ Error saving game: {ex.Message}");
            }
        }

        private void LoadGame()
        {
            try
            {
                string jsonString = File.ReadAllText(SAVE_FILE);
                var saveData = JsonSerializer.Deserialize<SaveData>(jsonString); 
                
                this.player = saveData.Player;
                this.currentStoryChapter = saveData.CurrentStoryChapter;
                StoryProgression.ChapterCompletionStatus = saveData.ChapterCompletionStatus ?? new Dictionary<int, bool>();
                
                Console.WriteLine($"\n✔ Game loaded from {saveData.LastCheckpointName}.");
                
                // Recalculate stats to ensure equipped item bonuses are correctly applied after load
                player.RecalculateStats();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✘ Error loading game. Starting a new game. ({ex.Message})");
                CreateCharacter(); 
            }
        }
    }

    // MAIN PROGRAM
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}