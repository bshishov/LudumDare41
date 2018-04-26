Shader "Custom/Nice Toon" {
	Properties
	{
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(0.0, 1)) = .005
		_MainTex("Base (RGB)", 2D) = "white" { }
		_Ramp("Color Ramp", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Geometry" "IgnoreProjector" = "True" }
		
		Pass{
			Stencil
			{
				Ref 64
				Comp always
				Pass replace
			}						
			ColorMask A
		}

		CGPROGRAM		

		#pragma surface surf Ramp alpha

		sampler2D _Ramp;
		sampler2D _MainTex;
		fixed4 _Color;

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
			fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = col.rgb;
			o.Alpha = col.a;
		}
		ENDCG

		/*
		Pass
		{
			Name "BASE"
			Tags{ "LightMode" = "ForwardBase" }

			Stencil
			{
				Ref 64
				Comp always
				Pass replace
			}

			ZWrite On
			ZTest LEqual
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag 
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform float4 _LightColor0;
			uniform float4 _Color;			

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv: TEXCOORD0;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float2 uv: TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				output.posWorld = mul(modelMatrix, input.vertex);
				output.normalDir = normalize(
					mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				output.pos = UnityObjectToClipPos(input.vertex);
				output.uv = input.uv;
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				fixed4 col = tex2D(_MainTex, input.uv) * _Color;
				return col;
			}
			ENDCG
		}*/

		
		Pass
		{
			Name "OUTLINE"
			Tags{ "LightMode" = "Always" }

			Stencil {
				Ref 64
				Comp NotEqual
			}
		
			Blend SrcAlpha OneMinusSrcAlpha
			//Cull Off
			Cull Back
			ZWrite Off			

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;				
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : POSITION;				
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform float _Outline;
			uniform float4 _OutlineColor;

			v2f vert(appdata v) 
			{				
				v2f o;
				float4 p = UnityObjectToClipPos(v.vertex);
				float4 pnorm = UnityObjectToClipPos(v.vertex + v.normal);				
				float2 ofset = normalize((pnorm - p).xy);				
				o.pos.xy = p.xy + ofset * _Outline;
				o.pos.zw = p.zw;				
				o.color = _OutlineColor;
				o.uv = v.uv;
				return o;
			}

			half4 frag(v2f i) :COLOR
			{	
				fixed4 tex = tex2D(_MainTex, i.uv);
				return i.color * tex.a;
			}
			ENDCG
		}

		/*
		Pass
		{
			Name "BASE"
			ZWrite On
			ZTest LEqual
			Cull Back
			//Blend Off  
			Blend SrcAlpha OneMinusSrcAlpha
			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}
			Lighting On
			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
				Combine texture * constant
			}
			SetTexture[_MainTex]
			{
				Combine previous * primary DOUBLE
			}
		}*/
	}

	Fallback "Diffuse"
}