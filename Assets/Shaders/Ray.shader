Shader "Custom/Ray"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TintColor("Color", COLOR) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Color(0,0,0,0) }

		Pass
		{
			Lighting Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag						

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;				
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _TintColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float y = sin(v.uv.x * 50 + _Time.w * 10) * 0.3;
				o.vertex.y += y;

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;				
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed a = col.r;
				fixed4 dist = fixed4(1,1,1,1) - col;
				fixed l = sqrt(length(dist));
				col.rgb = fixed3(1, 1, 1) * (1 - l) + _TintColor.xyz * l;
				col.rgb *= i.color.xyz;
				col.a = a * i.color.a;

				return col;
			}
			ENDCG
		}
	}

	Fallback "Transparent/Cutout/VertexLit"
}