Shader "Custom/DebrisShader" {
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "gray" {}
        _Displacement ("Displacement Strength", Range(0, 1)) = 0.2
        _Color ("Tint Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _Displacement;
            float4 _Color;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // Sample noise texture using tex2Dlod (which works in vertex shaders)
                float4 noiseColor = tex2Dlod(_NoiseTex, float4(v.uv, 0, 0));
                float displacement = (noiseColor.r - 0.5) * _Displacement;

                // Apply displacement in object space
                v.vertex.xyz += v.vertex.xyz * displacement;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // Multiply the sampled texture color by the tint color
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
