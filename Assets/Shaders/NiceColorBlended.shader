Shader "Custom/NiceColorBlended"
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
		
		
			
		Fog{ Color(0,0,0,0) }

		// extra pass that renders to depth buffer only
		Pass{
			ZWrite On
			ColorMask 0			
		}

		Pass
		{
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaTest Greater .01

			ColorMask RGB
			Cull Back
			Lighting Off
			ZWrite Off

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
				//col.rgb = i.color.xyz;
				col.a *= a;
				
				return col;
			}
			ENDCG
		}
	}

	Fallback "Transparent/Cutout/VertexLit"
}