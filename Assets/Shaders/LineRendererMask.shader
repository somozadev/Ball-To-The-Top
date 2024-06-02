Shader "Custom/LineRendererMask"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {"Queue"="Overlay"}
        Pass
        {
            // Stencil setup
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
            // Do not render this pass, only write to stencil buffer
            ColorMask 0
        }
        Pass
        {
            // Render normally (if needed)
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture [_MainTex] { combine primary }
        }
    }
    FallBack "Diffuse"
}
