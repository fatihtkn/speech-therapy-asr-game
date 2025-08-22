Shader "Unlit/LightningTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gloss ("Gloss", Range(0,1)) = 1
        _Color ("Color",Color) = (1,1,1,1)

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
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;

            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Gloss;
            float4 _Color;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                //diffuse lighting
                float3 N= normalize(i.normal);
                float3 L = _WorldSpaceLightPos0.xyz;
                float3 lambert= saturate(dot(N,L));
                float3 diffuseLight = lambert * _LightColor0.xyz; // scale by light color and color property


               //specular lighting
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);// surface to camera vector
                float3 R= reflect(-L, N); // reflection vector
                float3 specular = saturate(dot(R, V)); // dot product of reflection vector and view vector
                specular = pow(specular, _Gloss); // apply glossiness
                

                //Blinn-Phong specular
                float3 halfVector = normalize(L + V); 
                float3 specularLight = saturate(dot( halfVector,N))*(lambert>0);//Blinn Phong

                float specularExponent = exp2( _Gloss*11)+1; 
                specularLight = pow(specularLight, specularExponent)*_Gloss; // multiply by _Gloss is more accurate in energy conservation context  // apply glossiness
                specularLight *= _LightColor0.xyz; // scale by light color
           
                return float4(diffuseLight*_Color+specularLight,1); 




            }
            ENDCG
        }
    }
}
