using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    [SerializeField] private GameObject indicatorObject;

    public void SetSelected(bool selected)
    {
        indicatorObject.SetActive(selected);
    }
}