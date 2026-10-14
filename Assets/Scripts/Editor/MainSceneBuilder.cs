using System;
using System.Linq;
using Statecraft.Game;
using Statecraft.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

namespace Statecraft.Editor
{
    /// <summary>
    /// Builds Assets/Scenes/Main.unity from code, plus the assets it needs (panel settings, the
    /// placeholder marker material). The scene is a build artifact: change this file, not the
    /// scene, so every value lives in code where Claude and Norm can both read it.
    /// </summary>
    public static class MainSceneBuilder
    {
        public const string ScenePath = "Assets/Scenes/Main.unity";
        const string PanelSettingsPath = "Assets/UI/PanelSettings.asset";
        const string ThemePath = "Assets/UI/Themes/StatecraftRuntimeTheme.tss";
        const string HudLayoutPath = "Assets/UI/DevHud/DevHud.uxml";
        const string MarkerMaterialPath = "Assets/Art/Placeholder/NationMarker.mat";
        const string DefaultUiActionsPath = "Packages/com.unity.inputsystem/InputSystem/Plugins/PlayerInput/DefaultInputActions.inputactions";

        [MenuItem("Statecraft/Rebuild Main Scene")]
        public static void Build()
        {
            var panelSettings = EnsurePanelSettings();
            var markerMaterial = EnsureMarkerMaterial();
            var hudLayout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(HudLayoutPath);
            if (hudLayout == null) throw new InvalidOperationException($"Missing {HudLayoutPath}.");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraObject = new GameObject("Main Camera") { tag = "MainCamera" };
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Hex("#1B1F24");
            camera.fieldOfView = 40f;
            cameraObject.AddComponent<AudioListener>();
            cameraObject.transform.SetPositionAndRotation(new Vector3(0f, 13f, -13f), Quaternion.Euler(45f, 0f, 0f));

            var sun = new GameObject("Sun");
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var rootObject = new GameObject("GameRoot");
            var host = rootObject.AddComponent<SimulationHost>();
            var gameRoot = rootObject.AddComponent<GameRoot>();
            Set(gameRoot, "host", host);
            SetInt(gameRoot, "seed", 1);
            SetInt(host, "startSpeed", 1);
            SetInt(host, "maxTicksPerFrame", 8);

            var worldObject = new GameObject("WorldView");
            var worldView = worldObject.AddComponent<WorldView>();
            Set(worldView, "host", host);
            Set(worldView, "markerMaterial", markerMaterial);

            var hudObject = new GameObject("DevHud");
            var document = hudObject.AddComponent<UIDocument>();
            document.panelSettings = panelSettings;
            document.visualTreeAsset = hudLayout;
            var hud = hudObject.AddComponent<DevHud>();
            Set(hud, "document", document);
            Set(hud, "host", host);

            // An EventSystem with the Input System module, wired to the package's shared default UI
            // actions exactly as the working mobile setup in InsignificAnts is.
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            WireDefaultUiActions(eventSystemObject.AddComponent<InputSystemUIInputModule>());

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new InvalidOperationException($"Could not save {ScenePath}.");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log($"[Statecraft] Built {ScenePath}.");
        }

        // References the package asset and its action sub-assets, so nothing is copied into the scene.
        static void WireDefaultUiActions(InputSystemUIInputModule module)
        {
            var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(DefaultUiActionsPath);
            if (asset == null) throw new InvalidOperationException($"Missing {DefaultUiActionsPath}.");
            var references = AssetDatabase.LoadAllAssetsAtPath(DefaultUiActionsPath).OfType<InputActionReference>().ToArray();

            InputActionReference Find(string actionName)
            {
                foreach (var reference in references)
                {
                    var action = reference.action;
                    if (action != null && action.actionMap != null && action.actionMap.name == "UI" && action.name == actionName)
                        return reference;
                }
                throw new InvalidOperationException($"Default UI actions have no UI/{actionName}.");
            }

            module.actionsAsset = asset;
            module.point = Find("Point");
            module.leftClick = Find("Click");
            module.rightClick = Find("RightClick");
            module.middleClick = Find("MiddleClick");
            module.scrollWheel = Find("ScrollWheel");
            module.move = Find("Navigate");
            module.submit = Find("Submit");
            module.cancel = Find("Cancel");
            module.trackedDevicePosition = Find("TrackedDevicePosition");
            module.trackedDeviceOrientation = Find("TrackedDeviceOrientation");
        }

        static PanelSettings EnsurePanelSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (settings == null)
            {
                EnsureFolder("Assets/UI");
                settings = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(settings, PanelSettingsPath);
            }

            var theme = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(ThemePath);
            if (theme == null) throw new InvalidOperationException($"Missing {ThemePath}.");
            settings.themeStyleSheet = theme;
            settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            settings.referenceResolution = new Vector2Int(1920, 1080);
            settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            settings.match = 1f; // match height: landscape phones vary in width, not height
            EditorUtility.SetDirty(settings);
            return settings;
        }

        static Material EnsureMarkerMaterial()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(MarkerMaterialPath);
            if (material != null) return material;

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit shader not found.");
            EnsureFolder("Assets/Art/Placeholder");
            material = new Material(shader) { name = "NationMarker" };
            AssetDatabase.CreateAsset(material, MarkerMaterialPath);
            return material;
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int slash = path.LastIndexOf('/');
            string parent = path.Substring(0, slash);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, path.Substring(slash + 1));
        }

        static void Set(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(field) ?? throw new InvalidOperationException($"{target.GetType().Name} has no serialized field '{field}'.");
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        static void SetInt(UnityEngine.Object target, string field, int value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(field) ?? throw new InvalidOperationException($"{target.GetType().Name} has no serialized field '{field}'.");
            property.intValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        static Color Hex(string hex) => ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.magenta;
    }
}
