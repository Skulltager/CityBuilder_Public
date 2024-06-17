
Shader "InstancedWorldResources" {
	
	Properties
	{
		mainTexture("Texture", 2D) = "white" {}
		tintColor("Color", Color) = (1,1,1,1)
	}
    SubShader 
    {
        Tags {"RenderType"="Opaque" "Queue"="Geometry" "DisableBatching" = "true"}
		CGINCLUDE
        sampler2D mainTexture;
        float4 tintColor;
        
		StructuredBuffer<float3> positionBuffer;
		StructuredBuffer<float4> rotationBuffer;
        float scale;

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
               

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                SHADOW_COORDS(0)
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD1;
                float3 diffuse : TEXCOORD2;
                float3 normal : TEXCOORD3;
            };
            
		    float4 qmul(float4 q1, float4 q2)
		    {
			    return float4(
				    q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
				    q1.w * q2.w - dot(q1.xyz, q2.xyz)
			    );
		    }

		    float3 rotate_vector(float3 v, float4 r)
		    {
			    float4 r_c = r * float4(-1, -1, -1, 1);
			    return qmul(r, qmul(float4(v, 0), r_c)).xyz;
		    }

            v2f vert( appdata input, uint instanceID : SV_InstanceID)
            {
                v2f output;
                float3 worldPosition = positionBuffer[instanceID];
                float4 rotation = rotationBuffer[instanceID];

			    float4 position = input.vertex;
			    position.xyz *= scale;
			    position.xyz = rotate_vector(position.xyz, rotation);
			    position.xyz += worldPosition;

				output.pos = UnityObjectToClipPos(position);
                output.uv =  input.uv;

                output.normal = rotate_vector(input.normal, rotation);
                 
                TRANSFER_SHADOW(output)
                output.diffuse = ShadeSH9(half4(output.normal.xyz, 1));
                return output;
            } 

            fixed4 frag (v2f input) : SV_TARGET
            {      				
                float4 textureColor = tex2D(mainTexture, input.uv);
				float worldLightFactor = max(0, dot(input.normal, _WorldSpaceLightPos0));
                fixed shadow = SHADOW_ATTENUATION(input); 
				float4 lightColor = _LightColor0 * worldLightFactor * shadow;
                lightColor.rgb += input.diffuse;
				textureColor = textureColor * tintColor * lightColor;
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

            struct appdata {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
            };
            
            struct v2f {
                V2F_SHADOW_CASTER;
            };          
        
		    float4 qmul(float4 q1, float4 q2)
		    {
			    return float4(
				    q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
				    q1.w * q2.w - dot(q1.xyz, q2.xyz)
			    );
		    }

		    float3 rotate_vector(float3 v, float4 r)
		    {
			    float4 r_c = r * float4(-1, -1, -1, 1);
			    return qmul(r, qmul(float4(v, 0), r_c)).xyz;
		    }

            v2f vert( appdata v, uint instanceID : SV_InstanceID)
            {
                v2f output;
                float3 worldPosition = positionBuffer[instanceID];
                float4 rotation = rotationBuffer[instanceID];

			    v.vertex.xyz *= scale;
			    v.vertex.xyz = rotate_vector(v.vertex.xyz, rotation);
			    v.vertex.xyz += worldPosition;

                TRANSFER_SHADOW_CASTER_NORMALOFFSET(output);
                return output;
            }
        
            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}