using UnityEngine;

public abstract class VillagerSubTask
{
    public readonly Villager villager;

    public VillagerSubTask(Villager villager)
    {
        this.villager = villager;
    }

    public virtual void StartTask() { }
    public virtual void InterruptTask() { }
    public virtual void EndTask() { }
    public virtual void FinishTask() { }

    public abstract bool Update(float deltaTime);

    public void RotateTowardsDirection(float deltaTime, Vector2 direction)
    {
        Rotate(deltaTime, Vector2.SignedAngle(direction, Vector2.up));
    }

    public void RotateTowardsTilePositionDirection(float deltaTime, TilePosition tilePosition)
    {
        switch (tilePosition)
        {
            case TilePosition.TopCenter:
                Rotate(deltaTime, 0);
                break;
            case TilePosition.TopRight:
                Rotate(deltaTime, 45);
                break;
            case TilePosition.CenterRight:
                Rotate(deltaTime, 90);
                break;
            case TilePosition.BottomRight:
                Rotate(deltaTime, 135);
                break;
            case TilePosition.BottomCenter:
                Rotate(deltaTime, 180);
                break;
            case TilePosition.BottomLeft:
                Rotate(deltaTime, 225);
                break;
            case TilePosition.CenterLeft:
                Rotate(deltaTime, 270);
                break;
            case TilePosition.TopLeft:
                Rotate(deltaTime, 315);
                break;
        }
    }

    public void RotateTowardsDirection(float deltaTime, CardinalDirection direction)
    {   
        switch (direction)
        {
            case CardinalDirection.Top:
                Rotate(deltaTime, 0);
                break;
            case CardinalDirection.Right:
                Rotate(deltaTime, 90);
                break;
            case CardinalDirection.Bottom:
                Rotate(deltaTime, 180);
                break;
            case CardinalDirection.Left:
                Rotate(deltaTime, 270);
                break;
        }
    }

    public void Rotate(float deltaTime, float targetRotation)
    {
        if (targetRotation == villager.currentRotation.value)
            return;

        float rotationDifference = targetRotation - villager.currentRotation.value;
        while (rotationDifference < 0)
            rotationDifference += 360;

        float newRotation;
        if (rotationDifference > 180)
        {
            rotationDifference = 360 - rotationDifference;
            float rotateAmount = (rotationDifference * villager.rotateDifferenceSpeedFactor + villager.baseRotateSpeed) * deltaTime;

            if (rotateAmount > rotationDifference)
            {
                newRotation = targetRotation;
            }
            else
            {
                newRotation = villager.currentRotation.value - rotateAmount;
            }
        }
        else
        {
            float rotateAmount = (rotationDifference * villager.rotateDifferenceSpeedFactor + villager.baseRotateSpeed) * deltaTime;
            if (rotateAmount > rotationDifference)
            {
                newRotation = targetRotation;
            }
            else
            {
                newRotation = villager.currentRotation.value + rotateAmount;
            }
        }

        if (newRotation < 0)
            newRotation += 360;
        else if (newRotation >= 360)
            newRotation -= 360;

        villager.currentRotation.value = newRotation;
    }
}