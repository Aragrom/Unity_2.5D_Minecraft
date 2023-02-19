using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

// The larger cross section to cover most sub chunks should be attached to the chunk. (moved up and down manually)
// All sub chunks that

[BurstCompile]
public class SurroundingCrossSectionEngine : MonoBehaviour
{
    public Main main = null;

    public LargeSurroundingCrossSectionPool largeSurroundingCrossSectionPool = null;
    public SurroundingCrossSectionPool surroundingCrossSectionPool = null;    // The pool of all sub chunk cross sections.
    public SurroundingCrossSectionMap SurroundingCrossSectionMap = null;
    public NewSurroundingCrossSectionQueue newSurroundingCrossSectionQueue = null;
    public RawSurroundingCrossSectionData rawSurroundingCrossSectionData = null;

    public bool on = false;
    public bool setUp = false;
    public bool isGenerating = false;

    [BurstCompile]
    private void OnDestroy()
    {
        largeSurroundingCrossSectionPool = null;
        surroundingCrossSectionPool = null;    // The pool of all sub chunk cross sections.
        SurroundingCrossSectionMap = null;
        newSurroundingCrossSectionQueue = null;
        rawSurroundingCrossSectionData = null;
    }

    [BurstCompile]
    private void Awake()
    {
        main = GameObject.Find("Main").GetComponent<Main>();

        largeSurroundingCrossSectionPool = GetComponent<LargeSurroundingCrossSectionPool>();
        surroundingCrossSectionPool = GetComponent<SurroundingCrossSectionPool>();
        SurroundingCrossSectionMap = GetComponent<SurroundingCrossSectionMap>();
        newSurroundingCrossSectionQueue = GetComponent<NewSurroundingCrossSectionQueue>();
        rawSurroundingCrossSectionData = GetComponent<RawSurroundingCrossSectionData>();
    }

    // Start is called before the first frame update
    [BurstCompile]
    void Start()
    {

    }

    // Update is called once per frame
    [BurstCompile]
    // Update is called once per frame
    void Update()
    {
        if (!on && main.terrainEngine.setUp)
        {
            StartCoroutineFreshWorld();
        }

        if (on && setUp)
        {

        }
    }

    [BurstCompile]
    public void StartCoroutineFreshWorld()
    {
        on = true;  // Show cross section engine to be "on"

        StartCoroutine(GenerateFresh());
    }

    [BurstCompile]
    public IEnumerator GenerateFresh()
    {
        yield return null;
    }
}
