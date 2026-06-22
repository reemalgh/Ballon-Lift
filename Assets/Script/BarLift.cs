using UnityEngine;

namespace BalloonPuzzle2D
{
    public class BarLift : MonoBehaviour
    {
        public int balloonsNeeded = 3;
        public float moveSpeed = 2f;
        public float targetY = 3f;

        private int attachedBalloons = 0;
        private bool shouldMove = false;

        public void AddBalloon()
        {
            attachedBalloons++;

            if (attachedBalloons >= balloonsNeeded)
            {
                shouldMove = true;
            }
        }

        void Update()
        {
            if (!shouldMove) return;

            Vector3 targetPosition = new Vector3(
                transform.position.x,
                targetY,
                transform.position.z
            );

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
    }
}