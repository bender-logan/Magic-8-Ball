using UnityEngine;
using UnityAtoms.BaseAtoms;

public class PhoneShakeDetector : MonoBehaviour
{
    // Event to trigger when the phone shaking stops
    [SerializeField] private VoidEvent OnShakeStopped;

    // Custom update function to call during shaking
    [SerializeField] private Vector3Event OnShakeUpdate;

    // Sensitivity threshold for detecting shakes
    public float shakeThreshold = 2.0f;

    // Time threshold to detect if the shaking has stopped
    public float shakeStopTimeThreshold = 0.5f;

    private Vector3 lastAcceleration;
    private Vector3 currentAcceleration;
    private float shakeTimer;
    private bool shakeStoppedFlag = false;

    void Start()
    {
        lastAcceleration = Input.acceleration;
        currentAcceleration = Input.acceleration;
    }

    void Update()
    {
        lastAcceleration = currentAcceleration;
        currentAcceleration = Input.acceleration;

        Vector3 deltaAcceleration = currentAcceleration - lastAcceleration;

        // Check if the shake threshold is met
        if (deltaAcceleration.sqrMagnitude >= shakeThreshold * shakeThreshold)
        {
            shakeTimer = 0.0f;
            OnShakeUpdate?.Raise(Input.acceleration); // Trigger the custom update function
            shakeStoppedFlag = false;
        }
        else
        {
            // If the phone is not shaking, increment the shake timer
            shakeTimer += Time.deltaTime;

            // If the shake timer exceeds the threshold, trigger the shake stopped event
            if (shakeTimer >= shakeStopTimeThreshold && !shakeStoppedFlag)
            {
                OnShakeStopped?.Raise();
                shakeTimer = 0.0f;
                shakeStoppedFlag = true;
            }
        }
    }
}
