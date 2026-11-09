Shader "Hidden/CryptCleaners/GooMaskPaint"
{
    Properties
    {
        _MainTex ("Previous Mask", 2D) = "white" {}
        _BrushUV ("Brush UV", Vector) = (0.5, 0.5, 0, 0)
        _BrushRadius ("Brush Radius", Float) = 0.05
        _BrushHardness ("Brush Hardness", Range(0, 1)) = 0.5
        _BrushStrength ("Brush Strength", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _BrushUV;
            float4 _SurfaceSize;
            float _BrushRadius;
            float _BrushHardness;
            float _BrushStrength;

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.uv = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 previousMask = SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    input.uv
                );

                float2 delta = input.uv - _BrushUV.xy;
                delta *= _SurfaceSize.xy;
                float distanceToBrush = length(delta);

                float radius = max(_BrushRadius, 0.0001);

                float hardness = saturate(_BrushHardness);

                float innerRadius = radius * hardness;

                float brush = 1.0 - smoothstep(
                    innerRadius,
                    radius,
                    distanceToBrush
                );

                brush *= _BrushStrength;

                // Erase mask values.
                previousMask.r *= (1.0 - brush);

                return previousMask;
            }

            ENDHLSL
        }
    }
}
