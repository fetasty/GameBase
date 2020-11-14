using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TestMono
{
    public void TestUpdate()
    {
        Debug.Log("TestUpdate");
    }
    public IEnumerator TestCoroutine()
    {
        for (int i = 0; i < 10; ++i)
        {
            Debug.Log("TestCoroutine: " + i + " - " + Time.time);
            yield return new WaitForSeconds(2f);
        }
    }
}

public class MonoTest : MonoBehaviour
{
    private IEnumerator testEnumerator;
    private void Start() {
        TestMono t = new TestMono();
        MonoManager.Instance.AddUpdateListener(t.TestUpdate);
        testEnumerator = t.TestCoroutine();
        MonoManager.Instance.StartCoroutine(testEnumerator);
    }
    private void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left click, stopcoroutine");
            MonoManager.Instance.StopCoroutine(testEnumerator);
        }
    }
}
