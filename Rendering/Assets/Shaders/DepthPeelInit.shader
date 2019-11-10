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
					float2 uv: TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
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

					return o;
				}

				f2out frag(v2f i) : SV_Target {


					f2out o;

					o.col = float4(0,0,0, 1); //fixed alpha for now
					//o.col = float4(tex2D(_MainTex, i.uv).rgb, .25); 
					//o.col = float4(i.normal, 1);
					o.uv = float4(0,0,0, 1);
					o.depth = EncodeFloatRGBA(1);

					return o;
				}

				ENDCG
			}
	}
		FallBack "Diffuse/VertexLit"
}