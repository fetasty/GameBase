using UnityEngine;

public class UIManagerTest : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        InputManager.Instance.AddListenKey(KeyCode.A);
        InputManager.Instance.AddListenKey(KeyCode.S);
        MessageCenter.Instance.AddListener<KeyCode>(BaseMessage.KeyDown, OnKeyDown);
    }
    private void OnDestroy() {
        MessageCenter.Instance.RemoveListener<KeyCode>(BaseMessage.KeyDown, OnKeyDown);
    }

    private void OnKeyDown(KeyCode key) {
        switch (key) {
            case KeyCode.A:
                UIManager.Instance.ShowPanel<LoginPanel>("LoginPanel");
                break;
            case KeyCode.S:
                UIManager.Instance.HidePanel("LoginPanel");
                break;
        }
    }
}
