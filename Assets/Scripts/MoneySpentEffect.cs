using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneySpentEffect : MonoBehaviour
{
    public void OnEffectFinished()
    {
        gameObject.SetActive(false);
    }
}
