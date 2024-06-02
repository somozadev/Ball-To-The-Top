using System;
using System.Collections;
using UnityEngine;

public class Loading : MonoBehaviour
{
    [SerializeField] private GameObject LoadingImg;


    private void OnEnable()
    {
        StartCoroutine(Load());
    }

    private void OnDisable()
    {
        StopCoroutine(Load());
    }

    private IEnumerator Load()
    {
        while (gameObject.activeSelf)
        {
            LoadingImg.transform.Rotate(-Vector3.forward,1f);
            yield return null;
        }
        yield return null;
    }
}
