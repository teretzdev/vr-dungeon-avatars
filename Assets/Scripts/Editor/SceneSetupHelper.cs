using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

namespace DungeonYou.Editor
{
    /// <summary>
    /// Editor tool to help set up the main game scene with all required managers
    /// </summary>
    public class SceneSetupHelper : EditorWindow
    {
        [MenuItem("DungeonYou/Scene Setup Helper")]
        public static void ShowWindow()
        {
            GetWindow<SceneSetupHelper>("Scene Setup Helper");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("DungeonYou Scene Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Setup Complete Game Scene"))
            {
                SetupCompleteScene();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Individual Setups:", EditorStyles.boldLabel);

            if (GUILayout.Button("Setup Core Managers"))
                SetupCoreManagers();

            if (GUILayout.Button("Setup XR Rig"))
                SetupXRRig();

            if (GUILayout.Button("Setup Lighting for MR"))
                SetupMRLighting();

            if (GUILayout.Button("Create Sample Dungeon Room"))
                CreateSampleDungeonRoom();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This will create all necessary GameObjects and components for your scene.", MessageType.Info);
        }

        private static void SetupCompleteScene()
        {
            SetupCoreManagers();
            SetupXRRig();
            SetupMRLighting();
            CreateSampleDungeonRoom();
            
            Debug.Log("Complete scene setup finished!");
        }

        private static void SetupCoreManagers()
        {
            // Create Managers GameObject
            GameObject managers = new GameObject("--- MANAGERS ---");
            
            // Game Manager
            GameObject gameManager = new GameObject("GameManager");
            gameManager.transform.SetParent(managers.transform);
            gameManager.AddComponent<GameManager>();
            
            // MR Controller
            GameObject mrController = new GameObject("MRController");
            mrController.transform.SetParent(managers.transform);
            mrController.AddComponent<MRController>();
            
            // Network Manager
            GameObject networkManager = new GameObject("NetworkManager");
            networkManager.transform.SetParent(managers.transform);
            networkManager.AddComponent<NetworkManager>();
            
            // Performance Monitor
            GameObject perfMonitor = new GameObject("PerformanceMonitor");
            perfMonitor.transform.SetParent(managers.transform);
            perfMonitor.AddComponent<PerformanceMonitor>();
            
            // Save System
            GameObject saveSystem = new GameObject("SaveSystem");
            saveSystem.transform.SetParent(managers.transform);
            saveSystem.AddComponent<SaveSystem>();
            
            // Combat Manager
            GameObject combatManager = new GameObject("EmeraldCombatManager");
            combatManager.transform.SetParent(managers.transform);
            combatManager.AddComponent<EmeraldCombatManager>();
            
            // Item System
            GameObject itemSystem = new GameObject("EmeraldItemSystem");
            itemSystem.transform.SetParent(managers.transform);
            itemSystem.AddComponent<EmeraldItemSystem>();
            
            // Voice Controller
            GameObject voiceController = new GameObject("VRIFVoiceController");
            voiceController.transform.SetParent(managers.transform);
            voiceController.AddComponent<VRIFVoiceController>();
            
            // Command Interpreter
            GameObject commandInterpreter = new GameObject("CommandInterpreter");
            commandInterpreter.transform.SetParent(managers.transform);
            commandInterpreter.AddComponent<CommandInterpreter>();
            
            // XR Interaction Manager
            GameObject xrManager = new GameObject("XRInteractionManager");
            xrManager.transform.SetParent(managers.transform);
            xrManager.AddComponent<DungeonYou.Interaction.XRInteractionManager>();
            
            // Dungeon Controllers
            GameObject dungeonControllers = new GameObject("--- DUNGEON CONTROLLERS ---");
            
            GameObject edgarController = new GameObject("EdgarDungeonController");
            edgarController.transform.SetParent(dungeonControllers.transform);
            edgarController.AddComponent<EdgarDungeonController>();
            
            GameObject dungeonScaler = new GameObject("DungeonScaler");
            dungeonScaler.transform.SetParent(dungeonControllers.transform);
            dungeonScaler.AddComponent<DungeonScaler>();
            
            GameObject dungeonRenderer = new GameObject("DungeonRenderer");
            dungeonRenderer.transform.SetParent(dungeonControllers.transform);
            dungeonRenderer.AddComponent<DungeonRenderer>();
            
            Debug.Log("Core managers setup complete!");
        }

