Shader "Unlit/DistortionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		dudv("DUDV Map", 2D) = "white"{}
		animationTimer("AnimationTimer", Float) = 0.0
		waveStrength("Wave Strength", Float) = 0.01
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D dudv;
			Float animationTimer;
			Float waveStrength;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 distortedTexCoords = tex2D(dudv, float2(i.uv.x  + animationTimer, i.uv.y))*0.1;
				distortedTexCoords = i.uv + float2(distortedTexCoords.x, distortedTexCoords.y + animationTimer);
				float2 totalDistortion = (tex2D(dudv, distortedTexCoords)*2.0 -1.0) * waveStrength;
				i.uv += totalDistortion;
				i.uv.x = clamp(i.uv.x, 0.001, 0.999);
				i.uv.y = clamp(i.uv.y, 0.001, 0.999);

				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
