
public class ContentRelation
{
    public readonly ChunkMapPointContent content;
    public readonly ContentRelationType relationType;

    public ContentRelation(ChunkMapPointContent content, ContentRelationType relationType)
    {
        this.content = content;
        this.relationType = relationType;
    }
}