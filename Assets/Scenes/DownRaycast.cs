using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DownRaycast : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float rayLength = 10f;
    [SerializeField] private Color rayColor = Color.cyan;
    [SerializeField] private bool useGradient = true;
    [SerializeField] private Gradient rayGradient;
    
    [Header("Visual Effects")]
    [SerializeField] private int lineSegments = 20;

    private LineRenderer lineRenderer;
    private RaycastHit hitInfo;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = lineSegments;
        
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }

        lineRenderer.enabled = false;

        if (rayGradient.Equals(default(Gradient)))
        {
            rayGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(rayColor, 0f);
            colorKeys[1] = new GradientColorKey(new Color(rayColor.r, rayColor.g, rayColor.b, 0f), 1f);
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(0f, 1f);
            
            rayGradient.SetKeys(colorKeys, alphaKeys);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        lineRenderer.enabled = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (!isGrabbed) return;

        Vector3 rayStart = transform.position;
        Vector3 rayDirection = Vector3.down;
        
        bool hit = Physics.Raycast(rayStart, rayDirection, out hitInfo, rayLength);
        float currentRayLength = hit ? hitInfo.distance : rayLength;

        for (int i = 0; i < lineSegments; i++)
        {
            float t = i / (float)(lineSegments - 1);
            Vector3 position = rayStart + (rayDirection * currentRayLength * t);
            lineRenderer.SetPosition(i, position);
        }
        
        if (useGradient)
        {
            lineRenderer.colorGradient = rayGradient;
        }
        else
        {
            lineRenderer.startColor = rayColor;
            lineRenderer.endColor = new Color(rayColor.r, rayColor.g, rayColor.b, 0f);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}
