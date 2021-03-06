matrix WorldViewProjection;

//Shader largly by Boffin and used and modified with permission!

struct VertexShaderInput
{
	float2 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float2 TextureCoordinates : TEXCOORD0;
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

float2 coordMultiplier;
float2 coordOffset;
float strength;
bool twoWayAlpha;

//custom passes
texture imageTexture;
sampler imageSampler = sampler_state
{
    Texture = (imageTexture);
    AddressU = Wrap;
    AddressV = Wrap;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;

	output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 BasicEffect(VertexShaderOutput input) : COLOR
{
	float4 pixel = (tex2D(imageSampler, coordOffset + input.TextureCoordinates * coordMultiplier) * input.Color)*strength;
	pixel = saturate(pixel);
	return pixel;
}

float4 BasicEffectFaded(VertexShaderOutput input) : COLOR
{

	float4 pixel = (tex2D(imageSampler, coordOffset + input.TextureCoordinates * coordMultiplier) * input.Color)*strength;
	pixel = saturate(pixel);
	return pixel * (sin(input.TextureCoordinates.x * 3.14159265));
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return input.Color * (sin(input.TextureCoordinates.x * 3.14159265)) * strength;
}

float4 MainPSSinShade(VertexShaderOutput input) : COLOR
{
	float base = sin(input.TextureCoordinates.y * 3.14159265);

	return input.Color * (sin(input.TextureCoordinates.x * 3.14159265)*base) * strength;
}


float4 BasicImage(VertexShaderOutput input) : COLOR
{
    float alpha = (1.0 - strength) + tex2D(imageSampler, coordOffset + input.TextureCoordinates * coordMultiplier).r * strength;
	return input.Color * alpha;
}

technique BasicColorDrawing
{
	pass BasicEffectPass
	{
		VertexShader = compile vs_2_0 MainVS();
		PixelShader = compile ps_2_0 BasicEffect();
	}	
		pass FadedBasicEffectPass
	{
		VertexShader = compile vs_2_0 MainVS();
		PixelShader = compile ps_2_0 BasicEffectFaded();
	}
	pass DefaultPass
	{
		VertexShader = compile vs_2_0 MainVS();
		PixelShader = compile ps_2_0 MainPS();
	}
	pass DefaultPassSinShade
	{
		VertexShader = compile vs_2_0 MainVS();
		PixelShader = compile ps_2_0 MainPSSinShade();
	}
	pass BasicImagePass
	{
		VertexShader = compile vs_2_0 MainVS();
		PixelShader = compile ps_2_0 BasicImage();
	}
}