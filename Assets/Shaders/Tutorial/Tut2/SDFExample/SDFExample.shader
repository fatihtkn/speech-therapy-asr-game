Shader "Unlit/SDFExample"
{
    Properties{
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _Health ("Health", Range(0,1)) = 1
        _BorderSize ("Border Size", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
       

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
             
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Health;
            float _BorderSize;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
               
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float2 coords= i.uv;
                coords.x*=8;
                float2 lineSegment= float2(clamp(coords.x,0.5,7.5),0.5);
                float sdf = distance(coords,lineSegment )*2-1;
               
             
                clip(-sdf);
                
                float borderSdf= sdf + _BorderSize;


                float pd= fwidth(borderSdf);// screen space partial derivative. anti aliasing
                // pd is the distance in screen space between two pixels.
                // fwidth is a built-in function that calculates the screen space partial derivative.
                // It is used to determine how much the value changes across pixels, which helps in anti-aliasing.

                float borderMask= 1-saturate(borderSdf/pd);
                
              
                //Bar adjustments
                float healthbarMask= _Health>i.uv.x;
                float3 healthBarColor = tex2D(_MainTex, float2(_Health, i.uv.y)); 

                  
               
                float flash= cos(_Time.y*4)*0.4+1;
                

                if(_Health < 0.2) 
                {
                    
                    healthBarColor *= flash; 
                }
                
                return float4((healthBarColor*healthbarMask*borderMask), 1);
                //return float4(distance.xxx,0);
            }
            ENDCG
        }
    }
}
