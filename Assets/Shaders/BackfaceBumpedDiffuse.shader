﻿Shader "Custom/Backface Bumped Diffuse"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Transparency("Transparency", Range(0.0,1)) = 1
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Cull front    // FLIP THE SURFACES
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag Lambert alpha:fade

#include "UnityCG.cginc"

		struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		half2 texcoord : TEXCOORD0;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float _Transparency;


	v2f vert(appdata_t v) {
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		v.texcoord.x = 1 - v.texcoord.x;
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target{
		fixed4 col = tex2D(_MainTex, i.texcoord);
	col.a = _Transparency;   // ADDED
	return col;
	}
		ENDCG
	}
	}
}