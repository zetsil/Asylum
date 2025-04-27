using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ExitMessageEmitter : MonoBehaviour
{
    private Emitter playerEmitter;
    [SerializeField] private string exitMessage = "player_exited";
    [SerializeField] private float delayBeforeEmit = 0f;
    
    [Header("Gizmo Settings")]
    [SerializeField] private Color gizmoColor = new Color(1, 0.5f, 0, 0.3f); // Orange with transparency
    [SerializeField] private bool showGizmo = true;

    private void Start()
    {            
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerEmitter = player.GetComponent<Emitter>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (delayBeforeEmit > 0)
            {
                StartCoroutine(EmitAfterDelay());
            }
            else
            {
                EmitExitSignal();
            }
        }
    }

    private IEnumerator EmitAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeEmit);
        EmitExitSignal();
    }

    void EmitExitSignal()
    {
        if (playerEmitter != null)
        {
            Debug.Log($"Emitting exit message: {exitMessage}", this);
            playerEmitter.NotifyObservers(exitMessage);
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        var collider = GetComponent<Collider2D>();
        if (collider == null) return;

        Gizmos.color = gizmoColor;

        // Handle different collider types
        switch (collider)
        {
            case BoxCollider2D box:
                Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
                break;
                
            case CircleCollider2D circle:
                Gizmos.DrawSphere(transform.position + (Vector3)circle.offset, circle.radius);
                break;
                
            case PolygonCollider2D poly:
                // Draw approximate polygon shape
                for (int i = 0; i < poly.points.Length; i++)
                {
                    Vector2 point1 = transform.TransformPoint(poly.points[i] + poly.offset);
                    Vector2 point2 = transform.TransformPoint(poly.points[(i + 1) % poly.points.Length] + poly.offset);
                    Gizmos.DrawLine(point1, point2);
                }
                break;
        }
    }
    #endif
}