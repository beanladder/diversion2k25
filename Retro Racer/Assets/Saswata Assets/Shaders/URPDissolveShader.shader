Shader "Custom/URPDissolveShader"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map (RGB)", 2D) = "white" {}
        [HDR] _EdgeColor ("Edge Color", Color) = (1, 0.5, 0, 1)
        _EdgeWidth ("Edge Width", Range(0, 1)) = 0.1
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _DissolveValue ("Dissolve Value", Range(0, 1)) = 0
        _DissolveDirection ("Dissolve Direction", Vector) = (0, 1, 0, 0)
        _DissolveSharpness ("Dissolve Sharpness", Range(1, 20)) = 5
        
        // Render states
        [Toggle] _ZWrite("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            ZWrite [_ZWrite]
            Cull [_Cull]
            Blend [_SrcBlend] [_DstBlend]
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float3 posWorld : TEXCOORD1;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 posWorld : TEXCOORD1;
                float2 noiseUV : TEXCOORD2;
            };
            
            TEXTURE2D(_BaseMap);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_BaseMap);
            SAMPLER(sampler_NoiseTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _EdgeColor;
                float _EdgeWidth;
                float _DissolveValue;
                float4 _DissolveDirection;
                float _DissolveSharpness;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.posWorld = TransformObjectToWorld(input.positionOS.xyz);
                output.noiseUV = input.uv;
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                // Sample original texture exactly as is
                half4 originalColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                
                // Early return if dissolve hasn't started
                if (_DissolveValue <= 0)
                    return originalColor;
                
                // Calculate dissolution threshold based on position
                float dissolveHeight = dot(normalize(input.posWorld), _DissolveDirection.xyz);
                dissolveHeight = dissolveHeight * 0.5 + 0.5; // Normalize to 0-1 range
                
                // Sample and adjust noise
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.noiseUV).r;
                float dissolveThreshold = lerp(-0.1, 1.1, _DissolveValue);
                
                // Combine position and noise
                float finalNoise = (noise + dissolveHeight) * 0.5;
                finalNoise = pow(finalNoise, _DissolveSharpness);
                
                // Calculate edge and alpha
                float edge = dissolveThreshold + _EdgeWidth;
                
                if (finalNoise < dissolveThreshold)
                    discard;
                
                half4 finalColor = originalColor;
                
                if (finalNoise < edge)
                {
                    float edgeLerp = (finalNoise - dissolveThreshold) / _EdgeWidth;
                    finalColor.rgb = lerp(_EdgeColor.rgb, originalColor.rgb, edgeLerp);
                    finalColor.a = originalColor.a * edgeLerp;
                }
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}