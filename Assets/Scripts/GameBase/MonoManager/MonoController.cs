using UnityEngine;
using UnityEngine.Events;

public class MonoController : MonoBehaviour {
    public event UnityAction UpdateEvents = null;
    void Update() {
        if (UpdateEvents != null) {
            UpdateEvents();
        }
    }
}
