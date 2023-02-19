#include "pch-cpp.hpp"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <limits>
#include <stdint.h>



// System.Collections.Generic.Dictionary`2<System.Int64,System.Object>
struct Dictionary_2_t240BB5F785CC3B2A17B14447F3C0E0BB6AAB8E26;
// System.Collections.Generic.Dictionary`2<System.Int64,System.Type>
struct Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D;
// System.Collections.Generic.IEqualityComparer`1<System.Int64>
struct IEqualityComparer_1_tBD7EB381E8B25356EF3AED6C41B65AECA6B91A19;
// System.Collections.Generic.Dictionary`2/KeyCollection<System.Int64,System.Type>
struct KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A;
// System.Collections.Generic.Dictionary`2/ValueCollection<System.Int64,System.Type>
struct ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C;
// System.Collections.Generic.Dictionary`2/Entry<System.Int64,System.Type>[]
struct EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E;
// System.Byte[]
struct ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726;
// System.Char[]
struct CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34;
// System.Int32[]
struct Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32;
// System.Attribute
struct Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71;
// Unity.Burst.BurstCompileAttribute
struct BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1;
// System.String
struct String_t;
// Unity.Burst.BurstRuntime/PreserveAttribute
struct PreserveAttribute_t5582719532581A928FFB965E9E6F529B1162FF42;

IL2CPP_EXTERN_C RuntimeClass* Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_mEDDD9C0EDBF84B20D8EEC0295487B7446286353C_RuntimeMethod_var;


IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_tCD5BF6D3AAB147F28F15DED35776F8117AE07F16 
{
public:

public:
};


// System.Object


// System.Collections.Generic.Dictionary`2<System.Int64,System.Type>
struct  Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.Dictionary`2::buckets
	Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* ___buckets_0;
	// System.Collections.Generic.Dictionary`2/Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::entries
	EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E* ___entries_1;
	// System.Int32 System.Collections.Generic.Dictionary`2::count
	int32_t ___count_2;
	// System.Int32 System.Collections.Generic.Dictionary`2::version
	int32_t ___version_3;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeList
	int32_t ___freeList_4;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeCount
	int32_t ___freeCount_5;
	// System.Collections.Generic.IEqualityComparer`1<TKey> System.Collections.Generic.Dictionary`2::comparer
	RuntimeObject* ___comparer_6;
	// System.Collections.Generic.Dictionary`2/KeyCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::keys
	KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A * ___keys_7;
	// System.Collections.Generic.Dictionary`2/ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::values
	ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C * ___values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject * ____syncRoot_9;

