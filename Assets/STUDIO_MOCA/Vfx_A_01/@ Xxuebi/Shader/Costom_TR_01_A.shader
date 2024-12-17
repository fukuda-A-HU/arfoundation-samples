// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Costom_/Costom_TR_01_A"
{
	Properties
	{
		_Tex_Main("Tex_Main", 2D) = "white" {}
		_Tex_Mask("Tex_Mask", 2D) = "white" {}
		_Tex_Noise("Tex_Noise", 2D) = "white" {}
		[HDR]_Main_Color("Main_Color", Color) = (0.4386792,0.7203457,1,0)
		_Main_UVSpeed("Main_UVSpeed", Vector) = (0,0,0,0)
		_Mask_UVSpeed("Mask_UVSpeed", Vector) = (0,0,0,0)
		_Main_Noise("Main_Noise", Vector) = (0,0,0,0)
		_Noist_Int("Noist_Int", Float) = 0
		_Mask_Int("Mask_Int", Float) = 0
		_Add_Tex("Add_Tex", 2D) = "white" {}
		_Add_Tex_UVSpeed("Add_Tex_UVSpeed", Vector) = (0,0,0,0)
		[HDR]_Add_Tex_Color("Add_Tex_Color", Color) = (0,0,0,0)
		_Add_Mask("Add_Mask", 2D) = "white" {}
		_Add_Mask_UVSpeed("Add_Mask_UVSpeed", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Overlay+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Add
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float4 uv_tex4coord;
			float4 vertexColor : COLOR;
		};

		uniform float4 _Add_Tex_Color;
		uniform sampler2D _Add_Mask;
		uniform float2 _Add_Mask_UVSpeed;
		uniform float4 _Add_Mask_ST;
		uniform sampler2D _Add_Tex;
		uniform float2 _Add_Tex_UVSpeed;
		uniform float4 _Add_Tex_ST;
		uniform float4 _Main_Color;
		uniform sampler2D _Tex_Main;
		uniform float _Noist_Int;
		uniform sampler2D _Tex_Noise;
		uniform float2 _Main_Noise;
		uniform float4 _Tex_Noise_ST;
		uniform float2 _Main_UVSpeed;
		uniform float4 _Tex_Main_ST;
		uniform sampler2D _Tex_Mask;
		uniform float2 _Mask_UVSpeed;
		uniform float4 _Tex_Mask_ST;
		uniform float _Mask_Int;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv0_Add_Mask = i.uv_texcoord * _Add_Mask_ST.xy + _Add_Mask_ST.zw;
			float2 panner43 = ( 1.0 * _Time.y * _Add_Mask_UVSpeed + uv0_Add_Mask);
			float2 uv0_Add_Tex = i.uv_texcoord * _Add_Tex_ST.xy + _Add_Tex_ST.zw;
			float2 panner39 = ( 1.0 * _Time.y * _Add_Tex_UVSpeed + uv0_Add_Tex);
			float2 uv0_Tex_Noise = i.uv_texcoord * _Tex_Noise_ST.xy + _Tex_Noise_ST.zw;
			float2 panner20 = ( 1.0 * _Time.y * _Main_Noise + uv0_Tex_Noise);
			float2 uv0_Tex_Main = i.uv_texcoord * _Tex_Main_ST.xy + _Tex_Main_ST.zw;
			float2 panner10 = ( 1.0 * _Time.y * _Main_UVSpeed + uv0_Tex_Main);
			float4 tex2DNode2 = tex2D( _Tex_Main, ( ( _Noist_Int * tex2D( _Tex_Noise, panner20 ).r ) + panner10 ) );
			o.Emission = ( ( _Add_Tex_Color * ( tex2D( _Add_Mask, panner43 ).r * tex2D( _Add_Tex, panner39 ).r ) ) + ( ( i.uv_tex4coord.z + 1.0 ) * ( _Main_Color * tex2DNode2.r * i.vertexColor ) ) ).rgb;
			float2 uv0_Tex_Mask = i.uv_texcoord * _Tex_Mask_ST.xy + _Tex_Mask_ST.zw;
			float2 panner17 = ( 1.0 * _Time.y * _Mask_UVSpeed + uv0_Tex_Mask);
			o.Alpha = ( ( i.vertexColor.a * tex2DNode2.r * tex2D( _Tex_Mask, panner17 ).r ) * _Mask_Int );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
796;209;1338;937;1222.53;1484.681;1.6;True;True
Node;AmplifyShaderEditor.Vector2Node;18;-2130.68,-540.9243;Inherit;False;Property;_Main_Noise;Main_Noise;7;0;Create;True;0;0;False;0;0,0;-0.4,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-2153.637,-829.9697;Inherit;True;0;21;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;20;-1818.824,-716.5173;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;21;-1454.981,-736.5979;Inherit;True;Property;_Tex_Noise;Tex_Noise;2;0;Create;True;0;0;False;0;-1;None;b0730eaea09fe1041af4274f09337ce8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;27;-1219.671,-873.6426;Inherit;False;Property;_Noist_Int;Noist_Int;8;0;Create;True;0;0;False;0;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1570.79,-131.8786;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;11;-1548.428,14.53491;Inherit;False;Property;_Main_UVSpeed;Main_UVSpeed;5;0;Create;True;0;0;False;0;0,0;-1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;37;-265.6574,-928.2768;Inherit;False;0;30;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;41;-300.5528,-1075.695;Inherit;False;Property;_Add_Mask_UVSpeed;Add_Mask_UVSpeed;14;0;Create;True;0;0;False;0;0,0;-0.6,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;10;-1259.332,-119.3493;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1042.845,-686.9025;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;38;-245.6781,-781.8633;Inherit;False;Property;_Add_Tex_UVSpeed;Add_Tex_UVSpeed;11;0;Create;True;0;0;False;0;0,0;-0.8,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;-322.9148,-1222.109;Inherit;False;0;40;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-1007.402,-406.4938;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;39;45.8005,-915.7475;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;43;-11.45691,-1209.58;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;15;-1245.15,368.3235;Inherit;False;Property;_Mask_UVSpeed;Mask_UVSpeed;6;0;Create;True;0;0;False;0;0,0;-0.2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1267.512,221.91;Inherit;False;0;8;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;6;-116.5753,-80.139;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;17;-956.0536,234.4393;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;30;308.6082,-820.0499;Inherit;True;Property;_Add_Tex;Add_Tex;10;0;Create;True;0;0;False;0;-1;None;cf3023a54377d2e47a898131998589bb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-56.99945,-543.7614;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-782.4898,-315.7866;Inherit;True;Property;_Tex_Main;Tex_Main;0;0;Create;True;0;0;False;0;-1;None;38c9f9b2926a48f4a8e1971010434458;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;40;264.3853,-1278.017;Inherit;True;Property;_Add_Mask;Add_Mask;13;0;Create;True;0;0;False;0;-1;None;b0730eaea09fe1041af4274f09337ce8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-346.9977,-398.8534;Inherit;False;Property;_Main_Color;Main_Color;4;1;[HDR];Create;True;0;0;False;0;0.4386792,0.7203457,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;35;897.399,-999.8058;Inherit;False;Property;_Add_Tex_Color;Add_Tex_Color;12;1;[HDR];Create;True;0;0;False;0;0,0,0,0;106.2642,173.2645,1024,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;13;250.0005,-388.7614;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-713.9947,174.3029;Inherit;True;Property;_Tex_Mask;Tex_Mask;1;0;Create;True;0;0;False;0;-1;None;b0730eaea09fe1041af4274f09337ce8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;612.6703,-908.6809;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;166.741,-224.9896;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;29;317.1176,318.2271;Inherit;False;Property;_Mask_Int;Mask_Int;9;0;Create;True;0;0;False;0;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;184.8472,77.40984;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;428.0005,-240.7614;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;914.3524,-512.371;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;870.0847,-282.2255;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;579.0507,135.9467;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1171.194,-213.6322;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Costom_/Costom_TR_01_A;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Overlay;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;3;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;19;0
WireConnection;20;2;18;0
WireConnection;21;1;20;0
WireConnection;10;0;9;0
WireConnection;10;2;11;0
WireConnection;26;0;27;0
WireConnection;26;1;21;1
WireConnection;25;0;26;0
WireConnection;25;1;10;0
WireConnection;39;0;37;0
WireConnection;39;2;38;0
WireConnection;43;0;42;0
WireConnection;43;2;41;0
WireConnection;17;0;16;0
WireConnection;17;2;15;0
WireConnection;30;1;39;0
WireConnection;2;1;25;0
WireConnection;40;1;43;0
WireConnection;13;0;12;3
WireConnection;8;1;17;0
WireConnection;44;0;40;1
WireConnection;44;1;30;1
WireConnection;3;0;5;0
WireConnection;3;1;2;1
WireConnection;3;2;6;0
WireConnection;7;0;6;4
WireConnection;7;1;2;1
WireConnection;7;2;8;1
WireConnection;14;0;13;0
WireConnection;14;1;3;0
WireConnection;32;0;35;0
WireConnection;32;1;44;0
WireConnection;36;0;32;0
WireConnection;36;1;14;0
WireConnection;28;0;7;0
WireConnection;28;1;29;0
WireConnection;0;2;36;0
WireConnection;0;9;28;0
ASEEND*/
//CHKSM=BE578CDDCEAA4459182BAB23609ABF6BE03DA339