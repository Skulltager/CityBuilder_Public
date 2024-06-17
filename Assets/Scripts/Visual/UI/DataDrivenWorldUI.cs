
using UnityEngine;

public abstract class DataDrivenWorldUI<T> : DataDrivenUI<T>
{
    private const float MIN_SCALE = 20;
    private const float BASE_SCALE = 25;

    [field: SerializeField] public float heightOffset { private set; get; }

    public abstract Vector3 followPosition { get; }

    private void LateUpdate()
    {
        Vector3 difference = (followPosition - Camera.main.transform.position).normalized;
        if (Vector3.Dot(Camera.main.transform.forward, difference) <= 0)
        {
            rectTransform.anchoredPosition = new Vector3(-10000, 0);
            return;
        }

        Vector2 screenPoint = Camera.main.WorldToScreenPoint(followPosition);
        screenPoint.x /= canvas.scaleFactor;
        screenPoint.y /= canvas.scaleFactor;
        rectTransform.anchoredPosition = screenPoint;
        rectTransform.localScale = Vector3.one / Mathf.Max(Camera.main.transform.position.y, MIN_SCALE) * BASE_SCALE;
    }
}