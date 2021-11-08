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
        public static Settings settings;

        // Send a response to the mod manager about the launch status, success or not.
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnFixedGUI = OnFixedGUI;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
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
                if (Input.GetKeyUp(settings.CameraEditor.keyCode) && !GameState.IsInPhotoMode)
                {
                    ModState.IsCameraEditor = !ModState.IsCameraEditor;
                }
            }
            if(ModState.IsCameraEditor)
            {
                bool isKeyPressed = false;
                int camIndex = (int)_camera.CurrentCameraAngle.cameraType;
                // -Height
                if (Input.GetKeyUp(settings.HeightMinus.keyCode))
                {
                    _cameraAngles[camIndex].height -= 0.5f;
                    isKeyPressed = true;
                }
                // +Height
                if (Input.GetKeyUp(settings.HeightPlus.keyCode))
                {
                    _cameraAngles[camIndex].height += 0.5f;
                    isKeyPressed = true;
                }
                // -Distance
                if (Input.GetKeyUp(settings.DistanceMinus.keyCode))
                {
                    _cameraAngles[camIndex].distance -= 0.5f;
                    isKeyPressed = true;
                }
                // +Distance
                if (Input.GetKeyUp(settings.DistancePlus.keyCode))
                {
                    _cameraAngles[camIndex].distance += 0.5f;
                    isKeyPressed = true;
                }
                // -Angle
                if (Input.GetKeyUp(settings.AngleMinus.keyCode))
                {
                    _cameraAngles[camIndex].initialPitchAngle -= 0.5f;
                    isKeyPressed = true;
                }
                // +Angle
                if (Input.GetKeyUp(settings.AnglePlus.keyCode))
                {
                    _cameraAngles[camIndex].initialPitchAngle += 0.5f;
                    isKeyPressed = true;
                }
                // Reset Camera
                if (Input.GetKeyUp(settings.ResetCamera.keyCode))
                {
                    _cameraAngles[camIndex].distance = _cameraAnglesOriginals[camIndex].distance;
                    _cameraAngles[camIndex].height = _cameraAnglesOriginals[camIndex].height;
                    _cameraAngles[camIndex].initialPitchAngle = _cameraAnglesOriginals[camIndex].initialPitchAngle;
                    isKeyPressed = true;
                }
                // Update Current Cam
                if (isKeyPressed)
                {
                    Main.UpdateCamera(camIndex);
                    // if custom camera is active -> save changed settings to mod settings
                    switch(camIndex)
                    {
                        case 8:
                            settings.Camera8Distance = _cameraAngles[camIndex].distance;
                            settings.Camera8Height = _cameraAngles[camIndex].height;
                            settings.Camera8Angle = _cameraAngles[camIndex].initialPitchAngle;
                            settings.Save(modEntry);
                            break;
                        case 9:
                            settings.Camera9Distance = _cameraAngles[camIndex].distance;
                            settings.Camera9Height = _cameraAngles[camIndex].height;
                            settings.Camera9Angle = _cameraAngles[camIndex].initialPitchAngle;
                            settings.Save(modEntry);
                            break;
                    }
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
                    $"{settings.HeightMinus.keyCode}: Height -0.5\n" +
                    $"{settings.HeightPlus.keyCode}: Height +0.5\n" +
                    $"{settings.DistanceMinus.keyCode}: Distance -0.5\n" +
                    $"{settings.DistancePlus.keyCode}: Distance +0.5\n" +
                    $"{settings.AngleMinus.keyCode}: Pitch Angle -0.5\n" +
                    $"{settings.AnglePlus.keyCode}: Pitch Angle +0.5\n" +
                    $"\n" +
                    $"{settings.ResetCamera.keyCode}: Reset Camera");
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
                Main.LoadCamerasFromSettings();
                modEntry.Logger.Log("Initialized cameras and loaded cameras from settings");
            }
            prop.SetValue(_camera, _cameraAngles);

            modEntry.Logger.Log("Camera Mod initalized");

            ModState.IsInitalized = true;
        }

        /// <summary>
        /// Update camera to a given index
        /// </summary>
        /// <param name="camIndex"></param>
        private static void UpdateCamera(int camIndex)
        {
            _camera.distance = _cameraAngles[camIndex].distance;
            _camera.height = _cameraAngles[camIndex].height;
            _camera.initialPitchAngle = _cameraAngles[camIndex].initialPitchAngle;
            _camera.SetToWantedPositionImmediate();
        }

        /// <summary>
        /// Apply saved mod settings to mod
        /// </summary>
        public static void ApplySettings()
        {
            // load changed camera settings from mod settings
            Main.LoadCamerasFromSettings();
            // if current camera is a custom camera -> update current camera
            int camIndex = (int)_camera.CurrentCameraAngle.cameraType;
            if(camIndex > 7)
            {
                UpdateCamera(camIndex);
            }
        }

        /// <summary>
        /// Mod menu GUI
        /// </summary>
        /// <param name="modEntry"></param>
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        /// <summary>
        /// Save Mod Menu settings
        /// </summary>
        /// <param name="modEntry"></param>
        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        /// <summary>
        /// Load camera settings from saved mod settings
        /// </summary>
        static void LoadCamerasFromSettings()
        {
            _cameraAngles[8].distance = settings.Camera8Distance;
            _cameraAngles[8].height = settings.Camera8Height;
            _cameraAngles[8].initialPitchAngle = settings.Camera8Angle;

            _cameraAngles[9].distance = settings.Camera9Distance;
            _cameraAngles[9].height = settings.Camera9Height;
            _cameraAngles[9].initialPitchAngle = settings.Camera9Angle;
        }
    }
}
