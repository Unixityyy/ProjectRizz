using Liv.Lck.Settings;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Liv.Lck.Core
{
    public static class LckCoreHandler
    {
        internal static Result<bool> LckCoreInitializationResult { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
#else
            InitializeInternal();
#endif
        }

        private static void InitializeInternal()
        {
            var settings = LckSettings.Instance;
            LckCore.SetMaxLogLevel(settings.CoreLogLevel);

            if(settings.CoreLogLevel == LevelFilter.Info)
                Debug.Log("LCK Core Handler initializing...");
            
            var gameInfo = new GameInfo
            {
                GameName = settings.GameName,
                GameVersion = Application.version,
                ProjectName = Application.productName,
                CompanyName = Application.companyName,
                EngineVersion = Application.unityVersion,
                RenderPipeline = GetRenderPipelineType(),
                GraphicsAPI = SystemInfo.graphicsDeviceType.ToString(),
                Platform = Application.platform.ToString()
            };

            var lckInfo = new LckInfo
            {
                Version = LckSettings.Version,
                BuildNumber = LckSettings.Build
            };

            LckCoreInitializationResult = LckCore.Initialize(settings.TrackingId, gameInfo, lckInfo);
            if (!LckCoreInitializationResult.IsOk)
            {
                if (LckCoreInitializationResult.Err == CoreError.MissingTrackingId)
                {
                    Debug.LogError("LCK: Missing or bad Tracking ID supplied. Recording and streaming will not be available.");
                } else {
                    Debug.LogError($"LCK: LCK Core initialization failed: {LckCoreInitializationResult.Err} - {LckCoreInitializationResult.Message}");
                }

                return;
            }
        }


        private static string GetRenderPipelineType()
        {
            if (GraphicsSettings.defaultRenderPipeline)
            {
                if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("HDRenderPipelineAsset"))
                {
                    return "High Definition render pipeline";
                }
                else if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
                {
                    return "Universal render pipeline";
                }
                else
                {
                    return "Custom render pipeline";
                }
            }
            else
            {
                return "Built-in render pipeline";
            }
        }

#if UNITY_EDITOR
        private static void HandlePlayModeStateChange(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingPlayMode)
            {
                if(LckSettings.Instance.CoreLogLevel == LevelFilter.Info)
                    Debug.Log("LCK Core Handler initializing...");

                LckCoreInitializationResult = null;
                LckCore.Dispose();
            } else if (change == PlayModeStateChange.EnteredPlayMode)
            {
                InitializeInternal();
            }
        }
#endif
    }
}
