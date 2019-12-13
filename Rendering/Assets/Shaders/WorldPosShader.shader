Shader "Custom/WorldPos"
{

	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ObjectID("ObjectID", Int) = 0 
		_MaxVisObjects("MaxVisObjects", Int) = 2
		_WorldRadius("WorldRadius", Float) = 100.0
		_WorldPosTextureRes("WorldPosTextureRes", Int) = 512
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" }

		Pass
		{

			ZWrite Off
			Cull Off
			ZTest Always

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			RWTexture2D<float4> _WorldPosOut : register(u1);


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 wpos: TEXCOORD1;

			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.wpos = mul(unity_ObjectToWorld ,v.vertex);
				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				float3 p = i.wpos.xyz / 100 + 0.5f;
				_WorldPosOut[uint2(i.uv*512)] = float4(p,1);
				//if (p.x > 1 ||p.y >1 || p.z>1)
				//	_WorldPosOut[uint2(i.uv * 512)] = float4(1, 0, 0, 1); 
				//if (p.x <0|| p.y <0|| p.z <0)
				//	_WorldPosOut[uint2(i.uv * 512)] = float4(0, 1, 0, 1);
		


				return fixed4(0,0, 0, 1);
			}
			ENDCG
		}
	}
}