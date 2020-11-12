using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TestSingleton : SingletonBase<TestSingleton>
{
    public static int Index = 99;
    public int myIndex;
    public TestSingleton()
    {
        this.myIndex = Index;
        ++Index;
    }
    public void ShowIndex()
    {
        Debug.Log("TestSingleton index: " + this.myIndex);
    }
}

public class SingletonTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestSingleton.Instance.ShowIndex();
    }

    private void OnEnable()
    {
        TestSingleton.Instance.ShowIndex();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
