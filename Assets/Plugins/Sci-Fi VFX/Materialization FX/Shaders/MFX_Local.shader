// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "QFX/MFX/Materialization_Local"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_AlbedoColor("Albedo Color", Color) = (0,0,0,0)
		_MetallicTexture("Metallic Texture", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_EmissionTexture("Emission Texture", 2D) = "white" {}
		[HDR]_Emission("Emission", Color) = (0,0,0,0)
		_DissolveTexture("Dissolve Texture", 2D) = "white" {}
		_ScaleDissolveTex("Scale Dissolve Tex", Range( 0 , 20)) = 1
		_DissolveSpeed("Dissolve Speed", Vector) = (0.19,0,0,0)
		_GlowDistributionTex("Glow Distribution Tex", 2D) = "white" {}
		_ScaleGlowTex("Scale Glow Tex", Range( 0 , 20)) = 1
		_GlowSpeed("Glow Speed", Vector) = (0.19,0,0,0)
		[HDR]_DissolveColor("Dissolve Color", Color) = (0,0,0,0)
		[HDR]_GlowDistributionColor("Glow Distribution Color", Color) = (0,0,0,0)
		_Cutoff( "Mask Clip Value", Float ) = 0
		_Dissolve("Dissolve", Range( 0 , 1)) = 0
		_DissolveDistance("Dissolve Distance", Range( 0 , 1)) = 0
		_GlowDistance("Glow Distance", Range( 0 , 1)) = 0.1
		_GlowDistribution("Glow Distribution", Range( 0 , 1)) = 0
		_StartPoint("Start Point", Range( -3 , 3)) = 0
		[Toggle]_MirrorMode("Mirror Mode", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float4 _AlbedoColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _MirrorMode;
		uniform float _StartPoint;
		uniform float _DissolveDistance;
		uniform sampler2D _DissolveTexture;
		uniform float2 _DissolveSpeed;
		uniform float _ScaleDissolveTex;
		uniform float _Dissolve;
		uniform float _GlowDistance;
		uniform float4 _DissolveColor;
		uniform sampler2D _EmissionTexture;
		uniform float4 _EmissionTexture_ST;
		uniform float4 _Emission;
		uniform float _GlowDistribution;
		uniform float4 _GlowDistributionColor;
		uniform sampler2D _GlowDistributionTex;
		uniform float2 _GlowSpeed;
		uniform float _ScaleGlowTex;
		uniform sampler2D _MetallicTexture;
		uniform float4 _MetallicTexture_ST;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Cutoff = 0;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = tex2D( _NormalMap, uv_NormalMap ).rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = ( _AlbedoColor * tex2D( _Albedo, uv_Albedo ) ).rgb;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_102_0 = ( _StartPoint - ase_vertex3Pos.y );
			float temp_output_112_0 = ( lerp(temp_output_102_0,length( temp_output_102_0 ),_MirrorMode) - _DissolveDistance );
			float2 panner15 = ( float2( 0,0 ) + _Time.y * _DissolveSpeed);
			float2 uv_TexCoord16 = i.uv_texcoord * float2( 1,1 ) + panner15;
			float temp_output_118_0 = ( temp_output_112_0 + ( _DissolveDistance * ( tex2D( _DissolveTexture, (uv_TexCoord16*_ScaleDissolveTex + float2( 0,0 )) ).r * ( 1.0 - _Dissolve ) ) ) );
			float2 uv_EmissionTexture = i.uv_texcoord * _EmissionTexture_ST.xy + _EmissionTexture_ST.zw;
			float2 panner245 = ( float2( 0,0 ) + _Time.y * _GlowSpeed);
			float2 uv_TexCoord247 = i.uv_texcoord * float2( 1,1 ) + panner245;
			o.Emission = (( temp_output_118_0 <= _GlowDistance ) ? _DissolveColor :  ( ( ( tex2D( _EmissionTexture, uv_EmissionTexture ) * _Emission ) + saturate( ( 1.0 - ( ( 1.0 - (-1.0 + (_GlowDistribution - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) + temp_output_112_0 ) ) ) ) * ( _GlowDistributionColor * tex2D( _GlowDistributionTex, (uv_TexCoord247*_ScaleGlowTex + float2( 0,0 )) ) ) ) ).rgb;
			float2 uv_MetallicTexture = i.uv_texcoord * _MetallicTexture_ST.xy + _MetallicTexture_ST.zw;
			float4 tex2DNode22 = tex2D( _MetallicTexture, uv_MetallicTexture );
			o.Metallic = ( tex2DNode22.r * _Metallic );
			o.Smoothness = ( tex2DNode22.a * _Smoothness );
			o.Alpha = 1;
			clip( temp_output_118_0 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
26;695;1458;297;1104.762;65.7856;1.219759;True;False
Node;AmplifyShaderEditor.CommentaryNode;108;-449.3996,436.1732;Float;False;916.839;553.1674;Length from start point to dissolve and glow;9;118;116;112;111;121;109;102;54;53;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-427.2501,479.042;Float;False;Property;_StartPoint;Start Point;21;0;Create;0;0;-3;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;71;-1795.89,439.7515;Float;False;1268.338;563.6168;Dissolve;11;120;6;28;27;16;15;8;1;258;259;260;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;54;-429.106,616.7678;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;52;-1551.625,-583.8807;Float;False;1698.142;921.6166;Glow;19;235;184;43;168;169;171;187;162;247;243;244;245;237;238;236;242;262;261;279;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;8;-1761.339,550.4435;Float;False;Property;_DissolveSpeed;Dissolve Speed;10;0;Create;0.19,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;102;-133.9673,484.7874;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;1;-1760.064,702.8375;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LengthOpNode;109;65.79552,482.0582;Float;False;1;0;FLOAT;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-1540.383,-532.3555;Float;False;Property;_GlowDistribution;Glow Distribution;20;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;15;-1537.951,609.0345;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;244;-1509.222,-18.74087;Float;False;Property;_GlowSpeed;Glow Speed;13;0;Create;0.19,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TFHCRemapNode;279;-1238.251,-527.8315;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;-1.0;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-426.9607,787.8958;Float;False;Property;_DissolveDistance;Dissolve Distance;18;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;260;-1351.626,699.2545;Float;False;Property;_ScaleDissolveTex;Scale Dissolve Tex;9;0;Create;1;0;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1340.499,565.7324;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;121;257.4265,478.0683;Float;False;Property;_MirrorMode;Mirror Mode;22;0;Create;0;2;0;FLOAT;0,0,0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;243;-1507.947,133.653;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;112;-138.3934,678.3228;Float;False;2;0;FLOAT;0,0,0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;259;-1050.126,575.6544;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;187;-1030.728,-528.7416;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;245;-1259.068,-19.40447;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;171;-875.3428,-531.549;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;72;-1553.69,-1079.764;Float;False;497.2284;451.5944;Emission;3;158;154;157;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1766.626,879.2119;Float;False;Property;_Dissolve;Dissolve;17;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;247;-1071.497,91.41237;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;262;-1085.293,214.4002;Float;False;Property;_ScaleGlowTex;Scale Glow Tex;12;0;Create;1;0;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-834.1752,496.6715;Float;True;Property;_DissolveTexture;Dissolve Texture;8;0;Create;None;None;True;0;True;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;28;-1468.956,882.8278;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;258;-641.5264,784.5545;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;154;-1524.936,-1025.039;Float;True;Property;_EmissionTexture;Emission Texture;6;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;261;-763.2472,47.58806;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;157;-1438.803,-813.0695;Float;False;Property;_Emission;Emission;7;1;[HDR];Create;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;169;-736.6902,-529.8808;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-1266.298,858.3749;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;237;-551.6085,-325.0674;Float;False;Property;_GlowDistributionColor;Glow Distribution Color;15;1;[HDR];Create;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;168;-561.0245,-532.0078;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;235;-477.6174,20.49034;Float;True;Property;_GlowDistributionTex;Glow Distribution Tex;11;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;-1202.753,-1021.278;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;236;-374.142,-532.3733;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-225.6248,-160.9512;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;-129.7461,870.5278;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;480.5063,-450.7509;Float;True;Property;_MetallicTexture;Metallic Texture;2;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;184;-1539.681,-457.5076;Float;False;Property;_GlowDistance;Glow Distance;19;0;Create;0.1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;242;-21.0689,-348.6241;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;43;-115.9648,154.1475;Float;False;Property;_DissolveColor;Dissolve Color;14;1;[HDR];Create;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;511.2095,-259.668;Float;False;Property;_Metallic;Metallic;3;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;486.7973,-675.9405;Float;True;Property;_Albedo;Albedo;0;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;18;495.5656,-855.3488;Float;False;Property;_AlbedoColor;Albedo Color;1;0;Create;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;118;307.9797,675.1584;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;511.7144,-185.3674;Float;False;Property;_Smoothness;Smoothness;4;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;848.6904,-253.89;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;225;1064.39,-708.577;Float;True;Property;_NormalMap;Normal Map;5;1;[Normal];Create;None;None;True;0;True;bump;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareLowerEqual;282;652.7615,50.52546;Float;False;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;830.6909,-403.6713;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;784.6685,-750.3572;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1280.644,-2.991783;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;QFX/Materialization_Local;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;0;False;0;0;Custom;0;True;True;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;6.91;1,0.3529412,0.4734279,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;16;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;102;0;53;0
WireConnection;102;1;54;2
WireConnection;109;0;102;0
WireConnection;15;2;8;0
WireConnection;15;1;1;2
WireConnection;279;0;162;0
WireConnection;16;1;15;0
WireConnection;121;0;102;0
WireConnection;121;1;109;0
WireConnection;112;0;121;0
WireConnection;112;1;111;0
WireConnection;259;0;16;0
WireConnection;259;1;260;0
WireConnection;187;0;279;0
WireConnection;245;2;244;0
WireConnection;245;1;243;2
WireConnection;171;0;187;0
WireConnection;171;1;112;0
WireConnection;247;1;245;0
WireConnection;6;1;259;0
WireConnection;28;0;27;0
WireConnection;258;0;6;1
WireConnection;261;0;247;0
WireConnection;261;1;262;0
WireConnection;169;0;171;0
WireConnection;120;0;258;0
WireConnection;120;1;28;0
WireConnection;168;0;169;0
WireConnection;235;1;261;0
WireConnection;158;0;154;0
WireConnection;158;1;157;0
WireConnection;236;0;158;0
WireConnection;236;1;168;0
WireConnection;238;0;237;0
WireConnection;238;1;235;0
WireConnection;116;0;111;0
WireConnection;116;1;120;0
WireConnection;242;0;236;0
WireConnection;242;1;238;0
WireConnection;118;0;112;0
WireConnection;118;1;116;0
WireConnection;45;0;22;4
WireConnection;45;1;20;0
WireConnection;282;0;118;0
WireConnection;282;1;184;0
WireConnection;282;2;43;0
WireConnection;282;3;242;0
WireConnection;25;0;22;1
WireConnection;25;1;24;0
WireConnection;19;0;18;0
WireConnection;19;1;17;0
WireConnection;0;0;19;0
WireConnection;0;1;225;0
WireConnection;0;2;282;0
WireConnection;0;3;25;0
WireConnection;0;4;45;0
WireConnection;0;10;118;0
ASEEND*/
//CHKSM=65BCB409BEC0B2C38BE8B706C001A1A1ED0D8D16