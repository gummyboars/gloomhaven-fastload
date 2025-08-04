#define ENABLE_LOGS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;

using Gloomhaven;
using Platforms;
using Platforms.Steam;

namespace FastLoad;

[BepInPlugin(pluginGUID, pluginName, pluginVersion)]
public class FastLoadPlugin : BaseUnityPlugin
{
    const string pluginGUID = "com.gummyboars.gloomhaven.fastload";
    const string pluginName = "Fast Load";
    const string pluginVersion = "1.0.0";

    private Harmony HarmonyInstance = null;

    public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource(pluginName);

    private void Awake()
    {
        FastLoadPlugin.logger.LogInfo($"Loading plugin {pluginName}.");
        try
        {
            HarmonyInstance = new Harmony(pluginGUID);
            Assembly assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.PatchAll(assembly);
            FastLoadPlugin.logger.LogInfo($"Plugin {pluginName} loaded.");
        }
        catch (Exception e)
        {
            FastLoadPlugin.logger.LogError($"Could not load plugin {pluginName}: {e}");
        }
    }
}

// The hydra api is attached to a DNS name that no longer resolves. Disable it.
[HarmonyPatch(typeof(PlatformSteam), MethodType.Constructor, new Type[] {typeof(IGameProvider), typeof(bool), typeof(bool), typeof(bool), typeof(bool)})]
public static class Patch_Constructor
{
    public static void Prefix(ref bool initHydra)
    {
        initHydra = false;
    }
}

// Use this to load the scene while the intro video and logos are showing. The intro will lag a LOT.
[HarmonyPatch(typeof(Bootstrap), "ShowSplash")]
public static class Patch_ShowSplash
{
    public static bool Prefix(Bootstrap __instance, ref IEnumerator __result)
    {
        __result = ShowSplash(__instance);
        return false;
    }

    public static IEnumerator ShowSplash(Bootstrap instance)
    {
        FieldInfo loadScene = typeof(Bootstrap).GetField("_loadScene", BindingFlags.Instance | BindingFlags.NonPublic);
        Stopwatch _stopwatch = (Stopwatch) typeof(Bootstrap).GetField("_stopwatch", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        Debug.Log("[Bootstrap] Showing splash screen");
        string introSceneName = "Intro";
        AsyncOperation introLoadAsyncOperation = SceneManager.LoadSceneAsync(introSceneName, LoadSceneMode.Single);
        bool introCompleted = false;
        introLoadAsyncOperation.completed += OnIntroLoadCompleted;
        string sceneName = "Gloomhaven_unified";
        AsyncOperation _loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadScene.SetValue(instance, _loadScene);
        _loadScene.allowSceneActivation = true;
        _loadScene.completed += OnLoadCompleted;
        while (!introCompleted)
        {
            yield return null;
        }
        Debug.Log($"[Bootstrap] Finished showing splash screen in {_stopwatch.ElapsedMilliseconds}ms");
        void OnIntroLoadCompleted(AsyncOperation operation)
        {
            introLoadAsyncOperation.completed -= OnIntroLoadCompleted;
            IntroPlayer player = SceneManager.GetSceneByName(introSceneName).GetRootGameObjects().First((GameObject x) => x.GetComponentInChildren<IntroPlayer>() != null).GetComponentInChildren<IntroPlayer>();
            player.EventCompleted += OnIntroCompleted;
            void OnIntroCompleted()
            {
                player.EventCompleted -= OnIntroCompleted;
                SceneManager.UnloadSceneAsync(introSceneName);
                introCompleted = true;
            }
        }
        void OnLoadCompleted(AsyncOperation operation)
        {
            _loadScene.completed -= OnLoadCompleted;
            Debug.Log($"[Bootstrap] Loaded scene in:{_stopwatch.ElapsedMilliseconds}ms RealTime:{Time.realtimeSinceStartup}s");
        }
    }
}

// Start loading all assets in parallel as soon as the bundle manager is awake. These are
// otherwise loaded sequentially later, but there is no need for them to be sequential.
// The bundle manager keeps a map of label -> AsyncOperation, so these are always waited on
// later when the code attempts to load them again.
// This list does not need to be comprehensive; it just needs to get the most expensive labels.
[HarmonyPatch(typeof(AssetBundleManager), "Awake")]
public static class Patch_AssetBundleManager_Awake
{
    private static void Postfix(Dictionary<string, AsyncOperationHandle<IList<UnityEngine.Object>>> ____alwaysloadedHandles)
    {
        FastLoadPlugin.logger.LogInfo($"AssetBundleManager awake - loading assets now");
        MethodInfo loadAddressable = typeof(AssetBundleManager).GetMethod("LoadAlwaysLoadedAddressable", BindingFlags.NonPublic | BindingFlags.Instance);
        loadAddressable.Invoke(AssetBundleManager.Instance, new object[] { "always_loaded_base" });
        loadAddressable.Invoke(AssetBundleManager.Instance, new object[] { "always_loaded_base_high" });
        loadAddressable.Invoke(AssetBundleManager.Instance, new object[] { "always_loaded_standalone" });
    }
}
