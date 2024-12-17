Shader "Custom/Transparent"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 vertex : POSITION;
            };
            
            struct Varyings
            {
                float4 position : SV_POSITION;
            };
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.position = TransformObjectToHClip(input.vertex);
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                return half4(0, 0, 0, 1);
            }
            
            ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
}