// SaveData.cs
using System.Collections.Generic;

namespace ShadowsOfEldoria
{
    public class SaveData
    {
        public Character Player { get; set; }
        public int CurrentStoryChapter { get; set; }
        public string LastCheckpointName { get; set; }
        public Dictionary<int, bool> ChapterCompletionStatus { get; set; }
    }
}