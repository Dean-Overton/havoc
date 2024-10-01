// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Good/ToonShader"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0.005
		_ASEOutlineColor( "Outline Color", Color ) = (0.1037736,0.001468492,0.001468492,0)
		_ToonRamp("Toon Ramp", 2D) = "white" {}
		[HDR]_RimColor("Rim Color", Color) = (0,1,0.8758622,0)
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_RimPower("Rim Power", Range( 0 , 10)) = 0.5
		_RimOffset("Rim Offset", Float) = 0.24
		_Tint("Tint", Color) = (0,0,0,0)
		_DiffuseColorPower("DiffuseColorPower", Range( 0 , 20)) = 1
		_Diffuse("Diffuse", 2D) = "white" {}
		[HDR]_GlowColor("Glow Color", Color) = (0.08627451,0.8588235,0.7568628,0)
		_Speed("Speed", Float) = 0
		_Tiling("Tiling", Vector) = (5,5,0,0)
		[HDR]_Emission("Emission", Color) = (0,0,0,0)
		_Teleport("Teleport", Range( -10 , 10)) = 0
		[Toggle]_TeleportReverse("TeleportReverse", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		float4 _ASEOutlineColor;
		float _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _GlowColor;
		uniform float2 _Tiling;
		uniform float _Speed;
		uniform float _Teleport;
		uniform float _TeleportReverse;
		uniform float4 _Emission;
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform sampler2D _ToonRamp;
		uniform float4 _Tint;
		uniform float _DiffuseColorPower;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float4 _RimColor;
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


		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform18 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Y_Gradient16 = saturate( ( ( transform18.y + _Teleport ) / (( _TeleportReverse )?( 10.0 ):( -10.0 )) ) );
			float mulTime6 = _Time.y * _Speed;
			float2 panner5 = ( mulTime6 * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord1 = i.uv_texcoord * _Tiling + panner5;
			float simplePerlin2D2 = snoise( uv_TexCoord1 );
			float Noise10 = ( simplePerlin2D2 + 1.0 );
			float temp_output_30_0 = ( Y_Gradient16 * 1.0 );
			float OpacityMask26 = ( ( ( ( 1.0 - Y_Gradient16 ) * Noise10 ) - temp_output_30_0 ) + ( 1.0 - temp_output_30_0 ) );
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult175 = dot( ase_worldNormal , ase_worldViewDir );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult78 = dot( ase_worldNormal , ase_worldlightDir );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			UnityGI gi85 = gi;
			float3 diffNorm85 = ase_worldNormal;
			gi85 = UnityGI_Base( data, 1, diffNorm85 );
			float3 indirectDiffuse85 = gi85.indirect.diffuse + diffNorm85 * 0.0001;
			float dotResult73 = dot( ase_worldNormal , ase_worldViewDir );
			float4 Toon103 = ( ( ( tex2D( _Diffuse, uv_Diffuse ) * tex2D( _ToonRamp, ( ( saturate( (dotResult175*0.49 + 0.49) ) * ( 1.0 - i.vertexColor ) ) + ( saturate( (dotResult78*0.5 + 0.5) ) * i.vertexColor ) ).rg ) * _Tint ) * ( ase_lightColor * float4( ( indirectDiffuse85 + ase_lightAtten ) , 0.0 ) ) * _DiffuseColorPower ) + ( saturate( ( ( ase_lightAtten * dotResult78 ) * pow( ( 1.0 - saturate( ( dotResult73 + _RimOffset ) ) ) , _RimPower ) ) ) * ( _RimColor * ase_lightColor ) ) );
			c.rgb = Toon103.rgb;
			c.a = 1;
			clip( OpacityMask26 - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float mulTime6 = _Time.y * _Speed;
			float2 panner5 = ( mulTime6 * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord1 = i.uv_texcoord * _Tiling + panner5;
			float simplePerlin2D2 = snoise( uv_TexCoord1 );
			float Noise10 = ( simplePerlin2D2 + 1.0 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform18 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Y_Gradient16 = saturate( ( ( transform18.y + _Teleport ) / (( _TeleportReverse )?( 10.0 ):( -10.0 )) ) );
			float4 Emission39 = ( ( _GlowColor * ( Noise10 * Y_Gradient16 ) ) + _Emission );
			o.Emission = Emission39.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma only_renderers d3d9 d3d11_9x d3d11 glcore gles3 vulkan 
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18100
294;-904;1347;754;7104.879;1222.818;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;109;-7400.891,-968.4506;Inherit;False;3337.5;1726;Comment;22;66;67;68;69;70;95;92;108;97;100;102;103;122;118;173;174;175;178;179;180;183;184;Toon;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;12;-3761.914,-940.9484;Inherit;False;1807.254;703.8394;Comment;9;7;6;5;4;1;2;8;9;10;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;22;-3765.485,-45.62938;Inherit;False;1632.813;688.2547;Comment;9;20;64;15;21;19;14;18;13;65;Y_Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-3711.914,-495.1088;Float;True;Property;_Speed;Speed;9;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;66;-7350.891,-86.45068;Inherit;False;507.201;385.7996;Comment;3;73;72;71;N . V;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;13;-3715.485,7.495445;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;68;-7350.891,-582.4505;Inherit;False;540.401;320.6003;Comment;3;78;77;76;N . L;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-3618.621,181.6492;Float;True;Property;_Teleport;Teleport;16;0;Create;True;0;0;False;0;False;0;10;-10;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-3297.355,192.4253;Float;True;Constant;_NegativeNumber;Negative Number;4;0;Create;True;0;0;False;0;False;-10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;76;-7238.891,-534.4505;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;174;-7201.579,-911.3986;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;173;-7189.355,-754.5626;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;72;-7254.891,121.5483;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;18;-3396.762,4.370626;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;6;-3444.214,-491.7627;Inherit;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;67;-6678.891,153.5483;Inherit;False;1617.938;553.8222;;13;101;99;98;94;93;89;88;86;83;81;80;75;74;Rim Light;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;77;-7252.617,-396.5623;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;69;-6552.95,-733.4809;Inherit;False;723.599;290;Also know as Lambert Wrap or Half Lambert;3;87;84;79;Diffuse Wrap;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-3297.043,420.2769;Float;True;Constant;_PositiveNumber;PositiveNumber;11;0;Create;True;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;71;-7302.891,-38.45119;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;175;-6912.474,-882.7772;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;65;-2999.818,193.6615;Float;True;Property;_TeleportReverse;TeleportReverse;17;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;178;-6640.955,-964.6791;Float;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;False;0.49;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;78;-6949.786,-505.8291;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-6514.827,-572.5306;Float;False;Constant;_WrapperValue;Wrapper Value;1;0;Create;True;0;0;False;0;False;0.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;73;-6998.891,41.54881;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-3060.857,9.551842;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;5;-3231.872,-524.5632;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-6598.891,425.5483;Float;False;Property;_RimOffset;Rim Offset;4;0;Create;True;0;0;False;0;False;0.24;0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;4;-3309.787,-890.9483;Float;True;Property;_Tiling;Tiling;10;0;Create;True;0;0;False;0;False;5,5;5,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-3016.67,-877.1157;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;19;-2779.75,7.624839;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;84;-6309.547,-687.481;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;177;-6435.674,-1079.629;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;179;-6485.73,-886.8307;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-6390.891,313.5483;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;21;-2544.55,7.6249;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;180;-6269.603,-910.9294;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;87;-6007.125,-608.015;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;176;-6143.629,-1072.8;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;118;-6271.797,-521.6021;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-2667.251,-699.0061;Float;False;Constant;_Booster;Booster;3;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;70;-6214.891,-294.4499;Inherit;False;812;304;Comment;5;96;91;90;85;82;Attenuation and Ambient;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;80;-6230.891,313.5483;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-2690.07,-878.3156;Inherit;False;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;81;-6054.891,313.5483;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;34;-3754.55,750.2947;Inherit;False;1884.174;830.5926;Comment;10;17;29;25;23;24;30;32;31;33;26;OpacityMask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-2458.544,-790.3158;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-6166.892,441.5483;Float;False;Property;_RimPower;Rim Power;3;0;Create;True;0;0;False;0;False;0.5;3.57;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-2316.272,4.925375;Float;False;Y_Gradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;82;-6199.732,-76.82364;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;181;-5941.43,-1014.616;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;-5830.651,-595.3448;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;-5878.892,201.5483;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;184;-5746.534,-755.3345;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;-3704.55,817.9334;Inherit;False;16;Y_Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;85;-6202.755,-145.897;Inherit;False;World;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;86;-5862.892,313.5483;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-2197.66,-847.2358;Float;True;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;42;-1616.402,-950.9901;Inherit;False;1139.918;677.533;Comment;8;35;36;40;38;41;39;105;107;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;95;-5558.891,-918.4506;Inherit;True;Property;_Diffuse;Diffuse;7;0;Create;True;0;0;False;0;False;-1;None;45645bc4f2eb8974e8ee3eb3fd2fb1d1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;92;-5553.883,-713.038;Inherit;True;Property;_ToonRamp;Toon Ramp;0;0;Create;True;0;0;False;0;False;-1;None;8ec9da9fc6849ce45be69c765eb8eaa3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1566.402,-503.4571;Inherit;False;16;Y_Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;91;-6202.002,-260.7175;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-5622.891,281.5483;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;93;-5702.891,425.5483;Float;False;Property;_RimColor;Rim Color;1;1;[HDR];Create;True;0;0;False;0;False;0,1,0.8758622,0;0,0.1008735,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;35;-1557.402,-724.4573;Inherit;False;10;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-3407.453,1029.689;Inherit;False;10;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;90;-5829.218,-190.7428;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;108;-5451.36,-514.4695;Float;False;Property;_Tint;Tint;5;0;Create;True;0;0;False;0;False;0,0,0,0;0.8490566,0.8490566,0.8490566,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;29;-3609.143,1255.717;Inherit;False;16;Y_Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;89;-5590.891,601.5482;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;23;-3398.316,803.2947;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-5100.101,-739.9604;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1270.402,-672.4573;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-5060.415,-222.2624;Float;False;Property;_DiffuseColorPower;DiffuseColorPower;6;0;Create;True;0;0;False;0;False;1;1.506319;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-3127.316,800.2947;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;40;-1285.437,-900.9901;Float;False;Property;_GlowColor;Glow Color;8;1;[HDR];Create;True;0;0;False;0;False;0.08627451,0.8588235,0.7568628,0;1,0.5886716,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-5398.89,409.5483;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-3349.944,1261.401;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-5596.063,-257.847;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;99;-5430.89,281.5483;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;-2859.763,1327.888;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-5238.891,281.5483;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-984.6053,-672.6795;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;105;-1089.813,-549.9413;Float;False;Property;_Emission;Emission;12;1;[HDR];Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-4849.292,-461.2502;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;31;-2872.292,964.348;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-2511.014,1085.18;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;-4502.892,-230.45;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;107;-758.4429,-618.233;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;117;-1501.912,896.5027;Inherit;False;1232.912;362.1022;Comment;6;111;110;112;113;115;114;Custom Outline;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-580.3841,-683.9204;Float;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-2113.375,840.249;Float;False;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;103;-4306.393,-220.6351;Float;False;Toon;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-3148.129,1994.3;Float;False;Property;_VertOffsetStrength;VertOffset Strength;11;0;Create;True;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-1151.053,1143.605;Float;False;Property;_Width;Width;15;0;Create;True;0;0;False;0;False;0;2.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;115;-511.9998,983.2164;Float;False;Outline;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;110;-1451.912,946.5027;Float;False;Property;_OutlineColor;Outline Color;13;0;Create;True;0;0;False;0;False;0,0,0,0;0.3,0.3,0.3,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;-2392.181,1769.276;Float;False;VertOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;393.4903,134.5127;Inherit;True;26;OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;111;-1106.623,947.785;Inherit;False;True;True;True;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;392.5855,-59.48207;Inherit;True;103;Toon;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;28;389.9956,-248.3918;Inherit;True;39;Emission;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-2799.129,1914.3;Inherit;False;10;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;391.3257,533.1406;Inherit;True;58;VertOffset;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-3147.181,1772.276;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-2858.129,1771.3;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;56;-3468.28,1936.076;Inherit;True;16;Y_Gradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;116;391.1243,326.0656;Inherit;True;115;Outline;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-1198.774,1038.291;Float;False;Property;_OutlineOpacity;Outline Opacity;14;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;55;-3475.779,1702.396;Inherit;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-2625.129,1776.3;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OutlineNode;112;-815.3624,965.8862;Inherit;False;0;False;Transparent;1;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;716.8643,-77.59871;Float;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;Good/ToonShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;7;Custom;0.5;True;True;0;True;Transparent;;Geometry;ForwardOnly;6;d3d9;d3d11_9x;d3d11;glcore;gles3;vulkan;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.005;0.1037736,0.001468492,0.001468492,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;18;0;13;0
WireConnection;6;0;7;0
WireConnection;175;0;174;0
WireConnection;175;1;173;0
WireConnection;65;0;20;0
WireConnection;65;1;64;0
WireConnection;78;0;76;0
WireConnection;78;1;77;0
WireConnection;73;0;71;0
WireConnection;73;1;72;0
WireConnection;14;0;18;2
WireConnection;14;1;15;0
WireConnection;5;1;6;0
WireConnection;1;0;4;0
WireConnection;1;1;5;0
WireConnection;19;0;14;0
WireConnection;19;1;65;0
WireConnection;84;0;78;0
WireConnection;84;1;79;0
WireConnection;84;2;79;0
WireConnection;177;0;175;0
WireConnection;177;1;178;0
WireConnection;177;2;178;0
WireConnection;75;0;73;0
WireConnection;75;1;74;0
WireConnection;21;0;19;0
WireConnection;180;0;179;0
WireConnection;87;0;84;0
WireConnection;176;0;177;0
WireConnection;80;0;75;0
WireConnection;2;0;1;0
WireConnection;81;0;80;0
WireConnection;9;0;2;0
WireConnection;9;1;8;0
WireConnection;16;0;21;0
WireConnection;181;0;176;0
WireConnection;181;1;180;0
WireConnection;183;0;87;0
WireConnection;183;1;118;0
WireConnection;88;0;82;0
WireConnection;88;1;78;0
WireConnection;184;0;181;0
WireConnection;184;1;183;0
WireConnection;86;0;81;0
WireConnection;86;1;83;0
WireConnection;10;0;9;0
WireConnection;92;1;184;0
WireConnection;94;0;88;0
WireConnection;94;1;86;0
WireConnection;90;0;85;0
WireConnection;90;1;82;0
WireConnection;23;0;17;0
WireConnection;97;0;95;0
WireConnection;97;1;92;0
WireConnection;97;2;108;0
WireConnection;38;0;35;0
WireConnection;38;1;36;0
WireConnection;24;0;23;0
WireConnection;24;1;25;0
WireConnection;98;0;93;0
WireConnection;98;1;89;0
WireConnection;30;0;29;0
WireConnection;96;0;91;0
WireConnection;96;1;90;0
WireConnection;99;0;94;0
WireConnection;32;0;30;0
WireConnection;101;0;99;0
WireConnection;101;1;98;0
WireConnection;41;0;40;0
WireConnection;41;1;38;0
WireConnection;100;0;97;0
WireConnection;100;1;96;0
WireConnection;100;2;122;0
WireConnection;31;0;24;0
WireConnection;31;1;30;0
WireConnection;33;0;31;0
WireConnection;33;1;32;0
WireConnection;102;0;100;0
WireConnection;102;1;101;0
WireConnection;107;0;41;0
WireConnection;107;1;105;0
WireConnection;39;0;107;0
WireConnection;26;0;33;0
WireConnection;103;0;102;0
WireConnection;115;0;112;0
WireConnection;58;0;62;0
WireConnection;111;0;110;0
WireConnection;57;0;55;0
WireConnection;57;1;56;0
WireConnection;60;0;57;0
WireConnection;60;1;61;0
WireConnection;62;0;60;0
WireConnection;62;1;63;0
WireConnection;112;0;111;0
WireConnection;112;2;113;0
WireConnection;112;1;114;0
WireConnection;0;2;28;0
WireConnection;0;10;27;0
WireConnection;0;13;104;0
ASEEND*/
//CHKSM=712EE91D16A6FE5E2E35C1E2007C4D858A359DB4