Shader "Custom/ReplacementDepth"
{
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

		Cull Off ZWrite On ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _CameraDepthTexture;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float depth : DEPTH;

				float4 scrPos:TEXCOORD1;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w;

				o.scrPos = ComputeScreenPos(o.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float depthValue = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)).r);
				half4 depth;

				depth.r = depthValue;
				depth.g = depthValue;
				depth.b = depthValue;
				depth.a = 1;
				return fixed4(i.depth, i.depth, i.depth, 1);
				//return depth;
			}
			ENDCG
		}
	}
}
