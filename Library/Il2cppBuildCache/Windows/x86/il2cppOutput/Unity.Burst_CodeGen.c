#include "pch-c.h"
#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include "codegen/il2cpp-codegen-metadata.h"





// 0x00000001 System.Void Unity.Burst.BurstCompileAttribute::.ctor()
extern void BurstCompileAttribute__ctor_m75F0EF699FB9E5AD644F26B04ACE674ED2C81628 (void);
// 0x00000002 System.Int64 Unity.Burst.BurstRuntime::GetHashCode64()
// 0x00000003 System.Int64 Unity.Burst.BurstRuntime::HashStringWithFNV1A64(System.String)
extern void BurstRuntime_HashStringWithFNV1A64_m36354952510DAE5551D8576CAEE0CC23BC49864E (void);
// 0x00000004 System.Void Unity.Burst.BurstRuntime::Log(System.Byte*,System.Int32,System.Byte*,System.Int32)
extern void BurstRuntime_Log_mA326750916D8B27060FFE335CFE3BB74F12E8632 (void);
// 0x00000005 System.Void Unity.Burst.BurstRuntime/HashCode64`1::.cctor()
// 0x00000006 System.Void Unity.Burst.BurstRuntime/PreserveAttribute::.ctor()
extern void PreserveAttribute__ctor_m03ACCEB4A3006FFF47CF22847FDC44963A6775DE (void);
// 0x00000007 System.Void Unity.Burst.FunctionPointer`1::.ctor(System.IntPtr)
// 0x00000008 T Unity.Burst.FunctionPointer`1::get_Invoke()
// 0x00000009 System.Void Unity.Burst.SharedStatic`1::.ctor(System.Void*)
// 0x0000000A T& Unity.Burst.SharedStatic`1::get_Data()
// 0x0000000B Unity.Burst.SharedStatic`1<T> Unity.Burst.SharedStatic`1::GetOrCreate(System.UInt32)
// 0x0000000C System.Void* Unity.Burst.SharedStatic::GetOrCreateSharedStaticInternal(System.Int64,System.Int64,System.UInt32,System.UInt32)
extern void SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254 (void);
// 0x0000000D System.Void Unity.Burst.SharedStatic::.cctor()
extern void SharedStatic__cctor_m91BEC2BB24D9ABA5B5998E30913C1C0DE51E9E36 (void);
static Il2CppMethodPointer s_methodPointers[13] = 
{
	BurstCompileAttribute__ctor_m75F0EF699FB9E5AD644F26B04ACE674ED2C81628,
	NULL,
	BurstRuntime_HashStringWithFNV1A64_m36354952510DAE5551D8576CAEE0CC23BC49864E,
	BurstRuntime_Log_mA326750916D8B27060FFE335CFE3BB74F12E8632,
	NULL,
	PreserveAttribute__ctor_m03ACCEB4A3006FFF47CF22847FDC44963A6775DE,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254,
	SharedStatic__cctor_m91BEC2BB24D9ABA5B5998E30913C1C0DE51E9E36,
};
static const int32_t s_InvokerIndices[13] = 
{
	4619,
	-1,
	7246,
	5686,
	-1,
	4619,
	-1,
	-1,
	-1,
	-1,
	-1,
	5339,
	7653,
};
static const Il2CppTokenRangePair s_rgctxIndices[5] = 
{
	{ 0x02000004, { 1, 2 } },
	{ 0x02000006, { 3, 1 } },
	{ 0x02000007, { 4, 4 } },
	{ 0x06000002, { 0, 1 } },
	{ 0x0600000B, { 8, 1 } },
};
static const Il2CppRGCTXDefinition s_rgctxValues[9] = 
{
	{ (Il2CppRGCTXDataType)2, 2382 },
	{ (Il2CppRGCTXDataType)1, 710 },
	{ (Il2CppRGCTXDataType)2, 2383 },
	{ (Il2CppRGCTXDataType)3, 36302 },
	{ (Il2CppRGCTXDataType)3, 36788 },
	{ (Il2CppRGCTXDataType)3, 36968 },
	{ (Il2CppRGCTXDataType)2, 4252 },
	{ (Il2CppRGCTXDataType)3, 27693 },
	{ (Il2CppRGCTXDataType)3, 35462 },
};
extern const CustomAttributesCacheGenerator g_Unity_Burst_AttributeGenerators[];
IL2CPP_EXTERN_C const Il2CppCodeGenModule g_Unity_Burst_CodeGenModule;
const Il2CppCodeGenModule g_Unity_Burst_CodeGenModule = 
{
	"Unity.Burst.dll",
	13,
	s_methodPointers,
	0,
	NULL,
	s_InvokerIndices,
	0,
	NULL,
	5,
	s_rgctxIndices,
	9,
	s_rgctxValues,
	NULL,
	g_Unity_Burst_AttributeGenerators,
	NULL, // module initializer,
	NULL,
	NULL,
	NULL,
};
