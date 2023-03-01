Shader "Custom/Healthbar"
{
    Properties
    {
        _Health ("Health Percentage", Range(0,1)) = 0.5
        _BorderSize ("Border Size", Range(0,1)) = 0.3
        _FullColor ("Full Color", Color) = (1,1,1,1)
        _EmptyColor ("Empty Color", Color) = (0,0,0,0)
        _FullColorEdge ("Full Color Edge", Range(0,1)) = 0.7
        _EmptyColorEdge ("Empty Color Edge", Range(0,1)) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
 
        Pass
        {
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Health;
            float _BorderSize;
            float4 _FullColor;
            float4 _EmptyColor;
            float _FullColorEdge;
            float _EmptyColorEdge;

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

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // когда значение будет равно а, выдаем t=0, когда равно b, выдаем t=1
            float InvLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }

            float InvLerpClamped(float a, float b, float v)
            {
                return saturate((v-a)/(b-a));
            }

            float4 Remap(float iMin, float iMax, float4 oMin, float4 oMax, float value)
            {
                float t = InvLerpClamped(iMin,iMax,value);
                return lerp(oMin,oMax,t);
            }
            
            float4 frag (Interpolators i) : SV_Target
            {
                // любая точка в пространстве бара
                float2 coords = i.uv;
                coords.x *= 4;

                // ближайшая точка на сегменте линии, которая помещается внутри хелсбара
                float2 pointOnTheLine = float2(clamp(coords.x,0.5,3.5), 0.5);

                // теперь ищем расстояние между ними, если оно больше порогового, маска будет нулевая, иначе единичная.
                float sdf = distance(coords,pointOnTheLine) * 2 - 1;

                clip(-sdf);

                float borderSdf = sdf + _BorderSize;

                float screenSpacePartialDerivative = fwidth(borderSdf);

                float borderMask = saturate(borderSdf/screenSpacePartialDerivative);
                
                float mask = i.uv.x < _Health;

                float4 color = Remap(_EmptyColorEdge, _FullColorEdge, _EmptyColor, _FullColor, _Health);

                float4 borderColor = float4(0,0,0,1);
                
                color = lerp(color, borderColor, borderMask);
                
                // тут маска имеет всего два значения - 0 и 1, поэтому лерп происходит мгновенно, и применяет цвет
                // либо бгшки, либо бара, в зависимости от T, который является маской, которая является булевым
                // условием, проверкой
                return color * mask;
            }
            
            ENDCG
        }
    }
}
