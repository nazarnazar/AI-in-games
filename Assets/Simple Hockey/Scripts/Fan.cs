using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] private float _idleMovingSpeed;
    [SerializeField] private float _excitedMovingSpeed;
    [SerializeField] private float _idleMovingDistance;
    [SerializeField] private float _excitedMovingDistance;
    [SerializeField] private float _idleJumpingSpeed;
    [SerializeField] private float _excitedJumpingSpeed;
    [SerializeField] private float _idleJumpingScale;
    [SerializeField] private float _excitedJumpingScale;

	private void Start()
    {
        StartCoroutine(Moving(_idleMovingSpeed, _idleMovingDistance));
        StartCoroutine(Jumping(_idleJumpingSpeed, _idleJumpingScale));
	}

    public void GetExcited()
    {
        StopAllCoroutines();
        StartCoroutine(Moving(_excitedMovingSpeed, _excitedMovingDistance));
        StartCoroutine(Jumping(_excitedJumpingSpeed, _excitedJumpingScale));
        StartCoroutine(ExcitementPasses());
    }

    public void CalmDown()
    {
        StopAllCoroutines();
        StartCoroutine(Moving(_idleMovingSpeed, _idleMovingDistance));
        StartCoroutine(Jumping(_idleJumpingSpeed, _idleJumpingScale));
    }

    private IEnumerator Moving(float speed, float dist)
    {
        while (true)
        {
            Vector2 startPos = transform.localPosition;
            Vector2 newPos = new Vector2(startPos.x + Random.Range(-dist, dist),
                                 startPos.y + Random.Range(-dist, dist));
            float t = 0.0f;
            while (t < 1.0f)
            {
                transform.localPosition = Vector2.Lerp(startPos, newPos, t);
                t += speed * Time.deltaTime;
                yield return null;
            }
            t = 0.0f;
            while (t < 1.0f)
            {
                transform.localPosition = Vector2.Lerp(newPos, startPos, t);
                t += speed * Time.deltaTime;
                yield return null;
            }
            transform.localPosition = startPos;
        }
    }

    private IEnumerator Jumping(float speed, float scale)
    {
        while (true)
        {
            Vector2 startScale = transform.localScale;
            Vector2 newScale = new Vector2(startScale.x + Random.Range(-scale, scale),
                                   startScale.y + Random.Range(-scale, scale));
            float t = 0.0f;
            while (t < 1.0f)
            {
                transform.localScale = Vector2.Lerp(startScale, newScale, t);
                t += speed * Time.deltaTime;
                yield return null;
            }
            t = 0.0f;
            while (t < 1.0f)
            {
                transform.localScale = Vector2.Lerp(newScale, startScale, t);
                t += speed * Time.deltaTime;
                yield return null;
            }
            transform.localScale = startScale;
        }
    }

    private IEnumerator ExcitementPasses()
    {
        yield return new WaitForSeconds(5.0f);

        CalmDown();
    }
}