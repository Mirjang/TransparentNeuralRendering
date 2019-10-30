﻿Shader "Custom/DepthPeel"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ObjectID("ObjectID", Int) = 0
		_MaxVisObjects("MaxVisObjects", Int) = 2
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" }

		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			ZWrite On
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _PrevDepthTex;
			int _ObjectID;
			int _MaxVisObjects; 

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float2 uv : TEXCOORD0;

			};
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 lightDir: TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float4 screenPos: TEXCOORD3;
				float3 normal : NORMAL;

				float depth : TEXCOORD4;
			};

			struct f2out
			{
				float4 col : COLOR0; 
				float4 uv : COLOR1;
				float4 depth : COLOR2;				
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				TANGENT_SPACE_ROTATION; 
				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;
				o.normal = v.normal;

				o.screenPos = ComputeScreenPos(o.pos);
				o.depth = COMPUTE_DEPTH_01;

				return o;
			}


			f2out frag(v2f i) : SV_Target
			{
				float depth = i.depth;
				float prevDepth = DecodeFloatRGBA(tex2Dproj(_PrevDepthTex, UNITY_PROJ_COORD(i.screenPos)));

				clip(depth - (prevDepth + 0.00001));

				//Lighting
				fixed3 tangentLightDir = normalize(i.lightDir);
				fixed3 tangentViewDir = normalize(i.viewDir);
				fixed3 tangentNormal = i.normal;

				fixed3 albedo = tex2D(_MainTex, i.uv).rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));


				f2out o;

				o.col = float4(ambient + diffuse, .5); //fixed alpha for now
				//o.col = float4(tex2D(_MainTex, i.uv).rgb, .25); 
				o.uv = float4(i.uv.x, i.uv.y, _ObjectID / float(255), 1);
				//o.mask = float4(float(, 0, 0, 1);

				/*if (o.mask == 1)
					o.uv = float4(1, 0, 0, 0); */
	
				//float fwd = 1; 
				//float back = 0;
				//if (mul((float3x3)unity_ObjectToWorld, i.normal).z < 0)
				//{
				//	fwd = 0; 
				//	back = 1; 
				//}
				//o.uv = float4(fwd,back,0, 1);

				o.depth = EncodeFloatRGBA(i.depth);

				return o;
			}
			ENDCG
		}
	}
}