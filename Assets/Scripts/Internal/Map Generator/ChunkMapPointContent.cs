using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkMapPointContent
{
    public abstract string name { get; }
    public abstract ContentTaskLocation contentTaskLocation { get; }
    public abstract ChunkMapPointContentType contentType { get; }
    public readonly CardinalDirection direction;
    public readonly List<VillagerTaskPoint> villagerTaskPoints;
    public readonly List<ChunkMapTileDirectionPoint> mapPoints;
    public readonly EventList<ContentRelation> contentRelations;
    public readonly Point pivotPoint;
    public readonly Map map;
    public readonly WorldContentSize size;
    public event Action<ChunkMapPointContent> onRemovedFromMap;
    private bool initialized;

    public Point centerPoint;
    public Vector2 centerPosition;

    protected ChunkMapPointContent(Map map, Point pivotPoint, CardinalDirection direction, WorldContentSize size)
    {
        this.map = map;
        this.size = size;
        this.direction = direction;
        this.pivotPoint = pivotPoint;
        villagerTaskPoints = new List<VillagerTaskPoint>();
        mapPoints = new List<ChunkMapTileDirectionPoint>();
        contentRelations = new EventList<ContentRelation>();
    }

    private void TriggerEvent_RemovedFromMap(ChunkMapPointContent replacement = null)
    {
        if (onRemovedFromMap != null)
            onRemovedFromMap(replacement);
    }

    public virtual void Hide() { }
    public virtual void Show() { }

    private void Initialize()
    {
        if (initialized)
            return;

        initialized = true;

        WorldContentSizeBounds sizeBounds = size.GetSizeBounds(direction);
        centerPosition = new Vector2(sizeBounds.xCenter + pivotPoint.xIndex, sizeBounds.yCenter + pivotPoint.yIndex);
        centerPoint = new Point(sizeBounds.width / 2 + sizeBounds.xOffset + pivotPoint.xIndex, sizeBounds.height / 2 + sizeBounds.yOffset + pivotPoint.yIndex);

        for (int x = 0; x < sizeBounds.width; x++)
        {
            bool leftOpen = x == 0;
            bool rightOpen = x == sizeBounds.width - 1;

            for (int y = 0; y < sizeBounds.height; y++)
            {
                bool bottomOpen = y == 0;
                bool topOpen = y == sizeBounds.height - 1;

                TileTraversalEdgeDirection tileTraversalDirection = TileTraversalEdgeDirectionExtension.GetTileTraversalEdgeDirection(leftOpen, rightOpen, bottomOpen, topOpen);
                ChunkMapPoint chunkMapPoint = map.GetChunkMapPoint(x + sizeBounds.xOffset + pivotPoint.xIndex, y + sizeBounds.yOffset + pivotPoint.yIndex);
                mapPoints.Add(new ChunkMapTileDirectionPoint(chunkMapPoint, tileTraversalDirection));
            }
        }

        InitializeSub();
    }

    public virtual void PlaceOnMap()
    {
        Initialize();

        foreach (ChunkMapTileDirectionPoint buildingPoint in mapPoints)
        {
            buildingPoint.chunkMapPoint.content.value = this;
            buildingPoint.chunkMapPoint.tileTraversalDirection.value = buildingPoint.tileEdgeDirection;
        }
    }

    public virtual void RemoveFromMap(ChunkMapPointContent replacement = null)
    {
        foreach (ChunkMapTileDirectionPoint buildingPoint in mapPoints)
        {
            buildingPoint.chunkMapPoint.content.value = null;
            buildingPoint.chunkMapPoint.tileTraversalDirection.value = TileTraversalEdgeDirection.Free;
        }

        TriggerEvent_RemovedFromMap(replacement);
    }

    protected void SetVillagerTaskPoints_AroundEdge()
    {
        WorldContentSizeBounds sizeBounds = size.GetSizeBounds(direction);

        for (int x = 0; x < sizeBounds.width; x++)
        {
            bool leftOpen = x == 0;
            bool rightOpen = x == sizeBounds.width - 1;

            for (int y = 0; y < sizeBounds.height; y++)
            {
                bool bottomOpen = y == 0;
                bool topOpen = y == sizeBounds.height - 1;

                Point point = new Point(x + sizeBounds.xOffset + pivotPoint.xIndex, y + sizeBounds.yOffset + pivotPoint.yIndex);
                TileTraversalEdgeDirection tileTraversalDirection = TileTraversalEdgeDirectionExtension.GetTileTraversalEdgeDirection(leftOpen, rightOpen, bottomOpen, topOpen);
                switch (tileTraversalDirection)
                {
                    case TileTraversalEdgeDirection.BottomLeft:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Left), TilePosition.CenterRight, CardinalDirection.Right));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Bottom), TilePosition.TopCenter, CardinalDirection.Top));
                        break;
                    case TileTraversalEdgeDirection.BottomCenter:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Bottom), TilePosition.TopCenter, CardinalDirection.Top));
                        break;
                    case TileTraversalEdgeDirection.BottomRight:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Right), TilePosition.CenterLeft, CardinalDirection.Left));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Bottom), TilePosition.TopCenter, CardinalDirection.Top));
                        break;
                    case TileTraversalEdgeDirection.CenterLeft:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Left), TilePosition.CenterRight, CardinalDirection.Right));
                        break;
                    case TileTraversalEdgeDirection.CenterRight:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Right), TilePosition.CenterLeft, CardinalDirection.Left));
                        break;
                    case TileTraversalEdgeDirection.TopLeft:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Left), TilePosition.CenterRight, CardinalDirection.Right));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Top), TilePosition.BottomCenter, CardinalDirection.Bottom));
                        break;
                    case TileTraversalEdgeDirection.TopCenter:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Top), TilePosition.BottomCenter, CardinalDirection.Bottom));
                        break;
                    case TileTraversalEdgeDirection.TopRight:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Right), TilePosition.CenterLeft, CardinalDirection.Left));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Top), TilePosition.BottomCenter, CardinalDirection.Bottom));
                        break;
                    case TileTraversalEdgeDirection.AllEdges:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Left), TilePosition.CenterRight, CardinalDirection.Right));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Right), TilePosition.CenterLeft, CardinalDirection.Left));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Top), TilePosition.BottomCenter, CardinalDirection.Bottom));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Bottom), TilePosition.TopCenter, CardinalDirection.Top));
                        break;
                    case TileTraversalEdgeDirection.TopBottom:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Top), TilePosition.BottomCenter, CardinalDirection.Bottom));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Bottom), TilePosition.TopCenter, CardinalDirection.Top));
                        break;
                    case TileTraversalEdgeDirection.LeftRight:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Left), TilePosition.CenterRight, CardinalDirection.Right));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Right), TilePosition.CenterLeft, CardinalDirection.Left));
                        break;
                    case TileTraversalEdgeDirection.TopRightBottom:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Top), TilePosition.BottomCenter, CardinalDirection.Bottom));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Right), TilePosition.CenterLeft, CardinalDirection.Left));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Bottom), TilePosition.TopCenter, CardinalDirection.Top));
                        break;
                    case TileTraversalEdgeDirection.RightBottomLeft:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Left), TilePosition.CenterRight, CardinalDirection.Right));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Bottom), TilePosition.TopCenter, CardinalDirection.Top));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Right), TilePosition.CenterLeft, CardinalDirection.Left));
                        break;
                    case TileTraversalEdgeDirection.BottomLeftTop:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Top), TilePosition.BottomCenter, CardinalDirection.Bottom));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Bottom), TilePosition.TopCenter, CardinalDirection.Top));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Left), TilePosition.CenterRight, CardinalDirection.Right));
                        break;
                    case TileTraversalEdgeDirection.LeftTopRight:
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Right), TilePosition.CenterLeft, CardinalDirection.Left));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Top), TilePosition.BottomCenter, CardinalDirection.Bottom));
                        villagerTaskPoints.Add(new VillagerTaskPoint(this, point.AddDirection(CardinalDirection.Left), TilePosition.CenterRight, CardinalDirection.Right));
                        break;
                }
            }
        }
    }

    protected virtual void InitializeSub() { }
}