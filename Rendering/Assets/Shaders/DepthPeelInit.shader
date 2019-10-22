// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/DepthPeel/Init" {
	Properties{
		_Color("Color Tint", Color) = (1,1,1,1)
		_MainTex("Main Tex", 2D) = "white" {}
	}
		SubShader{
			Tags {"Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

			Pass {
				Tags { "LightMode" = "ForwardBase" }

				ZWrite On

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#include "Lighting.cginc"

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;

				struct a2v {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float2 uv: TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 lightDir: TEXCOORD1;
					float3 viewDir : TEXCOORD2;
					float3 normal : NORMAL;

					float depth : TEXCOORD3;
				};

				struct f2out
				{
					float4 col : COLOR0;
					float4 uv : COLOR1;
					float4 depth : COLOR2;
				};


				v2f vert(a2v v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;

					TANGENT_SPACE_ROTATION;
					o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
					o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;
					o.normal = v.normal;

					o.depth = COMPUTE_DEPTH_01;

					return o;
				}

				f2out frag(v2f i) : SV_Target {
					fixed3 tangentLightDir = normalize(i.lightDir);
					fixed3 tangentViewDir = normalize(i.viewDir);
					fixed3 tangentNormal = UnpackNormal(tex2D(_BumpMap, i.uv));

					//Lighting
					fixed3 tangentLightDir = normalize(i.lightDir);
					fixed3 tangentViewDir = normalize(i.viewDir);
					fixed3 tangentNormal = i.normal;

					fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
					fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
					fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));



					f2out o;

					o.col = float4(ambient + diffuse, 1); //fixed alpha for now
					//o.col = float4(tex2D(_MainTex, i.uv).rgb, .25); 
					//o.col = float4(i.normal, 1);
					o.uv = float4(i.uv.x, i.uv.y, 0, 1);
					o.depth = EncodeFloatRGBA(i.depth);

					return o;
				}

				ENDCG
			}
	}
		FallBack "Diffuse/VertexLit"
}