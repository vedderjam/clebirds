using System;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public static Action UpdateEvent;
    public static Action LateUpdateEvent;
    public static Action FixedUpdateEvent;
    
    void Update()
    {
        UpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        LateUpdateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        FixedUpdateEvent?.Invoke();
    }
}
