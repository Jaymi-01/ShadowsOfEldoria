using System;
using System.Collections.Generic;
using System.Linq;

namespace ShadowsOfEldoria
{
    // Enums
    public enum CharacterClass { Warrior, Mage, Rogue }
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }
    public enum TrapType { Spike, PoisonGas, MagicRune, Arrow }

    // Base Character Class
    public class Character
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }
        public int AttackPower { get; set; }
        public int Defense { get; set; }
        public int Agility { get; set; }
        public int Experience { get; set; }
        public CharacterClass Class { get; set; }
        public List<Item> Inventory { get; set; }
        public Item EquippedWeapon { get; set; }
        public Item EquippedArmor { get; set; }

        public Character(string name, CharacterClass charClass)
        {
            Name = name;
            Class = charClass;
            Level = 1;
            Experience = 0;
            Inventory = new List<Item>();
            
            // Set base stats based on class
            switch (charClass)
            {
                case CharacterClass.Warrior:
                    MaxHealth = 120;
                    MaxMana = 30;
                    AttackPower = 15;
                    Defense = 12;
                    Agility = 5;
                    break;
                case CharacterClass.Mage:
                    MaxHealth = 70;
                    MaxMana = 100;
                    AttackPower = 8;
                    Defense = 5;
                    Agility = 7;
                    break;
                case CharacterClass.Rogue:
                    MaxHealth = 90;
                    MaxMana = 50;
                    AttackPower = 12;
                    Defense = 7;
                    Agility = 15;
                    break;
            }
            
            Health = MaxHealth;
            Mana = MaxMana;
        }

        public void TakeDamage(int damage)
        {
            int actualDamage = Math.Max(1, damage - Defense);
            Health -= actualDamage;
            Console.WriteLine($"{Name} takes {actualDamage} damage! HP: {Health}/{MaxHealth}");
        }

        public bool AttackEnemy(Enemy enemy)
        {
            int totalAttack = AttackPower + (EquippedWeapon?.AttackBonus ?? 0);
            
            // Critical hit chance based on agility
            bool isCritical = new Random().Next(100) < Agility;
            int damage = isCritical ? totalAttack * 2 : totalAttack;
            
            if (isCritical)
                Console.WriteLine("CRITICAL HIT!");
            
            enemy.TakeDamage(damage);
            return true;
        }

        public void UseSkill(Enemy enemy)
        {
            switch (Class)
            {
                case CharacterClass.Warrior:
                    if (Mana >= 15)
                    {
                        Console.WriteLine($"{Name} uses Power Strike!");
                        Mana -= 15;
                        enemy.TakeDamage(AttackPower * 2);
                    }
                    else
                    {
                        Console.WriteLine("Not enough mana!");
                    }
                    break;
                case CharacterClass.Mage:
                    if (Mana >= 25)
                    {
                        Console.WriteLine($"{Name} casts Fireball!");
                        Mana -= 25;
                        enemy.TakeDamage(AttackPower * 3);
                    }
                    else
                    {
                        Console.WriteLine("Not enough mana!");
                    }
                    break;
                case CharacterClass.Rogue:
                    if (Mana >= 20)
                    {
                        Console.WriteLine($"{Name} performs Backstab!");
                        Mana -= 20;
                        enemy.TakeDamage(AttackPower * 3);
                    }
                    else
                    {
                        Console.WriteLine("Not enough mana!");
                    }
                    break;
            }
        }

        public void GainExperience(int exp)
        {
            Experience += exp;
            Console.WriteLine($"{Name} gains {exp} experience!");
            
            int expNeeded = Level * 100;
            if (Experience >= expNeeded)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            Experience = 0;
            
            MaxHealth += 10;
            MaxMana += 10;
            AttackPower += 3;
            Defense += 2;
            Agility += 1;
            
            Health = MaxHealth;
            Mana = MaxMana;
            
            Console.WriteLine($"\n*** LEVEL UP! {Name} is now level {Level}! ***");
            Console.WriteLine($"Stats increased! HP: {MaxHealth}, Mana: {MaxMana}, ATK: {AttackPower}");
        }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
            Console.WriteLine($"Obtained: {item.Name} ({item.Rarity})");
        }
    }

    // Enemy Class
    public class Enemy
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int ExpReward { get; set; }
        public bool IsBoss { get; set; }

        public Enemy(string name, int health, int attack, int defense, int expReward, bool isBoss = false)
        {
            Name = name;
            MaxHealth = health;
            Health = health;
            Attack = attack;
            Defense = defense;
            ExpReward = expReward;
            IsBoss = isBoss;
        }

        public void TakeDamage(int damage)
        {
            int actualDamage = Math.Max(1, damage - Defense);
            Health -= actualDamage;
            Console.WriteLine($"{Name} takes {actualDamage} damage! HP: {Health}/{MaxHealth}");
        }

        public void AttackPlayer(Character player)
        {
            player.TakeDamage(Attack);
        }

        public bool IsAlive()
        {
            return Health > 0;
        }
    }

    // Item Class
    public class Item
    {
        public string Name { get; set; }
        public ItemRarity Rarity { get; set; }
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
        public int HealthBonus { get; set; }
        public bool IsWeapon { get; set; }
        public bool IsArmor { get; set; }

        public Item(string name, ItemRarity rarity, int attackBonus = 0, int defenseBonus = 0, int healthBonus = 0, bool isWeapon = false, bool isArmor = false)
        {
            Name = name;
            Rarity = rarity;
            AttackBonus = attackBonus;
            DefenseBonus = defenseBonus;
            HealthBonus = healthBonus;
            IsWeapon = isWeapon;
            IsArmor = isArmor;
        }
    }

    // Trap Class
    public class Trap
    {
        public TrapType Type { get; set; }
        public int Damage { get; set; }
        public bool IsDisarmed { get; set; }

        public Trap(TrapType type)
        {
            Type = type;
            IsDisarmed = false;
            
            switch (type)
            {
                case TrapType.Spike:
                    Damage = 20;
                    break;
                case TrapType.PoisonGas:
                    Damage = 15;
                    break;
                case TrapType.MagicRune:
                    Damage = 25;
                    break;
                case TrapType.Arrow:
                    Damage = 18;
                    break;
            }
        }

        public void Trigger(Character player)
        {
            if (IsDisarmed)
            {
                Console.WriteLine("The trap has already been disarmed.");
                return;
            }

            Random rand = new Random();
            
            switch (Type)
            {
                case TrapType.Spike:
                    if (player.Class == CharacterClass.Warrior)
                    {
                        Console.WriteLine("Your warrior's armor absorbs most of the spike trap damage!");
                        player.TakeDamage(Damage / 2);
                    }
                    else
                    {
                        Console.WriteLine("You trigger a spike trap!");
                        player.TakeDamage(Damage);
                    }
                    break;
                    
                case TrapType.PoisonGas:
                    Console.WriteLine("Poison gas fills the room!");
                    player.TakeDamage(Damage);
                    break;
                    
                case TrapType.MagicRune:
                    if (player.Class == CharacterClass.Mage)
                    {
                        Console.WriteLine("You sense the magical trap and avoid it!");
                    }
                    else
                    {
                        Console.WriteLine("A magic rune explodes!");
                        player.TakeDamage(Damage);
                    }
                    break;
                    
                case TrapType.Arrow:
                    if (rand.Next(100) < player.Agility * 3)
                    {
                        Console.WriteLine("You dodge the arrow trap!");
                    }
                    else
                    {
                        Console.WriteLine("An arrow shoots from the wall!");
                        player.TakeDamage(Damage);
                    }
                    break;
            }
        }

        public bool AttemptDisarm(Character player)
        {
            if (IsDisarmed)
            {
                Console.WriteLine("This trap is already disarmed.");
                return true;
            }

            Random rand = new Random();
            int chance = player.Class == CharacterClass.Rogue ? 80 : 30;
            
            if (rand.Next(100) < chance)
            {
                Console.WriteLine($"Successfully disarmed the {Type} trap!");
                IsDisarmed = true;
                return true;
            }
            else
            {
                Console.WriteLine("Failed to disarm the trap!");
                Trigger(player);
                return false;
            }
        }
    }

    // Game Class
    public class Game
    {
        private Character player;
        private Random rand = new Random();

        public void Start()
        {
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║   SHADOWS OF ELDORIA: AWAKENING   ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");
            
            CreateCharacter();
            
            Console.WriteLine($"\nWelcome, {player.Name} the {player.Class}!");
            Console.WriteLine("Your adventure in the realm of Eldoria begins...\n");
            
            MainGameLoop();
        }

        private void CreateCharacter()
        {
            Console.Write("Enter your character's name: ");
            string name = Console.ReadLine();
            
            Console.WriteLine("\nChoose your class:");
            Console.WriteLine("1. Warrior - High HP, strong defense, tanky");
            Console.WriteLine("2. Mage - Powerful spells, high mana, fragile");
            Console.WriteLine("3. Rogue - Critical hits, high agility, trap expert");
            Console.Write("\nChoice (1-3): ");
            
            int choice = int.Parse(Console.ReadLine());
            CharacterClass charClass = (CharacterClass)(choice - 1);
            
            player = new Character(name, charClass);
            
            // Give starting weapon
            Item startWeapon = charClass switch
            {
                CharacterClass.Warrior => new Item("Iron Sword", ItemRarity.Common, 5, 0, 0, true, false),
                CharacterClass.Mage => new Item("Wooden Staff", ItemRarity.Common, 3, 0, 0, true, false),
                CharacterClass.Rogue => new Item("Rusty Dagger", ItemRarity.Common, 4, 0, 0, true, false),
                _ => new Item("Stick", ItemRarity.Common, 2, 0, 0, true, false)
            };
            
            player.EquippedWeapon = startWeapon;
            player.AddItem(startWeapon);
        }

        private void MainGameLoop()
        {
            bool playing = true;
            int dungeonLevel = 1;
            
            while (playing && player.Health > 0)
            {
                Console.WriteLine($"\n--- Dungeon Level {dungeonLevel} ---");
                Console.WriteLine("1. Explore");
                Console.WriteLine("2. View Status");
                Console.WriteLine("3. View Inventory");
                Console.WriteLine("4. Rest (Restore HP & Mana)");
                Console.WriteLine("5. Exit Game");
                Console.Write("\nChoice: ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        ExploreDungeon(dungeonLevel);
                        if (rand.Next(100) < 70)
                            dungeonLevel++;
                        break;
                    case "2":
                        ShowStatus();
                        break;
                    case "3":
                        ShowInventory();
                        break;
                    case "4":
                        Rest();
                        break;
                    case "5":
                        playing = false;
                        Console.WriteLine("Thanks for playing Shadows of Eldoria!");
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

        private void ExploreDungeon(int level)
        {
            Console.WriteLine("\nYou venture deeper into the dungeon...");
            
            int encounter = rand.Next(100);
            
            if (encounter < 30)
            {
                EncounterTrap();
            }
            else if (encounter < 80)
            {
                EncounterEnemy(level);
            }
            else
            {
                FindTreasure(level);
            }
        }

        private void EncounterTrap()
        {
            TrapType trapType = (TrapType)rand.Next(4);
            Trap trap = new Trap(trapType);
            
            Console.WriteLine($"\n⚠ You encounter a {trapType} trap!");
            Console.WriteLine("1. Try to disarm it");
            Console.WriteLine("2. Trigger it and take the hit");
            Console.WriteLine("3. Try to avoid it");
            Console.Write("\nChoice: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    trap.AttemptDisarm(player);
                    break;
                case "2":
                    trap.Trigger(player);
                    break;
                case "3":
                    if (rand.Next(100) < player.Agility * 2)
                    {
                        Console.WriteLine("You successfully avoid the trap!");
                    }
                    else
                    {
                        trap.Trigger(player);
                    }
                    break;
                default:
                    trap.Trigger(player);
                    break;
            }
        }

        private void EncounterEnemy(int level)
        {
            Enemy enemy = GenerateEnemy(level);
            
            Console.WriteLine($"\n⚔ A {enemy.Name} appears!");
            
            Battle(enemy);
        }

        private Enemy GenerateEnemy(int level)
        {
            string[] normalEnemies = { "Goblin", "Skeleton", "Wolf", "Bandit", "Spider" };
            string[] bossEnemies = { "Goblin King", "Lich Lord", "Dragon", "Dark Knight" };
            
            bool isBoss = level % 5 == 0 && rand.Next(100) < 40;
            
            if (isBoss)
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

        private void Battle(Enemy enemy)
        {
            Console.WriteLine($"\nBattle Start! {player.Name} vs {enemy.Name}");
            
            while (player.Health > 0 && enemy.IsAlive())
            {
                Console.WriteLine($"\n{player.Name}: {player.Health}/{player.MaxHealth} HP | {player.Mana}/{player.MaxMana} MP");
                Console.WriteLine($"{enemy.Name}: {enemy.Health}/{enemy.MaxHealth} HP");
                Console.WriteLine("\n1. Attack");
                Console.WriteLine("2. Use Skill");
                Console.WriteLine("3. Run");
                Console.Write("\nChoice: ");
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        player.AttackEnemy(enemy);
                        break;
                    case "2":
                        player.UseSkill(enemy);
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
                
                if (enemy.IsAlive())
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
                    DropLoot(ItemRarity.Legendary);
                }
                else if (rand.Next(100) < 40)
                {
                    DropLoot(ItemRarity.Uncommon);
                }
            }
        }

        private void FindTreasure(int level)
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
            string[] weaponNames = { "Sword", "Staff", "Dagger", "Axe", "Bow" };
            string[] armorNames = { "Helmet", "Chestplate", "Boots", "Gloves" };
            
            bool isWeapon = rand.Next(2) == 0;
            string itemName = isWeapon ? weaponNames[rand.Next(weaponNames.Length)] : armorNames[rand.Next(armorNames.Length)];
            
            int multiplier = rarity switch
            {
                ItemRarity.Common => 1,
                ItemRarity.Uncommon => 2,
                ItemRarity.Rare => 3,
                ItemRarity.Epic => 5,
                ItemRarity.Legendary => 8,
                _ => 1
            };
            
            string prefix = rarity switch
            {
                ItemRarity.Epic => "Enchanted ",
                ItemRarity.Legendary => "Mythical ",
                _ => ""
            };
            
            Item item = new Item(
                prefix + itemName,
                rarity,
                isWeapon ? 5 * multiplier : 0,
                !isWeapon ? 3 * multiplier : 0,
                0,
                isWeapon,
                !isWeapon
            );
            
            player.AddItem(item);
        }

        private void ShowStatus()
        {
            Console.WriteLine($"\n=== {player.Name} the {player.Class} ===");
            Console.WriteLine($"Level: {player.Level}");
            Console.WriteLine($"Experience: {player.Experience}/{player.Level * 100}");
            Console.WriteLine($"Health: {player.Health}/{player.MaxHealth}");
            Console.WriteLine($"Mana: {player.Mana}/{player.MaxMana}");
            Console.WriteLine($"Attack: {player.AttackPower}");
            Console.WriteLine($"Defense: {player.Defense}");
            Console.WriteLine($"Agility: {player.Agility}");
            Console.WriteLine($"Equipped Weapon: {player.EquippedWeapon?.Name ?? "None"}");
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
                Console.WriteLine($"{i + 1}. {item.Name} ({item.Rarity}) - ATK+{item.AttackBonus} DEF+{item.DefenseBonus}");
            }
        }

        private void Rest()
        {
            Console.WriteLine("\nYou rest and recover...");
            player.Health = player.MaxHealth;
            player.Mana = player.MaxMana;
            Console.WriteLine("HP and Mana fully restored!");
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}