public:
	inline static int32_t get_offset_of_buckets_0() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___buckets_0)); }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* get_buckets_0() const { return ___buckets_0; }
	inline Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32** get_address_of_buckets_0() { return &___buckets_0; }
	inline void set_buckets_0(Int32U5BU5D_t70F1BDC14B1786481B176D6139A5E3B87DC54C32* value)
	{
		___buckets_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___buckets_0), (void*)value);
	}

	inline static int32_t get_offset_of_entries_1() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___entries_1)); }
	inline EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E* get_entries_1() const { return ___entries_1; }
	inline EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E** get_address_of_entries_1() { return &___entries_1; }
	inline void set_entries_1(EntryU5BU5D_t09835765F8D6593FB6AEFA6582E0E23F2B6C871E* value)
	{
		___entries_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___entries_1), (void*)value);
	}

	inline static int32_t get_offset_of_count_2() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___count_2)); }
	inline int32_t get_count_2() const { return ___count_2; }
	inline int32_t* get_address_of_count_2() { return &___count_2; }
	inline void set_count_2(int32_t value)
	{
		___count_2 = value;
	}

	inline static int32_t get_offset_of_version_3() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___version_3)); }
	inline int32_t get_version_3() const { return ___version_3; }
	inline int32_t* get_address_of_version_3() { return &___version_3; }
	inline void set_version_3(int32_t value)
	{
		___version_3 = value;
	}

	inline static int32_t get_offset_of_freeList_4() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___freeList_4)); }
	inline int32_t get_freeList_4() const { return ___freeList_4; }
	inline int32_t* get_address_of_freeList_4() { return &___freeList_4; }
	inline void set_freeList_4(int32_t value)
	{
		___freeList_4 = value;
	}

	inline static int32_t get_offset_of_freeCount_5() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___freeCount_5)); }
	inline int32_t get_freeCount_5() const { return ___freeCount_5; }
	inline int32_t* get_address_of_freeCount_5() { return &___freeCount_5; }
	inline void set_freeCount_5(int32_t value)
	{
		___freeCount_5 = value;
	}

	inline static int32_t get_offset_of_comparer_6() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___comparer_6)); }
	inline RuntimeObject* get_comparer_6() const { return ___comparer_6; }
	inline RuntimeObject** get_address_of_comparer_6() { return &___comparer_6; }
	inline void set_comparer_6(RuntimeObject* value)
	{
		___comparer_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comparer_6), (void*)value);
	}

	inline static int32_t get_offset_of_keys_7() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___keys_7)); }
	inline KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A * get_keys_7() const { return ___keys_7; }
	inline KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A ** get_address_of_keys_7() { return &___keys_7; }
	inline void set_keys_7(KeyCollection_t3F56F6EEC94E86F1DF2C49F41FBDEE053550D90A * value)
	{
		___keys_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___keys_7), (void*)value);
	}

	inline static int32_t get_offset_of_values_8() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ___values_8)); }
	inline ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C * get_values_8() const { return ___values_8; }
	inline ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C ** get_address_of_values_8() { return &___values_8; }
	inline void set_values_8(ValueCollection_t6AA94407EBA83DBB24A4BB8E756BFCECBC80A70C * value)
	{
		___values_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___values_8), (void*)value);
	}

	inline static int32_t get_offset_of__syncRoot_9() { return static_cast<int32_t>(offsetof(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D, ____syncRoot_9)); }
	inline RuntimeObject * get__syncRoot_9() const { return ____syncRoot_9; }
	inline RuntimeObject ** get_address_of__syncRoot_9() { return &____syncRoot_9; }
	inline void set__syncRoot_9(RuntimeObject * value)
	{
		____syncRoot_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_9), (void*)value);
	}
};

struct Il2CppArrayBounds;

// System.Array


// System.Attribute
struct  Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71  : public RuntimeObject
{
public:

public:
};


// Unity.Burst.BurstRuntime
struct  BurstRuntime_t725AE7FF2003494CEB5F987F3365BA847C762735  : public RuntimeObject
{
public:

public:
};


// Unity.Burst.SharedStatic
struct  SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC  : public RuntimeObject
{
public:

public:
};

struct SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_StaticFields
{
public:
	// System.Collections.Generic.Dictionary`2<System.Int64,System.Type> Unity.Burst.SharedStatic::HashToType
	Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * ___HashToType_0;

public:
	inline static int32_t get_offset_of_HashToType_0() { return static_cast<int32_t>(offsetof(SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_StaticFields, ___HashToType_0)); }
	inline Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * get_HashToType_0() const { return ___HashToType_0; }
	inline Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D ** get_address_of_HashToType_0() { return &___HashToType_0; }
	inline void set_HashToType_0(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * value)
	{
		___HashToType_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___HashToType_0), (void*)value);
	}
};


// System.String
struct  String_t  : public RuntimeObject
{
public:
	// System.Int32 System.String::m_stringLength
	int32_t ___m_stringLength_0;
	// System.Char System.String::m_firstChar
	Il2CppChar ___m_firstChar_1;

public:
	inline static int32_t get_offset_of_m_stringLength_0() { return static_cast<int32_t>(offsetof(String_t, ___m_stringLength_0)); }
	inline int32_t get_m_stringLength_0() const { return ___m_stringLength_0; }
	inline int32_t* get_address_of_m_stringLength_0() { return &___m_stringLength_0; }
	inline void set_m_stringLength_0(int32_t value)
	{
		___m_stringLength_0 = value;
	}

