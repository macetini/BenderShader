// Shader curves the item by translating the vertex through a function of a parabola.
Shader "Custom/CurveWorld"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert addshadow

		// Amount of XY bend
		uniform float2 _BendAmount;
		// Top of parabolas vertex function.
		uniform float3 _BendOrigin;		

		// The distance point beyond which the shaded does not apply the bend. (REMOVED - NEW IMPLEMENTATION)
		//uniform float _BendFalloff;

		// The value used to turn the integer bend amounts to decimal numbers.
		uniform float _BendAmountDiluter;

		uniform float _TilesLength;
		uniform int _CurveLength;

		uniform int _CurvePointsCount;
		uniform float4 _CurvePoints[999];

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input
		{
			  float2 uv_MainTex;
		};

		//The function passes the current vertex and returns the curved vertex.
		float4 Curve(float4 v)
		{			
			_BendAmount *= _BendAmountDiluter;

			float4 world = mul(unity_ObjectToWorld, v);

			float dist = length(world.xz - _BendOrigin.xz);

			//dist = max(0, dist - _BendFalloff);
			
			dist = dist * dist;

			world.xy += dist * _BendAmount;

			return mul(unity_WorldToObject, world);
	  }

	  void vert(inout appdata_full v)
	  {
			v.vertex = Curve(v.vertex);
	  }

	  void surf(Input IN, inout SurfaceOutput o)
	  {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
	  }

	  ENDCG
	}

		Fallback "Mobile/Diffuse"
}