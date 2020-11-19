using UnityEngine;

public class Control : MonoBehaviour
{
    public GameObject testObj;
    private void Awake()
    {
        testObj = GameObject.Find("/TestObject");
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (testObj != null)
            {
                testObj.SetActive(!testObj.activeSelf);
            }
        }
    }
}
