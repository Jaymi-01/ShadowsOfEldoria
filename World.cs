// World.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShadowsOfEldoria
{
    // ENUMS
    public enum TrapType { Spike, PoisonGas, MagicRune, Arrow }

    public class Location
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int RecommendedLevel { get; set; }
        public List<Enemy> PossibleEnemies { get; set; } = new List<Enemy>();

        public Location(string name, string desc, int level)
        {
            Name = name; 
            Description = desc; 
            RecommendedLevel = level;
        }
    }

    public class NPC
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Dialogue { get; set; }
        public bool GivesQuest { get; set; }
        public Item RewardItem { get; set; }

        public NPC(string name, string role, string dialogue, bool givesQuest = false, Item reward = null)
        {
            Name = name;
            Role = role;
            Dialogue = dialogue;
            GivesQuest = givesQuest;
            RewardItem = reward;
        }
    }

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
                case TrapType.Spike: Damage = 20; break;
                case TrapType.PoisonGas: Damage = 15; break;
                case TrapType.MagicRune: Damage = 25; break;
                case TrapType.Arrow: Damage = 18; break;
            }
        }

        public void Trigger(Character player)
        {
            if (IsDisarmed)
            {
                Console.WriteLine("The trap has already been disarmed.");
                return;
            }
            
            switch (Type)
            {
                case TrapType.Spike: Console.WriteLine($"You step into a spike pit!"); break;
                case TrapType.PoisonGas: Console.WriteLine("Poison gas fills the room!"); break;
                case TrapType.MagicRune: Console.WriteLine("A magic rune explodes!"); break;
                case TrapType.Arrow: Console.WriteLine("An arrow shoots from the wall!"); break;
            }
            player.TakeDamage(Damage);
        }

        public bool AttemptDisarm(Character player)
        {
            if (IsDisarmed) return true;

            Random rand = new Random();
            int chance = player.Class == CharacterClass.Rogue ? 80 : 30 + player.Agility;
            
            if (rand.Next(100) < chance)
            {
                Console.WriteLine($"✔ Successfully disarmed the {Type} trap!");
                IsDisarmed = true;
                return true;
            }
            else
            {
                Console.WriteLine("❌ Failed to disarm the trap!");
                Trigger(player);
                return false;
            }
        }
    }
    
    // NEW: Choice class for story decisions
    public class Choice
    {
        public string Text { get; set; }
        public Action<Game> OnChoose { get; set; }
    }

    public static class StoryProgression
    {
        public const int TOTAL_CHAPTERS = 100; 
        public static Dictionary<int, bool> ChapterCompletionStatus = new Dictionary<int, bool>();

        private static string[] ZoneNames = new string[]
        {
            "Whispering Woods", "Ironclad Mountain Base", "Azure Coastline", "The Sunken City of Kaelen", 
            "Shadowfell Citadel Gates", "Dragon's Tooth Peaks", "The Crystal Desert", "The Royal Sewers",
            "The Aetherial Plane", "The Void's Edge"
        };
        
        public static string GetChapterPlot(int chapter)
        {
            switch (chapter)
            {
                case 1:
                    return "--- Whispering Woods (Chapter 1/100) ---" +
                           "**Standard Challenge**: Your first task is to find the source of the unnatural blight that is killing the woods." +
                           "\nLORE: The Whispering Woods were once a vibrant forest, but now they are slowly dying. The animals have fled, and a strange silence has fallen." +
                           "\nNARRATIVE: You are a new adventurer, drawn to the town of Eldoria by rumors of a growing darkness. You start your journey in the nearby woods, where the King's rangers have reported a strange blight.";
                case 2:
                    return "--- Whispering Woods (Chapter 2/100) ---" +
                           "**Standard Challenge**: You've discovered a hidden cave, which seems to be the source of the blight. You must explore it." +
                           "\nLORE: The cave is rumored to be the home of a coven of witches, who are using dark magic to corrupt the land." +
                           "\nNARRATIVE: The blight seems to emanate from a dark cave. The air is thick with the smell of decay and a faint, sinister chanting can be heard from within.";
                default:
                    Random rand = new Random(chapter);
                    int zoneIndex = (chapter - 1) / 10;
                    string locationName = ZoneNames[zoneIndex];
                    string difficulty = (chapter % 10 == 0) ? "**MAJOR BOSS CHAPTER**" : "**Standard Challenge**";
                    string[] objectives = {
                        "investigate a mysterious ritual site", "recover a lost artifact", "defeat the local Warlord",
                        "find the missing Elder of the village", "scout the terrain for the main army's advance",
                        "find a magical key to unlock the next region"
                    };
                    string objective = objectives[rand.Next(objectives.Length)];
                    string[] loreHooks = {
                        "Rumors persist that the Shadow Cult has already claimed a key piece of the Sunstone.",
                        "Ancient glyphs here warn of a spectral guardian bound to this place.",
                        "The terrain shifts constantly, evidence of powerful uncontrolled magic.",
                        "The air is thick with the scent of sulfur and old blood.",
                        "A forgotten hero's resting place is said to hold the clue you seek."
                    };
                    string lore = loreHooks[rand.Next(loreHooks.Length)];

                    return $"---" + locationName + " (Chapter " + chapter + "/" + TOTAL_CHAPTERS + ") --- \n" +
                           $"{difficulty}: Your current task is to " + objective + " within this segment of the zone. \n" +
                           $"LORE: " + lore + " \n" +
                           $"NARRATIVE: The gravity of your quest deepens as you realize the sheer scale of the corruption that plagues Eldoria. Be cautious; this area holds threats far greater than simple monsters.";
            }
        }
        
        public static List<Choice> GetChoices(Game game, int chapter)
        {
            List<Choice> choices = new List<Choice>();

            if (chapter == 1)
            {
                choices.Add(new Choice
                {
                    Text = "Follow the tracks of a large beast.",
                    OnChoose = (g) => {
                        Console.WriteLine("You follow the tracks and find a wounded dire wolf. You can try to heal it or put it out of its misery.");
                        g.EncounterEnemy(1); // Placeholder for a special encounter
                    }
                });
                choices.Add(new Choice
                {
                    Text = "Investigate the strange whispers you hear.",
                    OnChoose = (g) => {
                        Console.WriteLine("You follow the whispers and find a hidden shrine. You can pray at the shrine or defile it.");
                        g.FindTreasure(1); // Placeholder for a special event
                    }
                });
            }

            return choices;
        }

        public static Location GetLocationDetails(int chapter)
        {
            int zoneIndex = (chapter - 1) / 10;
            string locationName = ZoneNames[zoneIndex];
            int requiredLevel = 5 + zoneIndex * 5; 
            string description = $"You are deep within the {locationName}. Level Recommended: {requiredLevel}.";
            
            return new Location(locationName, description, requiredLevel);
        }

        public static bool IsMajorBossChapter(int chapter) => chapter % 10 == 0;
        public static bool IsLastChapter(int chapter) => chapter >= TOTAL_CHAPTERS;

        public static void CompleteChapter(int chapter)
        {
            if (!ChapterCompletionStatus.ContainsKey(chapter))
            {
                ChapterCompletionStatus.Add(chapter, true);
            }
            else
            {
                ChapterCompletionStatus[chapter] = true;
            }
        }

        public static bool IsChapterComplete(int chapter)
        {
            return ChapterCompletionStatus.ContainsKey(chapter) && ChapterCompletionStatus[chapter];
        }

        public static NPC GetRandomNPC(int chapter)
        {
            Random rand = new Random(chapter * 13);
            if (rand.Next(100) < 50) return null; // 50% chance of no NPC

            string[] names = { "Anya the Healer", "Old Man Grom", "The Wandering Trader", "Captain Elara" };
            string name = names[rand.Next(names.Length)];

            Item reward = (rand.Next(100) < 30) ? new Item("Mana Potion", ItemRarity.Common, slot: EquipSlot.None) : null;

            return new NPC(
                name: name,
                role: "Informant",
                dialogue: "You are the one the prophecy spoke of. Be wary of the shadows in the east, and take this for your journey.",
                givesQuest: true,
                reward: reward
            );
        }
    }
}