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
		int chunkXOffset;
		int chunkYOffset;

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

            float GetBiomeFogAmount(int xIndex, int yIndex)
            {
                int finalIndex = (yIndex + 1) * (biomeWidth + 2) + (xIndex + 1);
                return fogOfWarMap[finalIndex];
            }
			
            bool DepthIsSky(float depth)
            {
                #if defined(UNITY_REVERSED_Z)
                return (depth <= 0.0);
                #else
                return (depth >= 1.0);
                #endif
            }

            float4 frag(v2f input) : SV_TARGET
            {
				float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV);
                #if defined(SHADER_API_OPENGL)
                    depth=depth*2.0-1.0;
                #endif
				
				clip(DepthIsSky(depth) ? -1 : 1);

                float4 H = float4(screenUV.x*2.0-1.0, (screenUV.y)*2.0-1.0, depth, 1.0);

                float4 D = mul(viewProjInv,H);
                float4 col = D/D.w;

				if (col.y < -0.01)
					return float4(0, 0, 0, 1);


				float2 hitPosition = col.xz;
				hitPosition -= float2(chunkXOffset, chunkYOffset);
				bool outOfBounds = hitPosition.x < 0 || hitPosition.x > biomeWidth || hitPosition.y < 0 || hitPosition.y > biomeHeight;
				clip(outOfBounds ? -1 : 1);

                int leftIndex = floor(hitPosition.x - 0.5);
				int bottomIndex = floor(hitPosition.y - 0.5);

				int rightIndex = leftIndex + 1;
				int topIndex = bottomIndex + 1;
				
				float uvXInfluence = hitPosition.x < 0 ? 1 - fmod(abs(hitPosition.x) + 0.5, 1) : fmod(hitPosition.x + 0.5, 1);
				float uvYInfluence = hitPosition.y < 0 ? 1 - fmod(abs(hitPosition.y) + 0.5, 1) : fmod(hitPosition.y + 0.5, 1);
				
                uvXInfluence = max(0, min(1, (uvXInfluence - 0.5) / blendDistance + 0.5));
                uvYInfluence = max(0, min(1, (uvYInfluence - 0.5) / blendDistance + 0.5));
				
				float bottomLeftFogAmount = GetBiomeFogAmount(leftIndex, bottomIndex);
				float bottomRightFogAmount = GetBiomeFogAmount(rightIndex, bottomIndex);
				float topLeftFogAmount = GetBiomeFogAmount(leftIndex, topIndex);
				float topRightFogAmount = GetBiomeFogAmount(rightIndex, topIndex);
				
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