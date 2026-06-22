using UnityEngine;
namespace BalloonPuzzle2D
{
    public class WindZone : MonoBehaviour
    {
        public Vector2 windForce = new Vector2(0.2f, 0f); 
        public string balloonTag = "Balloon"; 

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag(balloonTag))
            {
                Rigidbody2D rb = other.attachedRigidbody;
                if (rb != null)
                {
                    rb.AddForce(windForce, ForceMode2D.Force);
                }
            }
        }
    }
}