	inline static int32_t get_offset_of_m_firstChar_1() { return static_cast<int32_t>(offsetof(String_t, ___m_firstChar_1)); }
	inline Il2CppChar get_m_firstChar_1() const { return ___m_firstChar_1; }
	inline Il2CppChar* get_address_of_m_firstChar_1() { return &___m_firstChar_1; }
	inline void set_m_firstChar_1(Il2CppChar value)
	{
		___m_firstChar_1 = value;
	}
};

struct String_t_StaticFields
{
public:
	// System.String System.String::Empty
	String_t* ___Empty_5;

public:
	inline static int32_t get_offset_of_Empty_5() { return static_cast<int32_t>(offsetof(String_t_StaticFields, ___Empty_5)); }
	inline String_t* get_Empty_5() const { return ___Empty_5; }
	inline String_t** get_address_of_Empty_5() { return &___Empty_5; }
	inline void set_Empty_5(String_t* value)
	{
		___Empty_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Empty_5), (void*)value);
	}
};


// System.ValueType
struct  ValueType_tDBF999C1B75C48C68621878250DBF6CDBCF51E52  : public RuntimeObject
{
public:

public:
};

// Native definition for P/Invoke marshalling of System.ValueType
struct ValueType_tDBF999C1B75C48C68621878250DBF6CDBCF51E52_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.ValueType
struct ValueType_tDBF999C1B75C48C68621878250DBF6CDBCF51E52_marshaled_com
{
};

// Unity.Burst.BurstCompileAttribute
struct  BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1  : public Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71
{
public:

public:
};


// System.Byte
struct  Byte_t0111FAB8B8685667EDDAF77683F0D8F86B659056 
{
public:
	// System.Byte System.Byte::m_value
	uint8_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Byte_t0111FAB8B8685667EDDAF77683F0D8F86B659056, ___m_value_0)); }
	inline uint8_t get_m_value_0() const { return ___m_value_0; }
	inline uint8_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint8_t value)
	{
		___m_value_0 = value;
	}
};


// System.Char
struct  Char_tFF60D8E7E89A20BE2294A003734341BD1DF43E14 
{
public:
	// System.Char System.Char::m_value
	Il2CppChar ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Char_tFF60D8E7E89A20BE2294A003734341BD1DF43E14, ___m_value_0)); }
	inline Il2CppChar get_m_value_0() const { return ___m_value_0; }
	inline Il2CppChar* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(Il2CppChar value)
	{
		___m_value_0 = value;
	}
};

struct Char_tFF60D8E7E89A20BE2294A003734341BD1DF43E14_StaticFields
{
public:
	// System.Byte[] System.Char::categoryForLatin1
	ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726* ___categoryForLatin1_3;

public:
	inline static int32_t get_offset_of_categoryForLatin1_3() { return static_cast<int32_t>(offsetof(Char_tFF60D8E7E89A20BE2294A003734341BD1DF43E14_StaticFields, ___categoryForLatin1_3)); }
	inline ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726* get_categoryForLatin1_3() const { return ___categoryForLatin1_3; }
	inline ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726** get_address_of_categoryForLatin1_3() { return &___categoryForLatin1_3; }
	inline void set_categoryForLatin1_3(ByteU5BU5D_tDBBEB0E8362242FA7223000D978B0DD19D4B0726* value)
	{
		___categoryForLatin1_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___categoryForLatin1_3), (void*)value);
	}
};


// System.Enum
struct  Enum_t23B90B40F60E677A8025267341651C94AE079CDA  : public ValueType_tDBF999C1B75C48C68621878250DBF6CDBCF51E52
{
public:

public:
};

