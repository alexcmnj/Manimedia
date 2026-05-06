using UnityEngine;
using UnityEngine.EventSystems;

public class Windmanager : MonoBehaviour
{
    [Header("Referencias")]
    public Windcontroller windController;
    public Transform containerCenter;
    public LayerMask groundLayer;          

    [Header("Visual")]
    public GameObject clickMarkerPrefab;
    public LineRenderer windLine;
    public Transform windArrow;

    private GameObject currentMarker;
    private Camera mainCamera;
    private bool selectionActive = true;

    void Start() => mainCamera = Camera.main;

    void Update()
    {
        if (!selectionActive) return;
        if (Input.GetMouseButtonDown(0))
            TrySelectWindOrigin();
    }

    void TrySelectWindOrigin()
    {
        // Ignorar clics sobre elementos UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 clickPoint = hit.point;

            if (windController.IsInside(clickPoint))
            {
                Debug.Log("⚠️ Haz clic FUERA del contenedor.");
                return;
            }

            
            PlaceMarker(clickPoint);
            DrawWindLine(clickPoint, containerCenter.position);
            UpdateArrow(clickPoint);
        }
    }

    void PlaceMarker(Vector3 pos)
    {
        if (currentMarker != null) Destroy(currentMarker);
        if (clickMarkerPrefab != null)
            currentMarker = Instantiate(clickMarkerPrefab, pos + Vector3.up * 0.15f, Quaternion.identity);
    }

    void DrawWindLine(Vector3 from, Vector3 to)
    {
        if (windLine == null) return;
        windLine.SetPosition(0, from + Vector3.up * 0.2f);
        windLine.SetPosition(1, to);
        windLine.enabled = true;
    }

    void UpdateArrow(Vector3 from)
    {
        if (windArrow == null) return;
        Vector3 dir = containerCenter.position - from;
        dir.y = 0f;
        windArrow.position = from + dir * 0.5f + Vector3.up * 0.5f;
        windArrow.rotation = Quaternion.LookRotation(dir.normalized);
        windArrow.gameObject.SetActive(true);
    }

    public void SetSelectionActive(bool active) => selectionActive = active;
}