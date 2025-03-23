#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VertexShaderInput
{
    int2 Position : POSITION0; // Two 16-bit integers for X and Y
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
	
    // Convert the 16-bit integers to floats for clip space
    float x = (float) input.Position.x;
    float y = (float) input.Position.y;

    // Transform position to clip space
    output.Position = float4(x / (1920 / 2) - 1, y / (1080 / 2) - 1, 0.0f, 1.0f);
    output.Color = input.Color;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
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