struct Enum_t23B90B40F60E677A8025267341651C94AE079CDA_StaticFields
{
public:
	// System.Char[] System.Enum::enumSeperatorCharArray
	CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34* ___enumSeperatorCharArray_0;

public:
	inline static int32_t get_offset_of_enumSeperatorCharArray_0() { return static_cast<int32_t>(offsetof(Enum_t23B90B40F60E677A8025267341651C94AE079CDA_StaticFields, ___enumSeperatorCharArray_0)); }
	inline CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34* get_enumSeperatorCharArray_0() const { return ___enumSeperatorCharArray_0; }
	inline CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34** get_address_of_enumSeperatorCharArray_0() { return &___enumSeperatorCharArray_0; }
	inline void set_enumSeperatorCharArray_0(CharU5BU5D_t7B7FC5BC8091AA3B9CB0B29CDD80B5EE9254AA34* value)
	{
		___enumSeperatorCharArray_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumSeperatorCharArray_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Enum
struct Enum_t23B90B40F60E677A8025267341651C94AE079CDA_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.Enum
struct Enum_t23B90B40F60E677A8025267341651C94AE079CDA_marshaled_com
{
};

// UnityEngine.Hash128
struct  Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A 
{
public:
	// System.UInt32 UnityEngine.Hash128::m_u32_0
	uint32_t ___m_u32_0_0;
	// System.UInt32 UnityEngine.Hash128::m_u32_1
	uint32_t ___m_u32_1_1;
	// System.UInt32 UnityEngine.Hash128::m_u32_2
	uint32_t ___m_u32_2_2;
	// System.UInt32 UnityEngine.Hash128::m_u32_3
	uint32_t ___m_u32_3_3;

public:
	inline static int32_t get_offset_of_m_u32_0_0() { return static_cast<int32_t>(offsetof(Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A, ___m_u32_0_0)); }
	inline uint32_t get_m_u32_0_0() const { return ___m_u32_0_0; }
	inline uint32_t* get_address_of_m_u32_0_0() { return &___m_u32_0_0; }
	inline void set_m_u32_0_0(uint32_t value)
	{
		___m_u32_0_0 = value;
	}

	inline static int32_t get_offset_of_m_u32_1_1() { return static_cast<int32_t>(offsetof(Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A, ___m_u32_1_1)); }
	inline uint32_t get_m_u32_1_1() const { return ___m_u32_1_1; }
	inline uint32_t* get_address_of_m_u32_1_1() { return &___m_u32_1_1; }
	inline void set_m_u32_1_1(uint32_t value)
	{
		___m_u32_1_1 = value;
	}

	inline static int32_t get_offset_of_m_u32_2_2() { return static_cast<int32_t>(offsetof(Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A, ___m_u32_2_2)); }
	inline uint32_t get_m_u32_2_2() const { return ___m_u32_2_2; }
	inline uint32_t* get_address_of_m_u32_2_2() { return &___m_u32_2_2; }
	inline void set_m_u32_2_2(uint32_t value)
	{
		___m_u32_2_2 = value;
	}

	inline static int32_t get_offset_of_m_u32_3_3() { return static_cast<int32_t>(offsetof(Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A, ___m_u32_3_3)); }
	inline uint32_t get_m_u32_3_3() const { return ___m_u32_3_3; }
	inline uint32_t* get_address_of_m_u32_3_3() { return &___m_u32_3_3; }
	inline void set_m_u32_3_3(uint32_t value)
	{
		___m_u32_3_3 = value;
	}
};


// System.Int32
struct  Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046 
{
public:
	// System.Int32 System.Int32::m_value
	int32_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int32_tFDE5F8CD43D10453F6A2E0C77FE48C6CC7009046, ___m_value_0)); }
	inline int32_t get_m_value_0() const { return ___m_value_0; }
	inline int32_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int32_t value)
	{
		___m_value_0 = value;
	}
};


// System.Int64
struct  Int64_t378EE0D608BD3107E77238E85F30D2BBD46981F3 
{
public:
	// System.Int64 System.Int64::m_value
	int64_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int64_t378EE0D608BD3107E77238E85F30D2BBD46981F3, ___m_value_0)); }
	inline int64_t get_m_value_0() const { return ___m_value_0; }
	inline int64_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int64_t value)
	{
		___m_value_0 = value;
	}
};


