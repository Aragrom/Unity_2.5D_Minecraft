#include "pch-c.h"
#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include "codegen/il2cpp-codegen-metadata.h"





// 0x00000001 System.Void Microsoft.CodeAnalysis.EmbeddedAttribute::.ctor()
extern void EmbeddedAttribute__ctor_m4F0374906116110F04D52494A86ED13E76735AF7 (void);
// 0x00000002 System.Void System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor()
extern void IsReadOnlyAttribute__ctor_m4BCBD7CF3859E4030420E2CEF1C9BE4426AEB1FB (void);
// 0x00000003 System.Void Unity.Burst.BurstCompileAttribute::.ctor()
extern void BurstCompileAttribute__ctor_m75F0EF699FB9E5AD644F26B04ACE674ED2C81628 (void);
// 0x00000004 System.Int64 Unity.Burst.BurstRuntime::GetHashCode64()
// 0x00000005 System.Int64 Unity.Burst.BurstRuntime::HashStringWithFNV1A64(System.String)
extern void BurstRuntime_HashStringWithFNV1A64_m36354952510DAE5551D8576CAEE0CC23BC49864E (void);
// 0x00000006 System.Void Unity.Burst.BurstRuntime::Log(System.Byte*,System.Int32,System.Byte*,System.Int32)
extern void BurstRuntime_Log_mA326750916D8B27060FFE335CFE3BB74F12E8632 (void);
// 0x00000007 System.Void Unity.Burst.BurstRuntime/HashCode64`1::.cctor()
// 0x00000008 System.Void Unity.Burst.BurstRuntime/PreserveAttribute::.ctor()
extern void PreserveAttribute__ctor_m03ACCEB4A3006FFF47CF22847FDC44963A6775DE (void);
// 0x00000009 System.Void Unity.Burst.FunctionPointer`1::.ctor(System.IntPtr)
// 0x0000000A T Unity.Burst.FunctionPointer`1::get_Invoke()
// 0x0000000B System.Void Unity.Burst.SharedStatic`1::.ctor(System.Void*)
// 0x0000000C T& Unity.Burst.SharedStatic`1::get_Data()
// 0x0000000D Unity.Burst.SharedStatic`1<T> Unity.Burst.SharedStatic`1::GetOrCreate(System.UInt32)
// 0x0000000E System.Void* Unity.Burst.SharedStatic::GetOrCreateSharedStaticInternal(System.Int64,System.Int64,System.UInt32,System.UInt32)
extern void SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254 (void);
// 0x0000000F System.Void Unity.Burst.SharedStatic::.cctor()
extern void SharedStatic__cctor_m91BEC2BB24D9ABA5B5998E30913C1C0DE51E9E36 (void);
static Il2CppMethodPointer s_methodPointers[15] = 
{
	EmbeddedAttribute__ctor_m4F0374906116110F04D52494A86ED13E76735AF7,
	IsReadOnlyAttribute__ctor_m4BCBD7CF3859E4030420E2CEF1C9BE4426AEB1FB,
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
static const int32_t s_InvokerIndices[15] = 
{
	3824,
	3824,
	3824,
	-1,
	5556,
	4529,
	-1,
	3824,
	-1,
	-1,
	-1,
	-1,
	-1,
	4324,
	5849,
};
static const Il2CppTokenRangePair s_rgctxIndices[5] = 
{
	{ 0x02000006, { 1, 2 } },
	{ 0x02000008, { 3, 1 } },
	{ 0x02000009, { 4, 4 } },
	{ 0x06000004, { 0, 1 } },
	{ 0x0600000D, { 8, 1 } },
};
static const Il2CppRGCTXDefinition s_rgctxValues[9] = 
{
	{ (Il2CppRGCTXDataType)2, 1842 },
	{ (Il2CppRGCTXDataType)1, 632 },
	{ (Il2CppRGCTXDataType)2, 1843 },
	{ (Il2CppRGCTXDataType)3, 30025 },
	{ (Il2CppRGCTXDataType)3, 30425 },
	{ (Il2CppRGCTXDataType)3, 30606 },
	{ (Il2CppRGCTXDataType)2, 3438 },
	{ (Il2CppRGCTXDataType)3, 22962 },
	{ (Il2CppRGCTXDataType)3, 29267 },
};
extern const CustomAttributesCacheGenerator g_Unity_Burst_AttributeGenerators[];
IL2CPP_EXTERN_C const Il2CppCodeGenModule g_Unity_Burst_CodeGenModule;
const Il2CppCodeGenModule g_Unity_Burst_CodeGenModule = 
{
	"Unity.Burst.dll",
	15,
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
