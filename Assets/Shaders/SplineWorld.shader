Shader "Custom/SplineWorld"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}		
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Cull Off		

		CGPROGRAM				
		#pragma surface surf Lambert vertex:vert addshadow
		
		//uniform float3 _OriginPoint = 0; 
		//uniform float _WorldLength;
		uniform int _SplineLength;

		uniform int _SplinePointsCount;		
		uniform float4 _SplinePoints[999]; 

		uniform float _splineProgress;		

		struct appdata 
		{
			float4 vertex : POSITION;				
		};

		struct Input
		{
			float2 uv_MainTex;
		};	

		sampler2D _MainTex;
		fixed4 _Color;

		float4 GetFirstDerivative(float4 p0, float4 p1, float4 p2, float4 p3, float t)
		{
			t = saturate(t);
			float oneMinusT = 1.0 - t;

			return
				3.0 * oneMinusT * oneMinusT * (p1 - p0) +
				6.0 * oneMinusT * t * (p2 - p1) +
				3.0 * t * t * (p3 - p2);
		}

		float4 GetVelocity(float t)
		{
			int i;
			
			if (t >= 1.0)
			{
				t = 1.0;
				i = _SplinePointsCount - 4.0;
			}
			else
			{			
				t = saturate(t) * ( (_SplinePointsCount - 1.0) / 3.0 );
				i = (int)t;
				t -= i;
				i *= 3;
			}
			
			return GetFirstDerivative(_SplinePoints[i], _SplinePoints[i + 1], _SplinePoints[i + 2], _SplinePoints[i + 3], t);
		}

		float4 GetTangent(float t)
		{
			return normalize(GetVelocity(t));
		}

		float4 GetBinormal(float t)
		{
			float4 tangent = GetTangent(t);
			float4 vectorUp = float4(0.0, 1.0, 0.0, 0.0);
			
			return float4(normalize(cross(vectorUp, tangent)), 0);
		}

		float4 GetNormal(float t)
		{
			float4 tangent = GetTangent(t);
			float4 binormal = GetBinormal(t);

			return float4(normalize(cross(tangent, binormal)), 0);
		}
		
		float4 GetPoint(float4 p0, float4 p1, float4 p2, float4 p3, float t)
		{			
			float oneMinusT = 1.0 - saturate(t);
			
			return 
				oneMinusT * oneMinusT * oneMinusT * p0 +
				3.0 * oneMinusT * oneMinusT * t * p1 +
				3.0 * oneMinusT * t * t * p2 +
				t * t * t * p3;
		}		

		float4 GetPointAlongSpline(float t)
		{
			int i;
						
			if (t >= 1.0)
			{
				t = 1.0;
				i = _SplinePointsCount - 4.0;
			}
			else
			{			
				t = saturate(t) * ( (_SplinePointsCount - 1.0) / 3.0 ); 
				i = (int)t;
				t -= i;
				i *= 3;
			}

			return GetPoint(_SplinePoints[i], _SplinePoints[i + 1], _SplinePoints[i + 2], _SplinePoints[i + 3], t); 
		}

		void vert(inout appdata_full v)
		{	
			float4 worldPoint = mul(unity_ObjectToWorld, v.vertex);
															
			float t = worldPoint.z / _SplineLength;
			float4 splinePoint = GetPointAlongSpline(t); 			

			float4 distanceBinormal = GetBinormal(t);
			half dotBinormal = dot(float4(0.0, 0.0, 1.0, 0.0), distanceBinormal);

			float angle = -asin(dotBinormal);

			float4 projectedPoint = float4(worldPoint.x, 0.0, 0.0, worldPoint.w);

			float4x4 transMatrix = float4x4(
								cos(angle),		0.0,	sin(angle),		splinePoint.x,
								0.0,			1.0,	0.0,			worldPoint.y + splinePoint.y,
								-sin(angle),	0.0,	cos(angle),		splinePoint.z,
								0.0,			0.0,	0.0,			1.0);

			worldPoint = mul(transMatrix, projectedPoint);			
			
			float4 progressPoint = GetPointAlongSpline(_splineProgress);
			worldPoint -= progressPoint;
			
			//
			float4 progressNormal = GetNormal(_splineProgress);			
			float4 forward = float4(0.0, 0.0, 1.0, 0.0);
			float dotNormal = dot(forward, -progressNormal);		
			
			float angleX = asin(dotNormal);

			float4x4 transMatrix_X = float4x4(											
											1.0,	0.0,			0.0,			0.0,
											0.0,	cos(angleX),	-sin(angleX),	0.0,
											0.0,	sin(angleX),	cos(angleX),	0.0,
											0.0,	0.0,			0.0,			1.0
										);

			worldPoint = mul(transMatrix_X, worldPoint);			
			//

			//			
			float4 progressBinormal = GetBinormal(_splineProgress);			
			float dotProgressBinormal = dot(float4(0.0, 0.0, 1.0, 0.0), progressBinormal);
			
			float angleY = asin(dotProgressBinormal);
			
			float4x4 transMatrix_Y = float4x4(
									cos(angleY),	0.0,	sin(angleY),	0.0,
									0.0,			1.0,	0.0,			0.0,
									-sin(angleY),	0.0,	cos(angleY),	0.0,
									0.0,			0.0,	0.0,			1.0
								);

			worldPoint = mul(transMatrix_Y, worldPoint);
			//

			float4 localPoint = mul(unity_WorldToObject, worldPoint);			
			
			v.vertex = localPoint;
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