// Ability.cs
using System.Collections.Generic;

namespace ShadowsOfEldoria
{
    public class Ability
    {
        public string Name { get; set; }
        public int ManaCost { get; set; }
        public int BaseDamage { get; set; }
        public CharacterClass RequiredClass { get; set; } 
        public int RequiredLevel { get; set; } // NEW
        public float CritChanceBonus { get; set; }
        public bool IsHealing { get; set; }

        public Ability(string name, int manaCost, int baseDamage, CharacterClass requiredClass, int requiredLevel, float critChanceBonus = 0f, bool isHealing = false)
        {
            Name = name;
            ManaCost = manaCost;
            BaseDamage = baseDamage;
            RequiredClass = requiredClass;
            RequiredLevel = requiredLevel;
            CritChanceBonus = critChanceBonus;
            IsHealing = isHealing;
        }

        // NEW: Master List of All Abilities in the Game
        public static List<Ability> MasterAbilityList = new List<Ability>()
        {
            // Warrior
            new Ability("Power Strike", 15, 20, CharacterClass.Warrior, 1),
            new Ability("Whirlwind", 30, 15, CharacterClass.Warrior, 5),
            new Ability("Shield Bash", 20, 10, CharacterClass.Warrior, 10),

            // Mage
            new Ability("Fireball", 25, 30, CharacterClass.Mage, 1),
            new Ability("Heal", 10, -15, CharacterClass.Mage, 1, isHealing: true),
            new Ability("Lightning Bolt", 40, 50, CharacterClass.Mage, 8),
            new Ability("Arcane Barrier", 35, 0, CharacterClass.Mage, 12),

            // Rogue
            new Ability("Backstab", 20, 25, CharacterClass.Rogue, 1, critChanceBonus: 40f),
            new Ability("Poison Strike", 25, 15, CharacterClass.Rogue, 6),
            new Ability("Vanish", 40, 0, CharacterClass.Rogue, 10)
        };
    }
}
