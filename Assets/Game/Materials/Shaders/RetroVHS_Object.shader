Shader "Custom/RetroVHS_Object"
{
    Properties
    {
        // Added MainTex so you can assign an image or Video Player RenderTexture
        _MainTex ("Base Texture", 2D) = "white" {}
        
        // Tweaked ranges to work better with object UVs rather than screen pixels
        _PixelSize("Pixelation Grid Size", Range(10, 500)) = 150
        _Distortion("Fish Eye Bend", Range(0, 0.5)) = 0.15
        _Aberration("VHS Color Bleed", Range(0, 0.05)) = 0.005
        _ScanlineAlpha("Scanline Darkness", Range(0, 1)) = 0.1
        _ScanlineDensity("Scanline Density", Range(10, 500)) = 250
    }
    SubShader
    {
        // Standard Opaque tags so it renders like a normal 3D object
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "Unlit"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Standard Texture2D instead of Texture2D_X for normal objects
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _PixelSize;
                float _Distortion;
                float _Aberration;
                float _ScanlineAlpha;
                float _ScanlineDensity;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                // Standard 3D transformation so it sticks to your mesh in the world
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float2 CRTBend(float2 uv)
            {
                uv = uv * 2.0 - 1.0;
                uv.x *= 1.0 + (uv.y * uv.y) * _Distortion;
                uv.y *= 1.0 + (uv.x * uv.x) * _Distortion;
                uv = uv * 0.5 + 0.5;
                return uv;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 1. Pixelation (Locked to the object's UVs, not the screen resolution)
                float2 gridSize = float2(_PixelSize, _PixelSize);
                uv = floor(uv * gridSize) / gridSize;

                // 2. Fish Eye Bend
                uv = CRTBend(uv);

                // Black borders for the curved edges
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                    return half4(0, 0, 0, 1);

                // 3. Chromatic Aberration
                float2 uvR = uv + float2(_Aberration, 0);
                float2 uvB = uv - float2(_Aberration, 0);

                half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvR).r;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).g;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvB).b;

                // 4. TV Scanlines (Locked to the object's UVs)
                float scanline = sin(uv.y * _ScanlineDensity) * _ScanlineAlpha;

                return half4(r - scanline, g - scanline, b - scanline, 1.0);
            }
            ENDHLSL
        }
    }
}