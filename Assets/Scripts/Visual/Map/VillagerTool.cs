
using UnityEngine;

public class VillagerTool : DataDrivenBehaviour<Villager>
{
    [SerializeField] private AnimationType_Villager animationType;
    [SerializeField] private MeshRenderer meshRenderer;

    protected override void OnValueChanged_Data(Villager oldValue, Villager newValue)
    {
        if(oldValue != null)
        {
            oldValue.currentAnimation.onValueChange -= OnValueChanged_CurrentAnimation;
        }

        if(newValue != null)
        {
            newValue.currentAnimation.onValueChangeImmediate += OnValueChanged_CurrentAnimation;
        }
    }

    private void OnValueChanged_CurrentAnimation(AnimationType_Villager oldValue, AnimationType_Villager newValue)
    {
        meshRenderer.enabled = newValue == animationType;
    }
}