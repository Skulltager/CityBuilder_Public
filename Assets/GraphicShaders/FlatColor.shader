Shader "Custom/FlatColor"
{
	Properties
	{
		flatColor("Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
        Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }  
		CGINCLUDE
		
		float4 flatColor;

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
				return flatColor;
            }

			ENDCG
		}
        Pass
        {
            Tags {"LightMode"="ShadowCaster"}
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
 
            struct v2f {
                V2F_SHADOW_CASTER;
            };
 
            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
 
            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
	}
}