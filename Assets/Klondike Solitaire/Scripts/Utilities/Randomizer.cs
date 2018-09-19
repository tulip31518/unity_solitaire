using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Randomizer : MonoBehaviour {

	public static float GetRandomNumber(float min, float max)
	{
		int seed = Guid.NewGuid().GetHashCode();
		UnityEngine.Random.InitState(seed);
		return UnityEngine.Random.Range(min, max);
	}

	public static int GetRandomNumber(int min, int max)
	{
		int seed = Guid.NewGuid().GetHashCode();
		UnityEngine.Random.InitState(seed);
		return UnityEngine.Random.Range(min, max);
	}

}
