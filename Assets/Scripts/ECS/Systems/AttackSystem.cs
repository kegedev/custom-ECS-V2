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
        var attackComponentContainer = world.GetComponentContainer<AttackComponent>(ComponentMask.AttackComponent);
        for (int i = 0; i < attackComponentContainer.EntityCount; i++)
        {
            var attackComponent = attackComponentContainer.GetComponent(attackComponentContainer.EntityIds[i]);
            if (attackComponent.TargetId!=-1)
            {
                var healthComponentContainer = world.GetComponentContainer<HealthComponent>(ComponentMask.HealthComponent);
                var targetHealthComponent = healthComponentContainer.GetComponent(attackComponent.TargetId);
                targetHealthComponent.Health -= attackComponent.Damage;
                healthComponentContainer.UpdateComponent(attackComponent.TargetId, targetHealthComponent);
                attackComponent.TargetId = -1;
                attackComponentContainer.UpdateComponent(attackComponentContainer.EntityIds[i],attackComponent);
            }
        }
    }
}
