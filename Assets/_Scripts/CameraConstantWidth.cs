using UnityEngine;

/// <summary>
/// Скрипт константной ширины камеры, независимо от соотношения сторон устройства. Скрипт не мой!!!
/// </summary>
public class CameraConstantWidth : MonoBehaviour
{
    public Vector2 defaultResolution = new(720, 1280);
    [Range(0f, 1f)] public float widthOrHeight;

    private Camera cameraComponent;
    
    private float initialSize;
    private float targetAspect;

    private float initialFov;
    private float horizontalFov = 120f;

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
        initialSize = cameraComponent.orthographicSize;

        targetAspect = defaultResolution.x / defaultResolution.y;

        initialFov = cameraComponent.fieldOfView;
        horizontalFov = CalcVerticalFov(initialFov, 1 / targetAspect);
    }

    private void Update()
    {
        if (cameraComponent.orthographic)
        {
            float constantWidthSize = initialSize * (targetAspect / cameraComponent.aspect);
            cameraComponent.orthographicSize = Mathf.Lerp(constantWidthSize, initialSize, widthOrHeight);
        }
        else
        {
            float constantWidthFov = CalcVerticalFov(horizontalFov, cameraComponent.aspect);
            cameraComponent.fieldOfView = Mathf.Lerp(constantWidthFov, initialFov, widthOrHeight);
        }
    }

    private float CalcVerticalFov(float _hFovInDeg, float _aspectRatio)
    {
        float hFovInRads = _hFovInDeg * Mathf.Deg2Rad;

        float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / _aspectRatio);

        return vFovInRads * Mathf.Rad2Deg;
    }
}