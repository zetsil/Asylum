using UnityEngine;

public class FireflyOrbitTracker : MonoBehaviour
{
    [Header("Index curent (sincronizat cu starea jocului)")]
    public int currentIndex = 0;

    [Header("Ținta licuriciului (Empty Object sau alt punct)")]
    public Transform target;

    [Header("Parametrii de zbor și orbită")]
    public float moveSpeed = 2f;
    public float orbitRadius = 0.5f;
    public float orbitSpeed = 2f;
    public float transitionDuration = 1.2f;

    [Header("Zbor organic")]
    public float flightWobble = 0.5f;
    public float flightWobbleSpeed = 1.5f;
    public float noiseAmplitude = 0.3f;
    public float noiseFrequency = 2f;

    private float orbitAngle;
    private Vector2 orbitCenter;
    private float transitionTimer;
    private bool transitioning = false;
    private bool orbiting = false;
    private float noiseOffsetX;
    private float noiseOffsetY;
    private Vector3 initialScale;

    void Start()
    {
        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetY = Random.Range(0f, 100f);
        initialScale = transform.localScale;

        int darkRoomCounter = GameStateManager.Instance.GetCounterDarkRoom();

        // dacă e mai mic, apare direct pe orbită
        if (currentIndex < darkRoomCounter)
        {
            if (target != null)
            {
                orbitCenter = target.position;
                orbitAngle = Random.Range(0f, Mathf.PI * 2f); // poziție random pe orbită
                SetPositionOnOrbit();
                orbiting = true;
            }
        }
        // dacă e egal, pornește zborul normal
        else if (currentIndex == darkRoomCounter)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (target == null || !gameObject.activeSelf) return;

        if (!transitioning && !orbiting)
            MoveTowardsTarget();
        else if (transitioning)
            SmoothTransitionToOrbit();
        else if (orbiting)
            OrbitAroundTarget();
    }

    void MoveTowardsTarget()
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = target.position;
        Vector2 direction = (targetPos - currentPos).normalized;

        // mișcare organică cu deviere
        float deviation = (Mathf.PerlinNoise(Time.time * flightWobbleSpeed + noiseOffsetX, 0f) - 0.5f) * 2f * flightWobble;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        Vector2 offset = perpendicular * deviation;

        Vector2 moveDir = (direction + offset * 0.3f).normalized;
        transform.position = Vector2.MoveTowards(currentPos, currentPos + moveDir, moveSpeed * Time.deltaTime);

        float distance = Vector2.Distance(currentPos, targetPos);
        if (distance <= orbitRadius)
        {
            transitioning = true;
            orbitCenter = targetPos;
            transitionTimer = 0f;
            Vector2 fromCenter = currentPos - orbitCenter;
            orbitAngle = Mathf.Atan2(fromCenter.y, fromCenter.x);
        }

        ApplyPulseEffect();
    }

    void SmoothTransitionToOrbit()
    {
        transitionTimer += Time.deltaTime;
        float t = Mathf.SmoothStep(0f, 1f, transitionTimer / transitionDuration);

        orbitAngle += orbitSpeed * Time.deltaTime * 0.5f;
        float x = orbitCenter.x + Mathf.Cos(orbitAngle) * orbitRadius;
        float y = orbitCenter.y + Mathf.Sin(orbitAngle) * orbitRadius;
        Vector2 orbitPos = new Vector2(x, y);

        transform.position = Vector2.Lerp(transform.position, orbitPos, t);

        if (t >= 1f)
        {
            transitioning = false;
            orbiting = true;
        }

        ApplyPulseEffect();
    }

    void OrbitAroundTarget()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;

        float x = orbitCenter.x + Mathf.Cos(orbitAngle) * orbitRadius;
        float y = orbitCenter.y + Mathf.Sin(orbitAngle) * orbitRadius;

        float noiseX = (Mathf.PerlinNoise(Time.time * noiseFrequency + noiseOffsetX, 0f) - 0.5f) * 2f * noiseAmplitude;
        float noiseY = (Mathf.PerlinNoise(0f, Time.time * noiseFrequency + noiseOffsetY) - 0.5f) * 2f * noiseAmplitude;

        transform.position = new Vector2(x + noiseX, y + noiseY);
        ApplyPulseEffect();
    }

    void SetPositionOnOrbit()
    {
        float x = orbitCenter.x + Mathf.Cos(orbitAngle) * orbitRadius;
        float y = orbitCenter.y + Mathf.Sin(orbitAngle) * orbitRadius;
        transform.position = new Vector2(x, y);
    }

    void ApplyPulseEffect()
    {
        float pulse = Mathf.Sin(Time.time * 4f + noiseOffsetX) * 0.1f + 1f;
        transform.localScale = initialScale * pulse;
    }
}
