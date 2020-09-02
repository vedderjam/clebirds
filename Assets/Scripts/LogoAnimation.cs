using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    public void OnAnimationEnded()
    {
        gameObject.SetActive(false);
    }
}
