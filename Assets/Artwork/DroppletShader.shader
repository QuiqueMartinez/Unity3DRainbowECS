// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/LedAlpha" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert alpha:fade
#pragma multi_compile_instancing
#pragma instancing_options procedural:setup

		sampler2D _MainTex;
	//fixed4 _Color;
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	StructuredBuffer<float4> positionBuffer;
	StructuredBuffer<float4> colorBuffer;
#endif

	struct Input {
		float2 uv_MainTex;
	};

	void setup()
	{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		float4 data = positionBuffer[unity_InstanceID];

		float rotation = data.w * data.w * _Time.y * 0.5f;
		//rotate2D(data.xz, rotation);

		unity_ObjectToWorld._11_21_31_41 = float4(data.w, 0, 0, 0);
		unity_ObjectToWorld._12_22_32_42 = float4(0, data.w, 0, 0);
		unity_ObjectToWorld._13_23_33_43 = float4(0, 0, data.w, 0);
		unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);
		unity_WorldToObject = unity_ObjectToWorld;
		unity_WorldToObject._14_24_34 *= -1;
		unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
#endif
	}

	void surf(Input IN, inout SurfaceOutput o) {
		float4 col = 1.0f;
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		//col.gb = (float)(unity_InstanceID % 256) / 255.0f;
		col = colorBuffer[unity_InstanceID];
#else
		//col.gb = float4(0, 0, 1, 1);
		col = float4(0, 0, 1, 1);
#endif

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * col;
		//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}

		Fallback "Legacy Shaders/Transparent/VertexLit"
}