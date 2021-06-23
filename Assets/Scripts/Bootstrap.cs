using Unity.Entities;
using UnityEngine;

namespace ECSRainbow
{
    public sealed class Bootstrap
    {
        public static EntityArchetype LedArchetype;
        public static Settings settings;
       
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {

        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeAfterSceneLoad()
        {
            settings = GameObject.FindObjectOfType<Settings>();
            DroppletData.mesh = settings.droppletMesh;
            DroppletData.M = settings.droppletMaterial;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var droppletArchetype = em.CreateArchetype(typeof(DroppletData));
            em.CreateEntity(droppletArchetype);
        }
        
    }
}
