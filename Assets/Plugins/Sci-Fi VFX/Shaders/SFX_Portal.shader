// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "QFX/SFX/Portal"
{
	Properties
	{
		[HDR]_MainColor("Main Color", Color) = (1,1,1,1)
		_MainTexture("Main Texture", 2D) = "white" {}
		_CutOutA("CutOut (A)", 2D) = "white" {}
		_DistortionTexture("Distortion Texture", 2D) = "bump" {}
		_DistortionSpeed("Distortion Speed", Vector) = (0,0,0,0)
		_Distortion("Distortion", Range( 0 , 1)) = 0
		_DisplaceTexture("Displace Texture", 2D) = "white" {}
		_DisplaceScale("Displace Scale", Range( 0 , 10)) = 0
		_DisplaceSpeedXY("Displace Speed XY", Vector) = (0,0,0,0)
		_Displace("Displace", Range( 0 , 2)) = 0
		[HDR]_EmissionColor("Emission Color", Color) = (0,0,0,0)
		_EmissionTexture("Emission Texture", 2D) = "white" {}
		[HDR]_DepthEmission("Depth Emission", Color) = (0,0,0,0)
		_DepthDistance("Depth Distance", Range( 0 , 0.5)) = 0
		_NormalTexture("Normal Texture", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 5)) = 0
		_SwirlSpeed("Swirl Speed", Range( 0 , 1)) = 1
		_Angle("Angle", Range( 0 , 90)) = 72
		_Radius("Radius", Range( 0 , 1)) = 0.3764706
		_SwirlRotationSpeed("Swirl Rotation Speed", Range( 0 , 2)) = 0
		[Toggle]_Rotation("Rotation", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _NormalScale;
		uniform sampler2D _NormalTexture;
		uniform float _Rotation;
		uniform float _SwirlRotationSpeed;
		uniform sampler2D _DisplaceTexture;
		uniform float4 _DisplaceTexture_ST;
		uniform float _Radius;
		uniform float _SwirlSpeed;
		uniform float _Angle;
		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform float2 _DisplaceSpeedXY;
		uniform float _DisplaceScale;
		uniform float _Displace;
		uniform float4 _MainColor;
		uniform sampler2D _GrabTexture;
		uniform float _Distortion;
		uniform sampler2D _DistortionTexture;
		uniform float2 _DistortionSpeed;
		uniform float4 _DistortionTexture_ST;
		uniform float4 _DepthEmission;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthDistance;
		uniform sampler2D _EmissionTexture;
		uniform float4 _EmissionColor;
		uniform sampler2D _CutOutA;
		uniform float4 _CutOutA_ST;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float temp_output_335_0 = ( _Time.w * _SwirlRotationSpeed );
			float2 uv_DisplaceTexture = i.uv_texcoord * _DisplaceTexture_ST.xy + _DisplaceTexture_ST.zw;
			float2 _Anchor = float2(0.5,0.5);
			float2 temp_output_192_0 = ( uv_DisplaceTexture - _Anchor );
			float2 SwirlCenter261 = temp_output_192_0;
			float temp_output_284_0 = (0.35 + (_Radius - 0.0) * (1.0 - 0.35) / (1.0 - 0.0));
			float SwirlLength252 = length( temp_output_192_0 );
			float SwirlPercent259 = ( ( temp_output_284_0 - SwirlLength252 ) / temp_output_284_0 );
			float SwirlAngle254 = ( ( _SwirlSpeed * -1.0 ) * _Angle );
			float temp_output_236_0 = ( pow( SwirlPercent259 , 2.0 ) * SwirlAngle254 );
			float temp_output_243_0 = cos( temp_output_236_0 );
			float temp_output_242_0 = sin( temp_output_236_0 );
			float2 appendResult240 = (float2(temp_output_243_0 , ( temp_output_242_0 * -1.0 )));
			float dotResult244 = dot( SwirlCenter261 , appendResult240 );
			float2 appendResult247 = (float2(temp_output_242_0 , temp_output_243_0));
			float dotResult248 = dot( SwirlCenter261 , appendResult247 );
			float2 appendResult250 = (float2(dotResult244 , dotResult248));
			float2 SwirlPivot264 = _Anchor;
			float2 temp_output_251_0 = ( appendResult250 + SwirlPivot264 );
			float2 panner357 = ( temp_output_251_0 + temp_output_335_0 * float2( 0,0.1 ));
			float cos333 = cos( temp_output_335_0 );
			float sin333 = sin( temp_output_335_0 );
			float2 rotator333 = mul( temp_output_251_0 - float2( 0.5,0.5 ) , float2x2( cos333 , -sin333 , sin333 , cos333 )) + float2( 0.5,0.5 );
			float2 SwirlUV171 = lerp(panner357,rotator333,_Rotation);
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float2 panner83 = ( uv_DisplaceTexture + 1.0 * _Time.y * _DisplaceSpeedXY);
			float4 temp_cast_2 = (1.0).xxxx;
			float3 DisplaceUV98 = ( float3( uv_MainTexture ,  0.0 ) + ( (( ( tex2D( _DisplaceTexture, (panner83*_DisplaceScale + 0.0) ) * 2.0 ) - temp_cast_2 )).rgb * _Displace ) );
			float3 lerpResult274 = lerp( float3( SwirlUV171 ,  0.0 ) , DisplaceUV98 , 0.3);
			float3 SwirlAndDisplaceUV290 = lerpResult274;
			float3 Normal134 = UnpackScaleNormal( tex2D( _NormalTexture, SwirlAndDisplaceUV290.xy ) ,_NormalScale );
			o.Normal = Normal134;
			float2 uv_DistortionTexture = i.uv_texcoord * _DistortionTexture_ST.xy + _DistortionTexture_ST.zw;
			float2 panner121 = ( uv_DistortionTexture + 1.0 * _Time.y * _DistortionSpeed);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor12 = tex2D( _GrabTexture, ( float4( ( _Distortion * UnpackNormal( tex2D( _DistortionTexture, (panner121*1.0 + SwirlAndDisplaceUV290.xy) ) ) ) , 0.0 ) + ase_grabScreenPosNorm ).xy );
			float4 Albedo74 = ( ( _MainColor * tex2D( _MainTexture, SwirlAndDisplaceUV290.xy ) ) + screenColor12 );
			o.Albedo = Albedo74.rgb;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth349 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth349 = abs( ( screenDepth349 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthDistance ) );
			float clampResult351 = clamp( ( 1.0 - distanceDepth349 ) , 0.0 , 1.0 );
			float4 Emission62 = ( ( _DepthEmission * clampResult351 ) + ( tex2D( _EmissionTexture, SwirlAndDisplaceUV290.xy ) * _EmissionColor ) );
			o.Emission = Emission62.rgb;
			float2 uv_CutOutA = i.uv_texcoord * _CutOutA_ST.xy + _CutOutA_ST.zw;
			float tmpspeed342 = _SwirlSpeed;
			float Opacity69 = ( tex2D( _CutOutA, uv_CutOutA ).a * _MainColor.a * tmpspeed342 );
			o.Alpha = Opacity69;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
7;646;1904;387;5355.067;-2180.534;3.895118;True;False
Node;AmplifyShaderEditor.CommentaryNode;138;-2450.254,2455.156;Float;False;4345.476;1365.14;Comment;47;271;270;290;274;342;330;228;171;358;357;333;335;251;250;264;265;334;336;244;248;240;247;263;261;245;243;242;236;254;235;256;260;259;258;234;337;231;233;202;284;253;252;226;230;192;191;157;Swirl UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;191;-78.13221,2919.698;Float;False;Constant;_Anchor;Anchor;16;0;Create;0.5,0.5;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;157;-79.66639,2771.108;Float;False;0;85;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;192;185.3686,2859.602;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LengthOpNode;226;375.5787,2857.598;Float;False;1;0;FLOAT2;0.0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;230;-1297.372,2672.538;Float;False;Property;_Radius;Radius;18;0;Create;0.3764706;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;284;-1025.817,2677.306;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;0.35;False;4;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;253;-1224.101,2886.142;Float;False;252;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;252;626.3124,2853.844;Float;True;SwirlLength;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;233;-719.4297,2684.598;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;202;-2375.132,2665.997;Float;False;Property;_SwirlSpeed;Swirl Speed;16;0;Create;1;0.43;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;337;-2046.45,2609.617;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;-1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;234;-570.2513,2849.558;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;-2374.131,2928.593;Float;False;Property;_Angle;Angle;17;0;Create;72;26.9;0;90;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-395.8813,2846.423;Float;True;SwirlPercent;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;260;-2402.177,3154.534;Float;True;259;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;258;-1676.292,2663.678;Float;False;2;2;0;FLOAT;1.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;235;-2172.158,3159.287;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;2.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;254;-1527.9,2660.466;Float;True;SwirlAngle;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;79;-2439.966,1482.983;Float;False;2385.626;843.2482;Comment;16;98;95;93;94;91;90;89;87;88;86;85;84;82;83;81;80;Displace UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;256;-2398.737,3352.774;Float;True;254;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-2380.229,1560.282;Float;False;0;85;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;236;-2002.67,3281.02;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;80;-2371.547,1743.971;Float;False;Property;_DisplaceSpeedXY;Displace Speed XY;8;0;Create;0,0;0.02,0.02;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;83;-2133.916,1725.113;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;242;-1787.376,3165.099;Float;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-2392.087,1905.619;Float;False;Property;_DisplaceScale;Displace Scale;7;0;Create;0;3;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;243;-1783.342,3409.899;Float;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;245;-1547.457,3082.41;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;-1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;84;-1918.104,1725.257;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;263;-1255.723,3309.575;Float;False;261;0;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;247;-1323.927,3489.472;Float;True;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;240;-1315.886,3056.167;Float;True;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;85;-1664.952,1696.277;Float;True;Property;_DisplaceTexture;Displace Texture;6;0;Create;None;d784595d7b8bfef41ac0a5bd8fa0a662;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;261;373.2307,2719.486;Float;False;SwirlCenter;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1352.439,1858.797;Float;False;Constant;_Float1;Float 1;4;0;Create;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;248;-997.0173,3504.929;Float;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;244;-1003.399,3099.996;Float;True;2;0;FLOAT2;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-1151.606,1965.043;Float;False;Constant;_Float3;Float 3;4;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-1171.466,1701.965;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;265;-694.4902,3535.688;Float;False;264;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;264;202.6849,3013.232;Float;False;SwirlPivot;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;250;-730.1219,3292.735;Float;True;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;89;-1006.826,1855.318;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;334;-542.4213,3737.162;Float;False;Property;_SwirlRotationSpeed;Swirl Rotation Speed;19;0;Create;0;1.45;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;336;-476.2109,3578.845;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;335;-253.2109,3650.845;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-927.3972,2228.345;Float;False;Property;_Displace;Displace;9;0;Create;0;0.15;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;90;-857.2131,2079.883;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;251;-385.5292,3328.222;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;333;-3.622242,3329.314;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;2.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-631.0292,2155.279;Float;False;2;2;0;FLOAT3;0,0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-716.0112,1925.607;Float;False;0;11;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;357;0.1584206,3603.721;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0.1;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-450.5053,2134.053;Float;False;2;2;0;FLOAT2;0.0,0,0;False;1;FLOAT3;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;358;312.1599,3546.522;Float;False;Property;_Rotation;Rotation;20;0;Create;1;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;270;1033.728,2732.76;Float;True;171;0;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;76;-2433.437,690.5338;Float;False;1673.671;597.9595;Comment;11;12;10;6;7;5;4;122;121;124;1;266;Distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;271;1035.137,2939.699;Float;True;98;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-281.7587,2128.946;Float;False;DisplaceUV;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;171;564.5297,3547.514;Float;False;SwirlUV;-1;True;1;0;FLOAT2;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;124;-2399.683,966.765;Float;False;Property;_DistortionSpeed;Distortion Speed;4;0;Create;0,0;0.2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;274;1352.104,2827.762;Float;True;3;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0,0;False;2;FLOAT;0.3;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-2397.611,811.4049;Float;False;0;4;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;121;-2056.949,908.8256;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;352;-4154.894,266.4591;Float;False;1559.273;502.6083;Comment;6;355;351;353;350;349;356;Depth Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;266;-2073.532,1109.882;Float;False;290;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;290;1619.819,2822.95;Float;False;SwirlAndDisplaceUV;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;356;-4121.672,520.419;Float;False;Property;_DepthDistance;Depth Distance;13;0;Create;0;0.05;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;122;-1831.214,903.0073;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;4;-1589.792,878.8887;Float;True;Property;_DistortionTexture;Distortion Texture;3;0;Create;None;d784595d7b8bfef41ac0a5bd8fa0a662;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-1571.188,775.3294;Float;False;Property;_Distortion;Distortion;5;0;Create;0;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;77;-1834.366,81.58202;Float;False;1053.893;514.6957;Comment;4;268;13;18;11;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;72;-3862.254,895.3718;Float;False;1274.095;513.4723;Comment;4;61;60;59;291;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.DepthFade;349;-3815.582,522.6196;Float;False;True;1;0;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;291;-3833.861,1005.434;Float;False;290;0;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;350;-3610.276,522.1484;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1280.91,830.9913;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;268;-1708.41,385.1587;Float;False;290;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GrabScreenPosition;7;-1532.458,1107.239;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;59;-3479.576,981.4251;Float;True;Property;_EmissionTexture;Emission Texture;11;0;Create;None;a0ae2e76d7a8a214e9da2809a62c2b64;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;351;-3416.391,520.9874;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;353;-3828.569,333.215;Float;False;Property;_DepthEmission;Depth Emission;12;1;[HDR];Create;0,0,0,0;0,4.172413,5,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;60;-3477.252,1195.118;Float;False;Property;_EmissionColor;Emission Color;10;1;[HDR];Create;0,0,0,0;3.2714,1.801471,7,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;78;-1854.631,-545.2136;Float;False;1403.744;589.037;Comment;4;338;69;24;14;Cutout;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;18;-1720.861,175.84;Float;False;Property;_MainColor;Main Color;0;1;[HDR];Create;1,1,1,1;0.6031726,0.5530926,1.213235,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-1067.072,830.4797;Float;False;2;2;0;FLOAT3;0.0,0,0,0;False;1;FLOAT4;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;292;-3677.047,1484.798;Float;False;1093.525;402.7771;Comment;4;134;133;137;136;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;11;-1427.032,361.6787;Float;True;Property;_MainTexture;Main Texture;1;0;Create;None;61c0b9c0523734e0e91bc6043c72a490;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;355;-3214.535,498.6913;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;12;-950.5045,820.9429;Float;False;Global;_GrabScreen0;Grab Screen 0;0;0;Create;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;342;-2055.233,2498.332;Float;False;tmpspeed;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;-3587.423,1701.751;Float;False;Property;_NormalScale;Normal Scale;15;0;Create;0;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-3580.423,1587.712;Float;False;290;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-3136.77,1120.687;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;338;-1510.132,-254.7318;Float;False;342;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-1801.657,-461.9332;Float;True;Property;_CutOutA;CutOut (A);2;0;Create;None;fc00ec05a89da4ff695a4273715cd5ce;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1070.354,284.3833;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;354;-2893.934,786.9523;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;133;-3238.584,1637.956;Float;True;Property;_NormalTexture;Normal Texture;14;0;Create;None;51b8778271de7b34998dd099619f8b35;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-1261.594,-254.5557;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-595.8491,593.2082;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;699.6691,1413.271;Float;False;74;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;62;-2729.043,781.875;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;699.7511,1551.247;Float;False;62;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;699.1342,1669.635;Float;False;69;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;135;495.4361,1466.697;Float;False;134;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SinOpNode;228;-1892.328,2707.45;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;330;-1785.127,2926.736;Float;False;TempSwirlSpeed;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-2836.605,1637.392;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-967.4624,-249.387;Float;True;Opacity;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-372.1494,587.5591;Float;True;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;21;969.2133,1467.586;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;QFX/SFX/Portal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Off;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;2;255;255;7;3;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;192;0;157;0
WireConnection;192;1;191;0
WireConnection;226;0;192;0
WireConnection;284;0;230;0
WireConnection;252;0;226;0
WireConnection;233;0;284;0
WireConnection;233;1;253;0
WireConnection;337;0;202;0
WireConnection;234;0;233;0
WireConnection;234;1;284;0
WireConnection;259;0;234;0
WireConnection;258;0;337;0
WireConnection;258;1;231;0
WireConnection;235;0;260;0
WireConnection;254;0;258;0
WireConnection;236;0;235;0
WireConnection;236;1;256;0
WireConnection;83;0;81;0
WireConnection;83;2;80;0
WireConnection;242;0;236;0
WireConnection;243;0;236;0
WireConnection;245;0;242;0
WireConnection;84;0;83;0
WireConnection;84;1;82;0
WireConnection;247;0;242;0
WireConnection;247;1;243;0
WireConnection;240;0;243;0
WireConnection;240;1;245;0
WireConnection;85;1;84;0
WireConnection;261;0;192;0
WireConnection;248;0;263;0
WireConnection;248;1;247;0
WireConnection;244;0;263;0
WireConnection;244;1;240;0
WireConnection;88;0;85;0
WireConnection;88;1;86;0
WireConnection;264;0;191;0
WireConnection;250;0;244;0
WireConnection;250;1;248;0
WireConnection;89;0;88;0
WireConnection;89;1;87;0
WireConnection;335;0;336;4
WireConnection;335;1;334;0
WireConnection;90;0;89;0
WireConnection;251;0;250;0
WireConnection;251;1;265;0
WireConnection;333;0;251;0
WireConnection;333;2;335;0
WireConnection;93;0;90;0
WireConnection;93;1;91;0
WireConnection;357;0;251;0
WireConnection;357;1;335;0
WireConnection;95;0;94;0
WireConnection;95;1;93;0
WireConnection;358;0;357;0
WireConnection;358;1;333;0
WireConnection;98;0;95;0
WireConnection;171;0;358;0
WireConnection;274;0;270;0
WireConnection;274;1;271;0
WireConnection;121;0;1;0
WireConnection;121;2;124;0
WireConnection;290;0;274;0
WireConnection;122;0;121;0
WireConnection;122;2;266;0
WireConnection;4;1;122;0
WireConnection;349;0;356;0
WireConnection;350;0;349;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;59;1;291;0
WireConnection;351;0;350;0
WireConnection;10;0;6;0
WireConnection;10;1;7;0
WireConnection;11;1;268;0
WireConnection;355;0;353;0
WireConnection;355;1;351;0
WireConnection;12;0;10;0
WireConnection;342;0;202;0
WireConnection;61;0;59;0
WireConnection;61;1;60;0
WireConnection;13;0;18;0
WireConnection;13;1;11;0
WireConnection;354;0;355;0
WireConnection;354;1;61;0
WireConnection;133;1;137;0
WireConnection;133;5;136;0
WireConnection;24;0;14;4
WireConnection;24;1;18;4
WireConnection;24;2;338;0
WireConnection;15;0;13;0
WireConnection;15;1;12;0
WireConnection;62;0;354;0
WireConnection;228;0;202;0
WireConnection;330;0;228;0
WireConnection;134;0;133;0
WireConnection;69;0;24;0
WireConnection;74;0;15;0
WireConnection;21;0;75;0
WireConnection;21;1;135;0
WireConnection;21;2;67;0
WireConnection;21;9;70;0
ASEEND*/
//CHKSM=28DE6A584CC1806C013FC5F4014ED08230ED572B