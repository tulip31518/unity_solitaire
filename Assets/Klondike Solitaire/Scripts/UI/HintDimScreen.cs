using UnityEngine;
using System.Collections;

public class HintDimScreen : DimScreen
{
    protected override void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    protected override void Update()
    {
        
    }
}
