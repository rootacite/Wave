Shader "Custom/CircleMask"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProject" = "True" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"}
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #define EMERGENCE 0.03

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

        sampler2D _MainTex;
        float _Front = 0.5;
        float _Back = 0.4;

        float distance(float2 x,float2 y)
        {
            return sqrt((x.x-y.x) * (x.x-y.x) + (x.y-y.y) * (x.y-y.y));
        }
        
        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = TransformObjectToHClip(v.vertex);
            o.uv = v.uv;
            return o;
        }
        
        float4 frag(v2f i) : SV_TARGET
        {
            float4 col = tex2D(_MainTex, i.uv);
            col.a*=0.65;

            const float d = distance(i.uv,float2(0.5,0.5)) * 2;

            if(d < _Back || d > _Front)
                return float4(0,0,0,0);
            
            if(d > _Back + EMERGENCE && d < _Front - EMERGENCE)
            {
                
            }
            else if(d - _Back < EMERGENCE)
            {
                col.a *= smoothstep(0,1,(d - _Back )/EMERGENCE);
                
            }else if(_Front - d < EMERGENCE)
            {
                col.a *=  smoothstep(0,1,(_Front - d )/EMERGENCE);;
                
            }
            if(_Front - _Back > 0.03 + EMERGENCE * 2)
                return col;

            const float rate = (_Front - _Back) / (0.03 + EMERGENCE * 2);
            col.a *= max(0,rate);
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
    FallBack "Diffuse"
}
