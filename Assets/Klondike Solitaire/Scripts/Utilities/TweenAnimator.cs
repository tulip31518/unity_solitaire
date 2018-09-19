using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAnimator : MonoBehaviour {

	public static TweenAnimator instance;
	public Enums.LerpType animationMethod;
	public float shakePower;
	public float shakeSpeed;
	private delegate float AnimationDelegate (float start, float end, float value);

	private void Awake() {
		instance = this;
	}

	private AnimationDelegate GetAnimationMethod() {
		switch (animationMethod)
		{
			case Enums.LerpType.hermite:
				return Mathfx.Hermite;
			case Enums.LerpType.sinerp:
				return Mathfx.Sinerp;
			case Enums.LerpType.coserp:
				return Mathfx.Coserp;
			case Enums.LerpType.berp:
				return Mathfx.Berp;
			case Enums.LerpType.clerp:
				return Mathfx.Clerp;
			case Enums.LerpType.lerp:
				return Mathfx.Lerp;
			default:
				return Mathfx.Lerp;
		}
	}

	public Coroutine RunMoveAnimation(Transform target, Vector3 to, float animTime, float delay = 0, Action onComplete = null, Action onUpdate = null, bool isLocal = false) {
		return StartCoroutine(MoveAnimation(target, to, animTime, delay, onComplete, onUpdate, isLocal));
	}

	public Coroutine RunRotationAnimation(Transform target, Vector3 to, float animTime, float delay = 0, Action onComplete = null, Action onUpdate = null) {
		return StartCoroutine(RotationAnimation(target, to, animTime, delay, onComplete, onUpdate));
	}

	public Coroutine RunScaleAnimation(Transform target, Vector3 to, float animTime, float delay = 0, Action onComplete = null, Action onUpdate = null) {
		return StartCoroutine(ScaleAnimation(target, to, animTime, delay, onComplete, onUpdate));
	}

	public Coroutine RunFadeAnimation(CanvasGroup target, float to, float animTime, float delay = 0, Action onComplete = null, Action onUpdate = null) {
		return StartCoroutine(FadeAnimation(target, to, animTime, delay, onComplete, onUpdate));
	}

	public Coroutine RunTimer(float animTime, Action onComplete) {
		return StartCoroutine(RunTimerCoroutine(animTime, onComplete));
	}

	public Coroutine RunShakeAnimation(Transform target, float animTime, Action onComplete = null) {
		return StartCoroutine(ShakeAnimation(target, animTime, onComplete));
	}

	IEnumerator MoveAnimation(Transform target, Vector3 to, float animTime, float delay, Action onComplete, Action onUpdate, bool isLocal) {
		if (delay > 0)
			yield return new WaitForSeconds(delay);

		AnimationDelegate animationDelegate = GetAnimationMethod();
		Vector3 from = isLocal ? target.localPosition : target.position;
		float timeRemaining = animTime;
		float t;
		while (timeRemaining > 0)
		{
			timeRemaining -= Time.deltaTime;
			t = timeRemaining / animTime;
			if (isLocal)
				target.localPosition = new Vector3(animationDelegate(to.x, from.x, t), animationDelegate(to.y, from.y, t), from.z);
			else
				target.position = new Vector3(animationDelegate(to.x, from.x, t), animationDelegate(to.y, from.y, t), from.z);
			onUpdate.RunAction();
			yield return null;
		}
		if(isLocal)
			target.localPosition = to;
		else
			target.position = to;
		onComplete.RunAction();
	}

	IEnumerator RotationAnimation(Transform target, Vector3 to, float animTime, float delay, Action onComplete, Action onUpdate) {
		if (delay > 0)
			yield return new WaitForSeconds(delay);

		AnimationDelegate animationDelegate = GetAnimationMethod();
		Vector3 from = target.rotation.eulerAngles;
		float timeRemaining = animTime;
		float t;
		while (timeRemaining > 0)
		{
			timeRemaining -= Time.deltaTime;
			t = timeRemaining / animTime;
			target.rotation = Quaternion.Euler(animationDelegate(to.x, from.x, t), animationDelegate(to.y, from.y,t), animationDelegate(to.z, from.z, t));
			onUpdate.RunAction();
			yield return null;
		}
		target.rotation = Quaternion.Euler(to);
		onComplete.RunAction();
	}

	IEnumerator ScaleAnimation(Transform target, Vector3 to, float animTime, float delay, Action onComplete, Action onUpdate) {
		if (delay > 0)
			yield return new WaitForSeconds(delay);

		AnimationDelegate animationDelegate = GetAnimationMethod();
		Vector3 from = target.localScale;
		float timeRemaining = animTime;
		float t;
		while (timeRemaining > 0)
		{
			timeRemaining -= Time.deltaTime;
			t = timeRemaining / animTime;
			target.localScale = new Vector3(animationDelegate(to.x, from.x, t), animationDelegate(to.y, from.y, t), animationDelegate(to.z, from.z, t));
			onUpdate.RunAction();
			yield return null;
		}
		target.localScale = to;
		onComplete.RunAction();
	}

	IEnumerator FadeAnimation(CanvasGroup target, float to, float animTime, float delay, Action onComplete, Action onUpdate) {
		if (delay > 0)
			yield return new WaitForSeconds(delay);

		AnimationDelegate animationDelegate = GetAnimationMethod();
		float from = target.alpha;
		float timeRemaining = animTime;
		float t;
		while (timeRemaining > 0)
		{
			timeRemaining -= Time.deltaTime;
			t = timeRemaining / animTime;
			target.alpha = animationDelegate(to, from, t);
			onUpdate.RunAction();
			yield return null;
		}
		target.alpha = to;
		onComplete.RunAction();
	}

	IEnumerator RunTimerCoroutine(float animTime, Action onComplete) {
		yield return new WaitForSeconds(animTime);
		onComplete.RunAction();
	}

	IEnumerator ShakeAnimation(Transform target, float animTime, Action onComplete) {

		Vector3 from = target.position;
		float timeRemaining = animTime;
		float t = 0;
		while (t < timeRemaining)
		{
			t += Time.deltaTime;
			target.position = from + Vector3.right * shakePower * Mathf.Sin(t * shakeSpeed);
			yield return null;
		}
		target.transform.position = from;
		onComplete.RunAction();
	}


	public void Kill(Coroutine animation) {
		if (animation != null)
			StopCoroutine(animation);
	}
}
