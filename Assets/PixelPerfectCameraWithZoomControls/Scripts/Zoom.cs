﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    [SerializeField]
    private Camera ppwzCamera;
    private PerfectPixelWithZoom ppwz;

    void Start()
    {
        ppwz = ppwzCamera.GetComponent<PerfectPixelWithZoom>();
    }
}
