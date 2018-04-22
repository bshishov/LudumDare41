// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Toon" {
	Properties {		
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Ramp("ColorRamo", 2D) = "white" {}
		_OutlineColor("Ouline Color", COLOR) = (0, 0, 0, 1)
		_XRayColor("XRay Color", COLOR) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }	
		LOD 200				

		Pass {			
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent+3" "IgnoreProjector" = "True" }
			ZTest Greater
			ZWrite Off			
			Lighting Off

			//Blend Off
			//Blend One One
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;								
			};

			struct v2f
			{				
				float4 vertex : SV_POSITION;				
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);				
				return o;
			}

			fixed4 _XRayColor;

			fixed4 frag(v2f i) : SV_Target
			{			
				return _XRayColor;
			}
			ENDCG
		}

		
		CGPROGRAM
		#pragma surface surf Ramp

		sampler2D _Ramp;
		sampler2D _MainTex;

		half4 LightingRamp(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half3 ramp = tex2D(_Ramp, float2(diff, diff)).rgb;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * atten;
			c.a = s.Alpha;
			return c;
		}

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		}
		ENDCG		
		
	}

	FallBack "Diffuse"
}
