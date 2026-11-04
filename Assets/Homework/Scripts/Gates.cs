using UnityEngine;

public class Gates : MonoBehaviour
{
    private int _score;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ball ball) == false)
            return;

        Destroy(ball.gameObject);
        _score++;
        Debug.Log($"Счёт: {_score}");
    }
}