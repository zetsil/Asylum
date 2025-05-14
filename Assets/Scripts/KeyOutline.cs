using System.Collections.Generic;
using UnityEngine;

public class KeyOutline : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector2> keyOutlinePoints = new List<Vector2>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Adaugă punctele manual (exemplu simplu)
        keyOutlinePoints.Add(new Vector2(-1, 0));
        keyOutlinePoints.Add(new Vector2(-0.5f, 0.5f));
        keyOutlinePoints.Add(new Vector2(0, 1));
        keyOutlinePoints.Add(new Vector2(0.5f, 0.5f));
        keyOutlinePoints.Add(new Vector2(1, 0));

        // Aplică punctele în Line Renderer
        lineRenderer.positionCount = keyOutlinePoints.Count;
        for (int i = 0; i < keyOutlinePoints.Count; i++)
        {
            lineRenderer.SetPosition(i, keyOutlinePoints[i]);
        }

        // Activarea LineRenderer pentru a fi vizibil
        lineRenderer.enabled = true;

        // Setează un material pentru LineRenderer (asigură-te că ai un material disponibil în Resources sau Inspector)
        if (lineRenderer.material == null)
        {
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // Setează culoarea și grosimea liniei
        lineRenderer.startWidth = 0.1f; // Grosimea liniei la început
        lineRenderer.endWidth = 0.1f; // Grosimea liniei la sfârșit
        lineRenderer.startColor = Color.yellow; // Culoarea liniei
        lineRenderer.endColor = Color.yellow; // Culoarea liniei la capăt
    }
}