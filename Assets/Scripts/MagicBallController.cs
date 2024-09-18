/** 
  * @file MagicBallController.cs
  * @brief Controls the magic 8 ball
  * 
  * @author Logan Bender
**/

using System;
using System.Collections;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class MagicBallController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody ballRb;
    [SerializeField] private GameObject die;
    [SerializeField] private Material dieMat;

    [Header("Scriptable Objects")]
    [SerializeField] private VoidEvent onShakeStoped;
    [SerializeField] private Vector3Event onShake;
    [SerializeField] private ResponsesScriptableObject Responses;

    [Header("Long Message First Text Boxes")]
    [SerializeField] private TMPro.TMP_Text upperLongText;
    [SerializeField] private TMPro.TMP_Text lowerLongText;

    [Header("Short Message First Text Boxes")]
    [SerializeField] private TMPro.TMP_Text upperShortText;
    [SerializeField] private TMPro.TMP_Text lowerShortText;

    [Header("Config")]
    [SerializeField, Range(1f, 10f)] private float ballSnapTime = 1f;
    [SerializeField, Range(1f, 1000f)] private float shakeForceMult = 100f;
    [SerializeField, Range(0f, 90f)] private float randomTiltMax = 20f;

    private Vector3 revealBallPosition;
    private Quaternion longLineFirstBallRotation;
    private Quaternion shortLineFirstBallRotation;

    private Coroutine revealCoroutine;

    private bool initShakeFlag;

    private Color dieColor;

    private void Awake()
    {
        revealBallPosition = ballRb.transform.position;
        longLineFirstBallRotation = ballRb.transform.rotation;
        ballRb.transform.Rotate(Vector3.forward, 180f);
        shortLineFirstBallRotation = ballRb.transform.rotation;

        // flip the ball around the x-axis to show the other side of the ball
        ballRb.transform.Rotate(Vector3.right, 180f);

        if (dieMat != null)
        {
            dieColor = dieMat.color;
        }
    }

    void OnEnable()
    {
        onShakeStoped.Register(OnShakeStopped);
        onShake.Register(OnShaking);
    }

    void OnDisable()
    {
        onShakeStoped.Unregister(OnShakeStopped);
        onShake.Unregister(OnShaking);
    }

    private void Update()
    {
        //if (Input.acceleration.sqrMagnitude > 3)
        //{
        //    ResponsesScriptableObject.Response response = Responses.GetRandomResponse();
        //    //Debug.Log(response.upperLineResponseText);
        //    //Debug.Log(response.lowerLineResponseText);
        //}
    }

    void OnShaking(Vector3 accel)
    {
        // Code to run during shaking
        Debug.Log("Phone is shaking!");

        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
            revealCoroutine = null;
        }

        // Play hide animation
        animator.SetBool("IsAnswerRevealed", false);

        ballRb.isKinematic = false;
        ballRb.AddForce(accel * shakeForceMult);
        //ballRb.AddForce(new Vector3(Mathf.PerlinNoise(Time.time, Time.time) * shakeForceMult, 0, Mathf.Sin(Time.time) * shakeForceMult));

        // hide die and text
        dieMat.color = Color.black;
        upperShortText.color = new Color(0f, 0f, 0f, 0f);
        lowerShortText.color = new Color(0f, 0f, 0f, 0f);
        upperLongText.color = new Color(0f,0f,0f,0f);
        lowerLongText.color = new Color(0f,0f,0f,0f);
    }

    void OnShakeStopped()
    {
        // the first time we load the game, don't fetch a response
        if (!initShakeFlag)
        {
            initShakeFlag = true;
            return;
        }

        // Code to run when shaking stops
        Debug.Log("Phone shaking stopped!");

        // Stop shaking animation
        //animator.SetBool("Shaking", false);

        ballRb.isKinematic = true;

        // change message on the ball
        ResponsesScriptableObject.Response response = Responses.GetRandomResponse();

        if (response.orientation == ResponsesScriptableObject.Orientation.LONG_LINE_FIRST)
        {
            // set the target rotation based on the response orientation
            revealCoroutine = StartCoroutine(RevealLongFirstCoroutine());

            // use long line first text boxes
            upperLongText.gameObject.SetActive(true);
            lowerLongText.gameObject.SetActive(true);

            upperLongText.SetText(response.upperLineResponseText);
            lowerLongText.SetText(response.lowerLineResponseText);

            // turn off short line first text boxes
            upperShortText.gameObject.SetActive(false);
            lowerShortText.gameObject.SetActive(false);

            // rotate die
            //transform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-randomTiltMax / 2f, randomTiltMax / 2f));
        }
        else
        {
            // set the target rotation based on the response orientation
            revealCoroutine = StartCoroutine(RevealShortFirstCoroutine());

            // use short line first text boxes
            upperShortText.gameObject.SetActive(true);
            lowerShortText.gameObject.SetActive(true);

            upperShortText.SetText(response.upperLineResponseText);
            lowerShortText.SetText(response.lowerLineResponseText);

            // turn off long line first text boxes
            upperLongText.gameObject.SetActive(false);
            lowerLongText.gameObject.SetActive(false);

            // rotate die
            //transform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-randomTiltMax / 2f, randomTiltMax / 2f) + 180f);
        }

    }

    private IEnumerator RevealLongFirstCoroutine()
    {
        float lerp = 0;
        bool revealFlag = false;

        Quaternion targetRotation = longLineFirstBallRotation;
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z + UnityEngine.Random.Range(-randomTiltMax / 2f, randomTiltMax / 2f));

        while (lerp < 1)
        {
            lerp += Time.deltaTime / ballSnapTime;

            ballRb.transform.position = Vector3.Lerp(ballRb.transform.position, revealBallPosition, lerp);

            ballRb.transform.rotation = Quaternion.Lerp(ballRb.transform.rotation, targetRotation, lerp);

            if (lerp >= 0.2f)
            {
                if (!revealFlag)
                {
                    revealFlag = true;
                    animator.SetBool("IsAnswerRevealed", true);
                }

                upperLongText.color = Color.Lerp(upperLongText.color, Color.white, lerp);
                lowerLongText.color = Color.Lerp(lowerLongText.color, Color.white, lerp);
                dieMat.color = Color.Lerp(dieMat.color, dieColor, lerp);
            }

            yield return null;
        }

        ballRb.transform.position = revealBallPosition;
        ballRb.transform.rotation = targetRotation;
    }

    private IEnumerator RevealShortFirstCoroutine()
    {
        float lerp = 0;
        bool revealFlag = false;

        Quaternion targetRotation = shortLineFirstBallRotation;
        targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, targetRotation.eulerAngles.z + UnityEngine.Random.Range(-randomTiltMax / 2f, randomTiltMax / 2f));

        while (lerp < 1)
        {
            lerp += Time.deltaTime / ballSnapTime;

            ballRb.transform.position = Vector3.Lerp(ballRb.transform.position, revealBallPosition, lerp);

            ballRb.transform.rotation = Quaternion.Lerp(ballRb.transform.rotation, targetRotation, lerp);

            if (lerp >= 0.1f )
            {
                if (!revealFlag)
                {
                    revealFlag = true;
                    animator.SetBool("IsAnswerRevealed", true);
                }

                upperShortText.color = Color.Lerp(upperShortText.color, Color.white, lerp);
                lowerShortText.color = Color.Lerp(lowerShortText.color, Color.white, lerp);
                dieMat.color = Color.Lerp(dieMat.color, dieColor, lerp);
            }

            yield return null;
        }

        ballRb.transform.position = revealBallPosition;
        ballRb.transform.rotation = targetRotation;
    }
}