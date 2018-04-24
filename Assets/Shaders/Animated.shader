Shader "Custom/Animated"
{
	Properties
	{
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
		_MainTex("Texture", 2D) = "white" {}
		_RowTiles("Tiles in row", Float) = 10.0
		_ColTiles("Tiles in column", Float) = 10.0
		_Speed("Animation Speed", Range(-50.0,50.0)) = 1.0
		_TransparentCutout("Cutout Color", Range(0.0,50.0)) = 1.0
		_BackgroundColor("BG Color", Color) = (0.0,0.0,0.0,1.0)
		_SrcBlend("Src Blend", Int) = 0
		_DstBlend("Dst Blend", Int) = 0
	}
	SubShader
	{
		Tags{ 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent"			
			"PreviewType" = "Plane"
			"DisableBatching" = "True"		
		}
		LOD 100

		Pass
		{
			//ZWrite On									
			Blend SrcAlpha OneMinusSrcAlpha
			//Blend One OneMinusSrcAlpha
			//Blend [_SrcBlend] [_DstBlend]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag						

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;				
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _BackgroundColor;
			float _TransparentCutout;
			float _RowTiles;
			float _ColTiles;
			float _Speed;

			float gauss(float x, float spread)
			{
				return (1.0 / (spread * 2.50662827)) * exp(-x * x / (2.0 * spread * spread));
			}

			v2f vert(appdata v)
			{
				v2f o;

				//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float scaleX = length(mul(unity_ObjectToWorld, float4(1.0, 0.0, 0.0, 0.0)));
				float scaleY = length(mul(unity_ObjectToWorld, float4(0.0, 1.0, 0.0, 0.0)));
				o.vertex = mul(
					UNITY_MATRIX_P,
					mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
					+ float4(v.vertex.x * scaleX, v.vertex.y * scaleY, 0.0, 0.0));

				o.uv = v.uv;

				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}					

			fixed4 frag(v2f i) : SV_Target
			{				

				float x = fmod(_Time.z * _Speed, _RowTiles);

				int2 currentFrame;
				currentFrame.x = floor(x);
				currentFrame.y = fmod(_Time.z * _Speed / _RowTiles, _ColTiles);

				int2 nextFrame;
				nextFrame.x = fmod(currentFrame.x + 1, _RowTiles);// fmod(_Time.z * _Speed + _Speed, _RowTiles);
				nextFrame.y = currentFrame.y; // fmod((_Time.z * _Speed + _Speed) / _RowTiles, _ColTiles);

				//float lerpFactor = fmod(_Time.z * _Speed, _Speed) / _Speed;
				float lerpFactor = x - floor(x);

				// sample the texture
				float2 d = float2(1.0 / _RowTiles, 1.0 / _ColTiles);


				fixed4 currentColor = tex2D(_MainTex, i.uv.xy * d + currentFrame * d);
				fixed4 nextColor = tex2D(_MainTex, i.uv.xy * d + nextFrame * d);
				

				fixed4 interpCol = lerp(currentColor, nextColor, lerpFactor);

				//fixed4 col =  * _Color;
				//col += (_Time.z * _Speed) % 1.0;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				//texcol.a = clamp(length(texcol.rgb) * _TransparentCutout, 0.0, 1.0);
				//clip(saturate(texcol - _BackgroundColor));

				float s = 1 - interpCol.r;
				fixed4 c = interpCol + s * _Color;
				return fixed4(c.rgb, interpCol.a * _Color.a);
			}
			ENDCG
		}
	}
}