// System.UInt32
struct  UInt32_tE60352A06233E4E69DD198BCC67142159F686B15 
{
public:
	// System.UInt32 System.UInt32::m_value
	uint32_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt32_tE60352A06233E4E69DD198BCC67142159F686B15, ___m_value_0)); }
	inline uint32_t get_m_value_0() const { return ___m_value_0; }
	inline uint32_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint32_t value)
	{
		___m_value_0 = value;
	}
};


// System.UInt64
struct  UInt64_tEC57511B3E3CA2DBA1BEBD434C6983E31C943281 
{
public:
	// System.UInt64 System.UInt64::m_value
	uint64_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt64_tEC57511B3E3CA2DBA1BEBD434C6983E31C943281, ___m_value_0)); }
	inline uint64_t get_m_value_0() const { return ___m_value_0; }
	inline uint64_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint64_t value)
	{
		___m_value_0 = value;
	}
};


// System.Void
struct  Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5 
{
public:
	union
	{
		struct
		{
		};
		uint8_t Void_t700C6383A2A510C2CF4DD86DABD5CA9FF70ADAC5__padding[1];
	};

public:
};


// Unity.Burst.BurstRuntime/PreserveAttribute
struct  PreserveAttribute_t5582719532581A928FFB965E9E6F529B1162FF42  : public Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71
{
public:

public:
};


