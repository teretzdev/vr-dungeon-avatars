using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

namespace DungeonYou.Editor
{
    /// <summary>
    /// Editor tool to help create prefabs with all required components
    /// </summary>
    public class PrefabCreator : EditorWindow
    {
        [MenuItem("DungeonYou/Prefab Creator")]
        public static void ShowWindow()
        {
            GetWindow<PrefabCreator>("Prefab Creator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("DungeonYou Prefab Creator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Room Entrance/Exit Prefab"))
                CreateRoomEntranceExitPrefab();

            if (GUILayout.Button("Create Door/Gate Prefab"))
                CreateDoorGatePrefab();

            if (GUILayout.Button("Create Treasure Chest Prefab"))
                CreateTreasureChestPrefab();

            if (GUILayout.Button("Create Lever/Switch/Button Prefab"))
                CreateLeverSwitchButtonPrefab();

            if (GUILayout.Button("Create Spatial UI Panel Prefab"))
                CreateSpatialUIPanelPrefab();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Click any button to create a prefab with all required components pre-configured.", MessageType.Info);
        }

        private static void CreateRoomEntranceExitPrefab()
        {
            GameObject go = new GameObject("RoomEntranceExit");
            
            // Add required components
            BoxCollider collider = go.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(3f, 3f, 0.5f);
            
            go.AddComponent<DungeonYou.Interaction.RoomEntranceExit>();
            
            // Create prefab
            string path = "Assets/Prefabs/Interactables/RoomEntranceExit.prefab";
            CreatePrefabAtPath(go, path);
        }

        private static void CreateDoorGatePrefab()
        {
            GameObject go = new GameObject("DoorGate");
            
            // Create door mesh placeholder
            GameObject doorMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorMesh.transform.SetParent(go.transform);
            doorMesh.transform.localScale = new Vector3(2f, 3f, 0.2f);
            doorMesh.name = "DoorMesh";
            
            // Add components
            go.AddComponent<AudioSource>();
            go.AddComponent<DungeonYou.Interaction.DoorGate>();
            
            // Create prefab
            string path = "Assets/Prefabs/Interactables/DoorGate.prefab";
            CreatePrefabAtPath(go, path);
        }

        private static void CreateTreasureChestPrefab()
        {
            GameObject go = new GameObject("TreasureChest");
            
            // Create chest mesh placeholder
            GameObject chestBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chestBase.transform.SetParent(go.transform);
            chestBase.transform.localScale = new Vector3(1f, 0.5f, 0.7f);
            chestBase.name = "ChestBase";
            
            GameObject chestLid = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chestLid.transform.SetParent(go.transform);
            chestLid.transform.localPosition = new Vector3(0, 0.5f, 0);
            chestLid.transform.localScale = new Vector3(1f, 0.3f, 0.7f);
            chestLid.name = "ChestLid";
            
            // Add components
            go.AddComponent<AudioSource>();
            go.AddComponent<Animator>();
            go.AddComponent<DungeonYou.Interaction.TreasureChest>();
            
            // Create loot spawn point
            GameObject spawnPoint = new GameObject("LootSpawnPoint");
            spawnPoint.transform.SetParent(go.transform);
            spawnPoint.transform.localPosition = new Vector3(0, 1f, 0);
            
            // Create prefab
            string path = "Assets/Prefabs/Interactables/TreasureChest.prefab";
            CreatePrefabAtPath(go, path);
        }

        private static void CreateLeverSwitchButtonPrefab()
        {
            GameObject go = new GameObject("LeverSwitchButton");
            
            // Create lever mesh placeholder
            GameObject leverBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leverBase.transform.SetParent(go.transform);
            leverBase.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);
            leverBase.name = "LeverBase";
            
            GameObject leverHandle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leverHandle.transform.SetParent(go.transform);
            leverHandle.transform.localPosition = new Vector3(0, 0.3f, 0);
            leverHandle.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
            leverHandle.name = "LeverHandle";
            
            // Add XR components
            go.AddComponent<Rigidbody>().isKinematic = true;
            go.AddComponent<XRGrabInteractable>();
            go.AddComponent<AudioSource>();
            go.AddComponent<DungeonYou.Interaction.LeverSwitchButton>();
            
            // Create prefab
            string path = "Assets/Prefabs/Interactables/LeverSwitchButton.prefab";
            CreatePrefabAtPath(go, path);
        }

        private static void CreateSpatialUIPanelPrefab()
        {
            GameObject go = new GameObject("SpatialUIPanel");
            
            // Create Canvas
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            
            RectTransform canvasRect = go.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(400, 300);
            canvasRect.localScale = Vector3.one * 0.001f; // Scale down for world space
            
            go.AddComponent<UnityEngine.UI.CanvasScaler>();
            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Create background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(go.transform);
            UnityEngine.UI.Image bgImage = background.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0, 0, 0, 0.8f);
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
            
            // Create text elements
            CreateTextElement(go.transform, "HealthText", new Vector2(0, 100), "Health: 100");
            CreateTextElement(go.transform, "GoldText", new Vector2(0, 50), "Gold: 0");
            CreateTextElement(go.transform, "InventoryText", new Vector2(0, 0), "Inventory:");
            CreateTextElement(go.transform, "CommandFeedbackText", new Vector2(0, -100), "Ready for commands...");
            
            // Add VRIFUIManager
            go.AddComponent<VRIFUIManager>();
            
            // Create prefab
            string path = "Assets/Prefabs/UI/SpatialUIPanel.prefab";
            CreatePrefabAtPath(go, path);
        }

        private static void CreateTextElement(Transform parent, string name, Vector2 position, string defaultText)
        {
            GameObject textGO = new GameObject(name);
            textGO.transform.SetParent(parent);
            
            TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = defaultText;
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            
            RectTransform rect = textGO.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(350, 40);
            rect.anchoredPosition = position;
        }

        private static void CreatePrefabAtPath(GameObject go, string path)
        {
            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!AssetDatabase.IsValidFolder(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
                AssetDatabase.Refresh();
            }
            
            // Create prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            DestroyImmediate(go);
            
            Debug.Log($"Created prefab: {path}");
            Selection.activeObject = prefab;
        }
    }
}