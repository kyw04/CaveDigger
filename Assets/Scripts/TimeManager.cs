using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float scale = 1f;
    public float deltaTime = 0f;

    private void FixedUpdate()
    {
        deltaTime = Time.deltaTime * scale;
    }
}
