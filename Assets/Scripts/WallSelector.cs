using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WallSelector : MonoBehaviour
{
    [Header("Paredes")]
    public GameObject Pared1;
    public GameObject Pared2;
    public GameObject Pared3;
    public GameObject Pared4;

    [Header("Materiales")]
    public Material wallNormalMat;
    public Material wallSelectedMat;

    [Header("Panel UI (Screen Space)")]
    public GameObject sliderPanel;
    public Slider angleSlider;
    public TextMeshProUGUI angleText;
    public Button confirmButton;

    [Header("Marcador del punto de salida")]
    public GameObject windPointMarker;

    [Header("Flecha de dirección")]
    public Transform directionArrow;

    [Header("Línea de dirección")]
    public LineRenderer windLine;

    private int selectedWallIndex = -1;
    private bool wallSelected = false;
    private bool pointSelected = false;
    private bool confirmed = false;

    private Vector3 windOriginPoint;
    private Vector3 windDirection;
    private float currentAngle = 90f;

    private Camera mainCamera;

    void Start()
    {
        enabled = false;
        mainCamera = Camera.main;

        sliderPanel.SetActive(false);

        if (windPointMarker != null) windPointMarker.SetActive(false);
        if (directionArrow != null) directionArrow.gameObject.SetActive(false);

        if (windLine != null)
        {
            windLine.positionCount = 2;
            windLine.enabled = false;
        }

        angleSlider.minValue = 0f;
        angleSlider.maxValue = 180f;
        angleSlider.value = 90f;
        angleSlider.onValueChanged.AddListener(OnSliderChanged);
        confirmButton.onClick.AddListener(OnConfirm);

        AddWallClick(Pared1, 0);
        AddWallClick(Pared2, 1);
        AddWallClick(Pared3, 2);
        AddWallClick(Pared4, 3);
    }

    void AddWallClick(GameObject wall, int index)
    {
        Clickpared existing = wall.GetComponent<Clickpared>();
        if (existing != null) Destroy(existing);

        Clickpared r = wall.AddComponent<Clickpared>();
        r.Init(this, index);
    }

    public void OnWallClicked(int wallIndex, Vector3 hitPoint)
    {
        if (confirmed) return;

        if (!wallSelected)
        {
            ResetWallColors();
            selectedWallIndex = wallIndex;
            GetWall(wallIndex).GetComponent<Renderer>().material = wallSelectedMat;
            wallSelected = true;
            return;
        }

        if (wallIndex != selectedWallIndex)
        {
            ResetWallColors();
            selectedWallIndex = wallIndex;
            GetWall(wallIndex).GetComponent<Renderer>().material = wallSelectedMat;
            pointSelected = false;
            sliderPanel.SetActive(false);
            if (windPointMarker != null) windPointMarker.SetActive(false);
            if (directionArrow != null) directionArrow.gameObject.SetActive(false);
            return;
        }

        windOriginPoint = hitPoint;
        pointSelected = true;

        if (windPointMarker != null)
        {
            windPointMarker.transform.position = hitPoint;
            windPointMarker.SetActive(true);
        }

        sliderPanel.SetActive(true);
        angleSlider.value = 90f;
        currentAngle = 90f;
        angleText.text = "Ángulo: 90°";
        UpdateWindDirection(selectedWallIndex, 90f);
    }

    void OnSliderChanged(float value)
    {
        currentAngle = value;
        angleText.text = $"Ángulo: {(int)value}°";
        if (pointSelected)
            UpdateWindDirection(selectedWallIndex, value);
    }

    void UpdateWindDirection(int wallIndex, float sliderValue)
    {
        Vector3 baseDir = wallIndex switch
        {
            0 => Vector3.back,
            1 => Vector3.forward,
            2 => Vector3.right,
            3 => Vector3.left,
            _ => Vector3.back
        };

        float offset = sliderValue - 90f;
        windDirection = Quaternion.Euler(0f, offset, 0f) * baseDir;
        windDirection.y = 0f;
        windDirection = windDirection.normalized;

        if (windLine != null && pointSelected)
        {
            windLine.SetPosition(0, windOriginPoint);
            windLine.SetPosition(1, windOriginPoint + windDirection * 1.5f);
            windLine.enabled = true;
        }
    }

    void OnConfirm()
    {
        if (!pointSelected)
        {
            Debug.Log("⚠️ Primero haz clic en un punto de la pared.");
            return;
        }

        confirmed = true;

        // ✅ Limpiar todo el UI de selección
        sliderPanel.SetActive(false);
        ResetWallColors(); // ✅ Volver materiales a normal
        if (windPointMarker != null) windPointMarker.SetActive(false);
        if (windLine != null) windLine.enabled = false;
        if (directionArrow != null) directionArrow.gameObject.SetActive(false);

        Windcontroller wind = GameManager.Instance.windController;
        if (wind == null)
        {
            Debug.LogError("❌ WindController no asignado en GameManager");
            return;
        }

        wind.SetWindOrigin(windOriginPoint, windDirection);

        // ✅ Solo SetPhase — internamente ya llama ShowBlowingUI, no llamarlo dos veces
        GameManager.Instance.SetPhase(GameManager.GamePhase.Blowing);
    }

    void ResetWallColors()
    {
        Pared1.GetComponent<Renderer>().material = wallNormalMat;
        Pared2.GetComponent<Renderer>().material = wallNormalMat;
        Pared3.GetComponent<Renderer>().material = wallNormalMat;
        Pared4.GetComponent<Renderer>().material = wallNormalMat;
    }

    GameObject GetWall(int index) => index switch
    {
        0 => Pared1,
        1 => Pared2,
        2 => Pared3,
        3 => Pared4,
        _ => Pared1
    };

    public void Show()
    {
        confirmed = false;
        wallSelected = false;
        pointSelected = false;
        enabled = true;
    }

    public void Hide()
    {
        sliderPanel.SetActive(false);
        if (windPointMarker != null) windPointMarker.SetActive(false);
        if (directionArrow != null) directionArrow.gameObject.SetActive(false);
        if (windLine != null) windLine.enabled = false;

        ResetWallColors();
        confirmed = false;
        wallSelected = false;
        pointSelected = false;
        enabled = false;
    }

    public void ResetSelection() => Hide();
}