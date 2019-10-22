Shader "Unlit/CopyShader"
{

	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		ZTest Off Cull Off ZWrite Off Fog { Mode Off }


		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			sampler2D _MainTex;
			sampler2D _LayerTex;

			struct appdata {
				float4 pos : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target {
				float src = DecodeFloatRGBA(tex2D(_MainTex, i.uv));
				float dst = DecodeFloatRGBA(tex2D(_LayerTex, i.uv));

				return EncodeFloatRGBA(max(src, dst));
			}

			ENDCG
		}
	}
}
