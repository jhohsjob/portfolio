Shader "Custom/Map"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Tiling("Tiling", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float _Tiling;

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);  // Object ¡æ Clip
                o.worldPos = TransformObjectToWorld(v.positionOS);      // Object ¡æ World
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.worldPos.xz * _Tiling;
                return tex2D(_MainTex, uv);
            }
            ENDHLSL
        }
    }
}
