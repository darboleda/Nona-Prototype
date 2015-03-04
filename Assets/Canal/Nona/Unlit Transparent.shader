Shader "Unlit/Transparent Tinted" {
	Properties {
		_Color ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white"
    }
	SubShader {
		Pass {
		
			Lighting Off
			ZWrite Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			Tags { "Queue" = "Transparent" }

			SetTexture [_MainTex] {
				ConstantColor [_Color]
				Combine Texture * Constant, Texture * Constant
			}
		}
	}
}
