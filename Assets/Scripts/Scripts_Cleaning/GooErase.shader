Shader "CryptCleaners/GooErase"
{
    Properties
    {
        [PerRendererData] _MainTex ("Goo Texture", 2D) = "white" {}
        _DirtMask ("Dirt Mask", 2D) = "white" {}
        _GooColor ("Goo Color", Color) = (0.2, 0.8, 0.2, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "GooErase"

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

            TEXTURE2D(_DirtMask);
            SAMPLER(sampler_DirtMask);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _GooColor;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 goo = SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    input.uv
                );

                half mask = SAMPLE_TEXTURE2D(
                    _DirtMask,
                    sampler_DirtMask,
                    input.uv
                ).r;

                goo *= _GooColor;

                // White mask = goo visible.
                // Black mask = goo erased.
                goo.a *= mask;

                return goo;
            }

            ENDHLSL
        }
    }
}

