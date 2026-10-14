using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Statecraft.Editor
{
    /// <summary>
    /// Player settings for a mobile-first project, applied from code so they are reviewable in
    /// git. Company name and bundle id are provisional until Norm confirms them (Docs/OPEN_QUESTIONS.md).
    /// </summary>
    public static class ProjectSetup
    {
        public const string CompanyName = "SES Interactive";
        public const string ProductName = "Statecraft Simulator";
        public const string BundleId = "com.SESInteractive.StatecraftSimulator";

        [MenuItem("Statecraft/Apply Project Settings")]
        public static void ApplyPlayerSettings()
        {
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, BundleId);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, BundleId);

            // Android: IL2CPP and ARM64 (required for Google Play); minimum API 26 as in InsignificAnts.
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)26;

            // Landscape only, either way up (provisional: Docs/OPEN_QUESTIONS.md).
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            AssetDatabase.SaveAssets();
            Debug.Log("[Statecraft] Player settings applied.");
        }

        /// <summary>Batch-mode entry: applies settings, rebuilds the main scene, exits 0 on success and 1 on failure.</summary>
        public static void RunFromCommandLine()
        {
            try
            {
                ApplyPlayerSettings();
                MainSceneBuilder.Build();
                AssetDatabase.SaveAssets();
                Debug.Log("[Statecraft] Project setup complete.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }
    }
}
