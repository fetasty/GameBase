using UnityEngine;

public class TestObject : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log(name + " Awake");
    }

    private void OnEnable()
    {
        Debug.Log(name + "OnEnable");
    }

    void Start()
    {
        Debug.Log(name + " Start");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        Debug.Log(name + "OnDisable");
    }
}
