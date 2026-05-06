using UnityEngine;

public class WallClicreceptor : MonoBehaviour
{
    private WallSelector selector;
    private int wallIndex;

    public void Init(WallSelector sel, int index)
    {
        selector = sel;
        wallIndex = index;
    }

    void OnMouseDown()
    {
        // Obtener el punto exacto donde se hizo clic en la pared
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
                selector.OnWallClicked(wallIndex, hit.point);
        }
    }
}
