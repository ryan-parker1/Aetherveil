using UnityEngine;

public class TargetingController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask targetLayerMask;

    private Camera mainCamera;

    public Targetable CurrentTarget { get; private set; }

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleTargetSelection();
    }

    private void HandleTargetSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, targetLayerMask))
            {
                Targetable target =
                    hit.collider.GetComponentInParent<Targetable>();

                if (target != null)
                {
                    SetTarget(target);
                }
            }
        }
    }

    private void SetTarget(Targetable newTarget)
{
    if (CurrentTarget != null)
    {
        SelectionIndicator oldIndicator =
            CurrentTarget.GetComponent<SelectionIndicator>();

        if (oldIndicator != null)
        {
            oldIndicator.SetSelected(false);
        }
    }

    CurrentTarget = newTarget;

    SelectionIndicator newIndicator =
        CurrentTarget.GetComponent<SelectionIndicator>();

    if (newIndicator != null)
    {
        newIndicator.SetSelected(true);
    }

    Debug.Log("Target Selected: " + newTarget.TargetName);
}
}