Shader "SimplestInstancedShaderWithTransparency_NoShadow"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2D) = "white" {}  // Declare _MainTex
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            // Enable transparency and blend mode
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            ColorMask RGB
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            // Declare the texture
            sampler2D _MainTex;

            // Declare per-instance texture transform (_MainTex_ST)
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST) // Declare texture scale and offset
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;  // Input UVs for texture
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;  // Store the modified UV here
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
                o.uv = v.uv * UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex_ST).xy + UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex_ST).zw; // Apply scale and offset to UVs
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                fixed4 col = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                fixed3 worldNormal = normalize(i.normal);
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float diff = max(0, dot(worldNormal, lightDir));
                fixed4 c = col * diff;
                c.rgb *= _LightColor0.rgb;

                // Sample the texture using the transformed UVs
                c.rgb *= tex2D(_MainTex, i.uv).rgb;

                // Output the color and use alpha for transparency
                c.a = tex2D(_MainTex, i.uv).a; // Set alpha from texture
                return c;
            }
            ENDCG
        }

        // No shadow caster pass since shadows are removed
    }

    Fallback "Diffuse"
}
