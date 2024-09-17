Shader "Custom/DepthDarkening"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _DarkestDepth ("Darkest Depth", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
            };

            float4 _BaseColor;
            float _DarkestDepth;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.depth = o.pos.z / o.pos.w; // Normalize depth
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate the darkening based on the depth and the _DarkestDepth parameter
                float depthDarkening = saturate(1.0 - i.depth / _DarkestDepth);
                return _BaseColor * depthDarkening;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}