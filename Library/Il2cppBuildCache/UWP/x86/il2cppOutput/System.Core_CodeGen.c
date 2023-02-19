#include "pch-c.h"
#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include "codegen/il2cpp-codegen-metadata.h"





// 0x00000001 System.Exception System.Linq.Error::ArgumentNull(System.String)
extern void Error_ArgumentNull_m0EDA0D46D72CA692518E3E2EB75B48044D8FD41E (void);
// 0x00000002 System.Exception System.Linq.Error::ArgumentOutOfRange(System.String)
extern void Error_ArgumentOutOfRange_m2EFB999454161A6B48F8DAC3753FDC190538F0F2 (void);
// 0x00000003 System.Exception System.Linq.Error::MoreThanOneMatch()
extern void Error_MoreThanOneMatch_m4C4756AF34A76EF12F3B2B6D8C78DE547F0FBCF8 (void);
// 0x00000004 System.Exception System.Linq.Error::NoElements()
extern void Error_NoElements_mB89E91246572F009281D79730950808F17C3F353 (void);
// 0x00000005 System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable::Where(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
// 0x00000006 System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable::Select(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,TResult>)
// 0x00000007 System.Func`2<TSource,System.Boolean> System.Linq.Enumerable::CombinePredicates(System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,System.Boolean>)
// 0x00000008 System.Func`2<TSource,TResult> System.Linq.Enumerable::CombineSelectors(System.Func`2<TSource,TMiddle>,System.Func`2<TMiddle,TResult>)
// 0x00000009 System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable::SelectMany(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Collections.Generic.IEnumerable`1<TResult>>)
// 0x0000000A System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable::SelectManyIterator(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Collections.Generic.IEnumerable`1<TResult>>)
// 0x0000000B System.Linq.IOrderedEnumerable`1<TSource> System.Linq.Enumerable::OrderBy(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,TKey>)
// 0x0000000C System.Linq.IOrderedEnumerable`1<TSource> System.Linq.Enumerable::ThenBy(System.Linq.IOrderedEnumerable`1<TSource>,System.Func`2<TSource,TKey>)
// 0x0000000D System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable::Distinct(System.Collections.Generic.IEnumerable`1<TSource>)
// 0x0000000E System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable::DistinctIterator(System.Collections.Generic.IEnumerable`1<TSource>,System.Collections.Generic.IEqualityComparer`1<TSource>)
// 0x0000000F TSource[] System.Linq.Enumerable::ToArray(System.Collections.Generic.IEnumerable`1<TSource>)
// 0x00000010 System.Collections.Generic.List`1<TSource> System.Linq.Enumerable::ToList(System.Collections.Generic.IEnumerable`1<TSource>)
// 0x00000011 TSource System.Linq.Enumerable::First(System.Collections.Generic.IEnumerable`1<TSource>)
// 0x00000012 TSource System.Linq.Enumerable::FirstOrDefault(System.Collections.Generic.IEnumerable`1<TSource>)
// 0x00000013 TSource System.Linq.Enumerable::FirstOrDefault(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
// 0x00000014 TSource System.Linq.Enumerable::Last(System.Collections.Generic.IEnumerable`1<TSource>)
// 0x00000015 TSource System.Linq.Enumerable::SingleOrDefault(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
// 0x00000016 TSource System.Linq.Enumerable::ElementAt(System.Collections.Generic.IEnumerable`1<TSource>,System.Int32)
// 0x00000017 System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable::Empty()
// 0x00000018 System.Boolean System.Linq.Enumerable::Any(System.Collections.Generic.IEnumerable`1<TSource>)
// 0x00000019 System.Boolean System.Linq.Enumerable::Any(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
// 0x0000001A System.Boolean System.Linq.Enumerable::All(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
// 0x0000001B System.Int32 System.Linq.Enumerable::Count(System.Collections.Generic.IEnumerable`1<TSource>)
// 0x0000001C System.Int32 System.Linq.Enumerable::Count(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
// 0x0000001D System.Boolean System.Linq.Enumerable::Contains(System.Collections.Generic.IEnumerable`1<TSource>,TSource)
// 0x0000001E System.Boolean System.Linq.Enumerable::Contains(System.Collections.Generic.IEnumerable`1<TSource>,TSource,System.Collections.Generic.IEqualityComparer`1<TSource>)
// 0x0000001F System.Void System.Linq.Enumerable/Iterator`1::.ctor()
// 0x00000020 TSource System.Linq.Enumerable/Iterator`1::get_Current()
// 0x00000021 System.Linq.Enumerable/Iterator`1<TSource> System.Linq.Enumerable/Iterator`1::Clone()
// 0x00000022 System.Void System.Linq.Enumerable/Iterator`1::Dispose()
// 0x00000023 System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable/Iterator`1::GetEnumerator()
// 0x00000024 System.Boolean System.Linq.Enumerable/Iterator`1::MoveNext()
// 0x00000025 System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable/Iterator`1::Select(System.Func`2<TSource,TResult>)
// 0x00000026 System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable/Iterator`1::Where(System.Func`2<TSource,System.Boolean>)
// 0x00000027 System.Object System.Linq.Enumerable/Iterator`1::System.Collections.IEnumerator.get_Current()
// 0x00000028 System.Collections.IEnumerator System.Linq.Enumerable/Iterator`1::System.Collections.IEnumerable.GetEnumerator()
// 0x00000029 System.Void System.Linq.Enumerable/Iterator`1::System.Collections.IEnumerator.Reset()
// 0x0000002A System.Void System.Linq.Enumerable/WhereEnumerableIterator`1::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>)
// 0x0000002B System.Linq.Enumerable/Iterator`1<TSource> System.Linq.Enumerable/WhereEnumerableIterator`1::Clone()
// 0x0000002C System.Void System.Linq.Enumerable/WhereEnumerableIterator`1::Dispose()
// 0x0000002D System.Boolean System.Linq.Enumerable/WhereEnumerableIterator`1::MoveNext()
// 0x0000002E System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable/WhereEnumerableIterator`1::Select(System.Func`2<TSource,TResult>)
// 0x0000002F System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable/WhereEnumerableIterator`1::Where(System.Func`2<TSource,System.Boolean>)
// 0x00000030 System.Void System.Linq.Enumerable/WhereArrayIterator`1::.ctor(TSource[],System.Func`2<TSource,System.Boolean>)
// 0x00000031 System.Linq.Enumerable/Iterator`1<TSource> System.Linq.Enumerable/WhereArrayIterator`1::Clone()
// 0x00000032 System.Boolean System.Linq.Enumerable/WhereArrayIterator`1::MoveNext()
// 0x00000033 System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable/WhereArrayIterator`1::Select(System.Func`2<TSource,TResult>)
// 0x00000034 System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable/WhereArrayIterator`1::Where(System.Func`2<TSource,System.Boolean>)
// 0x00000035 System.Void System.Linq.Enumerable/WhereListIterator`1::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>)
// 0x00000036 System.Linq.Enumerable/Iterator`1<TSource> System.Linq.Enumerable/WhereListIterator`1::Clone()
// 0x00000037 System.Boolean System.Linq.Enumerable/WhereListIterator`1::MoveNext()
// 0x00000038 System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable/WhereListIterator`1::Select(System.Func`2<TSource,TResult>)
// 0x00000039 System.Collections.Generic.IEnumerable`1<TSource> System.Linq.Enumerable/WhereListIterator`1::Where(System.Func`2<TSource,System.Boolean>)
// 0x0000003A System.Void System.Linq.Enumerable/WhereSelectEnumerableIterator`2::.ctor(System.Collections.Generic.IEnumerable`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
// 0x0000003B System.Linq.Enumerable/Iterator`1<TResult> System.Linq.Enumerable/WhereSelectEnumerableIterator`2::Clone()
// 0x0000003C System.Void System.Linq.Enumerable/WhereSelectEnumerableIterator`2::Dispose()
// 0x0000003D System.Boolean System.Linq.Enumerable/WhereSelectEnumerableIterator`2::MoveNext()
// 0x0000003E System.Collections.Generic.IEnumerable`1<TResult2> System.Linq.Enumerable/WhereSelectEnumerableIterator`2::Select(System.Func`2<TResult,TResult2>)
// 0x0000003F System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable/WhereSelectEnumerableIterator`2::Where(System.Func`2<TResult,System.Boolean>)
// 0x00000040 System.Void System.Linq.Enumerable/WhereSelectArrayIterator`2::.ctor(TSource[],System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
// 0x00000041 System.Linq.Enumerable/Iterator`1<TResult> System.Linq.Enumerable/WhereSelectArrayIterator`2::Clone()
// 0x00000042 System.Boolean System.Linq.Enumerable/WhereSelectArrayIterator`2::MoveNext()
// 0x00000043 System.Collections.Generic.IEnumerable`1<TResult2> System.Linq.Enumerable/WhereSelectArrayIterator`2::Select(System.Func`2<TResult,TResult2>)
// 0x00000044 System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable/WhereSelectArrayIterator`2::Where(System.Func`2<TResult,System.Boolean>)
// 0x00000045 System.Void System.Linq.Enumerable/WhereSelectListIterator`2::.ctor(System.Collections.Generic.List`1<TSource>,System.Func`2<TSource,System.Boolean>,System.Func`2<TSource,TResult>)
// 0x00000046 System.Linq.Enumerable/Iterator`1<TResult> System.Linq.Enumerable/WhereSelectListIterator`2::Clone()
// 0x00000047 System.Boolean System.Linq.Enumerable/WhereSelectListIterator`2::MoveNext()
// 0x00000048 System.Collections.Generic.IEnumerable`1<TResult2> System.Linq.Enumerable/WhereSelectListIterator`2::Select(System.Func`2<TResult,TResult2>)
// 0x00000049 System.Collections.Generic.IEnumerable`1<TResult> System.Linq.Enumerable/WhereSelectListIterator`2::Where(System.Func`2<TResult,System.Boolean>)
// 0x0000004A System.Void System.Linq.Enumerable/<>c__DisplayClass6_0`1::.ctor()
// 0x0000004B System.Boolean System.Linq.Enumerable/<>c__DisplayClass6_0`1::<CombinePredicates>b__0(TSource)
// 0x0000004C System.Void System.Linq.Enumerable/<>c__DisplayClass7_0`3::.ctor()
// 0x0000004D TResult System.Linq.Enumerable/<>c__DisplayClass7_0`3::<CombineSelectors>b__0(TSource)
// 0x0000004E System.Void System.Linq.Enumerable/<SelectManyIterator>d__17`2::.ctor(System.Int32)
// 0x0000004F System.Void System.Linq.Enumerable/<SelectManyIterator>d__17`2::System.IDisposable.Dispose()
// 0x00000050 System.Boolean System.Linq.Enumerable/<SelectManyIterator>d__17`2::MoveNext()
// 0x00000051 System.Void System.Linq.Enumerable/<SelectManyIterator>d__17`2::<>m__Finally1()
// 0x00000052 System.Void System.Linq.Enumerable/<SelectManyIterator>d__17`2::<>m__Finally2()
// 0x00000053 TResult System.Linq.Enumerable/<SelectManyIterator>d__17`2::System.Collections.Generic.IEnumerator<TResult>.get_Current()
// 0x00000054 System.Void System.Linq.Enumerable/<SelectManyIterator>d__17`2::System.Collections.IEnumerator.Reset()
// 0x00000055 System.Object System.Linq.Enumerable/<SelectManyIterator>d__17`2::System.Collections.IEnumerator.get_Current()
// 0x00000056 System.Collections.Generic.IEnumerator`1<TResult> System.Linq.Enumerable/<SelectManyIterator>d__17`2::System.Collections.Generic.IEnumerable<TResult>.GetEnumerator()
// 0x00000057 System.Collections.IEnumerator System.Linq.Enumerable/<SelectManyIterator>d__17`2::System.Collections.IEnumerable.GetEnumerator()
// 0x00000058 System.Void System.Linq.Enumerable/<DistinctIterator>d__68`1::.ctor(System.Int32)
// 0x00000059 System.Void System.Linq.Enumerable/<DistinctIterator>d__68`1::System.IDisposable.Dispose()
// 0x0000005A System.Boolean System.Linq.Enumerable/<DistinctIterator>d__68`1::MoveNext()
// 0x0000005B System.Void System.Linq.Enumerable/<DistinctIterator>d__68`1::<>m__Finally1()
// 0x0000005C TSource System.Linq.Enumerable/<DistinctIterator>d__68`1::System.Collections.Generic.IEnumerator<TSource>.get_Current()
// 0x0000005D System.Void System.Linq.Enumerable/<DistinctIterator>d__68`1::System.Collections.IEnumerator.Reset()
// 0x0000005E System.Object System.Linq.Enumerable/<DistinctIterator>d__68`1::System.Collections.IEnumerator.get_Current()
// 0x0000005F System.Collections.Generic.IEnumerator`1<TSource> System.Linq.Enumerable/<DistinctIterator>d__68`1::System.Collections.Generic.IEnumerable<TSource>.GetEnumerator()
// 0x00000060 System.Collections.IEnumerator System.Linq.Enumerable/<DistinctIterator>d__68`1::System.Collections.IEnumerable.GetEnumerator()
// 0x00000061 System.Void System.Linq.EmptyEnumerable`1::.cctor()
// 0x00000062 System.Linq.IOrderedEnumerable`1<TElement> System.Linq.IOrderedEnumerable`1::CreateOrderedEnumerable(System.Func`2<TElement,TKey>,System.Collections.Generic.IComparer`1<TKey>,System.Boolean)
// 0x00000063 System.Void System.Linq.Set`1::.ctor(System.Collections.Generic.IEqualityComparer`1<TElement>)
// 0x00000064 System.Boolean System.Linq.Set`1::Add(TElement)
// 0x00000065 System.Boolean System.Linq.Set`1::Find(TElement,System.Boolean)
// 0x00000066 System.Void System.Linq.Set`1::Resize()
// 0x00000067 System.Int32 System.Linq.Set`1::InternalGetHashCode(TElement)
// 0x00000068 System.Collections.Generic.IEnumerator`1<TElement> System.Linq.OrderedEnumerable`1::GetEnumerator()
// 0x00000069 System.Linq.EnumerableSorter`1<TElement> System.Linq.OrderedEnumerable`1::GetEnumerableSorter(System.Linq.EnumerableSorter`1<TElement>)
// 0x0000006A System.Collections.IEnumerator System.Linq.OrderedEnumerable`1::System.Collections.IEnumerable.GetEnumerator()
// 0x0000006B System.Linq.IOrderedEnumerable`1<TElement> System.Linq.OrderedEnumerable`1::System.Linq.IOrderedEnumerable<TElement>.CreateOrderedEnumerable(System.Func`2<TElement,TKey>,System.Collections.Generic.IComparer`1<TKey>,System.Boolean)
// 0x0000006C System.Void System.Linq.OrderedEnumerable`1::.ctor()
// 0x0000006D System.Void System.Linq.OrderedEnumerable`1/<GetEnumerator>d__1::.ctor(System.Int32)
// 0x0000006E System.Void System.Linq.OrderedEnumerable`1/<GetEnumerator>d__1::System.IDisposable.Dispose()
// 0x0000006F System.Boolean System.Linq.OrderedEnumerable`1/<GetEnumerator>d__1::MoveNext()
// 0x00000070 TElement System.Linq.OrderedEnumerable`1/<GetEnumerator>d__1::System.Collections.Generic.IEnumerator<TElement>.get_Current()
// 0x00000071 System.Void System.Linq.OrderedEnumerable`1/<GetEnumerator>d__1::System.Collections.IEnumerator.Reset()
// 0x00000072 System.Object System.Linq.OrderedEnumerable`1/<GetEnumerator>d__1::System.Collections.IEnumerator.get_Current()
// 0x00000073 System.Void System.Linq.OrderedEnumerable`2::.ctor(System.Collections.Generic.IEnumerable`1<TElement>,System.Func`2<TElement,TKey>,System.Collections.Generic.IComparer`1<TKey>,System.Boolean)
// 0x00000074 System.Linq.EnumerableSorter`1<TElement> System.Linq.OrderedEnumerable`2::GetEnumerableSorter(System.Linq.EnumerableSorter`1<TElement>)
// 0x00000075 System.Void System.Linq.EnumerableSorter`1::ComputeKeys(TElement[],System.Int32)
// 0x00000076 System.Int32 System.Linq.EnumerableSorter`1::CompareKeys(System.Int32,System.Int32)
// 0x00000077 System.Int32[] System.Linq.EnumerableSorter`1::Sort(TElement[],System.Int32)
// 0x00000078 System.Void System.Linq.EnumerableSorter`1::QuickSort(System.Int32[],System.Int32,System.Int32)
// 0x00000079 System.Void System.Linq.EnumerableSorter`1::.ctor()
// 0x0000007A System.Void System.Linq.EnumerableSorter`2::.ctor(System.Func`2<TElement,TKey>,System.Collections.Generic.IComparer`1<TKey>,System.Boolean,System.Linq.EnumerableSorter`1<TElement>)
// 0x0000007B System.Void System.Linq.EnumerableSorter`2::ComputeKeys(TElement[],System.Int32)
// 0x0000007C System.Int32 System.Linq.EnumerableSorter`2::CompareKeys(System.Int32,System.Int32)
// 0x0000007D System.Void System.Linq.Buffer`1::.ctor(System.Collections.Generic.IEnumerable`1<TElement>)
// 0x0000007E TElement[] System.Linq.Buffer`1::ToArray()
// 0x0000007F System.Void System.Collections.Generic.HashSet`1::.ctor()
// 0x00000080 System.Void System.Collections.Generic.HashSet`1::.ctor(System.Collections.Generic.IEqualityComparer`1<T>)
// 0x00000081 System.Void System.Collections.Generic.HashSet`1::.ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
// 0x00000082 System.Void System.Collections.Generic.HashSet`1::System.Collections.Generic.ICollection<T>.Add(T)
// 0x00000083 System.Void System.Collections.Generic.HashSet`1::Clear()
// 0x00000084 System.Boolean System.Collections.Generic.HashSet`1::Contains(T)
// 0x00000085 System.Void System.Collections.Generic.HashSet`1::CopyTo(T[],System.Int32)
// 0x00000086 System.Boolean System.Collections.Generic.HashSet`1::Remove(T)
// 0x00000087 System.Int32 System.Collections.Generic.HashSet`1::get_Count()
// 0x00000088 System.Boolean System.Collections.Generic.HashSet`1::System.Collections.Generic.ICollection<T>.get_IsReadOnly()
// 0x00000089 System.Collections.Generic.HashSet`1/Enumerator<T> System.Collections.Generic.HashSet`1::GetEnumerator()
// 0x0000008A System.Collections.Generic.IEnumerator`1<T> System.Collections.Generic.HashSet`1::System.Collections.Generic.IEnumerable<T>.GetEnumerator()
// 0x0000008B System.Collections.IEnumerator System.Collections.Generic.HashSet`1::System.Collections.IEnumerable.GetEnumerator()
// 0x0000008C System.Void System.Collections.Generic.HashSet`1::GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)
// 0x0000008D System.Void System.Collections.Generic.HashSet`1::OnDeserialization(System.Object)
// 0x0000008E System.Boolean System.Collections.Generic.HashSet`1::Add(T)
// 0x0000008F System.Void System.Collections.Generic.HashSet`1::CopyTo(T[])
// 0x00000090 System.Void System.Collections.Generic.HashSet`1::CopyTo(T[],System.Int32,System.Int32)
// 0x00000091 System.Void System.Collections.Generic.HashSet`1::Initialize(System.Int32)
// 0x00000092 System.Void System.Collections.Generic.HashSet`1::IncreaseCapacity()
// 0x00000093 System.Void System.Collections.Generic.HashSet`1::SetCapacity(System.Int32)
// 0x00000094 System.Boolean System.Collections.Generic.HashSet`1::AddIfNotPresent(T)
// 0x00000095 System.Int32 System.Collections.Generic.HashSet`1::InternalGetHashCode(T)
// 0x00000096 System.Void System.Collections.Generic.HashSet`1/Enumerator::.ctor(System.Collections.Generic.HashSet`1<T>)
// 0x00000097 System.Void System.Collections.Generic.HashSet`1/Enumerator::Dispose()
// 0x00000098 System.Boolean System.Collections.Generic.HashSet`1/Enumerator::MoveNext()
// 0x00000099 T System.Collections.Generic.HashSet`1/Enumerator::get_Current()
// 0x0000009A System.Object System.Collections.Generic.HashSet`1/Enumerator::System.Collections.IEnumerator.get_Current()
// 0x0000009B System.Void System.Collections.Generic.HashSet`1/Enumerator::System.Collections.IEnumerator.Reset()
static Il2CppMethodPointer s_methodPointers[155] = 
{
	Error_ArgumentNull_m0EDA0D46D72CA692518E3E2EB75B48044D8FD41E,
	Error_ArgumentOutOfRange_m2EFB999454161A6B48F8DAC3753FDC190538F0F2,
	Error_MoreThanOneMatch_m4C4756AF34A76EF12F3B2B6D8C78DE547F0FBCF8,
	Error_NoElements_mB89E91246572F009281D79730950808F17C3F353,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
};
static const int32_t s_InvokerIndices[155] = 
{
	5611,
	5611,
	5813,
	5813,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
	-1,
};
static const Il2CppTokenRangePair s_rgctxIndices[54] = 
{
	{ 0x02000004, { 85, 4 } },
	{ 0x02000005, { 89, 9 } },
	{ 0x02000006, { 100, 7 } },
	{ 0x02000007, { 109, 10 } },
	{ 0x02000008, { 121, 11 } },
	{ 0x02000009, { 135, 9 } },
	{ 0x0200000A, { 147, 12 } },
	{ 0x0200000B, { 162, 1 } },
	{ 0x0200000C, { 163, 2 } },
	{ 0x0200000D, { 165, 12 } },
	{ 0x0200000E, { 177, 11 } },
	{ 0x0200000F, { 188, 2 } },
	{ 0x02000011, { 190, 8 } },
	{ 0x02000013, { 198, 3 } },
	{ 0x02000014, { 203, 5 } },
	{ 0x02000015, { 208, 7 } },
	{ 0x02000016, { 215, 3 } },
	{ 0x02000017, { 218, 7 } },
	{ 0x02000018, { 225, 4 } },
	{ 0x02000019, { 229, 21 } },
	{ 0x0200001B, { 250, 2 } },
	{ 0x06000005, { 0, 10 } },
	{ 0x06000006, { 10, 10 } },
	{ 0x06000007, { 20, 5 } },
	{ 0x06000008, { 25, 5 } },
	{ 0x06000009, { 30, 1 } },
	{ 0x0600000A, { 31, 2 } },
	{ 0x0600000B, { 33, 2 } },
	{ 0x0600000C, { 35, 1 } },
	{ 0x0600000D, { 36, 1 } },
	{ 0x0600000E, { 37, 2 } },
	{ 0x0600000F, { 39, 3 } },
	{ 0x06000010, { 42, 2 } },
	{ 0x06000011, { 44, 4 } },
	{ 0x06000012, { 48, 4 } },
	{ 0x06000013, { 52, 3 } },
	{ 0x06000014, { 55, 4 } },
	{ 0x06000015, { 59, 3 } },
	{ 0x06000016, { 62, 3 } },
	{ 0x06000017, { 65, 1 } },
	{ 0x06000018, { 66, 1 } },
	{ 0x06000019, { 67, 3 } },
	{ 0x0600001A, { 70, 3 } },
	{ 0x0600001B, { 73, 2 } },
	{ 0x0600001C, { 75, 3 } },
	{ 0x0600001D, { 78, 2 } },
	{ 0x0600001E, { 80, 5 } },
	{ 0x0600002E, { 98, 2 } },
	{ 0x06000033, { 107, 2 } },
	{ 0x06000038, { 119, 2 } },
	{ 0x0600003E, { 132, 3 } },
	{ 0x06000043, { 144, 3 } },
	{ 0x06000048, { 159, 3 } },
	{ 0x0600006B, { 201, 2 } },
};
static const Il2CppRGCTXDefinition s_rgctxValues[252] = 
{
	{ (Il2CppRGCTXDataType)2, 2631 },
	{ (Il2CppRGCTXDataType)3, 14044 },
	{ (Il2CppRGCTXDataType)2, 4395 },
	{ (Il2CppRGCTXDataType)2, 3710 },
	{ (Il2CppRGCTXDataType)3, 25434 },
	{ (Il2CppRGCTXDataType)2, 2788 },
	{ (Il2CppRGCTXDataType)2, 3717 },
	{ (Il2CppRGCTXDataType)3, 25490 },
	{ (Il2CppRGCTXDataType)2, 3712 },
	{ (Il2CppRGCTXDataType)3, 25446 },
	{ (Il2CppRGCTXDataType)2, 2632 },
	{ (Il2CppRGCTXDataType)3, 14045 },
	{ (Il2CppRGCTXDataType)2, 4423 },
	{ (Il2CppRGCTXDataType)2, 3719 },
	{ (Il2CppRGCTXDataType)3, 25502 },
	{ (Il2CppRGCTXDataType)2, 2807 },
	{ (Il2CppRGCTXDataType)2, 3727 },
	{ (Il2CppRGCTXDataType)3, 25682 },
	{ (Il2CppRGCTXDataType)2, 3723 },
	{ (Il2CppRGCTXDataType)3, 25584 },
	{ (Il2CppRGCTXDataType)2, 902 },
	{ (Il2CppRGCTXDataType)3, 68 },
	{ (Il2CppRGCTXDataType)3, 69 },
	{ (Il2CppRGCTXDataType)2, 1677 },
	{ (Il2CppRGCTXDataType)3, 10452 },
	{ (Il2CppRGCTXDataType)2, 903 },
	{ (Il2CppRGCTXDataType)3, 84 },
	{ (Il2CppRGCTXDataType)3, 85 },
	{ (Il2CppRGCTXDataType)2, 1690 },
	{ (Il2CppRGCTXDataType)3, 10459 },
	{ (Il2CppRGCTXDataType)3, 29573 },
	{ (Il2CppRGCTXDataType)2, 910 },
	{ (Il2CppRGCTXDataType)3, 169 },
	{ (Il2CppRGCTXDataType)2, 3283 },
	{ (Il2CppRGCTXDataType)3, 20617 },
	{ (Il2CppRGCTXDataType)3, 11461 },
	{ (Il2CppRGCTXDataType)3, 29526 },
	{ (Il2CppRGCTXDataType)2, 906 },
	{ (Il2CppRGCTXDataType)3, 129 },
	{ (Il2CppRGCTXDataType)2, 1076 },
	{ (Il2CppRGCTXDataType)3, 1457 },
	{ (Il2CppRGCTXDataType)3, 1458 },
	{ (Il2CppRGCTXDataType)2, 2789 },
	{ (Il2CppRGCTXDataType)3, 15214 },
	{ (Il2CppRGCTXDataType)2, 2432 },
	{ (Il2CppRGCTXDataType)2, 1868 },
	{ (Il2CppRGCTXDataType)2, 1998 },
	{ (Il2CppRGCTXDataType)2, 2124 },
	{ (Il2CppRGCTXDataType)2, 2433 },
	{ (Il2CppRGCTXDataType)2, 1869 },
	{ (Il2CppRGCTXDataType)2, 1999 },
	{ (Il2CppRGCTXDataType)2, 2125 },
	{ (Il2CppRGCTXDataType)2, 2000 },
	{ (Il2CppRGCTXDataType)2, 2126 },
	{ (Il2CppRGCTXDataType)3, 10454 },
	{ (Il2CppRGCTXDataType)2, 2434 },
	{ (Il2CppRGCTXDataType)2, 1870 },
	{ (Il2CppRGCTXDataType)2, 2001 },
	{ (Il2CppRGCTXDataType)2, 2127 },
	{ (Il2CppRGCTXDataType)2, 2002 },
	{ (Il2CppRGCTXDataType)2, 2128 },
	{ (Il2CppRGCTXDataType)3, 10455 },
	{ (Il2CppRGCTXDataType)2, 2431 },
	{ (Il2CppRGCTXDataType)2, 1997 },
	{ (Il2CppRGCTXDataType)2, 2123 },
	{ (Il2CppRGCTXDataType)2, 1435 },
	{ (Il2CppRGCTXDataType)2, 1986 },
	{ (Il2CppRGCTXDataType)2, 1987 },
	{ (Il2CppRGCTXDataType)2, 2120 },
	{ (Il2CppRGCTXDataType)3, 10451 },
	{ (Il2CppRGCTXDataType)2, 1985 },
	{ (Il2CppRGCTXDataType)2, 2119 },
	{ (Il2CppRGCTXDataType)3, 10450 },
	{ (Il2CppRGCTXDataType)2, 1867 },
	{ (Il2CppRGCTXDataType)2, 1995 },
	{ (Il2CppRGCTXDataType)2, 1996 },
	{ (Il2CppRGCTXDataType)2, 2122 },
	{ (Il2CppRGCTXDataType)3, 10453 },
	{ (Il2CppRGCTXDataType)2, 1866 },
	{ (Il2CppRGCTXDataType)3, 29500 },
	{ (Il2CppRGCTXDataType)3, 9510 },
	{ (Il2CppRGCTXDataType)2, 1546 },
	{ (Il2CppRGCTXDataType)2, 1989 },
	{ (Il2CppRGCTXDataType)2, 2121 },
	{ (Il2CppRGCTXDataType)2, 2234 },
	{ (Il2CppRGCTXDataType)3, 14046 },
	{ (Il2CppRGCTXDataType)3, 14048 },
	{ (Il2CppRGCTXDataType)2, 661 },
	{ (Il2CppRGCTXDataType)3, 14047 },
	{ (Il2CppRGCTXDataType)3, 14056 },
	{ (Il2CppRGCTXDataType)2, 2635 },
	{ (Il2CppRGCTXDataType)2, 3713 },
	{ (Il2CppRGCTXDataType)3, 25447 },
	{ (Il2CppRGCTXDataType)3, 14057 },
	{ (Il2CppRGCTXDataType)2, 2051 },
	{ (Il2CppRGCTXDataType)2, 2159 },
	{ (Il2CppRGCTXDataType)3, 10467 },
	{ (Il2CppRGCTXDataType)3, 29464 },
	{ (Il2CppRGCTXDataType)2, 3724 },
	{ (Il2CppRGCTXDataType)3, 25585 },
	{ (Il2CppRGCTXDataType)3, 14049 },
	{ (Il2CppRGCTXDataType)2, 2634 },
	{ (Il2CppRGCTXDataType)2, 3711 },
	{ (Il2CppRGCTXDataType)3, 25435 },
	{ (Il2CppRGCTXDataType)3, 10466 },
	{ (Il2CppRGCTXDataType)3, 14050 },
	{ (Il2CppRGCTXDataType)3, 29463 },
	{ (Il2CppRGCTXDataType)2, 3720 },
	{ (Il2CppRGCTXDataType)3, 25503 },
	{ (Il2CppRGCTXDataType)3, 14063 },
	{ (Il2CppRGCTXDataType)2, 2636 },
	{ (Il2CppRGCTXDataType)2, 3718 },
	{ (Il2CppRGCTXDataType)3, 25491 },
	{ (Il2CppRGCTXDataType)3, 15265 },
	{ (Il2CppRGCTXDataType)3, 7673 },
	{ (Il2CppRGCTXDataType)3, 10468 },
	{ (Il2CppRGCTXDataType)3, 7672 },
	{ (Il2CppRGCTXDataType)3, 14064 },
	{ (Il2CppRGCTXDataType)3, 29465 },
	{ (Il2CppRGCTXDataType)2, 3728 },
	{ (Il2CppRGCTXDataType)3, 25683 },
	{ (Il2CppRGCTXDataType)3, 14077 },
	{ (Il2CppRGCTXDataType)2, 2638 },
	{ (Il2CppRGCTXDataType)2, 3726 },
	{ (Il2CppRGCTXDataType)3, 25587 },
	{ (Il2CppRGCTXDataType)3, 14078 },
	{ (Il2CppRGCTXDataType)2, 2054 },
	{ (Il2CppRGCTXDataType)2, 2162 },
	{ (Il2CppRGCTXDataType)3, 10472 },
	{ (Il2CppRGCTXDataType)3, 10471 },
	{ (Il2CppRGCTXDataType)2, 3715 },
	{ (Il2CppRGCTXDataType)3, 25449 },
	{ (Il2CppRGCTXDataType)3, 29474 },
	{ (Il2CppRGCTXDataType)2, 3725 },
	{ (Il2CppRGCTXDataType)3, 25586 },
	{ (Il2CppRGCTXDataType)3, 14070 },
	{ (Il2CppRGCTXDataType)2, 2637 },
	{ (Il2CppRGCTXDataType)2, 3722 },
	{ (Il2CppRGCTXDataType)3, 25505 },
	{ (Il2CppRGCTXDataType)3, 10470 },
	{ (Il2CppRGCTXDataType)3, 10469 },
	{ (Il2CppRGCTXDataType)3, 14071 },
	{ (Il2CppRGCTXDataType)2, 3714 },
	{ (Il2CppRGCTXDataType)3, 25448 },
	{ (Il2CppRGCTXDataType)3, 29473 },
	{ (Il2CppRGCTXDataType)2, 3721 },
	{ (Il2CppRGCTXDataType)3, 25504 },
	{ (Il2CppRGCTXDataType)3, 14084 },
	{ (Il2CppRGCTXDataType)2, 2639 },
	{ (Il2CppRGCTXDataType)2, 3730 },
	{ (Il2CppRGCTXDataType)3, 25685 },
	{ (Il2CppRGCTXDataType)3, 15266 },
	{ (Il2CppRGCTXDataType)3, 7675 },
	{ (Il2CppRGCTXDataType)3, 10474 },
	{ (Il2CppRGCTXDataType)3, 10473 },
	{ (Il2CppRGCTXDataType)3, 7674 },
	{ (Il2CppRGCTXDataType)3, 14085 },
	{ (Il2CppRGCTXDataType)2, 3716 },
	{ (Il2CppRGCTXDataType)3, 25450 },
	{ (Il2CppRGCTXDataType)3, 29475 },
	{ (Il2CppRGCTXDataType)2, 3729 },
	{ (Il2CppRGCTXDataType)3, 25684 },
	{ (Il2CppRGCTXDataType)3, 10463 },
	{ (Il2CppRGCTXDataType)3, 10464 },
	{ (Il2CppRGCTXDataType)3, 10475 },
	{ (Il2CppRGCTXDataType)3, 172 },
	{ (Il2CppRGCTXDataType)3, 171 },
	{ (Il2CppRGCTXDataType)2, 2046 },
	{ (Il2CppRGCTXDataType)2, 2155 },
	{ (Il2CppRGCTXDataType)3, 10465 },
	{ (Il2CppRGCTXDataType)2, 2061 },
	{ (Il2CppRGCTXDataType)2, 2177 },
	{ (Il2CppRGCTXDataType)3, 174 },
	{ (Il2CppRGCTXDataType)2, 827 },
	{ (Il2CppRGCTXDataType)2, 911 },
	{ (Il2CppRGCTXDataType)3, 170 },
	{ (Il2CppRGCTXDataType)3, 173 },
	{ (Il2CppRGCTXDataType)3, 131 },
	{ (Il2CppRGCTXDataType)2, 3432 },
	{ (Il2CppRGCTXDataType)3, 22943 },
	{ (Il2CppRGCTXDataType)2, 2043 },
	{ (Il2CppRGCTXDataType)2, 2153 },
	{ (Il2CppRGCTXDataType)3, 22944 },
	{ (Il2CppRGCTXDataType)3, 133 },
	{ (Il2CppRGCTXDataType)2, 658 },
	{ (Il2CppRGCTXDataType)2, 907 },
	{ (Il2CppRGCTXDataType)3, 130 },
	{ (Il2CppRGCTXDataType)3, 132 },
	{ (Il2CppRGCTXDataType)2, 4433 },
	{ (Il2CppRGCTXDataType)2, 1436 },
	{ (Il2CppRGCTXDataType)3, 9548 },
	{ (Il2CppRGCTXDataType)2, 1562 },
	{ (Il2CppRGCTXDataType)2, 4530 },
	{ (Il2CppRGCTXDataType)3, 22940 },
	{ (Il2CppRGCTXDataType)3, 22941 },
	{ (Il2CppRGCTXDataType)2, 2249 },
	{ (Il2CppRGCTXDataType)3, 22942 },
	{ (Il2CppRGCTXDataType)2, 579 },
	{ (Il2CppRGCTXDataType)2, 908 },
	{ (Il2CppRGCTXDataType)3, 143 },
	{ (Il2CppRGCTXDataType)3, 20604 },
	{ (Il2CppRGCTXDataType)2, 3284 },
	{ (Il2CppRGCTXDataType)3, 20618 },
	{ (Il2CppRGCTXDataType)2, 1077 },
	{ (Il2CppRGCTXDataType)3, 1459 },
	{ (Il2CppRGCTXDataType)3, 20610 },
	{ (Il2CppRGCTXDataType)3, 7644 },
	{ (Il2CppRGCTXDataType)2, 697 },
	{ (Il2CppRGCTXDataType)3, 20605 },
	{ (Il2CppRGCTXDataType)2, 3280 },
	{ (Il2CppRGCTXDataType)3, 1879 },
	{ (Il2CppRGCTXDataType)2, 1096 },
	{ (Il2CppRGCTXDataType)2, 1507 },
	{ (Il2CppRGCTXDataType)3, 7650 },
	{ (Il2CppRGCTXDataType)3, 20606 },
	{ (Il2CppRGCTXDataType)3, 7639 },
	{ (Il2CppRGCTXDataType)3, 7640 },
	{ (Il2CppRGCTXDataType)3, 7638 },
	{ (Il2CppRGCTXDataType)3, 7641 },
	{ (Il2CppRGCTXDataType)2, 1503 },
	{ (Il2CppRGCTXDataType)2, 4483 },
	{ (Il2CppRGCTXDataType)3, 10461 },
	{ (Il2CppRGCTXDataType)3, 7643 },
	{ (Il2CppRGCTXDataType)2, 1962 },
	{ (Il2CppRGCTXDataType)3, 7642 },
	{ (Il2CppRGCTXDataType)2, 1872 },
	{ (Il2CppRGCTXDataType)2, 4427 },
	{ (Il2CppRGCTXDataType)2, 2017 },
	{ (Il2CppRGCTXDataType)2, 2131 },
	{ (Il2CppRGCTXDataType)3, 9529 },
	{ (Il2CppRGCTXDataType)2, 1555 },
	{ (Il2CppRGCTXDataType)3, 11287 },
	{ (Il2CppRGCTXDataType)3, 11288 },
	{ (Il2CppRGCTXDataType)3, 11293 },
	{ (Il2CppRGCTXDataType)2, 2243 },
	{ (Il2CppRGCTXDataType)3, 11290 },
	{ (Il2CppRGCTXDataType)3, 30218 },
	{ (Il2CppRGCTXDataType)2, 1509 },
	{ (Il2CppRGCTXDataType)3, 7664 },
	{ (Il2CppRGCTXDataType)1, 1957 },
	{ (Il2CppRGCTXDataType)2, 4439 },
	{ (Il2CppRGCTXDataType)3, 11289 },
	{ (Il2CppRGCTXDataType)1, 4439 },
	{ (Il2CppRGCTXDataType)1, 2243 },
	{ (Il2CppRGCTXDataType)2, 4528 },
	{ (Il2CppRGCTXDataType)2, 4439 },
	{ (Il2CppRGCTXDataType)3, 11294 },
	{ (Il2CppRGCTXDataType)3, 11292 },
	{ (Il2CppRGCTXDataType)3, 11291 },
	{ (Il2CppRGCTXDataType)2, 497 },
	{ (Il2CppRGCTXDataType)3, 7676 },
	{ (Il2CppRGCTXDataType)2, 671 },
};
extern const CustomAttributesCacheGenerator g_System_Core_AttributeGenerators[];
IL2CPP_EXTERN_C const Il2CppCodeGenModule g_System_Core_CodeGenModule;
const Il2CppCodeGenModule g_System_Core_CodeGenModule = 
{
	"System.Core.dll",
	155,
	s_methodPointers,
	0,
	NULL,
	s_InvokerIndices,
	0,
	NULL,
	54,
	s_rgctxIndices,
	252,
	s_rgctxValues,
	NULL,
	g_System_Core_AttributeGenerators,
	NULL, // module initializer,
	NULL,
	NULL,
	NULL,
};
