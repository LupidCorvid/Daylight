// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Contrast"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert2
            #pragma fragment SpriteFrag2
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            struct v2f2
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 grabPos  : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };


            v2f2 SpriteVert2(appdata_t IN)
            {
                v2f2 OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                OUT.grabPos = ComputeGrabScreenPos(OUT.vertex);

                return OUT;
            }

            sampler2D _CameraSortingLayerTexture;
            
            fixed4 SpriteFrag2(v2f2 IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;

                float4 bgcolor = tex2D(_CameraSortingLayerTexture, IN.texcoord );
                if (bgcolor.x <= 23./255. && bgcolor.y <= 23./255. && bgcolor.z <= 23./255. && bgcolor.w == 1)
                {
                    c.x = 255.0/255.0 - 11*bgcolor.x;
                    // no clue why g/b need to be lower - normal hex code would have been FFDD30 = 255,221,48
                    c.y = 183.5/255.0 - 8*bgcolor.x;
                    c.z = 7.5/255.0 - 4*bgcolor.x;
                }
                
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
