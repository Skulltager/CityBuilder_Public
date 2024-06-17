
using System.Collections.Generic;

public class PathData
{
    public readonly VillagerTaskPoint taskPoint;
    public readonly List<PathPoint> pathPoints;

    public PathData(VillagerTaskPoint taskPoint, List<PathPoint> pathPoints)
    {
        this.taskPoint = taskPoint;
        this.pathPoints = pathPoints;
    }
}