using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class QuestPerformance : MonoBehaviour
{
    [Header("Render Scale Settings")]
    [Range(0.5f, 1.5f)] public float minRenderScale = 0.7f;
    [Range(0.5f, 1.5f)] public float maxRenderScale = 1.0f;
    public float adjustmentStep = 0.05f;

    [Header("Performance Targets")]
    public float targetFPS = 72f;
    public float tolerance = 5f;

    private UniversalRenderPipelineAsset urpAsset;
    private float currentRenderScale;
    private float fpsTimer;
    private int frameCount;

    void Start()
    {
        urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        currentRenderScale = urpAsset != null ? urpAsset.renderScale : 1.0f;

        #if UNITY_EDITOR
        Debug.Log($"[AdaptiveRenderScale] Initialized with target FPS: {targetFPS}");
        #endif
    }

    void Update()
    {
        frameCount++;
        fpsTimer += Time.unscaledDeltaTime;

        if (fpsTimer >= 1f)
        {
            float currentFPS = frameCount / fpsTimer;
            AdjustRenderScale(currentFPS);
            frameCount = 0;
            fpsTimer = 0f;
        }
    }

    void AdjustRenderScale(float currentFPS)
    {
        if (urpAsset == null) return;

        if (currentFPS < targetFPS - tolerance && currentRenderScale > minRenderScale)
        {
            currentRenderScale = Mathf.Max(minRenderScale, currentRenderScale - adjustmentStep);
            urpAsset.renderScale = currentRenderScale;

            #if UNITY_EDITOR
            Debug.Log($"[AdaptiveRenderScale] FPS low ({currentFPS}), decreasing scale to {currentRenderScale}");
            #endif
        }
        else if (currentFPS > targetFPS + tolerance && currentRenderScale < maxRenderScale)
        {
            currentRenderScale = Mathf.Min(maxRenderScale, currentRenderScale + adjustmentStep);
            urpAsset.renderScale = currentRenderScale;

            #if UNITY_EDITOR
            Debug.Log($"[AdaptiveRenderScale] FPS high ({currentFPS}), increasing scale to {currentRenderScale}");
            #endif
        }
    }
}
