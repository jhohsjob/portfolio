Shader "Custom/Dash"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0.5, 1, 1, 1)
        _BlurStrength ("Blur Strength", Range(0, 1)) = 0.5
        _Direction ("Direction", Vector) = (1, 0, 0, 0)
        _Alpha ("Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseMap_ST;
            float4 _TintColor;
            float _BlurStrength;
            float4 _Direction;
            float _Alpha;

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

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 dir = normalize(_Direction.xy);
                float blurStep = _BlurStrength * 0.05;

                half4 sum = 0;
                // »ùÇÃ¸µ ¿©·¯ ¹ø ÇØ¼­ ¹øÁü
                for (int j = -2; j <= 2; j++)
                {
                    sum += SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv + dir * j * blurStep);
                }

                sum /= 5;
                sum.rgb *= _TintColor.rgb;
                sum.a *= _Alpha;

                return sum;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
