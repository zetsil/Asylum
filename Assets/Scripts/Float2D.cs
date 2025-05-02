using UnityEngine;

public class Float2D : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float floatHeight = 0.5f; // How high it bobs
    [SerializeField] private float floatSpeed = 1f; // How fast it bobs
    [SerializeField] private bool rotateWhileFloating = true;
    [SerializeField] private float rotationAmount = 5f; // Degrees to rotate back and forth
    [SerializeField] private float rotationSpeed = 0.5f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float randomOffset; // Makes objects not perfectly synchronized

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        randomOffset = Random.Range(0f, 2f * Mathf.PI); // Random phase offset
    }

    private void Update()
    {
        // Vertical floating movement
        float newY = startPosition.y + Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Optional rotation effect
        if (rotateWhileFloating)
        {
            float rotationAngle = Mathf.Sin((Time.time + randomOffset) * rotationSpeed) * rotationAmount;
            transform.rotation = startRotation * Quaternion.Euler(0f, 0f, rotationAngle);
        }
    }
}