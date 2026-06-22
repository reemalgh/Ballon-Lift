using UnityEngine;

namespace BalloonPuzzle2D
{
    public class BalloonLift : MonoBehaviour
    {
        [Header("Effects")]
        public GameObject m_ConnectionEffectPrefab;
        public GameObject m_KillEffectPrefab;

        [Header("Sounds")]
        public AudioClip m_ConnectionSound;
        public AudioClip m_KillSound;

        [Header("Balloon Settings")]
        public float m_LiftForce = 5f;
        public float m_MaxLiftSpeed = 2f;
        public float m_SwayForce = 0.3f;
        public float m_SwaySpeed = 2f;

        [Header("Names")]
        public string m_BarName = "Bar";
        public string m_DangerName = "Danger";
        public string m_BladeName = "Blade";

        [Header("Camera Lose")]
        public float m_ViewportMargin = 0.1f;

        private Rigidbody2D m_Rigidbody;
        private Camera m_MainCamera;

        private bool m_IsDragging = false;
        private bool m_IsAttached = false;
        private bool m_IsDead = false;

        private Vector2 m_Offset;

        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_MainCamera = Camera.main;
        }

        void Update()
        {
            if (m_IsDead) return;

            CheckIfOutsideCamera();

            if (m_IsDead) return;

            if (Application.isMobilePlatform)
            {
                HandleTouchInput();
            }
            else
            {
                HandleMouseInput();
            }
        }

        void FixedUpdate()
        {
            if (m_IsDead) return;

            ApplyLiftForce();
        }

        void ApplyLiftForce()
        {
            if (m_Rigidbody == null) return;
            if (m_IsAttached) return;

            if (!m_IsDragging)
            {
                m_Rigidbody.AddForce(Vector2.up * m_LiftForce, ForceMode2D.Force);

                float sway = Mathf.Sin(Time.time * m_SwaySpeed) * m_SwayForce;
                m_Rigidbody.AddForce(Vector2.right * sway, ForceMode2D.Force);

                if (m_Rigidbody.linearVelocity.y > m_MaxLiftSpeed)
                {
                    m_Rigidbody.linearVelocity = new Vector2(m_Rigidbody.linearVelocity.x, m_MaxLiftSpeed);
                }
            }
        }

        void CheckIfOutsideCamera()
        {
            if (m_MainCamera == null) return;
            if (m_IsAttached) return;

            Vector3 viewPosition = m_MainCamera.WorldToViewportPoint(transform.position);

            bool outsideView =
                viewPosition.x < -m_ViewportMargin ||
                viewPosition.x > 1f + m_ViewportMargin ||
                viewPosition.y < -m_ViewportMargin ||
                viewPosition.y > 1f + m_ViewportMargin;

            if (outsideView)
            {
                PopBalloonAndCountLose();
            }
        }

        void HandleMouseInput()
        {
            Vector2 mousePosition = m_MainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                Collider2D hit = Physics2D.OverlapPoint(mousePosition);

                if (hit != null && hit.gameObject == gameObject)
                {
                    m_IsDragging = true;
                    m_Offset = (Vector2)transform.position - mousePosition;

                    if (m_Rigidbody != null)
                    {
                        m_Rigidbody.linearVelocity = Vector2.zero;
                    }
                }
            }

            if (Input.GetMouseButton(0) && m_IsDragging)
            {
                if (m_Rigidbody != null)
                {
                    m_Rigidbody.MovePosition(mousePosition + m_Offset);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_IsDragging = false;
            }
        }

        void HandleTouchInput()
        {
            if (Input.touchCount <= 0) return;

            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = m_MainCamera.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                Collider2D hit = Physics2D.OverlapPoint(touchPosition);

                if (hit != null && hit.gameObject == gameObject)
                {
                    m_IsDragging = true;
                    m_Offset = (Vector2)transform.position - touchPosition;

                    if (m_Rigidbody != null)
                    {
                        m_Rigidbody.linearVelocity = Vector2.zero;
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved && m_IsDragging)
            {
                if (m_Rigidbody != null)
                {
                    m_Rigidbody.MovePosition(touchPosition + m_Offset);
                }
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                m_IsDragging = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (m_IsDead) return;

            CheckHitObject(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (m_IsDead) return;

            CheckHitObject(collision.collider);
        }

        void CheckHitObject(Collider2D collision)
        {
            if (collision.gameObject.name.Contains(m_BarName) && !m_IsAttached)
            {
                AttachBalloon(collision);
                return;
            }

            if (collision.gameObject.name.Contains(m_DangerName) || collision.gameObject.name.Contains(m_BladeName))
            {
                PopBalloonAndCountLose();
            }
        }

        void AttachBalloon(Collider2D collision)
        {
            if (m_IsAttached) return;

            BarLift barLift = collision.GetComponent<BarLift>();

            if (barLift == null)
            {
                barLift = collision.GetComponentInParent<BarLift>();
            }

            if (barLift != null)
            {
                barLift.AddBalloon();
            }

            m_IsAttached = true;

            if (m_Rigidbody != null)
            {
                m_Rigidbody.linearVelocity = Vector2.zero;
                m_Rigidbody.gravityScale = 0;
                m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }

            transform.SetParent(collision.transform);

            if (m_ConnectionEffectPrefab != null)
            {
                Instantiate(m_ConnectionEffectPrefab, transform.position, Quaternion.identity);
            }

            if (m_ConnectionSound != null)
            {
                AudioSource.PlayClipAtPoint(m_ConnectionSound, transform.position);
            }
        }

        public void PopBalloonAndCountLose()
        {
            if (m_IsDead) return;

            m_IsDead = true;

            if (m_KillEffectPrefab != null)
            {
                Instantiate(m_KillEffectPrefab, transform.position, Quaternion.identity);
            }

            if (m_KillSound != null)
            {
                AudioSource.PlayClipAtPoint(m_KillSound, transform.position);
            }

            if (global::LevelManager.Instance != null)
            {
                global::LevelManager.Instance.AddLostBalloon();
            }

            Destroy(gameObject);
        }
    }
}