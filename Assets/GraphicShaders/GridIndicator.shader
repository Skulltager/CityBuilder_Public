Shader "Custom/GridIndicator"
{
	Properties
	{
		indicatorColor("Indicator Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
        Tags {"Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Cutout" }  
        Blend SrcAlpha OneMinusSrcAlpha
		ZTest Always
        LOD 200
		CGINCLUDE
		
		float4 indicatorColor;

		ENDCG
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION0;
			};

			v2f vert(appdata v)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(v.vertex);
                return output;
            }

            float4 frag(v2f input) : SV_TARGET
            {
				return indicatorColor;
            }

			ENDCG
		}
	}
}