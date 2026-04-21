using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class VHSFeature : ScriptableRendererFeature
{
    class VHSRenderPass : ScriptableRenderPass
    {
        public Material vhsMaterial;

        // Render Graph requires a data container for the pass inputs
        private class PassData
        {
            public TextureHandle source;
            public Material material;
        }

        public VHSRenderPass(Material material)
        {
            this.vhsMaterial = material;

            // Crucial for Unity 6: Tells the pipeline we intend to modify the camera target
            requiresIntermediateTexture = true;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (vhsMaterial == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            // Skip if there's no valid target (e.g., rendering directly to the backbuffer)
            if (resourceData.isActiveTargetBackBuffer)
                return;

            // Get the current camera color target
            TextureHandle source = resourceData.activeColorTexture;

            // Create a temporary texture to hold our VHS effect
            RenderTextureDescriptor descriptor = cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0; // We only need color
            TextureHandle tempTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, descriptor, "_VHSTempTexture", false);

            // --- PASS 1: Apply the VHS Material ---
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("VHS Filter", out var passData))
            {
                passData.source = source;
                passData.material = vhsMaterial;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderAttachment(tempTexture, 0, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // Blitter replaces the old cmd.Blit in the Render Graph
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // --- PASS 2: Copy the effect back to the Camera Target ---
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("VHS Filter Copy Back", out var passData))
            {
                passData.source = tempTexture;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderAttachment(source, 0, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // Blitting with a material index of 0 and passing 'false' just copies the texture
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0, false);
                });
            }
        }
    }

    [System.Serializable]
    public class VHSSettings
    {
        public Material material;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public VHSSettings settings = new VHSSettings();
    private VHSRenderPass vhsPass;

    public override void Create()
    {
        vhsPass = new VHSRenderPass(settings.material);
        vhsPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null)
        {
            Debug.LogWarning("Missing VHS Material in Renderer Feature");
            return;
        }
        renderer.EnqueuePass(vhsPass);
    }
}