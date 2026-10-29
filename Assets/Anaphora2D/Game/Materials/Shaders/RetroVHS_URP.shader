Shader "Custom/RetroVHS_URP"
{
    Properties
    {
        _PixelSize("Pixelation Size", Range(1, 10)) = 3
        _Distortion("Fish Eye Bend", Range(0, 0.5)) = 0.15
        _Aberration("VHS Color Bleed", Range(0, 0.05)) = 0.005
        _ScanlineAlpha("Scanline Darkness", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off ZTest Always

        Pass
        {
            Name "VHSFullScreen"

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

            TEXTURE2D_X(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _PixelSize;
                float _Distortion;
                float _Aberration;
                float _ScanlineAlpha;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                // THE FIX: Ignore 3D math! Pin the square directly to the flat screen coordinates.
                o.positionCS = float4(v.positionOS.xy, 0.0, 1.0);
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

                // 1. Pixelation
                float2 screenSize = _ScreenParams.xy / _PixelSize;
                uv = floor(uv * screenSize) / screenSize;

                // 2. Fish Eye Bend
                uv = CRTBend(uv);

                // Black borders for the edges of the curved screen
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                    return half4(0, 0, 0, 1);

                // 3. Chromatic Aberration
                float2 uvR = uv + float2(_Aberration, 0);
                float2 uvB = uv - float2(_Aberration, 0);

                half r = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uvR).r;
                half g = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uv).g;
                half b = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, uvB).b;

                // 4. TV Scanlines
                float scanline = sin(uv.y * _ScreenParams.y * 2.0) * _ScanlineAlpha;

                return half4(r - scanline, g - scanline, b - scanline, 1.0);
            }
            ENDHLSL
        }
    }
}