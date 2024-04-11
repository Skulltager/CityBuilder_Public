public enum VisionState
{
    None = 0,
    Half = 1,
    Full = 2,
}

public static class VisionStateExtension
{
    public static float GetFogValue(this VisionState visionState)
    {
        switch(visionState)
        {
            case VisionState.Full:
                return 0;
            case VisionState.Half:
                return 0.5f;
            case VisionState.None:
                return 1;
        }

        throw new System.Exception(string.Format("GetFogValue is missing implementation for {0}", visionState));
    }
}