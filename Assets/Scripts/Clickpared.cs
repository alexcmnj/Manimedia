using UnityEngine;
using UnityEngine.EventSystems;

public class Clickpared : MonoBehaviour
{
    private WallSelector selector;
    private int wallIndex;

    public void Init(WallSelector s, int index)
    {
        selector = s;
        wallIndex = index;
    }

    void OnMouseDown()
    {
        // 🚫 Solo permitir selección de paredes en la fase Selection
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.currentPhase != GameManager.GamePhase.Selection)
            return;

        // 🚫 Si el panel del slider está abierto, no permitir seleccionar paredes
        if (selector.sliderPanel.activeSelf)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            selector.OnWallClicked(wallIndex, hit.point);
        }
    }
}