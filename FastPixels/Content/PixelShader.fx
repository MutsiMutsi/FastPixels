#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

static const float4 g_ColorPalette[56] =
{
    // Reds and Warm Tones (0-31)
    float4(1.0, 0.0, 0.0, 1.0), // Bright Red
    float4(0.8, 0.0, 0.0, 1.0), // Dark Red
    float4(1.0, 0.2, 0.2, 1.0), // Soft Red
    float4(1.0, 0.5, 0.0, 1.0), // Orange
    float4(1.0, 0.3, 0.0, 1.0), // Deep Orange
    float4(0.8, 0.2, 0.2, 1.0), // Brick Red
    float4(1.0, 0.4, 0.4, 1.0), // Coral
    
    // Yellows and Oranges (32-63)
    float4(1.0, 1.0, 0.0, 1.0), // Bright Yellow
    float4(1.0, 0.8, 0.0, 1.0), // Gold
    float4(1.0, 0.9, 0.2, 1.0), // Canary Yellow
    float4(1.0, 0.6, 0.0, 1.0), // Dark Yellow
    float4(0.9, 0.7, 0.2, 1.0), // Mustard
    float4(1.0, 0.7, 0.3, 1.0), // Light Orange
    float4(1.0, 0.5, 0.2, 1.0), // Burnt Orange
    
    // Greens (64-95)
    float4(0.0, 1.0, 0.0, 1.0), // Bright Green
    float4(0.0, 0.8, 0.0, 1.0), // Dark Green
    float4(0.2, 1.0, 0.2, 1.0), // Lime Green
    float4(0.0, 0.5, 0.0, 1.0), // Forest Green
    float4(0.5, 1.0, 0.5, 1.0), // Mint Green
    float4(0.0, 0.7, 0.3, 1.0), // Emerald Green
    float4(0.2, 0.8, 0.2, 1.0), // Olive Green
    
    // Blues (96-127)
    float4(0.0, 0.0, 1.0, 1.0), // Bright Blue
    float4(0.0, 0.0, 0.8, 1.0), // Navy Blue
    float4(0.2, 0.2, 1.0, 1.0), // Royal Blue
    float4(0.0, 0.5, 1.0, 1.0), // Sky Blue
    float4(0.5, 0.5, 1.0, 1.0), // Periwinkle
    float4(0.0, 0.7, 1.0, 1.0), // Bright Cyan
    float4(0.2, 0.8, 1.0, 1.0), // Light Blue
    
    // Purples and Magentas (128-159)
    float4(1.0, 0.0, 1.0, 1.0), // Magenta
    float4(0.8, 0.0, 0.8, 1.0), // Dark Magenta
    float4(0.6, 0.2, 0.8, 1.0), // Purple
    float4(0.5, 0.0, 0.5, 1.0), // Deep Purple
    float4(0.7, 0.3, 1.0, 1.0), // Lavender
    float4(0.6, 0.4, 0.8, 1.0), // Violet
    float4(0.9, 0.5, 1.0, 1.0), // Orchid
    
    // Neutrals and Grays (160-191)
    float4(1.0, 1.0, 1.0, 1.0), // White
    float4(0.9, 0.9, 0.9, 1.0), // Light Gray
    float4(0.5, 0.5, 0.5, 1.0), // Medium Gray
    float4(0.3, 0.3, 0.3, 1.0), // Dark Gray
    float4(0.0, 0.0, 0.0, 1.0), // Black
    float4(0.7, 0.7, 0.7, 1.0), // Silver
    float4(0.4, 0.4, 0.4, 1.0), // Slate Gray
    
    // Pastel and Soft Tones (192-223)
    float4(1.0, 0.7, 0.7, 1.0), // Pastel Pink
    float4(0.7, 1.0, 0.7, 1.0), // Pastel Green
    float4(0.7, 0.7, 1.0, 1.0), // Pastel Blue
    float4(1.0, 1.0, 0.7, 1.0), // Pastel Yellow
    float4(0.9, 0.7, 1.0, 1.0), // Pastel Lavender
    float4(0.7, 1.0, 1.0, 1.0), // Pastel Cyan
    float4(1.0, 0.8, 0.6, 1.0), // Peach
    
    // Metallic and Special Tones (224-255)
    float4(0.7, 0.7, 0.0, 1.0), // Bronze
    float4(0.8, 0.6, 0.2, 1.0), // Gold
    float4(0.6, 0.6, 0.6, 1.0), // Steel
    float4(0.9, 0.9, 0.8, 1.0), // Cream
    float4(0.5, 0.5, 0.0, 1.0), // Olive
    float4(0.3, 0.3, 0.5, 1.0), // Indigo
    float4(0.2, 0.2, 0.2, 1.0) // Charcoal
};


struct VertexShaderInput
{
    uint Packed : TEXCOORD0; // Ensure semantic is correct
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;
    
    // Unpack the components
    uint ix = (input.Packed >> 20) & 0xFFF; // Extract 12 bits (bits 20-31)
    uint iy = (input.Packed >> 8) & 0xFFF; // Extract 12 bits (bits 8-19)
    uint ic = input.Packed & 0xFF; // Extract 8 bits (bits 0-7)
    
    // Provide a default implementation
    output.Position = float4(
         ix / (3440.0 / 2.0) - 1.0,
        1.0 - iy / (1440.0 / 2.0),
        0.0,
        1.0
    );
    
    output.Color = g_ColorPalette[ic];

    return output;
}

float4 MainPS(VertexShaderOutput input) : SV_TARGET
{
    return input.Color;
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL
            MainVS();
        PixelShader = compile PS_SHADERMODEL
            MainPS();
    }
};