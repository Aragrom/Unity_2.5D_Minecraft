using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Burst;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

/*
Available in release build:
Total Used Memory
Total Reserved Memory
GC Used Memory
GC Reserved Memory
Gfx Used Memory
Gfx Reserved Memory
Audio Used Memory
Audio Reserved Memory
Video Used Memory
Video Reserved Memory
Profiler Used Memory
Profiler Reserved Memory
System Used Memory

NOT available in release build:
Texture Count
Texture Memory
Mesh Count
Mesh Memory
Material Count
Material Memory
AnimationClip Count
AnimationClip Memory
Asset Count
GameObjects in Scenes
Total Objects in Scenes
Total Unity Object Count
GC Allocation In Frame Count
GC Allocated In Frame
 */

/*
Used Total 
- memory used right now if we don't take pools and allocation headers data into account. 
Essentially it is sum of all internally managed allocations which are not freed yet. 
Profiler.GetTotalAllocatedMemoryLong is the Scripting API getter.

Reserved Total
- is how much we took from the system. This includes all allocated memory including pools
and extra allocation headers. In our case it is commit memory - we allocate on heap 
(reserve+commit). Includes Used Total. Scripting API equivalent - Profiler.GetTotalReservedMemoryLong.

Total System Memory Usage
- total size of used (commit) memory by Unity process. This includes everything what
is used by the process - Reserved Total + executable's images (code and data). It is 
PrivateUsage of PROCESS_MEMORY_COUNTERS_EX on Windows.

Taking this into account in your case I would say that if Reserved Total >1GB player
might definitely run out of memory on end-goal device. If TSMU>1GB system might page
some some memory to disk, but that's not guaranteed and it might make performance
horrible. So it is always safer to have TSMU<1GB.
I would say TSMU is what should be mentioned as Required Memory in System Requirements
for your game.
(That's assuming you have TSMU data from the player.)
 */

[BurstCompile]
public class MemoryProfiling : MonoBehaviour
{
    string _statsText;

    public long totalAllocatedMemoryLong;
    public long totalReservedMemoryLong;
    public long totalUnusedReservedMemoryLong;

    public long allocatedMemoryForGraphicsDriver;

    public long monoHeapSizeLong;
    public long monoUsedSizeLong;

    // In build/release
    ProfilerRecorder totalAllocatedMemoryRecorder;
    ProfilerRecorder textureMemoryRecorder;
    ProfilerRecorder meshMemoryRecorder;
    ProfilerRecorder totalReservedMemoryRecorder;
    ProfilerRecorder gcReservedMemoryRecorder;

    // editor only
    ProfilerRecorder materialMemoryRecorder;
    ProfilerRecorder objectMemoryRecorder;

    public TMP_Text tmpTextStats = null;

    [BurstCompile]
    private void OnDestroy()
    {
        // Free memory

        tmpTextStats = null;
    }

    /*[BurstCompile]
    private void Awake()
    {
        tmpTextStats = GameObject.Find("Canvas Stats").transform.Find("Text Stats").GetComponent<TMP_Text>();
    }*/

    [BurstCompile]
    void OnEnable()
    {
        totalAllocatedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
        textureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Texture Memory");
        meshMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Mesh Memory");
        totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");

        materialMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Material Count");
        objectMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Object Count");
    }

    [BurstCompile]
    void OnDisable()
    {
        //totalAllocatedMemoryRecorder.Dispose();
        textureMemoryRecorder.Dispose();
        meshMemoryRecorder.Dispose();
        totalReservedMemoryRecorder.Dispose();
        gcReservedMemoryRecorder.Dispose();

        materialMemoryRecorder.Dispose();
        objectMemoryRecorder.Dispose();
    }

    [BurstCompile]
    void Update()
    {
        totalAllocatedMemoryLong = Profiler.GetTotalAllocatedMemoryLong();
        totalReservedMemoryLong = Profiler.GetTotalReservedMemoryLong();
        totalUnusedReservedMemoryLong = Profiler.GetTotalUnusedReservedMemoryLong();
        allocatedMemoryForGraphicsDriver = Profiler.GetAllocatedMemoryForGraphicsDriver();
        monoHeapSizeLong = Profiler.GetMonoHeapSizeLong();
        monoUsedSizeLong = Profiler.GetMonoUsedSizeLong();

        var sb = new StringBuilder(500);
        if (totalAllocatedMemoryRecorder.Valid)
            sb.AppendLine($"Total Allocated Memory (MB) {((((totalAllocatedMemoryRecorder.LastValue)/ 8f) / 1024f) / 1024f)}");
        if (textureMemoryRecorder.Valid)
            sb.AppendLine($"Texture Used Memory (MB) {((((textureMemoryRecorder.LastValue)/ 8f) / 1024f) / 1024f)}");
        if (meshMemoryRecorder.Valid)
            sb.AppendLine($"Mesh Used Memory (MB) {((((meshMemoryRecorder.LastValue)/ 8f) / 1024f) / 1024f)}");
        if (totalReservedMemoryRecorder.Valid)
            sb.AppendLine($"Total Reserved Memory (MB) {((((totalReservedMemoryRecorder.LastValue)/ 8f) / 1024f) / 1024f)}");
        if (gcReservedMemoryRecorder.Valid)
            sb.AppendLine($"GC Reserved Memory (MB) {((((gcReservedMemoryRecorder.LastValue)/ 8f) / 1024f) / 1024f)}");
        
        sb.AppendLine($"- Total Allocated (MB) {((((totalAllocatedMemoryLong)/ 8f) / 1024f) / 1024f)}");
        sb.AppendLine($"- Total Reserved (MB) {((((totalReservedMemoryLong) / 8f) / 1024f) / 1024f)}");
        sb.AppendLine($"- Total Unused Reserved (MB) {((((totalUnusedReservedMemoryLong) / 8f) / 1024f) / 1024f)}");
        sb.AppendLine($"- Allocated Graphics (MB) {((((allocatedMemoryForGraphicsDriver) / 8f) / 1024f) / 1024f)}");
        sb.AppendLine($"- Mono Heap (MB) {((((monoHeapSizeLong) / 8f) / 1024f) / 1024f)}");
        sb.AppendLine($"- Mono Used (MB) {((((monoUsedSizeLong) / 8f) / 1024f) / 1024f)}");

        if (materialMemoryRecorder.Valid)
            sb.AppendLine($"- Material Count {materialMemoryRecorder.LastValue}");
        if (objectMemoryRecorder.Valid)
            sb.AppendLine($"- Object Count {objectMemoryRecorder.LastValue}");
        _statsText = sb.ToString();

        tmpTextStats.text = _statsText;
    }
}
