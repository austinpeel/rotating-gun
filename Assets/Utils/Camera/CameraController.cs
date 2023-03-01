using System.Collections;
using UnityEngine;

namespace Utils
{
    public class CameraController : MonoBehaviour
    {
        public Vector3 targetPosition;
        public Vector3 targetRotation;
        public float moveTime = 1;

        [Header("Zoom")]
        public bool canZoom = false;
        public float minZoom = 1f;
        public float maxZoom = 10f;
        public float zoomSpeed = 1f;
        public Transform zoomTarget;
        private float initialDistance;

        [Header("State")]
        public CameraState state;

        private void OnEnable()
        {
            StopAllCoroutines();
            if (state)
            {
                transform.position = state.position;
                transform.rotation = state.rotation;
                transform.localScale = state.scale;
            }
            StartCoroutine(MoveTo(targetPosition, targetRotation, moveTime));
        }

        private void Start()
        {
            if (!zoomTarget) return;
            initialDistance = Vector3.Distance(transform.position, zoomTarget.position);
        }

        private void LateUpdate()
        {
            if (!zoomTarget || !canZoom) return;

            // Get the input from the mouse wheel
            float zoomInput = Input.GetAxis("Mouse ScrollWheel");

            // Calculate the new distance between camera and target
            float newDistance = Mathf.Clamp(initialDistance - zoomInput * zoomSpeed, minZoom, maxZoom);

            // Calculate the new camera position
            Vector3 direction = (transform.position - zoomTarget.position).normalized;
            transform.position = zoomTarget.position + direction * newDistance;
        }

        private IEnumerator MoveTo(Vector3 targetPosition, Vector3 targetRotation, float moveTime)
        {
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;
            float time = 0;

            while (time < moveTime)
            {
                time += Time.deltaTime;
                float t = time / moveTime;
                t = t * t * (3f - 2f * t);
                transform.position = Vector3.Slerp(startPosition, targetPosition, t);
                transform.rotation = Quaternion.Slerp(startRotation, Quaternion.Euler(targetRotation), t);
                // transform.rotation = Quaternion.Euler(Vector3.Slerp(startRotation.eulerAngles, targetRotation, t));
                yield return null;
            }

            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(targetRotation);
        }

        public void SetCamera()
        {
            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(targetRotation);
        }
    }
}
