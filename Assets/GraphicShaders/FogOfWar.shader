Shader "Custom/FogOfWar"
{
	Properties
	{
		blendDistance("Blend Distance", Range(0.1, 1)) = 0.1
	}
	
	SubShader
	{
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Cutout" }  
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
		CGINCLUDE
		
        uniform sampler2D _CameraDepthTexture;
        uniform float4x4 viewProjInv;
		float blendDistance;

		StructuredBuffer<float> fogOfWarMap;
		int biomeWidth;
		int biomeHeight;

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
                float4 screenPos : TEXCOORD0;
			};

			v2f vert(appdata v)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(v.vertex);
                output.screenPos = ComputeScreenPos(output.vertex);
                return output;
            }

            float GetBiomeFogAmount(int index)
            {
                int clampedIndex = clamp(index, 0, biomeWidth * biomeHeight);
                return fogOfWarMap[clampedIndex];
            }
			
            bool DepthIsNotSky(float depth)
            {
                #if defined(UNITY_REVERSED_Z)
                return (depth > 0.0);
                #else
                return (depth < 1.0);
                #endif
            }

            float4 frag(v2f input) : SV_TARGET
            {
				float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV);
                #if defined(SHADER_API_OPENGL)
                    depth=depth*2.0-1.0;
                #endif
				
                if (!DepthIsNotSky(depth))
					clip(-1);

                float4 H = float4(screenUV.x*2.0-1.0, (screenUV.y)*2.0-1.0, depth, 1.0);

                float4 D = mul(viewProjInv,H);
                float4 col = D/D.w;

				float2 hitPosition = col.xz;
                int leftIndex = hitPosition.x - 0.5;
				int bottomIndex = hitPosition.y - 0.5;
				int rightIndex = leftIndex + 1;
				int topIndex = bottomIndex + 1;
				
				float uvXInfluence = fmod(hitPosition.x - 0.5, 1);
				float uvYInfluence = fmod(hitPosition.y - 0.5, 1);
				
                uvXInfluence = max(0, min(1, (uvXInfluence - 0.5) / blendDistance + 0.5));
                uvYInfluence = max(0, min(1, (uvYInfluence - 0.5) / blendDistance + 0.5));
				
				float bottomLeftFogAmount = GetBiomeFogAmount(leftIndex + bottomIndex * biomeWidth);
				float bottomRightFogAmount = GetBiomeFogAmount(rightIndex + bottomIndex * biomeWidth);
				float topLeftFogAmount = GetBiomeFogAmount(leftIndex + topIndex * biomeWidth);
				float topRightFogAmount = GetBiomeFogAmount(rightIndex + topIndex * biomeWidth);
				
				float fogAmount = bottomLeftFogAmount * (1 - uvXInfluence) * (1 - uvYInfluence);
				fogAmount += bottomRightFogAmount * uvXInfluence * (1 - uvYInfluence);
				fogAmount += topLeftFogAmount * (1 - uvXInfluence) * uvYInfluence;
				fogAmount += topRightFogAmount * uvXInfluence * uvYInfluence;
				
				return float4(0, 0, 0, fogAmount);
            }

			ENDCG
		}
	}
}