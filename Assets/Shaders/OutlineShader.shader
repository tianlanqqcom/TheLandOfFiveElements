Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth ("Outline Width", Range (0.0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 texcol = tex2D(_MainTex, i.uv);
                return texcol;
            }
            ENDCG
        }

        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Back

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;
            uniform float4x4 _ObjectToWorld;

            v2f vert(appdata_t v)
            {
                float3 worldNormal = normalize(mul((float3x3)_ObjectToWorld, v.normal));
                float3 outline = v.vertex.xyz + worldNormal * _OutlineWidth;
                v2f o;
                o.pos = UnityObjectToClipPos(float4(outline, 1.0));
                o.color = _OutlineColor;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
