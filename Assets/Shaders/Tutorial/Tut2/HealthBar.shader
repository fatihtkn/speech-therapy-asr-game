Shader "Unlit/HealthBar"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _Health ("Health", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
       

        Pass
        {

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha // Alpha blending


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
          

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float _Health;

           Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv=v.uv;
                o.normal=v.normal;
                return o;
            }


            float InverveLerp(float a, float b, float v){
                return (v-a)/(b-a);
            }

            float4 frag (Interpolators i) : SV_Target //fragment s)hader run for all the pixels
            {
            
               
        
                float healthbarMask= _Health>i.uv.x;
                float3 healthBarColor = tex2D(_MainTex, float2(_Health, i.uv.y)); 

                  
                float flashFrequency = 5; // Adjust this value to change the speed of the flashing effect
                float flashAmplitude = 0.2; // Adjust this value to change the intensity of the flashing effect  
                float flash= cos(_Time.y*flashFrequency)*flashAmplitude+1;
                

                if(_Health < 0.2) // Flash when health is low
                {
                    
                    healthBarColor *= flash; // Add a flashing effect
                }
                
                return float4((healthBarColor*healthbarMask), 1);
            }
            ENDCG
        }
    }
}


    // Notes: Simple health bar 
  //float tHealthColor=saturate(InverveLerp(0.2,0.8, _Health));// clamp the value between 0 and 1. if the value is less than 0.2, it will be 0, if it is more than 0.8, it will be 1 
                //float3 barColor=lerp(float3(1,0,0), float3(0,1,0), tHealthColor); 
                //float healthbarMask= _Health>floor(i.uv.x*8)/8;// for 8 segments
                //clip(healthbarMask - 0.5); // clip the pixels that are not part of the health bar;
                //float3 backgroundColor = float3(0, 0, 0);
                //float3 outColor = lerp(backgroundColor, barColor, healthbarMask);