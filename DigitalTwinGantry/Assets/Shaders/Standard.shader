Shader "DigitalTwin/Standard"
{
    Properties
    {
        [Header(Albedo Settings)]
        [MainTexture] _MainTex ("Main Texture", 2D) = "white" {}
        [MainColor] _Color ("Main Color", Color) = (1, 1, 1, 1)
        _Transparency ("Transparency", Range(0, 1)) = 1

        [Space(10)]

        [Header(Alpha Cutoff)]
        [Toggle(_APPLY_CUTOFF)] _ApplyCutoff ("Apply Alpha Cutoff", Float) = 0
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0

        [Space(10)]

        [Header(Occlusion Settings)]
        [Toggle(_APPLY_OCCLUSION)] _ApplyOcclusion ("Apply Occlusion Texture", Float) = 0
        _OcclusionDampener ("Dampen", Range(0, 1)) = 0.5
        [NoScaleOffset] _OcclusionTex ("Occlusion Texture", 2D) = "white" {}

        [Space(10)]

        [Header(Emission Settings)]
        [Toggle(_APPLY_EMISSION)] _ApplyEmission ("Apply Emission", Float) = 0
        _EmissionFactor ("Emission Factor", Range(0, 100)) = 1
        [Toggle] _UseEmissionColor ("Use emission with separate color", Float) = 0
        _EmissionColor ("Emission Color", Color) = (1, 1, 1)
        [Toggle(_APPLY_EMISSION_MAP)] _ApplyEmissionMap ("Apply Emission Map", Float) = 0
        [NoScaleOffset] _EmissionMap ("Emission Map", 2D) = "white" {}

        [Space(10)]

        [Header(Detail Texture Settings)]
        [Toggle(_APPLY_DETAIL)] _ApplyDetail ("Apply Detail Texture", Float) = 0
        _DetailTex ("Detail Texture", 2D) = "gray" {}

        [Space(10)]

        [Header(Noise Settings)]
        [Toggle(_APPLY_NOISE)] _ApplyNoise ("Apply Noise", Float) = 0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _MoveNoiseX ("Move Texture X-axis", Float) = 0
        _MoveNoiseY ("Move Texture Y-axis", Float) = 0
        _NoiseColor ("Noise Color", Color) = (0.5, 0.5, 0.5, 1)

        [Space(10)]     

        [Header(Baked Lighting)]
        _BakedLightMultiplier ("Light Multiplier", Float) = 1

        [Space(10)]

        [Header(Realtime Lighting Settings)]
        [Toggle(_APPLY_LIGHTING)] _ApplyLighting ("Apply Realtime Directional Light", Float) = 0
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.6
        _AmbientLighting ("Ambient Lighting", Range(0, 1)) = 0.2
        [Toggle(_APPLY_SPECULAR)] _ApplySpucular ("Apply Specular Highlights", Float) = 1
        _SpecularColor ("Specular Color", Color) = (0.5, 0.5, 0.5)
        _Smoothness ("Smoothness", Range(0.01, 1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Cull Back

        // --------------------------------------------------
        // Main Pass
        // --------------------------------------------------
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile __ UNITY_STEREO_MULTIVIEW_ENABLED

            // --------------------------------------------------
            // Local shader features
            // --------------------------------------------------
                #pragma shader_feature_local _APPLY_DETAIL
                #pragma shader_feature_local _APPLY_LIGHTING
                #pragma shader_feature_local _APPLY_SPECULAR
                #pragma shader_feature_local _APPLY_OCCLUSION
                #pragma shader_feature_local _APPLY_NOISE
                #pragma shader_feature_local _APPLY_EMISSION
                #pragma shader_feature_local _APPLY_EMISSION_MAP
                #pragma shader_feature_local _APPLY_CUTOFF

            // --------------------------------------------------
            // Imports
            // --------------------------------------------------
                #include "UnityCG.cginc"
                #include "AutoLight.cginc"
                #include "UnityLightingCommon.cginc"
                #include "UnityStandardBRDF.cginc"

            // --------------------------------------------------
            // Vertex inputs
            // --------------------------------------------------
                struct VertexData
                {
                    half4 vertex : POSITION;
                    fixed2 uv : TEXCOORD0;

                    #ifdef LIGHTMAP_ON
                        fixed2 uv1 : TEXCOORD1;
                    #endif
                    
                    #ifdef _APPLY_LIGHTING
                        fixed3 normal : NORMAL;
                    #endif

                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

            // --------------------------------------------------
            // Properties
            // --------------------------------------------------
                sampler2D _MainTex;
                fixed4 _MainTex_ST;

                #ifdef _APPLY_DETAIL
                    sampler2D _DetailTex;
                    fixed4 _DetailTex_ST;
                #endif

                #ifdef LIGHTMAP_ON
                    fixed _BakedLightMultiplier;
                #endif

                #ifdef _APPLY_LIGHTING
                    fixed _ShadowIntensity;
                    fixed _AmbientLighting;
                    fixed _Smoothness;
                    fixed4 _SpecularColor;
                #endif

                #ifdef _APPLY_OCCLUSION
                    sampler2D _OcclusionTex;
                    fixed _OcclusionDampener;
                #endif

                #ifdef _APPLY_NOISE
                    sampler2D _NoiseTex;
                    fixed4 _NoiseTex_ST;

                    fixed _MoveNoiseX;
                    fixed _MoveNoiseY;

                    fixed4 _NoiseColor;
                #endif

                fixed _AlphaCutoff;

                fixed _UseEmissionColor;
                sampler2D _EmissionMap;

            // --------------------------------------------------
            // Per-instanced properties
            // --------------------------------------------------
                #ifndef UNITY_INSTANCING_ENABLED
                    fixed4 _Color;
                    
                    fixed _Transparency;

                    fixed4 _EmissionColor;
                    fixed _EmissionFactor;
                #else
                    UNITY_INSTANCING_BUFFER_START(Props)
                        UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
                        UNITY_DEFINE_INSTANCED_PROP(fixed, _Transparency)
                        UNITY_DEFINE_INSTANCED_PROP(fixed4, _EmissionColor)
                        UNITY_DEFINE_INSTANCED_PROP(fixed, _EmissionFactor)
                    UNITY_INSTANCING_BUFFER_END(Props)

                    #define _Color          UNITY_ACCESS_INSTANCED_PROP(Props, _Color)
                    #define _Transparency   UNITY_ACCESS_INSTANCED_PROP(Props, _Transparency)
                    #define _EmissionColor   UNITY_ACCESS_INSTANCED_PROP(Props, _EmissionColor)
                    #define _EmissionFactor   UNITY_ACCESS_INSTANCED_PROP(Props, _EmissionFactor)
                #endif

            // --------------------------------------------------
            // Fragment inputs
            // --------------------------------------------------
                struct FragmentData
                {
                    half4 vertex : SV_POSITION;
                    fixed2 uv : TEXCOORD0;

                    #ifdef _APPLY_DETAIL
                        fixed2 uvDetail : TEXCOORD1;
                    #endif

                    #ifdef LIGHTMAP_ON
                        fixed2 uvLightMap : TEXCOORD2;
                    #endif

                    #ifdef _APPLY_NOISE
                        fixed2 uvNoise : TEXCOORD3;
                    #endif

                    #ifdef _APPLY_LIGHTING
                        half3 worldNormal : TEXCOORD4;
                        fixed4 diffuseLighting : COLOR0;
                        half3 worldPos : TEXCOORD5;
                        SHADOW_COORDS(6)
                    #endif

                    UNITY_FOG_COORDS(7)

                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    UNITY_VERTEX_OUTPUT_STEREO
                };

            // --------------------------------------------------
            // Vertex shader
            // --------------------------------------------------
                FragmentData vert(VertexData v)
                {
                    FragmentData f;

                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, f);
                    UNITY_INITIALIZE_OUTPUT(FragmentData, f);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(f);

                    f.vertex = UnityObjectToClipPos(v.vertex);
                    f.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    #ifdef _APPLY_DETAIL   
                        f.uvDetail = TRANSFORM_TEX(v.uv, _DetailTex);
                    #endif 

                    #ifdef LIGHTMAP_ON
                        f.uvLightMap = v.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
                    #endif

                    #ifdef _APPLY_NOISE
                        fixed2 uvNoise = TRANSFORM_TEX(v.uv, _NoiseTex);
                        f.uvNoise = fixed2(uvNoise.x + _Time.y * _MoveNoiseX, uvNoise.y + _Time.y * _MoveNoiseY);
                    #endif

                    #ifdef _APPLY_LIGHTING
                        f.worldNormal = UnityObjectToWorldNormal(v.normal);
                        fixed lightBrightness = max(0, dot(f.worldNormal, _WorldSpaceLightPos0.xyz));
                        f.diffuseLighting = max(lightBrightness * _LightColor0, _AmbientLighting);
                        f.worldPos = mul(unity_ObjectToWorld, v.vertex);
                        TRANSFER_SHADOW(f)
                    #endif

                    UNITY_TRANSFER_FOG(f, f.vertex);

                    return f;
                }

            // --------------------------------------------------
            // Fragment shader
            // --------------------------------------------------
                fixed4 frag(FragmentData f) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(f)
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(f)

                    fixed4 mainTexture = tex2D(_MainTex, f.uv);
                    fixed4 albedo = fixed4(mainTexture.rgb * _Color, mainTexture.a);

                    #ifdef _APPLY_CUTOFF
                        if (albedo.a < _AlphaCutoff)
                        {
                            return fixed4(0, 0, 0, 0);
                        }
                    #endif

                    #ifdef _APPLY_DETAIL
                        albedo *= tex2D(_DetailTex, f.uvDetail) * unity_ColorSpaceDouble;
                    #endif

                    #ifdef _APPLY_OCCLUSION
                        fixed occlusion = tex2D(_OcclusionTex, f.uv).r;
                        occlusion = min(1, lerp(occlusion, 1, _OcclusionDampener));
                        albedo.rgb -= (1 - occlusion);
                    #endif

                    #ifdef _APPLY_NOISE
                        fixed3 noise = tex2D(_NoiseTex, f.uvNoise).rgb * _NoiseColor.rgb;
                        albedo.rgb += noise;
                    #endif

                    #ifdef _APPLY_LIGHTING
                        #ifdef _APPLY_SPECULAR
                            fixed3 viewDir = normalize(_WorldSpaceCameraPos - f.worldPos);
                            fixed3 halfVector = normalize(_WorldSpaceLightPos0.xyz + viewDir);
                            fixed3 specular = (_SpecularColor.rgb) * _LightColor0.rgb * pow(DotClamped(halfVector, f.worldNormal), _Smoothness * 100);
                            albedo.rgb *= max(_SpecularColor.r, max(_SpecularColor.g, _SpecularColor.b));  

                            f.diffuseLighting.rgb += specular;
                        #endif

                        albedo.rgb *= f.diffuseLighting.rgb * min(SHADOW_ATTENUATION(f) + _ShadowIntensity, 1);
                    #endif

                    #ifdef LIGHTMAP_ON
                        fixed4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, f.uvLightMap);
                        bakedColorTex.rgb = DecodeLightmap(bakedColorTex);
                        albedo.rgb *= bakedColorTex * _BakedLightMultiplier;
                    #endif

                    fixed4 color = albedo * fixed4(1, 1, 1, _Transparency);

                    #ifdef _APPLY_EMISSION
                        fixed3 emission = fixed3(_EmissionFactor, _EmissionFactor, _EmissionFactor);

                        #ifdef _APPLY_EMISSION_MAP
                            fixed4 emissionMap = tex2D(_EmissionMap, f.uv);
                            emission *= emissionMap;
                        #endif

                        if (_UseEmissionColor)
                        {
                            emission *= _EmissionColor;
                            color.rgb += emission;
                        } else
                        {
                            emission = max(1, emission);
                            color.rgb *= emission;
                        }
                    #endif    

                    UNITY_APPLY_FOG(f.fogCoord, color);

                    return color;
                }

            ENDCG
        }
    }

    // Use "Mobile/Diffuse" as a Fallback of this shader for a shadow casting pass
    Fallback "Mobile/Diffuse"
}
