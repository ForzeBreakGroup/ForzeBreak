// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "QFX/SFX/Displace"
{
	Properties
	{
		[HDR]_MainColor("Main Color", Color) = (1,1,1,1)
		_MainTexture("Main Texture", 2D) = "white" {}
		_DistortionTexture("Distortion Texture", 2D) = "bump" {}
		_Distortion("Distortion", Range( 0 , 1)) = 0
		_DisplaceTexture("Displace Texture", 2D) = "white" {}
		_CutOutA("CutOut (A)", 2D) = "white" {}
		_DisplaceScale("Displace Scale", Range( 0 , 4)) = 0
		_DisplaceSpeedXY("Displace Speed XY", Vector) = (0,0,0,0)
		_Displace("Displace", Range( 0 , 2)) = 0
		_EmissionTexture("Emission Texture", 2D) = "white" {}
		[HDR]_EmissionColor("Emission Color", Color) = (0,0,0,0)
		_MainSpeed("Main Speed", Vector) = (0,0,0,0)
		_EmissionSpeed("Emission Speed", Vector) = (0,0,0,0)
		_DistortionSpeed("Distortion Speed", Vector) = (0,0,0,0)
		_NormalTexture("Normal Texture", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 5)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Stencil
		{
			Ref 2
			Comp Always
			Pass Replace
		}
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _NormalScale;
		uniform sampler2D _NormalTexture;
		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform sampler2D _DisplaceTexture;
		uniform float2 _DisplaceSpeedXY;
		uniform float4 _DisplaceTexture_ST;
		uniform float _DisplaceScale;
		uniform float _Displace;
		uniform float4 _MainColor;
		uniform float2 _MainSpeed;
		uniform sampler2D _GrabTexture;
		uniform float _Distortion;
		uniform sampler2D _DistortionTexture;
		uniform float2 _DistortionSpeed;
		uniform float4 _DistortionTexture_ST;
		uniform sampler2D _EmissionTexture;
		uniform float2 _EmissionSpeed;
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
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float2 uv_DisplaceTexture = i.uv_texcoord * _DisplaceTexture_ST.xy + _DisplaceTexture_ST.zw;
			float2 panner83 = ( uv_DisplaceTexture + 1.0 * _Time.y * _DisplaceSpeedXY);
			float4 temp_cast_1 = (1.0).xxxx;
			float3 DisplaceUV98 = ( float3( uv_MainTexture ,  0.0 ) + ( (( ( tex2D( _DisplaceTexture, (panner83*_DisplaceScale + 0.0) ) * 2.0 ) - temp_cast_1 )).rgb * _Displace ) );
			float3 Normal134 = UnpackScaleNormal( tex2D( _NormalTexture, DisplaceUV98.xy ) ,_NormalScale );
			o.Normal = Normal134;
			float2 panner126 = ( DisplaceUV98.xy + 1.0 * _Time.y * _MainSpeed);
			float2 uv_DistortionTexture = i.uv_texcoord * _DistortionTexture_ST.xy + _DistortionTexture_ST.zw;
			float2 panner121 = ( uv_DistortionTexture + 1.0 * _Time.y * _DistortionSpeed);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor12 = tex2D( _GrabTexture, ( float4( ( _Distortion * UnpackNormal( tex2D( _DistortionTexture, (panner121*1.0 + 0.0) ) ) ) , 0.0 ) + ase_grabScreenPosNorm ).xy );
			float4 Albedo74 = ( ( _MainColor * tex2D( _MainTexture, (panner126*1.0 + 0.0) ) ) + screenColor12 );
			o.Albedo = Albedo74.rgb;
			float2 panner130 = ( DisplaceUV98.xy + 1.0 * _Time.y * _EmissionSpeed);
			float4 Emission62 = ( tex2D( _EmissionTexture, (panner130*1.0 + 0.0) ) * _EmissionColor );
			o.Emission = Emission62.rgb;
			float2 uv_CutOutA = i.uv_texcoord * _CutOutA_ST.xy + _CutOutA_ST.zw;
			float Opacity69 = ( tex2D( _CutOutA, uv_CutOutA ).a * _MainColor.a );
			o.Alpha = Opacity69;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
46;706;1865;321;5267.385;1608.195;7.395895;True;False
Node;AmplifyShaderEditor.CommentaryNode;79;-3089.332,1351.315;Float;False;2385.626;843.2482;Comment;16;98;95;93;94;91;90;89;87;88;86;85;84;82;83;81;80;Displace;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-3019.861,1438.348;Float;False;0;85;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;80;-3011.179,1622.037;Float;False;Property;_DisplaceSpeedXY;Displace Speed XY;8;0;Create;0,0;0,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;83;-2773.548,1603.179;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-3031.719,1783.685;Float;False;Property;_DisplaceScale;Displace Scale;7;0;Create;0;0.31;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;84;-2557.736,1603.323;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1992.071,1736.862;Float;False;Constant;_Float1;Float 1;4;0;Create;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;85;-2304.584,1574.343;Float;True;Property;_DisplaceTexture;Displace Texture;4;0;Create;None;cd460ee4ac5c1e746b7a734cc7cc64dd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;87;-1791.238,1843.109;Float;False;Constant;_Float3;Float 3;4;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-1811.098,1580.031;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;89;-1646.458,1733.383;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;90;-1496.845,1957.949;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-1567.029,2106.411;Float;False;Property;_Displace;Displace;11;0;Create;0;0.3;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;76;-2129.478,61.69361;Float;False;1673.671;597.9595;Comment;10;12;10;6;7;5;4;122;121;124;1;Distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-1270.661,2033.345;Float;False;2;2;0;FLOAT3;0,0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;94;-1355.643,1803.673;Float;False;0;11;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;124;-2067.124,453.6247;Float;False;Property;_DistortionSpeed;Distortion Speed;18;0;Create;0,0;0,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-2093.652,182.5648;Float;False;0;4;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-1090.137,2012.119;Float;False;2;2;0;FLOAT2;0.0,0,0;False;1;FLOAT3;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;77;-2135.401,-627.084;Float;False;1399.259;628.4575;Comment;8;13;18;11;110;125;126;128;127;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;121;-1752.992,279.9853;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;122;-1527.258,274.1672;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-921.3902,2007.012;Float;False;DisplaceUV;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-2029.516,-488.6697;Float;False;98;0;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;127;-1978.895,-186.42;Float;False;Property;_MainSpeed;Main Speed;16;0;Create;0,0;0,0.05;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;72;-4070.926,-93.35752;Float;False;1885.065;611.5266;Comment;9;62;61;59;60;99;129;130;131;132;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;126;-1714.259,-274.5673;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;-3796.859,-3.041283;Float;False;98;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1267.23,146.4892;Float;False;Property;_Distortion;Distortion;3;0;Create;0;0.019;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;131;-3772.493,319.2474;Float;False;Property;_EmissionSpeed;Emission Speed;17;0;Create;0,0;0,-0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;4;-1285.836,250.0484;Float;True;Property;_DistortionTexture;Distortion Texture;2;0;Create;None;10ff51d2d87fb7b46b70b55f8551c146;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;130;-3507.857,231.1001;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-980.9523,197.1513;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;125;-1515.522,-275.8859;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GrabScreenPosition;7;-1228.5,478.3993;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-787.114,198.6396;Float;False;2;2;0;FLOAT3;0.0,0,0,0;False;1;FLOAT4;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;78;-1717.01,-1110.014;Float;False;822.9071;369.2126;Comment;3;69;24;14;Cutout;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;18;-1676.529,-532.8258;Float;False;Property;_MainColor;Main Color;0;1;[HDR];Create;1,1,1,1;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-1297.905,-302.6712;Float;True;Property;_MainTexture;Main Texture;1;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;129;-3309.12,229.7815;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-900.5164,-406.3532;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;136;-94.94453,-344.9467;Float;False;Property;_NormalScale;Normal Scale;20;0;Create;0;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-94.9445,-495.9862;Float;False;98;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;59;-2939.566,-32.17612;Float;True;Property;_EmissionTexture;Emission Texture;14;0;Create;None;5798ded558355430c8a9b13ee12a847c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;60;-2937.242,181.5165;Float;False;Property;_EmissionColor;Emission Color;15;1;[HDR];Create;0,0,0,0;1.7104,1.654985,3.262,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;12;-646.5469,192.1028;Float;False;Global;_GrabScreen0;Grab Screen 0;0;0;Create;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;14;-1678.121,-1055.329;Float;True;Property;_CutOutA;CutOut (A);5;0;Create;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;133;209.8949,-432.7422;Float;True;Property;_NormalTexture;Normal Texture;19;0;Create;None;302951faffe230848aa0d3df7bb70faa;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;71;-3829.22,-838.7972;Float;False;1090.157;579.761;Comment;8;63;28;58;26;27;57;25;108;Rotation;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-2595.018,129.7444;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;73;-3105.707,711.7968;Float;False;1640.123;523.3501;Comment;11;53;47;68;49;54;52;55;51;50;56;48;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-372.4054,15.83344;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-1309.841,-858.675;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;301.6558,-20.9147;Float;False;74;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;108;-3216.808,-478.5905;Float;False;RotationSpeed;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;55;-2374.393,793.9199;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;26;-3569.495,-781.53;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-2852.004,793.5119;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;49;-3060.51,810.7309;Float;False;Property;_NoiseSize;Noise Size;10;0;Create;50,50;700,700;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;70;303.5073,235.2895;Float;False;69;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;128;-2054.92,-365.9884;Float;False;0;4;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;135;97.42255,32.51129;Float;False;134;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;575.8947,-431.7422;Float;False;Normal;-1;True;1;0;FLOAT3;0.0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;27;-3569.495,-653.5295;Float;False;Constant;_RotationAnchor;Rotation Anchor;0;0;Create;0.5,0.5;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-2854.179,933.4465;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-2125.012,787.9089;Float;False;Constant;_Float2;Float 2;28;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;62;-2415.424,125.796;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-3793.883,-378.2744;Float;False;Property;_RotationSpeed;Rotation Speed;6;0;Create;0;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-3059.803,974.0875;Float;False;Property;_NoiseSpeed;Noise Speed;13;0;Create;0;50;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-1694.64,881.8322;Float;False;Noise;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-1128.303,-860.655;Float;False;Opacity;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2564.486,885.1059;Float;False;Property;_NoiseScale;Noise Scale;12;0;Create;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;28;-3249.077,-698.1318;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;53;-2603.931,789.7339;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;56;-1948.745,879.9745;Float;False;Property;_EnableNoise;Enable Noise;9;0;Create;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;25;-3800.166,-531.6108;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-2957.643,-702.7955;Float;False;Rotation;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-3471.33,-474.8131;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;132;-3848.518,139.679;Float;False;0;4;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;67;301.7381,117.0611;Float;False;62;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-202.3442,12.0853;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TimeNode;47;-3066.28,1050.671;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;21;571.2,33.4;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;QFX/SFX/Displace;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;2;255;255;7;3;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;83;0;81;0
WireConnection;83;2;80;0
WireConnection;84;0;83;0
WireConnection;84;1;82;0
WireConnection;85;1;84;0
WireConnection;88;0;85;0
WireConnection;88;1;86;0
WireConnection;89;0;88;0
WireConnection;89;1;87;0
WireConnection;90;0;89;0
WireConnection;93;0;90;0
WireConnection;93;1;91;0
WireConnection;95;0;94;0
WireConnection;95;1;93;0
WireConnection;121;0;1;0
WireConnection;121;2;124;0
WireConnection;122;0;121;0
WireConnection;98;0;95;0
WireConnection;126;0;110;0
WireConnection;126;2;127;0
WireConnection;4;1;122;0
WireConnection;130;0;99;0
WireConnection;130;2;131;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;125;0;126;0
WireConnection;10;0;6;0
WireConnection;10;1;7;0
WireConnection;11;1;125;0
WireConnection;129;0;130;0
WireConnection;13;0;18;0
WireConnection;13;1;11;0
WireConnection;59;1;129;0
WireConnection;12;0;10;0
WireConnection;133;1;137;0
WireConnection;133;5;136;0
WireConnection;61;0;59;0
WireConnection;61;1;60;0
WireConnection;15;0;13;0
WireConnection;15;1;12;0
WireConnection;24;0;14;4
WireConnection;24;1;18;4
WireConnection;108;0;58;0
WireConnection;55;0;53;0
WireConnection;55;1;52;0
WireConnection;55;2;52;0
WireConnection;51;0;49;0
WireConnection;51;1;50;0
WireConnection;134;0;133;0
WireConnection;50;0;48;0
WireConnection;50;1;47;2
WireConnection;62;0;61;0
WireConnection;68;0;56;0
WireConnection;69;0;24;0
WireConnection;28;0;26;0
WireConnection;28;1;27;0
WireConnection;28;2;58;0
WireConnection;53;0;51;0
WireConnection;56;0;54;0
WireConnection;56;1;55;0
WireConnection;63;0;28;0
WireConnection;58;0;25;2
WireConnection;58;1;57;0
WireConnection;74;0;15;0
WireConnection;21;0;75;0
WireConnection;21;1;135;0
WireConnection;21;2;67;0
WireConnection;21;9;70;0
ASEEND*/
//CHKSM=E30C38919D5F5EF02FA5CA21C252793AB6EF6C71