Shader "Custom/Wave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"{}
        _Ctor("Ctor", float) = 84
        _Rate("Rate",float) = 0.5625
        _timeCtor("timector",float)=60
        _max_dis("maxdis",Range(0,1))=0.5
    }
    SubShader
    {   
        Tags { "RenderType"="Opaque" }
        LOD 100
        
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
             float fogCoord : TEXCOORD1;
             float4 vertex : POSITION;
         };

         sampler2D _MainTex;
         float4 _MainTex_ST;
         float4 _ArrayParams[100];
         float _Ctor; 
         float _timeCtor;
         float _max_dis;
         float _Rate;
         v2f vert (appdata v)
         {
             v2f o;
             o.vertex = TransformObjectToHClip(v.vertex);
             o.uv = TRANSFORM_TEX(v.uv, _MainTex);
             o.fogCoord = ComputeFogFactor(o.vertex.z);
             //UNITY_TRANSFER_FOG(o,o.vertex);
             return o;
         }

         float4 frag (v2f i) : SV_Target
         {
             float2 uv =float2(0,0);
             [unroll]
             for (int j = 0; j < 100; j++)
             {
                 float2 dv = float2(_ArrayParams[j].x,_ArrayParams[j].y) - i.uv;
                 
                 const float dis = sqrt(dv.x * dv.x   + dv.y * dv.y* _Rate* _Rate);
                 
                 if(max(0,_max_dis-dis)*step(dis,_ArrayParams[j].z)*step(_ArrayParams[j].w,dis) == 0)continue;
                 
                 const float sinFactor = sin(dis* _Ctor +_Time.y *_timeCtor ) ;
                 float2 dv1 = normalize(dv);
                 //dv1:Towards
                 float rate_vec = (_max_dis-dis) / _max_dis;
                 float rate_vec2 = abs(0.5 - rate_vec)/0.5;
                 const float2 offset = dv1  * sinFactor *rate_vec2*max(0,_max_dis-dis)*step(dis,_ArrayParams[j].z)*step(_ArrayParams[j].w,dis);

                 uv += offset;
             }
             uv=i.uv+uv/18;
             return tex2D(_MainTex, uv);
             
         }   
         ENDHLSL
         Pass
         {
             HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
             ENDHLSL
         }
    }   
}
