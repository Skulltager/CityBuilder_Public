
public class VillagerSubTask_Idle : VillagerSubTask
{
    private const string TASK_DESCRIPTION_FORMAT = "Chilling for a bit";

    public readonly float idleTime;
    public float idleTimeRemaining;

    public VillagerSubTask_Idle(Villager villager, float idleTime) 
        : base(villager)
    {
        this.idleTime = idleTime;
    }

    public override void StartTask()
    {
        idleTimeRemaining = idleTime;
        villager.currentAnimation.value = AnimationType_Villager.Idle;
        villager.currentTaskDescription.value = TASK_DESCRIPTION_FORMAT;
    }

    public override bool Update(float deltaTime)
    {
        idleTimeRemaining -= deltaTime;

        if (idleTimeRemaining > 0)
            return false;

        return true;
    }
}