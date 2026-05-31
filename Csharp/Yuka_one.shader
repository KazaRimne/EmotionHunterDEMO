Shader "Custom/Yuka_one_XFlip"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CellSize ("Cell Size (World Units)", Float) = 1.0
        [HideInInspector] _Seed ("Seed", Float) = 12.34
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Background" }
        Pass
        {   ZWrite Off 
            ZTest LEqual
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 worldUV : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _CellSize;
            float _Seed;

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233) + _Seed)) * 43758.5453);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.worldUV = worldPos.xy / _CellSize;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.worldUV;
                float2 grid = floor(uv);
                float2 fuv = frac(uv);

                // --- 左右鏡像邏輯 (50% 機率) ---
                // 使用 grid.yx 來確保翻轉的隨機值與旋轉的隨機值不完全同步
                if (hash(grid.yx + 0.123) > 0.5)
                {
                    fuv.x = 1.0 - fuv.x;
                }

                // --- 隨機旋轉邏輯 (0, 90, 180, 270 度) ---
                float r = floor(hash(grid) * 4.0);
                float2 rotUV;
                if (r < 1.0)      rotUV = fuv;
                else if (r < 2.0) rotUV = float2(1.0 - fuv.y, fuv.x);
                else if (r < 3.0) rotUV = 1.0 - fuv;
                else              rotUV = float2(fuv.y, 1.0 - fuv.x);

                // --- 解決黑線接縫的關鍵 ---
                float2 dx = ddx(input.worldUV);
                float2 dy = ddy(input.worldUV);

                return tex2Dgrad(_MainTex, rotUV, dx, dy);
            }
            ENDHLSL
        }
    }
}