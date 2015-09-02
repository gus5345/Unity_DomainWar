Shader "Custom/NewShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SubTex ("Back (RGB)", 2D) = "white" {}
		_FillRate("체워진 양", float) = -3.15
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf NoLighting

		sampler2D _MainTex;
		sampler2D _SubTex;
		float _FillRate;

		struct Input {
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			half4 c;

			float2 ruv = IN.uv_MainTex - float2(0.5, 0.5);
			if (atan2(ruv.x, -ruv.y) > _FillRate)
			{
				c = tex2D(_MainTex, IN.uv_MainTex);
			}
			else
			{
				c = tex2D(_SubTex, IN.uv_MainTex);
			}

			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
