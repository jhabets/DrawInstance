// derived from examples at https://catlikecoding.com/unity/tutorials/

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<float4> _Colors;
	StructuredBuffer<float4x4> _Matrices;
#endif

float4 GetColor()
{
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		return _Colors[unity_InstanceID];
	#else
		return float4(1,1,1,1);
	#endif
}

void UseMatrix()
{
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		unity_ObjectToWorld = _Matrices[unity_InstanceID];
	#endif
}

void ConfigureProcedural()
{
	//used by shadergraph to inject UNITY_PROCEEDURAL_INSTANCING_ENABLED code
	//In CustomFunction Node with type string.  Use the following text string
	//#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural
	//#pragma editor_sync_compilation
	//Out=In
	UseMatrix();
}

void ShaderGraphFunction_float (float3 In, out float3 Out, out float4 Color)
{
	Out = In;
	Color = GetColor();
}

void ShaderGraphFunction_half (half3 In, out half3 Out, out half4 Color)
{
	Out = In;
	Color = GetColor();
}