using System.Collections;
using UnityEngine;
using TMPro;
using UnityAtoms.BaseAtoms;

public class TutorialDisplayController : MonoBehaviour
{
    [SerializeField] private TMP_Text textBox;  // Assign your TextMeshPro UI element in the Inspector
    [SerializeField, Range(0, 10)] private float delayTime = 5f;  // Time in seconds before enabling the text box
    [SerializeField] private Vector3Event shakeEvent;

    private bool eventFired = false;
    private bool active = false;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delayTime);

        // Enable the text box only if the event hasn't fired
        if (!eventFired)
        {
            active = true;
            textBox.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (shakeEvent != null)
            shakeEvent.Register(OnEventTriggered);
    }

    private void OnDisable()
    {
        if (shakeEvent != null)
            shakeEvent.Unregister(OnEventTriggered);
    }

    // Call this method when the event occurs to prevent enabling the text box
    public void OnEventTriggered()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
