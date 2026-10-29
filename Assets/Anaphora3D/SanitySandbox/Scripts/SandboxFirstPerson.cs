using UnityEngine;

namespace Anaphora.SanitySandbox
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class SandboxFirstPerson : MonoBehaviour
    {
        public Transform View;
        public float Speed = 4;
        public float MouseSensitivity = 2;
        public bool Captured { get; private set; }
        private CharacterController controller;
        private float pitch;
        private float verticalSpeed;

        private void Awake() { controller = GetComponent<CharacterController>(); }

        public void Capture(bool capture)
        {
            Captured = capture;
            Cursor.lockState = capture ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !capture;
        }

        private void Update()
        {
            if (!Captured) return;
            transform.Rotate(0, Input.GetAxis("Mouse X") * MouseSensitivity, 0);
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * MouseSensitivity, -80, 80);
            View.localRotation = Quaternion.Euler(pitch, 0, 0);
            Vector3 move = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), 0,
                Input.GetAxisRaw("Vertical")), 1);
            if (controller.isGrounded && verticalSpeed < 0) verticalSpeed = -2;
            verticalSpeed += Physics.gravity.y * Time.deltaTime;
            controller.Move((transform.TransformDirection(move) * Speed + Vector3.up * verticalSpeed) * Time.deltaTime);
        }

        public void ReturnToStart()
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(new Vector3(0, 0.1f, -6), Quaternion.identity);
            pitch = 0;
            verticalSpeed = 0;
            View.localRotation = Quaternion.identity;
            controller.enabled = true;
        }

        private void OnDisable() { Capture(false); }
    }
}
