using UnityEngine;
using Cinemachine;

[ExecuteAlways]
public class SmoothConfiner2D : CinemachineExtension
{
    public Collider2D bounds;

    [Range(0.1f, 10f)]
    public float softZone = 2f; // distance before edge to start slowing

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (bounds == null || stage != CinemachineCore.Stage.Body)
            return;

        Vector3 pos = state.RawPosition;
        Bounds b = bounds.bounds;

        // 1. Get the camera's viewing dimensions
        float camHalfHeight = state.Lens.OrthographicSize;
        float camHalfWidth = camHalfHeight * state.Lens.Aspect;

        // 2. Shrink the bounds by the camera's dimensions
        float left = b.min.x + camHalfWidth;
        float right = b.max.x - camHalfWidth;
        float bottom = b.min.y + camHalfHeight;
        float top = b.max.y - camHalfHeight;

        // 3. Failsafe: If the camera view is larger than the bounding box itself, 
        // lock the camera to the center of the bounds to prevent flipping.
        if (left > right) left = right = b.center.x;
        if (bottom > top) bottom = top = b.center.y;

        // Apply soft clamp on X
        pos.x = SoftClamp(pos.x, left, right, softZone);

        // Apply soft clamp on Y
        pos.y = SoftClamp(pos.y, bottom, top, softZone);

        state.RawPosition = pos;
    }

    float SoftClamp(float value, float min, float max, float soft)
    {
        // Failsafe: Prevent the soft zone from overlapping itself if the playable 
        // area is smaller than double the soft zone.
        if (max - min < soft * 2)
        {
            soft = Mathf.Max(0, (max - min) / 2f);
        }

        if (value < min + soft)
        {
            float t = Mathf.InverseLerp(min, min + soft, value);
            return Mathf.Lerp(min, value, t);
        }
        else if (value > max - soft)
        {
            float t = Mathf.InverseLerp(max, max - soft, value);
            return Mathf.Lerp(max, value, t);
        }

        return value;
    }
}