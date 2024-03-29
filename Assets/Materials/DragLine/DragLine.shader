Shader "Custom/DragLine"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProject" = "True" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"}
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

            float4 frag(v2f i) : SV_TARGET
            {

                float4 col = tex2D(_MainTex, i.uv);
      
                float cy = abs(2 * (i.uv.y - 0.5));
                float cx = (2 * (i.uv.x - 0.5)) + 1;
                cx /= 2;

                 float ccx = abs(2 * (i.uv.x * 0.75 - 0.375)) * 2 - 1;

                col.r = cx;
                col.g = cx;
                col.b = cx;

                if (cy < 0.012)
                {
                 col.r = 1;
                col.g = 1;
                col.b = 1;
                col.a = 1;
                }
                else
                {
                if (cy > ccx)
                col.a = cy;
                else
                col.a = ccx;

                col.a /= 2;
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
