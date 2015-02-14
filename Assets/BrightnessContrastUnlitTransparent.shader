Shader "Custom/Brightness Contrast Unlit Transparent" {
	Properties {
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_Color ("Color Tint (RGBA)", Color) = (1.0, 1.0, 1.0, 1.0)
		_Screen ("Screen Tint (RGB)", Color) = (0, 0, 0, 0)
		_Contrast ("Contrast Factor", Range (-100, 100)) = 0
		_Brightness ("Brightness Factor", Range (-1, 1)) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		half4 _Color;
		half4 _Screen;
		half _Contrast;
		half _Brightness;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half cFactor = (100 + _Contrast) * 0.01;
			cFactor *= cFactor;
			o.Albedo = ((c.rgb - 0.5) * cFactor) + 0.5;
			o.Albedo += _Brightness;
			o.Albedo = (1 - (1 - o.Albedo) * (1 - _Screen.rgb));
			o.Albedo *= _Color.rgb * _Color.a;
			o.Alpha = c.a * _Color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
