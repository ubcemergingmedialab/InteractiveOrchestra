Shader "Custom/GestureTrace"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		[HDR]_CutoffColor("Cutoff Color", Color) = (1, 0, 0, 0)

		[HDR]_Emission("Emission", color) = (0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
        LOD 200

		Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
			float facing : VFACE;
        };

        half _Glossiness;
        half _Metallic;
		half3 _Emission;
        fixed4 _Color;
		float4 _TracingPlane;
		float4 _CutoffColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			//calculate signed distance to plane
			float distance = dot(IN.worldPos, _TracingPlane.xyz);
			distance = distance + _TracingPlane.w;
			//discard surface above plane
			clip(-distance);

			float facing = IN.facing * 0.5 + 0.5;

            // normal color stuff
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			c *= _Color;
            o.Albedo = c.rgb * facing;
            o.Metallic = _Metallic * facing;
            o.Smoothness = _Glossiness * facing;
			o.Emission = lerp(_CutoffColor, _Emission, facing);

        }
        ENDCG
    }
    FallBack "Standard"
}
