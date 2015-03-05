Shader "Custom/Brightness Contrast Unlit Transparent" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Color Tint (RGBA)", Color) = (1.0, 1.0, 1.0, 1.0)
		_Screen ("Screen Tint (RGB)", Color) = (0, 0, 0, 0)
		_Contrast ("Contrast Factor", Range (-100, 100)) = 0
		_Brightness ("Brightness Factor", Range (-1, 1)) = 0
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}
	SubShader {
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert nofog keepalpha
		#pragma multi_compile _ PIXELSNAP_ON

		sampler2D _MainTex;
		fixed4 _Color;
		fixed4 _Screen;
		fixed _Contrast;
		fixed _Brightness;

		struct Input {
			float2 uv_MainTex;
			fixed4 color;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON)
			v.vertex = UnityPixelSnap (v.vertex);
			#endif
		
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color;
		}
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			fixed cFactor = (100 + _Contrast) * 0.01;
			cFactor *= cFactor;
			
			o.Albedo.rgb = ((c.rgb - 0.5) * cFactor) + 0.5;
			o.Albedo.rgb += _Brightness;
			o.Albedo.rgb = (1 - (1 - o.Albedo.rgb) * (1 - _Screen.rgb));
			
			o.Albedo.rgb *= _Color.rgb * IN.color.rgb * c.a;
			o.Alpha = _Color.a * IN.color.a * c.a;
		}
		ENDCG
	} 
	Fallback "Transparent/VertexLit"
}
