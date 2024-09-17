using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera rattleVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera revealVirtualCamera;

    [SerializeField] private Vector3Event rattleEvent;
    [SerializeField] private VoidEvent shakeStopEvent;

    private void OnEnable()
    {
        rattleEvent.Register(OnShake);
        shakeStopEvent.Register(OnShakeStop);
    }

    private void OnDisable()
    {
        rattleEvent.Unregister(OnShake);
        shakeStopEvent.Unregister(OnShakeStop);
    }

    private void OnShake(Vector3 accel)
    {
        revealVirtualCamera.Priority = 0;
        rattleVirtualCamera.Priority = 10;
    }

    private void OnShakeStop()
    {
        revealVirtualCamera.Priority = 10;
        rattleVirtualCamera.Priority = 0;
    }
}
