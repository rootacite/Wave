Shader "Custom/BlurEffect"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white"{}
		_BlurSize("BlurSize", Range(0, 1)) = 1.0
	}

	SubShader
	{
		CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		float4 _MainTex_ST;
		half4 _MainTex_TexelSize;
		float _BlurSize;

		struct a2v
		{
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
		};

		struct v2f
		{
			float4 svPos : SV_POSITION;
			float2 uv[5] : TEXCOORD0;
		};

		v2f vert_hor(a2v v)
		{
			v2f o;
			o.svPos = UnityObjectToClipPos(v.vertex);
			float2 uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			o.uv[0] = uv;
			o.uv[1] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[2] = uv - float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
			o.uv[3] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
			o.uv[4] = uv - float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
			return o;
		}

		v2f vert_ver(a2v v)
		{
			v2f o;
			o.svPos = UnityObjectToClipPos(v.vertex);
			float2 uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			o.uv[0] = uv;
			o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
			o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
			o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
			return o;
		}
		
		fixed4 frag(v2f f) : SV_TARGET
		{
			half weight[3] = {0.4026, 0.2442, 0.0545};
			fixed3 color = tex2D(_MainTex, f.uv[0]).rgb * weight[0];
			color += tex2D(_MainTex, f.uv[1]).rgb * weight[1];
			color += tex2D(_MainTex, f.uv[2]).rgb * weight[1];
			color += tex2D(_MainTex, f.uv[3]).rgb * weight[2];
			color += tex2D(_MainTex, f.uv[4]).rgb * weight[2];
			return fixed4(color, 1.0);
		}
		ENDCG

		Pass
		{
			Name "BLUR_EFFECT_HORIZONTAL"
			CGPROGRAM
			#pragma vertex vert_hor
			#pragma fragment frag
			ENDCG
		}

		Pass
		{
			Name "BLUR_EFFECT_VERTICAL"
			CGPROGRAM
			#pragma vertex vert_ver
			#pragma fragment frag
			ENDCG
		}
	}
	Fallback Off
}
