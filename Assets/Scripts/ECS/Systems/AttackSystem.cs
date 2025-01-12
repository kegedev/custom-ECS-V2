using Game.ECS.Base;
using Game.ECS.Base.Components;
using Game.ECS.Base.Systems;
using UnityEngine;

public class AttackSystem : IUpdateSystem
{
    public ushort ActiveStateMask => (ushort)GameState.MainState;

    public void Update(SystemManager systemManager)
    {
        ECSWorld world=systemManager.GetWorld();
        var attackComponentContainer = world.GetComponentContainer<AttackComponent>();
        for (int i = 0; i < attackComponentContainer.EntityCount; i++)
        {
            ref var attackComponent = ref world.GetComponent<AttackComponent>(attackComponentContainer.EntityIds[i]);
           

            if (attackComponent.TargetId!=-1)
            {
                ref var targetHealthComponent = ref world.GetComponent<HealthComponent>(attackComponent.TargetId);
                
                targetHealthComponent.Health -= attackComponent.Damage;
                if(targetHealthComponent.Health<=0)
                {
                    Debug.Log("DISPOSE");
                    world.DisposeEntity(attackComponent.TargetId);
                }
                else
                {

                //world.UpdateComponent(attackComponent.TargetId, targetHealthComponent);
                }
                
          
                attackComponent.TargetId = -1;
               // world.UpdateComponent(attackComponentContainer.EntityIds[i], attackComponent);
     
            }
        }
    }
}
