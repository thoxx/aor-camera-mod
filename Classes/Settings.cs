using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace CameraMod.Classes
{
    /// <summary>
    /// Mod settings for camera editor and saved camera settings
    /// See: https://wiki.nexusmods.com/index.php/How_to_render_mod_options_(UMM)
    /// </summary>
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        /// <summary>
        /// Key bind camera editor
        /// </summary>
        [Header("Key Bindings")]
        [Draw("Camera Editor on/off")]
        public KeyBinding CameraEditor = new KeyBinding { keyCode = KeyCode.KeypadDivide };
       
        /// <summary>
        /// Key bind decrease camera height
        /// </summary>
        [Draw("Height -0.5")]
        public KeyBinding HeightMinus = new KeyBinding { keyCode = KeyCode.Keypad0 };

        /// <summary>
        /// Key bind increase camera height
        /// </summary>
        [Draw("Height +0.5")]
        public KeyBinding HeightPlus = new KeyBinding { keyCode = KeyCode.Keypad1 };

        /// <summary>
        /// Key bind decrease camera distance
        /// </summary>
        [Draw("Distance -0.5")]
        public KeyBinding DistanceMinus = new KeyBinding { keyCode = KeyCode.Keypad2 };

        /// <summary>
        /// Key bind increase camera distance
        /// </summary>
        [Draw("Distance +0.5")]
        public KeyBinding DistancePlus = new KeyBinding { keyCode = KeyCode.Keypad3 };

        /// <summary>
        /// Key bind decrease camera angle
        /// </summary>
        [Draw("Angle -0.5")]
        public KeyBinding AngleMinus = new KeyBinding { keyCode = KeyCode.Keypad4 };

        /// <summary>
        /// Key bind increase camera angle
        /// </summary>
        [Draw("Angle +0.5")]
        public KeyBinding AnglePlus = new KeyBinding { keyCode = KeyCode.Keypad7 };

        /// <summary>
        /// Key bind reset camera settings
        /// </summary>
        [Draw("Reset Camera")]
        public KeyBinding ResetCamera = new KeyBinding { keyCode = KeyCode.KeypadMultiply };

        /// <summary>
        /// Camera 8 distance
        /// </summary>
        [Header("Camera 8 (Defaults: 7.0 | 2.0 | -1.0)"), Space(15)]
        [Draw("Distance")]
        public float Camera8Distance = 7.0f;

        /// <summary>
        /// Camera 8 height
        /// </summary>
        [Draw("Height")]
        public float Camera8Height = 2.0f;

        /// <summary>
        /// Camera 9 angle
        /// </summary>
        [Draw("Angle")]
        public float Camera8Angle = -1.0f;

        /// <summary>
        /// Camera 9 distance
        /// </summary>
        [Header("Camera 9 (Defaults: 10.0 | 3.0 | -1.5)"), Space(15)]
        [Draw("Distance")]
        public float Camera9Distance = 10.0f;

        /// <summary>
        /// Camera 9 height
        /// </summary>
        [Draw("Height")]
        public float Camera9Height = 3.0f;

        /// <summary>
        /// Camera 9 angle
        /// </summary>
        [Draw("Angle")]
        public float Camera9Angle = -1.5f;

        /// <summary>
        /// Save settings
        /// </summary>
        /// <param name="modEntry"></param>
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        /// <summary>
        /// Triggered when settings got changed
        /// </summary>
        public void OnChange()
        {
            Main.ApplySettings();
        }
    }
}
