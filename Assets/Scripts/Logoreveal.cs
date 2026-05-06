using UnityEngine;
using System.Collections;


public class Logoreveal : MonoBehaviour
{
    [Header("Posiciones mundo")]
    public Vector3 hiddenPosition;       // Enterrado bajo el maní
    public Vector3 revealedPosition;     // Donde queda al salir antes de acercarse

    [Header("Acercamiento a cámara")]
    public float distanceFromCamera = 3f;
    public Vector3 cameraOffset = new Vector3(0f, 0f, 0f);

    [Header("Tiempos")]
    public float delayBeforeRise = 1.5f;
    public float delayBeforeApproach = 0.5f;

    [Header("Velocidades")]
    public float riseSpeed = 1.5f;
    public float approachSpeed = 0.7f;
    public float rotationSpeed = 30f;    // Grados/segundo, rotación suave final

    [Header("Curvas de animación")]
    public AnimationCurve riseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public AnimationCurve approachCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // ─── Estado interno ───────────────────────────────────────────
    private bool isRevealing = false;
    private bool shouldRotate = false;
    private Camera mainCamera;


    private Vector3 finalPosition;

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        mainCamera = Camera.main;
        transform.position = hiddenPosition;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (!shouldRotate) return;

        // Anclar posición mientras rota → evita deriva
        transform.position = finalPosition;
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }

    // ─── Punto de entrada público ─────────────────────────────────
    public void Reveal()
    {
        if (!isRevealing)
            StartCoroutine(RevealSequence());
    }

    // ─── Secuencia completa ───────────────────────────────────────
    IEnumerator RevealSequence()
    {
        isRevealing = true;

        yield return new WaitForSeconds(delayBeforeRise);
        yield return StartCoroutine(RiseUp());

        yield return new WaitForSeconds(delayBeforeApproach);
        yield return StartCoroutine(ApproachAndFace());

        shouldRotate = true;
    }

    // ─── Fase 1: Sube desde el maní ──────────────────────────────
    IEnumerator RiseUp()
    {
        float t = 0f;
        Vector3 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * riseSpeed;
            transform.position = Vector3.Lerp(startPos, revealedPosition,
                                              riseCurve.Evaluate(Mathf.Clamp01(t)));
            yield return null;
        }
        transform.position = revealedPosition;
    }

    // ─── Fase 2: Se acerca Y gira de frente en un solo movimiento ─
    IEnumerator ApproachAndFace()
    {
        // ── Posición destino frente a la cámara ───────────────────
        finalPosition = mainCamera.transform.position
                      + mainCamera.transform.forward * distanceFromCamera
                      + cameraOffset;

        Vector3 toCam = mainCamera.transform.position - finalPosition;
        toCam.y = 0f;

        Quaternion targetRot = toCam.sqrMagnitude > 0.001f
            ? Quaternion.LookRotation(toCam.normalized)
            : Quaternion.identity;

        // ── Interpolar posición y rotación juntas ─────────────────
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * approachSpeed;
            float curved = approachCurve.Evaluate(Mathf.Clamp01(t));

            transform.position = Vector3.Lerp(startPos, finalPosition, curved);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, curved);

            yield return null;
        }

        transform.position = finalPosition;
        transform.rotation = targetRot;
    }

    // ─── Reset ────────────────────────────────────────────────────
    public void ResetLogo()
    {
        StopAllCoroutines();
        isRevealing = false;
        shouldRotate = false;
        transform.position = hiddenPosition;
        transform.rotation = Quaternion.identity;
    }
}