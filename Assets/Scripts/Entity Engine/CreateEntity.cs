//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
//using Unity.Transforms;
//using Unity.Rendering;
//using Unity.Mathematics;
//using Unity.Entities;
//using Unity.Physics;
//using Unity.Collections;

// YOU HAVE NOT YET TRIED MAKE TERRAIN WERE THE SIZE OF ALL DATA NEVER CHANGES. ENTER EXTRA DATA IF NEEDED TO MAKE THE SIZE THE SAME. SO EMPTY TRIANGLE SPACE BUT SOME WIERD DATA IN OR SOMETHING!!!

/// <summary>
/// Used to create "pure ecs" entities.
/// </summary>

[BurstCompile]
public class CreateEntity : MonoBehaviour
{/*
    public EntityEngine entityEngine = null;

    public Entity prefab;

    //private EntityManager entityManager;
    //private World defaultWorld;

    Dictionary<float3, Entity> entities = new Dictionary<float3, Entity>();

    [SerializeField] public Mesh mesh;
    [SerializeField] public UnityEngine.Material material;

    private EntityArchetype entityArchetype;

    [BurstCompile]
    // Start is called before the first frame update
    void Start()
    {
        entityEngine = GetComponent<EntityEngine>();

        CreatePrefabEntity();
    }

    [BurstCompile]
    public void CreatePrefabEntity()
    {
        entityArchetype = CreateArchetype();
        prefab = CreateEntityPrefab(entityArchetype);

        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 0), 1);
        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 1), 1);
        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 2), 1);
        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 3), 1);
        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 4), 1);
        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 5), 1);
        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 6), 1);
        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 7), 1);
        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 8), 1);
        CreateEntityUsingEntityPrefab(prefab, new float3(0, 260, 9), 1);
    }

    [BurstCompile]
    private EntityArchetype CreateArchetype()
    {
        return World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld),
            typeof(Prefab),

            typeof(Scale),

            //typeof(PhysicsCollider),

            //typeof(PhysicsVelocity),
            //typeof(PhysicsGravityFactor),
            //typeof(PhysicsMass),
            //typeof(PhysicsDamping),

            typeof(PerInstanceCullingTag)
            );
    }

    [BurstCompile]
    public Entity CreateEntityPrefab(EntityArchetype entityArchetype)
    {
        Entity prefab = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(entityArchetype);

        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(prefab, new Translation
        {
            Value = new float3(0, 0, 0)
        });

        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(prefab, new Rotation
        {
            Value = new quaternion()
        });

        World.DefaultGameObjectInjectionWorld.EntityManager.SetSharedComponentData(prefab, new RenderMesh
        {
            mesh = this.mesh,
            material = this.material
        });

        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(prefab, new RenderBounds
        {
            Value = mesh.bounds.ToAABB()
        });

        //World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, new Translation { Value = new float3(position.x, position.y, position.z) });
        //World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, new Rotation { Value = new float4(0f, 0f, 0f, 1f) });

        //World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, new PhysicsVelocity
        //{
        //Angular = new float3(0, 0, 0),
        //Linear = new float3(0, 0, 0)
        //});

        //World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, new PhysicsGravityFactor { Value = 1.0f });


        //World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, new PhysicsMass
        //{
        //Transform = RigidTransform.identity,
        //CenterOfMass = new float3(0f, 0f, 0f),
        //InverseMass = 0.0f
        //});

        //var physMaterial = new Unity.Physics.Material
        //{
        //CustomTags = Unity.Physics.Material.Default.CustomTags,
        //Flags = Unity.Physics.Material.MaterialFlags.EnableCollisionEvents |
        // Unity.Physics.Material.MaterialFlags.EnableMassFactors |
        //Unity.Physics.Material.MaterialFlags.EnableSurfaceVelocity,
        //Friction = 1.0f,
        //FrictionCombinePolicy = Unity.Physics.Material.Default.FrictionCombinePolicy,
        //Restitution = 1.0f,
        //RestitutionCombinePolicy = Unity.Physics.Material.Default.RestitutionCombinePolicy,
        //};

        //World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, new PhysicsCollider
        //{
        //Value = Unity.Physics.SphereCollider.Create(new SphereGeometry()
        //{
        //Center = new float3(0f, 0f, 0f),
        //Radius = 0.25f,
        //}, //CollisionFilter.Default, physMaterial)
        //});

        return prefab;
    }

    [BurstCompile]
    public void CreateEntityUsingEntityPrefab(Entity prefab, float3 position, float scale)
    {
        Entity newEntity = World.DefaultGameObjectInjectionWorld.EntityManager.Instantiate(prefab);

        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(newEntity, new Translation 
        {
            Value = new float3(position.x, position.y, position.z) 
        });

        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(newEntity, new Scale
        {
            Value = scale
        });

        entities.Add(position, newEntity);
    }*/
}
