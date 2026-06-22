using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Crate"))
        {
            LevelManager.Instance.Win();
        }
    }
}