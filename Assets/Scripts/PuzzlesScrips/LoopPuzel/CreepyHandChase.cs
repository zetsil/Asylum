using UnityEngine;

public class CreepyHandChase : MonoBehaviour
{
    public float speed = 2f;
    private Transform player;
    private bool isChasing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("No Player with tag 'Player' found in the scene!");
        }
        else
        {
            isChasing = true;
        }
    }

    void Update()
    {
        if (isChasing && player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player Caught!");
            // Load the target scene
            GameStateManager.Instance.DecrementInfiniteLoopSolved();
            Debug.Log("Puzzle failed - decrementing loop count");
            SceneTransitionManager.Instance.LoadScene("HallWay2");
            // Here you can call your Game Over logic
        }
    }
}
