using CameraMod.Classes;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace CameraMod
{
    static class Main
    {
        public static UnityModManager.ModEntry mod;
        private static CarCameras _camera;
        private static Rect _labelRect = new Rect(70, 40, 200, 200);
        private static List<CameraAngle> _cameraAngles;
        private static List<CameraAngle> _cameraAnglesOriginals;
        private static SceneryManager sceneryManager;

        // Send a response to the mod manager about the launch status, success or not.
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnFixedGUI = OnFixedGUI;
            modEntry.Logger.Log("Camera Mod loaded but not initalized");
            
            return true; // If false the mod will show an error.
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            // wait for sceneryManager
            if(sceneryManager == null)
            {
                if(GameState.IsActiveRally)
                {                    
                    GameState.IsActiveRally = false;
                    modEntry.Logger.Log($"Active Rally State changed: {GameState.IsActiveRally}");
                    modEntry.Logger.Log("Camera Mod uninitalized");
                    ModState.IsInitalized = false;
                    ModState.IsCameraEditor = false;
                }
                sceneryManager = UnityEngine.Object.FindObjectOfType<SceneryManager>();
            }
            // check if ActiveRally state changes in the game
            if(sceneryManager != null && !GameState.IsActiveRally)
            {
                GameState.IsActiveRally = true;
                modEntry.Logger.Log($"Active Rally State changed: {GameState.IsActiveRally}");
            }
            if (GameState.IsActiveRally && !ModState.IsInitalized)
            {
                InitCams(modEntry);
            }
            if(ControllerButtonDisplay.inPhotoMode!=GameState.IsInPhotoMode)
            {
                GameState.IsInPhotoMode = ControllerButtonDisplay.inPhotoMode;
                modEntry.Logger.Log("Photo Mode active: " + GameState.IsInPhotoMode);
                if(GameState.IsInPhotoMode)
                {
                    ModState.IsCameraEditor = false;
                }
            }
            if(ModState.IsInitalized)
            {
                //Enable Camera Editor (only if not in Photo Mode)
                if (Input.GetKeyUp(KeyCode.KeypadDivide) && !GameState.IsInPhotoMode)
                {
                    ModState.IsCameraEditor = !ModState.IsCameraEditor;
                }
            }
            if(ModState.IsCameraEditor)
            {
                bool isKeyPressed = false;
                int camIndex = (int)_camera.CurrentCameraAngle.cameraType;
                // -Height
                if (Input.GetKeyUp(KeyCode.Keypad0))
                {
                    _cameraAngles[camIndex].height -= 0.5f;
                    isKeyPressed = true;
                }
                // +Height
                if (Input.GetKeyUp(KeyCode.Keypad1))
                {
                    _cameraAngles[camIndex].height += 0.5f;
                    isKeyPressed = true;
                }
                // -Distance
                if (Input.GetKeyUp(KeyCode.Keypad2))
                {
                    _cameraAngles[camIndex].distance -= 0.5f;
                    isKeyPressed = true;
                }
                // +Distance
                if (Input.GetKeyUp(KeyCode.Keypad3))
                {
                    _cameraAngles[camIndex].distance += 0.5f;
                    isKeyPressed = true;
                }
                // -Angle
                if (Input.GetKeyUp(KeyCode.Keypad4))
                {
                    _cameraAngles[camIndex].initialPitchAngle -= 0.5f;
                    isKeyPressed = true;
                }
                // +Angle
                if (Input.GetKeyUp(KeyCode.Keypad7))
                {
                    _cameraAngles[camIndex].initialPitchAngle += 0.5f;
                    isKeyPressed = true;
                }
                // Reset Camera
                if (Input.GetKeyUp(KeyCode.KeypadMultiply))
                {
                    _cameraAngles[camIndex].distance = _cameraAnglesOriginals[camIndex].distance;
                    _cameraAngles[camIndex].height = _cameraAnglesOriginals[camIndex].height;
                    _cameraAngles[camIndex].initialPitchAngle = _cameraAnglesOriginals[camIndex].initialPitchAngle;
                    isKeyPressed = true;
                }
                // Update Current Cam
                if (isKeyPressed)
                {
                    _camera.distance = _cameraAngles[camIndex].distance;
                    _camera.height = _cameraAngles[camIndex].height;
                    _camera.initialPitchAngle = _cameraAngles[camIndex].initialPitchAngle;
                    _camera.SetToWantedPositionImmediate();
                }
            }
        }

        static void OnFixedGUI(UnityModManager.ModEntry modEntry)
        {
            // show camera values when editor is enabled
            if (ModState.IsCameraEditor)
            {
                GUI.Label(_labelRect, $"Camera Editor\n" +
                    $"Current Camera: {(int)_camera.CurrentCameraAngle.cameraType}\n" +
                    $"Height: {_camera.height}\n" +
                    $"Distance: { _camera.distance}\n" +
                    $"Pitch Angle: { _camera.initialPitchAngle}\n" +
                    $"\n" +
                    $"KeyPad 0: Height -0.5\n" +
                    $"KeyPad 1: Height +0.5\n" +
                    $"KeyPad 2: Distance -0.5\n" +
                    $"KeyPad 3: Distance +0.5\n" +
                    $"KeyPad 4: Pitch Angle -0.5\n" +
                    $"KeyPad 7: Pitch Angle +0.5\n" +
                    $"\n" +
                    $"KeyPad *: Reset Camera");
            }
        }

        /// <summary>
        /// Initialize new cameras and add it to the existing game cameras
        /// </summary>
        /// <param name="modEntry">UnityModManager.ModEntry</param>
        static void InitCams(UnityModManager.ModEntry modEntry)
        {
            _camera = UnityEngine.Object.FindObjectOfType<CarCameras>();
            if (_camera == null)
            {
                return;
            }

            FieldInfo prop = _camera.GetType().GetField("CameraAnglesList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (_cameraAngles==null)
            {                
                _cameraAngles = ((List<CameraAngle>)prop.GetValue(_camera));
                //TODO: hood cam
                _cameraAngles.Add(new CameraAngle(7f, 2f, -1f, (CameraAngle.CameraAngles)8));
                _cameraAngles.Add(new CameraAngle(10f, 3f, -1.5f, (CameraAngle.CameraAngles)9));

                //Deep Copy original cameras
                _cameraAnglesOriginals = _cameraAngles.ConvertAll(camera => new CameraAngle(camera.distance, camera.height, camera.initialPitchAngle, camera.cameraType));
            }
            prop.SetValue(_camera, _cameraAngles);

            modEntry.Logger.Log("Camera Mod initalized");

            ModState.IsInitalized = true;
        }
    }
}
