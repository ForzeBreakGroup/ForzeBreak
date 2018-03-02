// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "QFX/SFX/Shield"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HDR]_MainColor("Main Color", Color) = (0,0,0,0)
		_MainTexture("Main Texture", 2D) = "white" {}
		[IntRange]_PatternSize("Pattern Size", Range( 1 , 20)) = 5
		_NormalTexture("Normal Texture", 2D) = "bump" {}
		_DisplaceTexture("Displace Texture", 2D) = "white" {}
		_DisplacePatternSize("Displace Pattern Size", Range( 0 , 4)) = 0
		_DisplaceSpeedXY("Displace Speed XY", Vector) = (0,0,0,0)
		_Displace("Displace", Range( 0 , 2)) = 0
		_EmissionTexture("Emission Texture", 2D) = "white" {}
		_DistortionTexture("Distortion Texture", 2D) = "bump" {}
		_DistortionPatternSize("Distortion Pattern Size", Range( 0 , 4)) = 0
		_DistortionSpeedXY("Distortion Speed XY", Vector) = (0,0,0,0)
		_Distortion("Distortion", Range( 0 , 1)) = 0
		[HDR]_IntersectionColor("IntersectionColor", Color) = (0.03137255,0.2588235,0.3176471,1)
		_Intersection("Intersection", Range( 0 , 1)) = 0.2
		[HDR]_RimColor("Rim Color", Color) = (0,0,0,0)
		_Rim("Rim", Range( 0 , 10)) = 7
		[Toggle]_EnableNoise("Enable Noise", Float) = 1
		_NoiseSize("Noise Size", Vector) = (50,50,0,0)
		_NoiseScale("Noise Scale", Float) = 1
		_NoiseSpeed("Noise Speed", Float) = 0
		_DissolveTex("Dissolve Tex", 2D) = "white" {}
		_DissolvePatternSize("Dissolve Pattern Size", Range( 0 , 20)) = 1
		_DissolveSpeed("Dissolve Speed", Vector) = (0.19,0,0,0)
		_StartPoint("Start Point", Range( -2 , 2)) = 0
		_DissolveDistance("Dissolve Distance", Range( 0 , 1)) = 0
		[HDR]_DissolveGlowColor("Dissolve Glow Color", Color) = (0,0,0,0)
		_DissolveGlow("Dissolve Glow", Range( 0 , 1)) = 0.1
		[Toggle]_DissolveDirection("Dissolve Direction", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalTexture;
		uniform float _PatternSize;
		uniform sampler2D _DisplaceTexture;
		uniform float2 _DisplaceSpeedXY;
		uniform float _DisplacePatternSize;
		uniform float _Displace;
		uniform float4 _MainColor;
		uniform sampler2D _MainTexture;
		uniform float _EnableNoise;
		uniform float2 _NoiseSize;
		uniform float _NoiseSpeed;
		uniform float _NoiseScale;
		uniform sampler2D _GrabTexture;
		uniform float _Distortion;
		uniform sampler2D _DistortionTexture;
		uniform float2 _DistortionSpeedXY;
		uniform float _DistortionPatternSize;
		uniform float _DissolveDirection;
		uniform float _StartPoint;
		uniform float _DissolveDistance;
		uniform sampler2D _DissolveTex;
		uniform float2 _DissolveSpeed;
		uniform float _DissolvePatternSize;
		uniform float _DissolveGlow;
		uniform float4 _DissolveGlowColor;
		uniform sampler2D _EmissionTexture;
		uniform float4 _IntersectionColor;
		uniform float _Rim;
		uniform float4 _RimColor;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Intersection;
		uniform float _Cutoff = 0.5;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


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
			float2 appendResult67 = (float2(_PatternSize , _PatternSize));
			float2 uv_TexCoord57 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 uv_TexCoord45 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner46 = ( uv_TexCoord45 + 1.0 * _Time.y * _DisplaceSpeedXY);
			float4 temp_cast_1 = (1.0).xxxx;
			float2 uv_TexCoord69 = i.uv_texcoord * appendResult67 + ( float3( uv_TexCoord57 ,  0.0 ) + ( (( ( tex2D( _DisplaceTexture, (panner46*_DisplacePatternSize + 0.0) ) * 2.0 ) - temp_cast_1 )).rgb * _Displace ) ).xy;
			float2 DisplaceUV70 = uv_TexCoord69;
			float3 Normal78 = UnpackNormal( tex2D( _NormalTexture, DisplaceUV70 ) );
			o.Normal = Normal78;
			float4 tex2DNode12 = tex2D( _MainTexture, DisplaceUV70 );
			float2 temp_cast_3 = (( _NoiseSpeed * _Time.y )).xx;
			float2 uv_TexCoord144 = i.uv_texcoord * _NoiseSize + temp_cast_3;
			float simplePerlin2D142 = snoise( uv_TexCoord144 );
			float2 uv_TexCoord2 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner19 = ( (uv_TexCoord2*_DistortionPatternSize + 0.0) + _Time.y * _DistortionSpeedXY);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor13 = tex2D( _GrabTexture, ( float4( ( _Distortion * UnpackNormal( tex2D( _DistortionTexture, panner19 ) ) ) , 0.0 ) + ase_grabScreenPosNorm ).xy );
			float4 Albedo75 = ( ( ( _MainColor * tex2DNode12 ) * lerp(1.0,(simplePerlin2D142*_NoiseScale + _NoiseScale),_EnableNoise) ) + screenColor13 );
			o.Albedo = Albedo75.rgb;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float2 panner163 = ( float2( 0,0 ) + _Time.y * _DissolveSpeed);
			float2 uv_TexCoord165 = i.uv_texcoord * float2( 1,1 ) + panner163;
			float Dissolve180 = ( ( lerp(( 1.0 + ( ase_vertex3Pos.y - _StartPoint ) ),( _StartPoint - ase_vertex3Pos.y ),_DissolveDirection) - _DissolveDistance ) + ( _DissolveDistance * tex2D( _DissolveTex, (uv_TexCoord165*_DissolvePatternSize + float2( 0,0 )) ).r ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNDotV63 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode63 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNDotV63, (10.0 + (_Rim - 0.0) * (0.0 - 10.0) / (10.0 - 0.0)) ) );
			float4 ShieldPattern86 = tex2DNode12;
			float4 Rim90 = ( fresnelNode63 * _RimColor * ShieldPattern86 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth25 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth25 = abs( ( screenDepth25 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Intersection ) );
			float clampResult28 = clamp( distanceDepth25 , 0.0 , 1.0 );
			float4 lerpResult30 = lerp( _IntersectionColor , Rim90 , clampResult28);
			float4 RimIntersectionEmission73 = ( tex2D( _EmissionTexture, DisplaceUV70 ) * lerpResult30 );
			float4 Emission187 = (( Dissolve180 < _DissolveGlow ) ? _DissolveGlowColor :  RimIntersectionEmission73 );
			o.Emission = Emission187.rgb;
			o.Alpha = 1;
			clip( Dissolve180 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
7;646;1904;387;14243.54;1972.294;15.15781;True;False
Node;AmplifyShaderEditor.CommentaryNode;83;-6040.501,-1319.306;Float;False;3122.321;904.4279;Comment;19;45;70;69;67;58;57;68;56;54;55;53;52;51;50;49;48;46;47;44;Displace;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-5971.03,-1232.274;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;44;-5948.348,-1050.585;Float;False;Property;_DisplaceSpeedXY;Displace Speed XY;7;0;Create;0,0;-0.03,0.06;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;47;-5957.888,-894.9364;Float;False;Property;_DisplacePatternSize;Displace Pattern Size;6;0;Create;0;0.88;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;46;-5724.717,-1067.443;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;48;-5508.905,-1067.298;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-4954.24,-906.7594;Float;False;Constant;_Float1;Float 1;4;0;Create;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;50;-5255.753,-1096.278;Float;True;Property;_DisplaceTexture;Displace Texture;5;0;Create;None;e80c3c84ea861404d8a427db8b7abf04;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;-4742.406,-827.5123;Float;False;Constant;_Float2;Float 2;4;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-4765.266,-1087.59;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;53;-4597.627,-937.2384;Float;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;54;-4448.014,-712.6714;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-4565.197,-577.2104;Float;False;Property;_Displace;Displace;8;0;Create;0;0.23;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;57;-4306.811,-866.9484;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-4221.829,-637.2755;Float;False;2;2;0;FLOAT3;0,0,0,0;False;1;FLOAT;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-3957.474,-822.9554;Float;False;Property;_PatternSize;Pattern Size;3;1;[IntRange];Create;5;4;1;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;159;-5157.39,2035.071;Float;False;1292.34;414.7519;;7;168;166;164;165;163;162;161;Dissolve;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-4041.307,-658.5025;Float;False;2;2;0;FLOAT2;0.0,0,0;False;1;FLOAT3;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;67;-3669.473,-829.9554;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;160;-3810.9,2031.493;Float;False;1376.854;798.9325;;10;175;177;178;179;180;195;199;198;200;201;Length from start point to dissolve and glow;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;172;-3768.014,2237.903;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;161;-5112.911,2152.492;Float;False;Property;_DissolveSpeed;Dissolve Speed;24;0;Create;0.19,0;0.19,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;171;-3779.987,2151.971;Float;False;Property;_StartPoint;Start Point;25;0;Create;0;0.98;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;89;-4757.918,941.3004;Float;False;1329.627;604.9286;Comment;7;87;71;88;63;62;60;90;Rim;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;69;-3472.385,-702.9555;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;85;-2608.396,-663.6056;Float;False;1729.777;584.2936;Comment;9;75;16;108;158;14;42;86;12;80;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.TimeNode;162;-5111.636,2304.886;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;84;-3238.403,61.63232;Float;False;1722.214;662.1016;Comment;12;13;11;7;8;6;5;19;21;20;4;2;3;Distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;154;-2587.482,-1310.963;Float;False;1362.714;491.8158;Comment;10;155;156;145;146;142;144;143;152;153;151;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-4711.71,1073.607;Float;False;Property;_Rim;Rim;17;0;Create;7;8.54;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;196;-3450.605,2083.857;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;163;-4889.523,2211.083;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-2550.148,-369.1746;Float;False;70;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-3194.186,-708.6984;Float;False;DisplaceUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;199;-3279.657,2070.708;Float;False;Constant;_Float3;Float 3;36;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;153;-2560.293,-1074.852;Float;False;Property;_NoiseSpeed;Noise Speed;21;0;Create;0;150;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;198;-3134.527,2122.43;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-3123.072,203.9741;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;62;-4412.004,1078.863;Float;False;5;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;10.0;False;3;FLOAT;10.0;False;4;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;151;-2566.77,-998.2684;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;173;-3444.034,2196.293;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-3174.883,337.4776;Float;False;Property;_DistortionPatternSize;Distortion Pattern Size;11;0;Create;0;1;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;12;-2355.629,-392.3217;Float;True;Property;_MainTexture;Main Texture;2;0;Create;None;9fbef4b79ca3b784ba023cb1331520d5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;164;-4703.198,2301.303;Float;False;Property;_DissolvePatternSize;Dissolve Pattern Size;23;0;Create;1;1;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;165;-4692.071,2167.781;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;92;-3238.209,918.743;Float;False;1511.384;778.6304;Emission;10;28;25;34;73;141;138;30;29;139;91;Rim, Intersection, Collision;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;88;-4130.199,1162.739;Float;False;Property;_RimColor;Rim Color;16;1;[HDR];Create;0,0,0,0;3.987,0.498375,0.498375,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;71;-4129.995,1346.227;Float;True;86;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-3179.964,1565.989;Float;False;Property;_Intersection;Intersection;15;0;Create;0.2;0.072;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;63;-4157.408,1011.431;Float;False;World;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;4;-2845.93,288.4775;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;143;-2561,-1238.208;Float;False;Property;_NoiseSize;Noise Size;19;0;Create;50,50;600,600;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;21;-2887.536,421.6207;Float;False;Property;_DistortionSpeedXY;Distortion Speed XY;12;0;Create;0,0;0.28,0.15;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;86;-1981.002,-220.191;Float;False;ShieldPattern;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TimeNode;20;-2889.603,558.4377;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;195;-2996.555,2179.964;Float;False;Property;_DissolveDirection;Dissolve Direction;29;0;Create;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;166;-4401.699,2177.703;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT;1.0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;-2354.668,-1115.492;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;25;-2889.822,1569.882;Float;False;True;1;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-3857.26,1009.586;Float;False;3;3;0;FLOAT;0.0;False;1;COLOR;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;144;-2352.494,-1255.427;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;201;-2766.924,2288.348;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;19;-2590.249,288.652;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;168;-4185.748,2098.72;Float;True;Property;_DissolveTex;Dissolve Tex;22;0;Create;None;cd460ee4ac5c1e746b7a734cc7cc64dd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;200;-3234.041,2386.219;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;139;-3187.815,1000.888;Float;False;70;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-3633.566,1003.182;Float;False;Rim;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-2064.974,-1163.833;Float;False;Property;_NoiseScale;Noise Scale;20;0;Create;1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;175;-3774.249,2461.872;Float;False;Property;_DissolveDistance;Dissolve Distance;26;0;Create;0;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;28;-2663.669,1570.31;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-3203.476,1382.265;Float;False;90;0;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-2386.181,260.3476;Float;True;Property;_DistortionTexture;Distortion Texture;10;0;Create;None;f53512d44b91e954dae7bf028209df1a;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-2367.575,156.7883;Float;False;Property;_Distortion;Distortion;13;0;Create;0;0.013;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;142;-2104.42,-1259.205;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;194;-3889.488,2485.304;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;-3202.409,1205.926;Float;False;Property;_IntersectionColor;IntersectionColor;14;1;[HDR];Create;0.03137255,0.2588235,0.3176471,1;2.102,0.5873237,0.5873237,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;138;-2953.536,978.0709;Float;True;Property;_EmissionTexture;Emission Texture;9;0;Create;None;9fbef4b79ca3b784ba023cb1331520d5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GrabScreenPosition;8;-2328.845,488.698;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;156;-1625.499,-1261.03;Float;False;Constant;_Float0;Float 0;28;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;30;-2514.719,1358.43;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2075.92,207.4505;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;-3404.18,2569.411;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;145;-1874.881,-1255.019;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;1.0;False;2;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;42;-2307.901,-583.1996;Float;False;Property;_MainColor;Main Color;1;1;[HDR];Create;0,0,0,0;0,0.3931031,3,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;177;-3159.719,2442.522;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;82;-4622.708,372.7072;Float;False;775.6544;286.8406;Comment;3;78;77;81;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;192;-1594.357,1224.346;Float;False;1065.606;476.2201;;6;187;186;193;185;184;190;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1988.183,-440.6245;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;155;-1458.818,-1149.793;Float;False;Property;_EnableNoise;Enable Noise;18;0;Create;1;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-2202.584,1336.947;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;179;-2916.025,2544.424;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-1916.126,208.3016;Float;False;2;2;0;FLOAT3;0.0,0,0,0;False;1;FLOAT4;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-4575.626,462.0993;Float;False;70;0;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;190;-1290.07,1623.996;Float;False;73;0;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;184;-1562.33,1518.757;Float;False;Property;_DissolveGlowColor;Dissolve Glow Color;27;1;[HDR];Create;0,0,0,0;3,0.485294,2.410345,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;-1644.535,-341.4165;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;180;-2689.13,2536.628;Float;False;Dissolve;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-2018.843,1333.727;Float;False;RimIntersectionEmission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;13;-1676.532,156.4727;Float;False;Global;_GrabScreen0;Grab Screen 0;0;0;Create;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;185;-1575.826,1413.658;Float;False;Property;_DissolveGlow;Dissolve Glow;28;0;Create;0.1;0.505;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;193;-1562.375,1297.611;Float;False;180;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;77;-4384.433,438.5211;Float;True;Property;_NormalTexture;Normal Texture;4;0;Create;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1379.049,-337.6564;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareLower;186;-946.0127,1417.541;Float;False;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;-551.8307,-27.52268;Float;False;75;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;187;-735.6656,1413.31;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;-549.8693,48.35181;Float;False;78;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-4069.944,438.6036;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;191;-547.2666,131.7832;Float;False;187;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;108;-1992.891,-581.9175;Float;False;ShieldColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;181;-544.5199,229.719;Float;False;180;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-1115.987,-343.2308;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;40;-271.6854,2.639563;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;QFX/SFX/Shield;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Off;0;0;False;0;0;Custom;0.5;True;False;0;True;TransparentCutout;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;46;0;45;0
WireConnection;46;2;44;0
WireConnection;48;0;46;0
WireConnection;48;1;47;0
WireConnection;50;1;48;0
WireConnection;51;0;50;0
WireConnection;51;1;49;0
WireConnection;53;0;51;0
WireConnection;53;1;52;0
WireConnection;54;0;53;0
WireConnection;56;0;54;0
WireConnection;56;1;55;0
WireConnection;58;0;57;0
WireConnection;58;1;56;0
WireConnection;67;0;68;0
WireConnection;67;1;68;0
WireConnection;69;0;67;0
WireConnection;69;1;58;0
WireConnection;196;0;172;2
WireConnection;196;1;171;0
WireConnection;163;2;161;0
WireConnection;163;1;162;2
WireConnection;70;0;69;0
WireConnection;198;0;199;0
WireConnection;198;1;196;0
WireConnection;62;0;60;0
WireConnection;173;0;171;0
WireConnection;173;1;172;2
WireConnection;12;1;80;0
WireConnection;165;1;163;0
WireConnection;63;3;62;0
WireConnection;4;0;2;0
WireConnection;4;1;3;0
WireConnection;86;0;12;0
WireConnection;195;0;198;0
WireConnection;195;1;173;0
WireConnection;166;0;165;0
WireConnection;166;1;164;0
WireConnection;152;0;153;0
WireConnection;152;1;151;2
WireConnection;25;0;34;0
WireConnection;87;0;63;0
WireConnection;87;1;88;0
WireConnection;87;2;71;0
WireConnection;144;0;143;0
WireConnection;144;1;152;0
WireConnection;201;0;195;0
WireConnection;19;0;4;0
WireConnection;19;2;21;0
WireConnection;19;1;20;2
WireConnection;168;1;166;0
WireConnection;200;0;201;0
WireConnection;90;0;87;0
WireConnection;28;0;25;0
WireConnection;5;1;19;0
WireConnection;142;0;144;0
WireConnection;194;0;168;1
WireConnection;138;1;139;0
WireConnection;30;0;29;0
WireConnection;30;1;91;0
WireConnection;30;2;28;0
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;178;0;175;0
WireConnection;178;1;194;0
WireConnection;145;0;142;0
WireConnection;145;1;146;0
WireConnection;145;2;146;0
WireConnection;177;0;200;0
WireConnection;177;1;175;0
WireConnection;14;0;42;0
WireConnection;14;1;12;0
WireConnection;155;0;156;0
WireConnection;155;1;145;0
WireConnection;141;0;138;0
WireConnection;141;1;30;0
WireConnection;179;0;177;0
WireConnection;179;1;178;0
WireConnection;11;0;7;0
WireConnection;11;1;8;0
WireConnection;158;0;14;0
WireConnection;158;1;155;0
WireConnection;180;0;179;0
WireConnection;73;0;141;0
WireConnection;13;0;11;0
WireConnection;77;1;81;0
WireConnection;16;0;158;0
WireConnection;16;1;13;0
WireConnection;186;0;193;0
WireConnection;186;1;185;0
WireConnection;186;2;184;0
WireConnection;186;3;190;0
WireConnection;187;0;186;0
WireConnection;78;0;77;0
WireConnection;108;0;42;0
WireConnection;75;0;16;0
WireConnection;40;0;76;0
WireConnection;40;1;79;0
WireConnection;40;2;191;0
WireConnection;40;10;181;0
ASEEND*/
//CHKSM=DBE64FBBD9ED37D9152A51000410BC0C12D21BDF