
Shader "TilingTest" {
	
	Properties
	{
		textureArray("Texture Array", 2DArray) = "white" {}
		normalArray("Normal Array", 2DArray) = "white" {}
		normal("Texture", 2D) = "white" {}
		blendTexture("Blend Texture", 2D) = "white" {}
		unrotatedTextureScale("Unrotated Texture Scale", Range(1.0, 100.0)) = 1
		rotatedTextureScale("Rotated Texture Scale", Range(1.0, 100.0)) = 1
		maskScale("Mask Scale", Range(1.0, 100.0)) = 1
		blendDistance("Blend Distance", Range(0.1, 1)) = 0.1
		gridSize("Grid Scale", Range(0.000001, 0.5)) = 0.01
        [MaterialToggle] showGrid("Show Grid", Range(0, 1)) = 0

	}
    SubShader 
    {
        Tags {"RenderType"="Opaque" "Queue"="Geometry" "DisableBatching" = "true"}
		CGINCLUDE
		UNITY_DECLARE_TEX2DARRAY(textureArray);
		UNITY_DECLARE_TEX2DARRAY(normalArray);
		StructuredBuffer<int> biomeMap;
        sampler2D normal;
		uniform int biomeWidth;
		uniform int biomeHeight;

		sampler2D blendTexture;
		float unrotatedTextureScale;
		float rotatedTextureScale;
		float maskScale;
		float blendDistance;
        float gridSize;
        bool showGrid;
		ENDCG

        Pass {
            Tags {"LightMode"="ForwardBase"}
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fwdbase
            #pragma nolightmap nodirlightmap nodynlightmap novertexlight
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"
               
            struct ColorAndNormal {
                float4 color;
                float3 normal;
            };

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
				float3 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                SHADOW_COORDS(1)

                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD2;
                
				float3 T : TEXCOORD3;
				float3 B : TEXCOORD4;
				float3 N : TEXCOORD5;
                float3 diffuse : TEXCOORD6;
            };

            v2f vert( appdata input)
            {
                v2f output;
                output.pos = UnityObjectToClipPos (input.vertex);
                output.uv =  input.uv;
                output.normal = UnityObjectToWorldNormal(input.normal);
                 
                output.worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;

				float3 worldNormal = mul((float3x3)unity_ObjectToWorld, input.normal);
				float3 worldTangent = mul((float3x3)unity_ObjectToWorld, input.tangent);
				
				float3 binormal = cross(input.normal, input.tangent.xyz); // *input.tangent.w;
				float3 worldBinormal = mul((float3x3)unity_ObjectToWorld, binormal);
				output.N = normalize(worldNormal);
				output.T = normalize(worldTangent);
				output.B = normalize(worldBinormal);
                TRANSFER_SHADOW(output)
                output.diffuse = ShadeSH9(half4(worldNormal,1));
                return output;
            } 
            
            float3 BlendNormals(float3 n1, float3 n2, float blendFactor) {
                return normalize(n1 * (1 - blendFactor) + n2 * blendFactor);
            }

            ColorAndNormal GetBiomeColorAndNormal(float2 uvCoords, int index)
            {
                int clampedIndex = clamp(index, 0, biomeWidth * biomeHeight);
				int biomeTextureIndex = biomeMap[clampedIndex];
                
                float3 uvCoords3D = float3(uvCoords / unrotatedTextureScale, biomeTextureIndex);
                float3 uvRotatedCoords3D = float3(-uvCoords.y / rotatedTextureScale, uvCoords.x / rotatedTextureScale, biomeTextureIndex);

                float4 rotatedColor = UNITY_SAMPLE_TEX2DARRAY(textureArray, uvRotatedCoords3D);
                float4 unrotatedColor = UNITY_SAMPLE_TEX2DARRAY(textureArray, uvCoords3D);
                float3 rotatedNormal =  UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(normalArray, uvRotatedCoords3D));
                rotatedNormal.x = - rotatedNormal.x;
                rotatedNormal.y = - rotatedNormal.y;
                float3 unrotatedNormal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(normalArray, uvCoords3D));
                unrotatedNormal.x = - unrotatedNormal.x;
                unrotatedNormal.y = - unrotatedNormal.y;

                float maskAlpha = tex2D(blendTexture, uvCoords / maskScale).a;
                ColorAndNormal result;
                result.color = maskAlpha * rotatedColor + (1 - maskAlpha) * unrotatedColor;
                result.normal = BlendNormals(rotatedNormal, unrotatedNormal, maskAlpha);
                return result;
            }
            
			float fmodNegative(float value, float divider)
			{
				float result = value >= 0 ? fmod(value, divider) : divider - fmod(-value, divider);
				return result;
			}

            fixed4 frag (v2f input) : SV_TARGET
            {      				
                int leftIndex = input.uv.x * biomeWidth - 0.5;
				int bottomIndex = input.uv.y * biomeHeight - 0.5;
                 
                float toCamera = length(_WorldSpaceCameraPos - input.worldPos.xyz);
                if(showGrid)
                {
				    float horizontalDistanceFromEdge = frac(input.uv.x * biomeWidth);
				    float verticalDistanceFromEdge = frac(input.uv.y * biomeHeight);

                    float adjustedGridSize = toCamera * gridSize;
                    if (horizontalDistanceFromEdge < adjustedGridSize || verticalDistanceFromEdge < adjustedGridSize || 1 - horizontalDistanceFromEdge < adjustedGridSize || 1 - verticalDistanceFromEdge < adjustedGridSize)
                        return float4(0, 0, 0, 1);
                }

				int rightIndex = leftIndex + 1;
				int topIndex = bottomIndex + 1;
                
				float uvXInfluence = fmod(input.uv.x * biomeWidth - 0.5, 1);
				float uvYInfluence = fmod(input.uv.y * biomeHeight - 0.5, 1);

                uvXInfluence = max(0, min(1, (uvXInfluence - 0.5) / blendDistance + 0.5));
                uvYInfluence = max(0, min(1, (uvYInfluence - 0.5) / blendDistance + 0.5));
                
                float2 scaledUV = float2(input.uv.x * biomeWidth, input.uv.y * biomeHeight);
				ColorAndNormal bottomLeftColor = GetBiomeColorAndNormal(scaledUV, leftIndex + bottomIndex * biomeWidth);
				ColorAndNormal bottomRightColor = GetBiomeColorAndNormal(scaledUV, rightIndex + bottomIndex * biomeWidth);
				ColorAndNormal topLeftColor = GetBiomeColorAndNormal(scaledUV, leftIndex + topIndex * biomeWidth);
				ColorAndNormal topRightColor = GetBiomeColorAndNormal(scaledUV, rightIndex + topIndex * biomeWidth);

				float4 textureColor = bottomLeftColor.color * (1 - uvXInfluence) * (1 - uvYInfluence);
				textureColor += bottomRightColor.color * uvXInfluence * (1 - uvYInfluence);
				textureColor += topLeftColor.color * (1 - uvXInfluence) * uvYInfluence;
				textureColor += topRightColor.color * uvXInfluence * uvYInfluence;
                
                float3 topNormal = BlendNormals(topRightColor.normal, topLeftColor.normal, uvXInfluence);
                float3 bottomNormal = BlendNormals(bottomRightColor.normal, bottomLeftColor.normal, uvXInfluence);
                float3 finalNormal = BlendNormals(topNormal, bottomNormal, uvYInfluence);

                float3x3 TBN = float3x3(normalize(input.T), normalize(input.B), normalize(input.N));
				TBN = transpose(TBN);
				float3 worldNormal = mul(TBN, finalNormal);
                
				float worldLightFactor = max(0, dot(worldNormal, _WorldSpaceLightPos0));
                fixed shadow = SHADOW_ATTENUATION(input);
				float4 lightColor = _LightColor0 * worldLightFactor * shadow;
                lightColor.rgb += input.diffuse;
				textureColor = textureColor * lightColor;
				textureColor.a = 1;

                return textureColor;
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