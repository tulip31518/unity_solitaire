using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScaleFactorConverter  {

    private static Canvas gameCanvas;

	public static float MultiplyByScaleFactor(float value)
    {
        if(CheckIfCanvasIsEmpty())
        {
            return 0;
        }
        return value * gameCanvas.scaleFactor;
    }


    public static float DivideByScaleFactor(float value)
    {
        if (CheckIfCanvasIsEmpty())
        {
            return 0;
        }
        return value / gameCanvas.scaleFactor;
    }

    private static bool CheckIfCanvasIsEmpty()
    {
        if (gameCanvas == null)
        {
            GameObject canvasGO = GameObject.FindGameObjectWithTag("GameCanvas");
            if (canvasGO != null)
            {
                gameCanvas = canvasGO.GetComponent<Canvas>();
                return false;
            }
            return true;
        }
        return false;
    }
}
