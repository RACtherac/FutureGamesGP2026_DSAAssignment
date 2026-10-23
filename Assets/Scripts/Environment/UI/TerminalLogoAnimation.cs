
using UnityEngine;

public class TerminalLogoAnimation : MonoBehaviour {
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 18f;

    [Header("Pulse")]
    [SerializeField] private float pulseSpeed = 1.5f;
    [SerializeField] private float pulseAmount = 0.04f;

    private Vector3 originalScale;

    private void Awake() {
        originalScale = transform.localScale;
    }

    private void Update() {
        transform.Rotate(
            0f,
            0f,
            rotationSpeed * Time.unscaledDeltaTime
        );

        float pulse = 1f + Mathf.Sin(
            Time.unscaledTime * pulseSpeed
        ) * pulseAmount;

        transform.localScale = originalScale * pulse;
    }
}
