using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(CRTScanLineRenderer), PostProcessEvent.AfterStack, "Unlit/CRTScanLine")]
public sealed class CRTScanLineEffect : PostProcessEffectSettings
{
    public IntParameter lineFrequency = new IntParameter { value = 500 };

    // You can add more parameters here if needed for your shader.
}

public sealed class CRTScanLineRenderer : PostProcessEffectRenderer<CRTScanLineEffect>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Unlit/CRTScanLine"));
        sheet.properties.SetInt("_LineFrequency", settings.lineFrequency);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
