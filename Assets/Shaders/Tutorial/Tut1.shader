Shader "Unlit/Tut1"
{
    Properties
    {
      _ColorA ("Color A", Color )= (1,1,1,1)
      _ColorB ("Color B", Color )= (1,1,1,1)

      _Scale  ("UV Scale", Float)= 1
      _Offset ("UV Offset", Float)= 0

      _ColorStart ("Color Start", Range(0,1))=0
        _ColorEnd ("Color End", Range(0,1))=1 
    }
    SubShader
    {
        //subshader tags 
        Tags { "RenderType"="Transparent"// tag to inform the render pipeline of what type this is. Usually used for postprocess reasons. This does not change the sorting. Useful tag for postprocess effects.
        "Queue"="Transparent" }//changes the render order.

        Pass
        {   
            
            //Cull Back // disable backface culling. This is default value for rendering
            //Cull Front // disable frontface culling
            Cull Off // Renders both sides 
            ZWrite Off // disable depth writing
            //ZTest Always // always draws the object, even if it is behind another object
            Blend One One // additive blend mode
           // Blend DstColor Zero // multiply blend mode
            //pass tags 
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


            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);// local space to clip space
                o.normal= v.normals;
                //o.normal= UnityObjectToWorldNormal(v.normals); //mul((float3x3)unity_ObjectToWorld, v.normals); // transform normals from local space to world space
                o.uv= v.uv0; //(v.uv0+_Offset)*_Scale;// pasthrough the uv0 coordinates
                return o;

            }

            float InverseLerp(float a, float b, float value)
            {
                return (value - a) / (b - a);
            }

            float4 frag (Interpolators i) : SV_Target
            {
                //blend between two colors based on the x coordinate of the uv
                //float t = saturate( InverseLerp(_ColorStart, _ColorEnd, i.uv.x));
                
                //float t = frac(i.uv.x*5);
               // float t= abs(frac(i.uv.x*5)*2-1); // triangle wave pattern


                float xOffset= cos(i.uv.x*TAU*8)*0.01; // vertical offset for the wave pattern
                float t= cos((i.uv.y+xOffset-_Time.y*0.2)*TAU*5)*0.5+0.5; // cosine wave pattern)
                 t*=1-i.uv.y;

                float topBottomRemover= abs(i.normal.y)<0.999;
              
                //return t*topBottomRemover;
                float waves = t* topBottomRemover;
                float4 gradient=lerp(_ColorA, _ColorB, i.uv.y);
                return waves;
                //float4 outColor= lerp(_ColorA, _ColorB, t);
                 
                //return outColor;
            }
            ENDCG
        }
    }
}
