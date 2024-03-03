Shader "Custom/LineShader2"
{
     Properties
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
                
                float cy = abs(2 * (i.uv.y - 0.5));
                float cx = (2 * (i.uv.x - 0.5)) + 1;
                cx /= 2;

                float ccx = abs(2 * (i.uv.x * 0.75 - 0.375)) * 2 - 1;
                //float ola = col.a;

                col.r = 1;
                col.g = 1;
                col.b = 1;
                col.a = 0;

                float lcy = cy - 0.05;
                lcy /= 9.0;
                lcy *= 10.0;
                
                
                if (cy < 0.05)
                {
                col.a = 1;
                }
                else if(cy > 0.95)
                {
                col.a = 0;
                }
                else
                {
                    float la = cos(0.5*3.1415+lcy*1.7)+1;//cos(0.5?+?)+1
                    col.a= la <= 0 ? 0 : la;
                }

                col *= _Color;
                col.a *= 0.3;
                return col;
            }
        ENDHLSL

        BindChannels {
            Bind "vertex", vertex
            Bind "color", color 
        }
        
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
