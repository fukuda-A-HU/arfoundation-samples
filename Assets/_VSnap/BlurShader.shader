Shader "Unlit/BlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurRadius ("Blur Radius", Range(0.0, 10.0)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Name "Blur"
            Tags { "LightMode" = "UniversalForward" }

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

            sampler2D _MainTex;
            float _BlurRadius;
            float4 _MainTex_TexelSize;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 texelSize = _MainTex_TexelSize.xy;
                float radius = _BlurRadius;

                // Sample offsets for a simple 9-tap blur
                float2 offsets[9] = {
                    float2(-1, -1), float2(0, -1), float2(1, -1),
                    float2(-1,  0), float2(0,  0), float2(1,  0),
                    float2(-1,  1), float2(0,  1), float2(1,  1)
                };

                float weights[9] = {
                    0.0625, 0.125, 0.0625,
                    0.125,  0.25,  0.125,
                    0.0625, 0.125, 0.0625
                };

                half4 color = 0;
                for (int j = 0; j < 9; j++)
                {
                    float2 sampleUV = i.uv + offsets[j] * texelSize * radius;
                    color += tex2D(_MainTex, sampleUV) * weights[j];
                }

                return color;
            }
            ENDHLSL
        }
    }
}
