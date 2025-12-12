// Character.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShadowsOfEldoria
{
    // ENUMS (Required for the Character class)
    public enum CharacterClass { Warrior, Mage, Rogue }

    // BASE CHARACTER CLASS
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
        public List<Ability> Abilities { get; set; } = new List<Ability>();
        
        // NEW: Equipment Dictionary for full-body slots
        public Dictionary<EquipSlot, Item> Equipment { get; set; }

        private Random rand = new Random();

        public Character() 
        { 
            Equipment = new Dictionary<EquipSlot, Item>();
        } 

        public Character(string name, CharacterClass charClass)
        {
            Name = name;
            Class = charClass;
            Level = 1;
            Experience = 0;
            Inventory = new List<Item>();
            
            Equipment = new Dictionary<EquipSlot, Item>
            {
                { EquipSlot.Weapon, null },
                { EquipSlot.Head, null },
                { EquipSlot.Chest, null },
                { EquipSlot.Hands, null },
                { EquipSlot.Legs, null },
                { EquipSlot.Feet, null }
            };
            
            InitializeStats(charClass);
            
            Health = MaxHealth; 
            Mana = MaxMana;
        }

        // Helper method for setting base stats
        private void InitializeStats(CharacterClass charClass)
        {
            switch (charClass)
            {
                case CharacterClass.Warrior:
                    MaxHealth = 120; MaxMana = 30; AttackPower = 15; Defense = 12; Agility = 5;
                    break;
                case CharacterClass.Mage:
                    MaxHealth = 70; MaxMana = 100; AttackPower = 8; Defense = 5; Agility = 7;
                    break;
                case CharacterClass.Rogue:
                    MaxHealth = 90; MaxMana = 50; AttackPower = 12; Defense = 7; Agility = 15;
                    break;
            }
        }
        
        // NEW: Recalculates stats based on current level (used to reverse GM cheat)
        public void RecalculateStats()
        {
            // 1. Reset to base level 1 stats
            InitializeStats(this.Class);
            
            // 2. Apply level-up bonuses for levels 2 through current Level
            for (int i = 2; i <= this.Level; i++)
            {
                MaxHealth += 10;
                MaxMana += 10;
                AttackPower += 3;
                Defense += 2;
                Agility += 1;
            }

            // 3. Re-apply ALL equipped item bonuses (keeping your loot buffs)
            foreach (var kvp in Equipment)
            {
                if (kvp.Value != null)
                {
                    ApplyEquipStats(kvp.Value);
                }
            }

            // 4. Restore current HP/Mana, ensuring they don't exceed the new max
            Health = Math.Min(Health, MaxHealth);
            Mana = Math.Min(Mana, MaxMana);
            
            Console.WriteLine("\n✨ Your core stats have been reset and recalculated based on your current level!");
        }

        public void ApplyEquipStats(Item item) 
        {
            AttackPower += item.AttackBonus;
            Defense += item.DefenseBonus;
            MaxHealth += item.HealthBonus;
            Health += item.HealthBonus;
        }

        public void RemoveEquipStats(Item item) 
        {
            AttackPower -= item.AttackBonus;
            Defense -= item.DefenseBonus;
            MaxHealth -= item.HealthBonus;
            Health = Math.Min(Health, MaxHealth); 
        }
        
        // NEW: Equip Logic handling slots
        public void EquipItem(Item item)
        {
            if (item.Slot == EquipSlot.None || !Inventory.Contains(item))
            {
                Console.WriteLine($"{item.Name} cannot be equipped or is not in inventory.");
                return;
            }

            if (Equipment.ContainsKey(item.Slot) && Equipment[item.Slot] != null)
            {
                UnequipItem(Equipment[item.Slot]); // Unequip old item
            }

            Equipment[item.Slot] = item;
            ApplyEquipStats(item);
            Console.WriteLine($"Equipped: {item.Name} into {item.Slot} slot. Stats applied.");
        }
        
        // NEW: Unequip Logic handling slots
        public void UnequipItem(Item item)
        {
            if (item.Slot == EquipSlot.None || Equipment.GetValueOrDefault(item.Slot) != item)
            {
                Console.WriteLine($"{item.Name} is not currently equipped in its slot.");
                return;
            }

            RemoveEquipStats(item);
            Equipment[item.Slot] = null;
            Console.WriteLine($"Unequipped: {item.Name}. Stats removed.");
        }
        
        public void LearnAbility(Ability ability)
        {
            if (!Abilities.Any(a => a.Name == ability.Name))
            {
                Abilities.Add(ability);
                Console.WriteLine($"✨ {Name} learned a new ability: {ability.Name}!");
            }
        }

        public bool AttackEnemy(Enemy enemy)
        {
            // Sum all attack bonuses from weapon slot
            int weaponBonus = Equipment.GetValueOrDefault(EquipSlot.Weapon)?.AttackBonus ?? 0;
            int totalAttack = AttackPower + weaponBonus;
            
            bool isCritical = rand.Next(100) < Agility;
            int damage = isCritical ? totalAttack * 2 : totalAttack;
            
            if (isCritical)
                Console.WriteLine("CRITICAL HIT!");
            
            enemy.TakeDamage(damage);
            return true;
        }

        public void UseAbility(Enemy enemy)
        {
            if (!Abilities.Any())
            {
                Console.WriteLine("You have no special abilities to use.");
                return;
            }

            Console.WriteLine("\n--- Abilities ---");
            for (int i = 0; i < Abilities.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Abilities[i].Name} (Cost: {Abilities[i].ManaCost} MP)");
            }
            Console.Write("Choose an ability (or 0 to cancel): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= Abilities.Count)
            {
                Ability selectedAbility = Abilities[choice - 1];
                if (Mana >= selectedAbility.ManaCost)
                {
                    Mana -= selectedAbility.ManaCost;
                    
                    if (selectedAbility.IsHealing) 
                    {
                        int healAmount = Math.Abs(selectedAbility.BaseDamage) + (MaxMana / 10);
                        Health = Math.Min(MaxHealth, Health + healAmount);
                        Console.WriteLine($"✨ {Name} casts {selectedAbility.Name} and restores {healAmount} HP!");
                    }
                    else 
                    {
                        Console.WriteLine($"{Name} uses {selectedAbility.Name}!");
                        int totalDamage = selectedAbility.BaseDamage + AttackPower;
                        
                        bool isCritical = false;
                        if (selectedAbility.CritChanceBonus > 0f)
                        {
                            isCritical = rand.Next(100) < (Agility + (int)selectedAbility.CritChanceBonus);
                        }
                        
                        if (isCritical)
                        {
                            Console.WriteLine("✨ ASSASSIN'S CRIT!");
                            totalDamage *= 3; 
                        }

                        enemy.TakeDamage(totalDamage);
                    }
                }
                else
                {
                    Console.WriteLine("Not enough mana for this ability!");
                }
            }
            else if (choice != 0)
            {
                Console.WriteLine("Invalid choice.");
            }
        }

        public void TakeDamage(int damage)
        {
            // Sum all defense bonuses from all equipped armor slots
            int armorDefense = Equipment.Where(kvp => kvp.Key != EquipSlot.Weapon && kvp.Value != null)
                                        .Sum(kvp => kvp.Value.DefenseBonus);
                                        
            int totalDefense = Defense + armorDefense;

            int actualDamage = Math.Max(1, damage - totalDefense);
            Health -= actualDamage;
            Console.WriteLine($"{Name} takes {actualDamage} damage! HP: {Health}/{MaxHealth}");
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
            
            // Increase base stats
            MaxHealth += 10;
            MaxMana += 10;
            AttackPower += 3;
            Defense += 2;
            Agility += 1;
            
            Health = MaxHealth;
            Mana = MaxMana;
            
            Console.WriteLine($"\n*** LEVEL UP! {Name} is now level {Level}! ***");
            Console.WriteLine($"Stats increased! HP: {MaxHealth}, Mana: {MaxMana}, ATK: {AttackPower}");

            // Learn new abilities
            foreach (var ability in Ability.MasterAbilityList)
            {
                if (ability.RequiredClass == Class && ability.RequiredLevel == Level)
                {
                    LearnAbility(ability);
                }
            }
        }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
            Console.WriteLine($"Obtained: {item.Name} ({item.Rarity})");
        }
    }
}