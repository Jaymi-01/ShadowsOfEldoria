// Item.cs
namespace ShadowsOfEldoria
{
    public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }
    
    // NEW: Detailed Equipment Slots
    public enum EquipSlot { Weapon, Head, Chest, Hands, Legs, Feet, None }

    public class Item
    {
        public string Name { get; set; }
        public ItemRarity Rarity { get; set; }
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
        public int HealthBonus { get; set; }
        public EquipSlot Slot { get; set; } // REPLACES IsWeapon, IsArmor

        // Parameterless constructor for JSON deserialization
        public Item() { }

        public Item(string name, ItemRarity rarity, int attackBonus = 0, int defenseBonus = 0, int healthBonus = 0, EquipSlot slot = EquipSlot.None)
        {
            Name = name;
            Rarity = rarity;
            AttackBonus = attackBonus;
            DefenseBonus = defenseBonus;
            HealthBonus = healthBonus;
            Slot = slot;
        }
    }
}