// Unity.Burst.LowLevel.BurstCompilerService/BurstLogType
struct  BurstLogType_t733DB4EBA1397A6A02872942292EB97C18C2EA34 
{
public:
	// System.Int32 Unity.Burst.LowLevel.BurstCompilerService/BurstLogType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(BurstLogType_t733DB4EBA1397A6A02872942292EB97C18C2EA34, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif


// System.Void System.Collections.Generic.Dictionary`2<System.Int64,System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_m8333BBB4DDAE242DCF8984A6A5D7A2E469405BDB_gshared (Dictionary_2_t240BB5F785CC3B2A17B14447F3C0E0BB6AAB8E26 * __this, const RuntimeMethod* method);

// System.Void System.Attribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Attribute__ctor_m5C1862A7DFC2C25A4797A8C5F681FBB5CB53ECE1 (Attribute_t037CA9D9F3B742C063DB364D2EEBBF9FC5772C71 * __this, const RuntimeMethod* method);
// System.Char System.String::get_Chars(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar String_get_Chars_m9B1A5E4C8D70AA33A60F03735AF7B77AB9DBBA70 (String_t* __this, int32_t ___index0, const RuntimeMethod* method);
// System.Int32 System.String::get_Length()
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m129FC0ADA02FECBED3C0B1A809AE84A5AEE1CF09_inline (String_t* __this, const RuntimeMethod* method);
// System.Void Unity.Burst.LowLevel.BurstCompilerService::Log(System.Void*,Unity.Burst.LowLevel.BurstCompilerService/BurstLogType,System.Byte*,System.Byte*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompilerService_Log_m8B72F315B8A8DCCB6ED31880F5B4ECD0C516DE5E (void* ___userData0, int32_t ___logType1, uint8_t* ___message2, uint8_t* ___filename3, int32_t ___lineNumber4, const RuntimeMethod* method);
// System.Void UnityEngine.Hash128::.ctor(System.UInt64,System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Hash128__ctor_m7958BAB68961412F06FACF1F09E04B4CEAE8FB9D (Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A * __this, uint64_t ___u64_00, uint64_t ___u64_11, const RuntimeMethod* method);
// System.Void* Unity.Burst.LowLevel.BurstCompilerService::GetOrCreateSharedMemory(UnityEngine.Hash128&,System.UInt32,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* BurstCompilerService_GetOrCreateSharedMemory_m861188CFBCAC0D14519A4E88D5485ACADA2C0858 (Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A * ___key0, uint32_t ___size_of1, uint32_t ___alignment2, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int64,System.Type>::.ctor()
inline void Dictionary_2__ctor_mEDDD9C0EDBF84B20D8EEC0295487B7446286353C (Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D *, const RuntimeMethod*))Dictionary_2__ctor_m8333BBB4DDAE242DCF8984A6A5D7A2E469405BDB_gshared)(__this, method);
}
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Burst.BurstCompileAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstCompileAttribute__ctor_m75F0EF699FB9E5AD644F26B04ACE674ED2C81628 (BurstCompileAttribute_t845F9073B3F7D817A249B1F2191CE96B8ADA00D1 * __this, const RuntimeMethod* method)
{
	{
		// public BurstCompileAttribute()
		Attribute__ctor_m5C1862A7DFC2C25A4797A8C5F681FBB5CB53ECE1(__this, /*hidden argument*/NULL);
		// }
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Int64 Unity.Burst.BurstRuntime::HashStringWithFNV1A64(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t BurstRuntime_HashStringWithFNV1A64_m36354952510DAE5551D8576CAEE0CC23BC49864E (String_t* ___text0, const RuntimeMethod* method)
{
	uint64_t V_0 = 0;
	String_t* V_1 = NULL;
	int32_t V_2 = 0;
	Il2CppChar V_3 = 0x0;
	{
		// ulong result = offsetBasis;
		V_0 = ((int64_t)-3750763034362895579LL);
		// foreach (var c in text)
		String_t* L_0 = ___text0;
		V_1 = L_0;
		V_2 = 0;
		goto IL_0044;
	}

IL_0010:
	{
		// foreach (var c in text)
		String_t* L_1 = V_1;
		int32_t L_2 = V_2;
		NullCheck(L_1);
		Il2CppChar L_3;
		L_3 = String_get_Chars_m9B1A5E4C8D70AA33A60F03735AF7B77AB9DBBA70(L_1, L_2, /*hidden argument*/NULL);
		V_3 = L_3;
		// result = prime * (result ^ (byte)(c & 255));
		uint64_t L_4 = V_0;
		Il2CppChar L_5 = V_3;
		V_0 = ((int64_t)il2cpp_codegen_multiply((int64_t)((int64_t)1099511628211LL), (int64_t)((int64_t)((int64_t)L_4^(int64_t)((int64_t)((uint64_t)((uint32_t)((uint32_t)((int32_t)((uint8_t)((int32_t)((int32_t)L_5&(int32_t)((int32_t)255)))))))))))));
		// result = prime * (result ^ (byte)(c >> 8));
		uint64_t L_6 = V_0;
		Il2CppChar L_7 = V_3;
		V_0 = ((int64_t)il2cpp_codegen_multiply((int64_t)((int64_t)1099511628211LL), (int64_t)((int64_t)((int64_t)L_6^(int64_t)((int64_t)((uint64_t)((uint32_t)((uint32_t)((int32_t)((uint8_t)((int32_t)((int32_t)L_7>>(int32_t)8))))))))))));
		int32_t L_8 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_8, (int32_t)1));
	}

IL_0044:
	{
		// foreach (var c in text)
		int32_t L_9 = V_2;
		String_t* L_10 = V_1;
		NullCheck(L_10);
		int32_t L_11;
		L_11 = String_get_Length_m129FC0ADA02FECBED3C0B1A809AE84A5AEE1CF09_inline(L_10, /*hidden argument*/NULL);
		if ((((int32_t)L_9) < ((int32_t)L_11)))
		{
			goto IL_0010;
		}
	}
	{
		// return (long)result;
		uint64_t L_12 = V_0;
		return L_12;
	}
}
// System.Void Unity.Burst.BurstRuntime::Log(System.Byte*,System.Int32,System.Byte*,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BurstRuntime_Log_mA326750916D8B27060FFE335CFE3BB74F12E8632 (uint8_t* ___message0, int32_t ___logType1, uint8_t* ___fileName2, int32_t ___lineNumber3, const RuntimeMethod* method)
{
	{
		// BurstCompilerService.Log((byte*) 0, (BurstCompilerService.BurstLogType)logType, message, fileName, lineNumber);
		int32_t L_0 = ___logType1;
		uint8_t* L_1 = ___message0;
		uint8_t* L_2 = ___fileName2;
		int32_t L_3 = ___lineNumber3;
		BurstCompilerService_Log_m8B72F315B8A8DCCB6ED31880F5B4ECD0C516DE5E((void*)(void*)((intptr_t)0), L_0, (uint8_t*)(uint8_t*)L_1, (uint8_t*)(uint8_t*)L_2, L_3, /*hidden argument*/NULL);
		// }
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void* Unity.Burst.SharedStatic::GetOrCreateSharedStaticInternal(System.Int64,System.Int64,System.UInt32,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* SharedStatic_GetOrCreateSharedStaticInternal_m1FDD96EE55D49263DA5EA34EB6502507F1EAD254 (int64_t ___getHashCode640, int64_t ___getSubHashCode641, uint32_t ___sizeOf2, uint32_t ___alignment3, const RuntimeMethod* method)
{
	Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// var hash128 = new Hash128((ulong)getHashCode64, (ulong)getSubHashCode64);
		int64_t L_0 = ___getHashCode640;
		int64_t L_1 = ___getSubHashCode641;
		Hash128__ctor_m7958BAB68961412F06FACF1F09E04B4CEAE8FB9D((Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A *)(&V_0), L_0, L_1, /*hidden argument*/NULL);
		// var result = BurstCompilerService.GetOrCreateSharedMemory(ref hash128, sizeOf, alignment);
		uint32_t L_2 = ___sizeOf2;
		uint32_t L_3 = ___alignment3;
		void* L_4;
		L_4 = BurstCompilerService_GetOrCreateSharedMemory_m861188CFBCAC0D14519A4E88D5485ACADA2C0858((Hash128_t1858EA099934FD6F2B769E5661C17A276A2AFE7A *)(&V_0), L_2, L_3, /*hidden argument*/NULL);
		// return result;
		return (void*)(L_4);
	}
}
// System.Void Unity.Burst.SharedStatic::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SharedStatic__cctor_m91BEC2BB24D9ABA5B5998E30913C1C0DE51E9E36 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Dictionary_2__ctor_mEDDD9C0EDBF84B20D8EEC0295487B7446286353C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private static readonly Dictionary<long, Type> HashToType = new Dictionary<long, Type>();
		Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D * L_0 = (Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D *)il2cpp_codegen_object_new(Dictionary_2_t9FC6861E09F23C7035FD1CE506D4404DCE331C9D_il2cpp_TypeInfo_var);
		Dictionary_2__ctor_mEDDD9C0EDBF84B20D8EEC0295487B7446286353C(L_0, /*hidden argument*/Dictionary_2__ctor_mEDDD9C0EDBF84B20D8EEC0295487B7446286353C_RuntimeMethod_var);
		((SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_StaticFields*)il2cpp_codegen_static_fields_for(SharedStatic_t4A478B3109091C26ED6F772B9BEC1EA0802B2FFC_il2cpp_TypeInfo_var))->set_HashToType_0(L_0);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Unity.Burst.BurstRuntime/PreserveAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PreserveAttribute__ctor_m03ACCEB4A3006FFF47CF22847FDC44963A6775DE (PreserveAttribute_t5582719532581A928FFB965E9E6F529B1162FF42 * __this, const RuntimeMethod* method)
{
	{
		Attribute__ctor_m5C1862A7DFC2C25A4797A8C5F681FBB5CB53ECE1(__this, /*hidden argument*/NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m129FC0ADA02FECBED3C0B1A809AE84A5AEE1CF09_inline (String_t* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->get_m_stringLength_0();
		return L_0;
	}
}
