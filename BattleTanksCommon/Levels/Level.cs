using System;
using System.Collections.Generic;
using System.Text;

namespace BattleTanksCommon.Levels
{
    /// <summary>
    /// Class representing the current level players are on.
    /// </summary>
    public class Level
    {
        /// <summary>
        /// Name of level to load.
        /// </summary>
        public string LevelName { get; }

        public Level(string levelName)
        {
            LevelName = levelName;
        }
    }
}
