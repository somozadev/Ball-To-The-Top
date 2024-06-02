Shader "Custom/WaterFill"
{
    Properties
    {
        _FillSpeed ("Fill Speed", Range(0, 1)) = 0.1
        _pointsAmount ("Points Amount", Range(0, 400)) = 0
        _WaterPoints ("Water Points", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float _FillSpeed;
            uniform int _pointsAmount;
            uniform float4 _WaterPoints[400];

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.pos.xy / _ScreenParams.xy;

                float fillAmount = 0;

                // Iteramos sobre los puntos de agua
                for (int j = 0; j < _pointsAmount; j++) // Cambia el valor 10 al tamaño máximo del arreglo
                {
                    float2 waterPoint = _WaterPoints[j].xy;

                    if (i.pos.y <= waterPoint.y) // Por debajo o en el punto de agua actual
                    {
                        float fillStartY = waterPoint.y - 400; // Límite inferior de llenado
                        float maxY = waterPoint.y;
                        fillAmount += saturate((fillStartY - i.pos.y) / (fillStartY - maxY)); // Sumamos el llenado
                    }
                }

                // Normalizamos el llenado
                fillAmount = clamp(fillAmount, 0, 1);

                return half4(fillAmount, fillAmount, fillAmount, 1); // Devolvemos el color de llenado con alfa completo
            }
            ENDCG
        }
    }
}