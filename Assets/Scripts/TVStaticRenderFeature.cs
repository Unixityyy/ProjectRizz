using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class TVStaticRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class TVStaticSettings
    {
        public Material staticMaterial;
        [Range(0, 1)] public float noiseIntensity = 0.5f;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public TVStaticSettings settings = new TVStaticSettings();

    class TVStaticPass : ScriptableRenderPass
    {
        Material material;
        float intensity;

#pragma warning disable CS0618
        RenderTargetHandle tempTexture;
#pragma warning restore CS0618

        public TVStaticPass(Material mat, float noise)
        {
            material = mat;
            intensity = noise;
            tempTexture.Init("_TempTVStaticTex");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("TV Static Pass");

            material.SetFloat("_NoiseIntensity", intensity);

#pragma warning disable CS0618
            var source = renderingData.cameraData.renderer.cameraColorTarget;
#pragma warning restore CS0618

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTexture.id, opaqueDesc);
            cmd.Blit(source, tempTexture.Identifier());
            cmd.Blit(tempTexture.Identifier(), source, material);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null) return;
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    TVStaticPass pass;

    public override void Create()
    {
        pass = new TVStaticPass(settings.staticMaterial, settings.noiseIntensity)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isSceneViewCamera || Application.isPlaying)
        {
            renderer.EnqueuePass(pass);
        }
    }
}