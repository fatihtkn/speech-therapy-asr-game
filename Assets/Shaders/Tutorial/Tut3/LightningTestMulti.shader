Shader "Unlit/LightningTestMulti"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gloss ("Gloss", Range(0,1)) = 1
        _Color ("Color",Color) = (1,1,1,1)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        
        Pass //Base Pass
        {
            //Tags { "LightMode" = "ForwardBase" } // This tag does not work in URP 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "FGLighting.cginc"
            ENDCG
        }
            //we have to add tags for to tell the engine which one is the base pass and which one is the add pass
        Pass //Add Pass 
        {
           
            Tags { "LightMode" = "ForwardAdd" } // This tag does not work in URP
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "FGLighting.cginc"
            ENDCG
        }
    }
}
