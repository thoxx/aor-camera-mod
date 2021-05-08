using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraMod.Classes
{
    /// <summary>
    /// States related to the mod
    /// </summary>
    public class ModState
    {
        /// <summary>
        /// True if mod is initialized and the cameras are added to the game
        /// </summary>
        public static bool IsInitalized;
        /// <summary>
        /// True if camera editor of the mod is active
        /// </summary>
        public static bool IsCameraEditor;
    }
}
