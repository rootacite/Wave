Shader "Unlit/AvatarMaskShader"
{ Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"}
        HLSLINCLUDE
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        

          struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);

                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _Color;

            float4 frag(v2f i) : SV_TARGET
            {

                float4 col = tex2D(_MainTex, i.uv);
                float lx = (i.uv.x - 0.5) * 2.0;
                float ly = (i.uv.y - 0.5) * 2.0;
                float d = sqrt(lx * lx + ly * ly);
                
                if(d >= 1) col.a = 0;
                else if(d <= 0.975) col.a = 1;
                else
                {
                    col.a = ((1 - d) * 40);
                }
                return col;
            }
        ENDHLSL
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL

        }
    }
}