        private static void SetupXRRig()
        {
            // Create XR Origin
            GameObject xrOrigin = new GameObject("XR Origin (XR Rig)");
            XROrigin xrOriginComponent = xrOrigin.AddComponent<XROrigin>();
            xrOrigin.AddComponent<XRInteractionManager>();
            
            // Camera Offset
            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(xrOrigin.transform);
            
            // Main Camera
            GameObject mainCamera = new GameObject("Main Camera");
            mainCamera.transform.SetParent(cameraOffset.transform);
            mainCamera.tag = "MainCamera";
            Camera cam = mainCamera.AddComponent<Camera>();
            mainCamera.AddComponent<AudioListener>();
            mainCamera.AddComponent<TrackedPoseDriver>();
            
            // Left Controller
            GameObject leftController = new GameObject("LeftHand Controller");
            leftController.transform.SetParent(cameraOffset.transform);
            XRController leftXR = leftController.AddComponent<XRController>();
            leftXR.controllerNode = UnityEngine.XR.XRNode.LeftHand;
            leftController.AddComponent<XRDirectInteractor>();
            leftController.AddComponent<XRInteractorLineVisual>();
            leftController.AddComponent<LineRenderer>();
            
            // Right Controller
            GameObject rightController = new GameObject("RightHand Controller");
            rightController.transform.SetParent(cameraOffset.transform);
            XRController rightXR = rightController.AddComponent<XRController>();
            rightXR.controllerNode = UnityEngine.XR.XRNode.RightHand;
            rightController.AddComponent<XRDirectInteractor>();
            rightController.AddComponent<XRInteractorLineVisual>();
            rightController.AddComponent<LineRenderer>();
            
            // Locomotion System
            GameObject locomotion = new GameObject("Locomotion System");
            locomotion.transform.SetParent(xrOrigin.transform);
            locomotion.AddComponent<LocomotionSystem>();
            locomotion.AddComponent<TeleportationProvider>();
            locomotion.AddComponent<ContinuousMoveProvider>();
            locomotion.AddComponent<ContinuousTurnProvider>();
            
            xrOriginComponent.CameraFloorOffsetObject = cameraOffset;
            
            Debug.Log("XR Rig setup complete!");
        }

        private static void SetupMRLighting()
        {
            // Remove default directional light
            Light[] lights = FindObjectsOfType<Light>();
            foreach (var light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DestroyImmediate(light.gameObject);
                }
            }
            
            // Create MR-friendly lighting
            GameObject lighting = new GameObject("--- MR LIGHTING ---");
            
            // Ambient light
            GameObject ambientLight = new GameObject("Ambient Light");
            ambientLight.transform.SetParent(lighting.transform);
            Light ambient = ambientLight.AddComponent<Light>();
            ambient.type = LightType.Directional;
            ambient.intensity = 0.5f;
            ambient.color = new Color(0.9f, 0.9f, 1f);
            ambientLight.transform.rotation = Quaternion.Euler(45f, -30f, 0);
            
            // Configure render settings for MR
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.4f, 0.4f, 0.4f);
            
            Debug.Log("MR lighting setup complete!");
        }

        private static void CreateSampleDungeonRoom()
        {
            GameObject dungeonRoot = new GameObject("--- SAMPLE DUNGEON ---");
            
            // Create floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(dungeonRoot.transform);
            floor.transform.localScale = new Vector3(2f, 1f, 2f);
            
            // Create walls
            CreateWall(dungeonRoot.transform, "North Wall", new Vector3(0, 1, 10), new Vector3(20, 2, 0.5f));
            CreateWall(dungeonRoot.transform, "South Wall", new Vector3(0, 1, -10), new Vector3(20, 2, 0.5f));
            CreateWall(dungeonRoot.transform, "East Wall", new Vector3(10, 1, 0), new Vector3(0.5f, 2, 20));
            CreateWall(dungeonRoot.transform, "West Wall", new Vector3(-10, 1, 0), new Vector3(0.5f, 2, 20));
            
            // Create room wrapper
            GameObject roomWrapper = new GameObject("Room_01");
            roomWrapper.transform.SetParent(dungeonRoot.transform);
            roomWrapper.AddComponent<EdgarRoomWrapper>();
            
            // Add entrance/exit
            GameObject entrance = new GameObject("Entrance");
            entrance.transform.SetParent(roomWrapper.transform);
            entrance.transform.position = new Vector3(0, 1, -9.5f);
            BoxCollider entranceCollider = entrance.AddComponent<BoxCollider>();
            entranceCollider.isTrigger = true;
            entranceCollider.size = new Vector3(3f, 3f, 0.5f);
            entrance.AddComponent<DungeonYou.Interaction.RoomEntranceExit>();
            
            // Add some spawn points
            CreateSpawnPoint(roomWrapper.transform, "EnemySpawnPoint_1", new Vector3(3, 0, 3));
            CreateSpawnPoint(roomWrapper.transform, "EnemySpawnPoint_2", new Vector3(-3, 0, 3));
            CreateSpawnPoint(roomWrapper.transform, "TreasureSpawnPoint_1", new Vector3(0, 0, 5));
            
            Debug.Log("Sample dungeon room created!");
        }

        private static void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = position;
            wall.transform.localScale = scale;
        }

        private static void CreateSpawnPoint(Transform parent, string name, Vector3 position)
        {
            GameObject spawnPoint = new GameObject(name);
            spawnPoint.transform.SetParent(parent);
            spawnPoint.transform.position = position;
            
            // Add a gizmo icon for visibility in editor
            GameObject gizmo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gizmo.name = "Gizmo";
            gizmo.transform.SetParent(spawnPoint.transform);
            gizmo.transform.localScale = Vector3.one * 0.3f;
            gizmo.GetComponent<Renderer>().enabled = false; // Hide in game
        }
    }
}