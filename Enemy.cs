// Enemy.cs
using System;

namespace ShadowsOfEldoria
{
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
}
