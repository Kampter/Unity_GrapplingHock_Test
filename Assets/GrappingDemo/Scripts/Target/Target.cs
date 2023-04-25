using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public event Action<Target> onDestoryed;

    public void Start()
    {
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        onDestoryed?.Invoke(this);
    }
}
