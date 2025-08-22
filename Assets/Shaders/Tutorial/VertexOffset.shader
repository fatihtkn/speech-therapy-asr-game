    Shader "Unlit/VertexOffset"
{
    Properties
    {
        _ColorA ("Color A", Color )= (1,1,1,1)
         _ColorB ("Color B", Color )= (1,1,1,1)

        _Scale  ("UV Scale", Float)= 1
        _Offset ("UV Offset", Float)= 0

        _ColorStart ("Color Start", Range(0,1))=0
        _ColorEnd ("Color End", Range(0,1))=1 
        _WaveAmp("Wave Amplitude", Range(0,0.2))=0.1


    }
    SubShader
    {
        //subshader tags 
        Tags { "RenderType"="Opaque"
         
         }

        Pass
        {   
            
          
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            #define TAU 6.28318530718


            float4 _ColorA;
            float4 _ColorB;

            float _Scale;
            float _Offset;

            float _ColorStart;
            float _ColorEnd;
            float _WaveAmp;

            struct MeshData
            {
                float4 vertex : POSITION;
          
                float3 normals : NORMAL;
                float4 uv0 : TEXCOORD0;// uv0 diffuse/normal map textures
                float4 uv1 : TEXCOORD1;// uv1 coordinates is lightmap coordinates 
            };

            //data passed from the vertex shader to the fragment shader 
            //this will interpolate/blend across triangle 
            struct Interpolators
            {
                float2 uv : TEXCOORD1; // interpolated uv coordinates
                float4 vertex : SV_POSITION;// clip space position 
                float3 normal : TEXCOORD0;
            };

            float GetWave(float2 uv){
                float2 uvsCentered=uv*2-1;
                float4 radialDistance= length(uvsCentered);
                float wave= cos((radialDistance-_Time.y*0.1)*TAU*5)*0.5+0.5; 
                wave*=1-radialDistance;// fade out effect;
                return wave;
            }

            Interpolators vert (MeshData v)
            {
                Interpolators o;

                //float wave= cos((v.uv0.x-_Time.y*0.1)*TAU*5); 
                //v.vertex.x = wave * _WaveAmp;
                
                v.vertex.x= GetWave(v.uv0) * _WaveAmp;

                o.vertex = UnityObjectToClipPos(v.vertex);// local space to clip space
                o.normal= UnityObjectToWorldNormal(v.normals); 
                o.uv= v.uv0; //(v.uv0+_Offset)*_Scale;// pasthrough the uv0 coordinates
                return o;

            }

            float InverseLerp(float a, float b, float value)
            {
                return (value - a) / (b - a);
            }

            

            float4 frag (Interpolators i) : SV_Target
            {
                
               return GetWave(i.uv) ;
            }
            ENDCG
        }
    }
}
