Shader "Anaphora/Sanity Sandbox Surface"
{
    Properties { _BaseColor ("Color", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            ZWrite On
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            CBUFFER_END
            struct Attributes { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct Varyings { float4 position : SV_POSITION; float shade : TEXCOORD0; };
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.shade = 0.55 + 0.45 * saturate(dot(UnityObjectToWorldNormal(input.normal), normalize(float3(-0.3, 0.8, -0.5))));
                return output;
            }
            half4 frag(Varyings input) : SV_Target { return half4(_BaseColor.rgb * input.shade, 1); }
            ENDHLSL
        }
    }
}
