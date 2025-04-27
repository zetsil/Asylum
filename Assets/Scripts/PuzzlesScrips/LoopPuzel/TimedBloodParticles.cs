using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TimedBloodParticles : MonoBehaviour
{
    [Header("Color Transition Settings")]
    [SerializeField] private Color startColor = Color.white; // Initial particle color
    [SerializeField] private Color maxBloodColor = new Color(0.8f, 0f, 0f); // Deep red
    [SerializeField] private float colorChangeDuration = 3f; // Time to reach max red

    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;
    private float _timer = 0f;
    private bool _isTransitioning = true;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        if (_particleSystem == null)
        {
            Debug.LogError("No ParticleSystem found!", this);
            return;
        }

        _mainModule = _particleSystem.main;
        _mainModule.startColor = startColor; // Set initial color
    }

    private void Update()
    {
        if (!_isTransitioning) return;

        _timer += Time.deltaTime;
        float progress = Mathf.Clamp01(_timer / colorChangeDuration);

        // Lerp from startColor to maxBloodColor
        Color currentColor = Color.Lerp(startColor, maxBloodColor, progress);
        _mainModule.startColor = currentColor;

        if (progress >= 1f)
        {
            _isTransitioning = false; // Stop when max red is reached
        }
    }

    // Call this to restart the color transition
    public void ResetColorTransition()
    {
        _timer = 0f;
        _isTransitioning = true;
    }
}