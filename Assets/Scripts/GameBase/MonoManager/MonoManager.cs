using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MonoManager : Singleton<MonoManager>
{
    private MonoController controller = null;
    public MonoManager()
    {
        var controllerObj = new GameObject("MonoController");
        Object.DontDestroyOnLoad(controllerObj);
        controller = controllerObj.AddComponent<MonoController>();
    }
    public void AddUpdateListener(UnityAction listener)
    {
        controller.UpdateEvents += listener;
    }
    public void RemoveUpdateListener(UnityAction listener)
    {
        controller.UpdateEvents -= listener;
    }
    public void StartCoroutine(IEnumerator enumerator)
    {
        controller.StartCoroutine(enumerator);
    }
    public void StopCoroutine(IEnumerator enumerator)
    {
        controller.StopCoroutine(enumerator);
    }
}
