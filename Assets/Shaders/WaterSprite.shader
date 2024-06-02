Shader "Custom/WaterSprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent"}
        Pass
        {
            // Stencil setup
            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Keep
            }
            // Render the sprite normally, but respect the stencil buffer
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture [_MainTex] { combine texture * primary }
        }
    }
    FallBack "Diffuse"
}
