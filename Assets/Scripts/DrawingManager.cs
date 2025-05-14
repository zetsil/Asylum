using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DrawingManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float tolerance = 0.2f; // Toleranță la erori
    public GameObject keyPrefab; // Prefab pentru cheia care va fi generată

    private List<Vector3> drawnPoints = new List<Vector3>();
    private List<Vector2> keyOutlinePoints; // Punctele care formează conturul cheii (vor trebui setate manual sau importate)

    void Start()
    {
        // Inițializare LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        // Setează punctele conturului cheii (poți adăuga punctele pentru forma cheii aici)
        keyOutlinePoints = new List<Vector2>
        {
            new Vector2(-0.5f, 0.0f), // Exemplu de puncte pentru conturul cheii
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, -0.5f),
            new Vector2(-0.5f, 0.0f)
        };
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Când începe desenul
        {
            drawnPoints.Clear();
            lineRenderer.positionCount = 0;
        }

        if (Input.GetMouseButton(0)) // Dacă ții apăsat pe mouse
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0; // Ne asigurăm că rămâne în 2D

            if (drawnPoints.Count == 0 || Vector3.Distance(drawnPoints[drawnPoints.Count - 1], mousePos) > 0.1f)
            {
                drawnPoints.Add(mousePos);
                lineRenderer.positionCount = drawnPoints.Count;
                lineRenderer.SetPosition(drawnPoints.Count - 1, mousePos);
            }
        }

        if (Input.GetMouseButtonUp(0)) // Când eliberezi mouse-ul
        {
            CheckDrawing();
        }
    }

    void CheckDrawing()
    {
        if (keyOutlinePoints == null || keyOutlinePoints.Count == 0) return;

        if (drawnPoints.Count < keyOutlinePoints.Count / 2) return; // Desen prea scurt -> invalid

        int correctPoints = 0;
        foreach (Vector2 drawnPoint in drawnPoints)
        {
            foreach (Vector2 keyPoint in keyOutlinePoints)
            {
                if (Vector2.Distance(drawnPoint, keyPoint) < tolerance)
                {
                    correctPoints++;
                    break; // Nu verifica mai departe, punctul e deja valid
                }
            }
        }

        float accuracy = (float)correctPoints / keyOutlinePoints.Count;
        if (accuracy > 0.75f) // Dacă 75% din puncte sunt corecte
        {
            
            GameStateManager.Instance.UpdateObjectState("ClassroomKey", true);
            Debug.Log("Desenul este corect! Transformăm în cheie.");
            ConvertToKey();
        }
        else
        {
            Debug.Log("Desen incorect. Încearcă din nou!");
            drawnPoints.Clear();
            lineRenderer.positionCount = 0;
        }
    }

    void ConvertToKey()
    {
        GameObject key = Instantiate(keyPrefab, drawnPoints[drawnPoints.Count / 2], Quaternion.identity);
        StartCoroutine(ShowSuccessAndLoadScene());
        Debug.Log("Cheia a fost creată!");
    }


     IEnumerator ShowSuccessAndLoadScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("classroom"); // Schimbă cu numele scenei tale
    }
}