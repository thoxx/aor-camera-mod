using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace CameraMod.Classes
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        // https://wiki.nexusmods.com/index.php/How_to_render_mod_options_(UMM)
        [Header("Key Bindings")]
        [Draw("Camera Editor")]
        public KeyBinding CameraEditor = new KeyBinding { keyCode = KeyCode.KeypadDivide };
       
        [Draw("Height -0.5")]
        public KeyBinding HeightMinus = new KeyBinding { keyCode = KeyCode.Keypad0 };

        [Draw("Height +0.5")]
        public KeyBinding HeightPlus = new KeyBinding { keyCode = KeyCode.Keypad1 };

        [Draw("Distance -0.5")]
        public KeyBinding DistanceMinus = new KeyBinding { keyCode = KeyCode.Keypad2 };

        [Draw("Distance +0.5")]
        public KeyBinding DistancePlus = new KeyBinding { keyCode = KeyCode.Keypad3 };

        [Draw("Angle -0.5")]
        public KeyBinding AngleMinus = new KeyBinding { keyCode = KeyCode.Keypad4 };

        [Draw("Angle +0.5")]
        public KeyBinding AnglePlus = new KeyBinding { keyCode = KeyCode.Keypad7 };

        [Draw("Reset Camera")]
        public KeyBinding ResetCamera = new KeyBinding { keyCode = KeyCode.KeypadMultiply };

        [Header("Camera 8")]
        [Draw("Distance")]
        public float Camera8Distance = 7.0f;
        [Draw("Height")]
        public float Camera8Height = 2.0f;
        [Draw("Angle")]
        public float Camera8Angle = -1.0f;

        [Header("Camera 9")]
        [Draw("Distance")]
        public float Camera9Distance = 10.0f;
        [Draw("Height")]
        public float Camera9Height = 3.0f;
        [Draw("Angle")]
        public float Camera9Angle = -1.5f;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
            Main.ApplySettings();
        }
    }
}
