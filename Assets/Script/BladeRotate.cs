using UnityEngine;

namespace BalloonPuzzle2D
{
    public class BladeRotate : MonoBehaviour
    {
        public float rotationSpeed = 200f;

        void Update()
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Balloon"))
            {
                BalloonLift balloon = col.gameObject.GetComponent<BalloonLift>();

                if (balloon != null)
                {
                    balloon.PopBalloonAndCountLose();
                }
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Balloon"))
            {
                BalloonLift balloon = col.gameObject.GetComponent<BalloonLift>();

                if (balloon != null)
                {
                    balloon.PopBalloonAndCountLose();
                }
            }
        }
    }
}