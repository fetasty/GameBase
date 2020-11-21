using UnityEngine;

public class LoginPanel : BasePanel {
    protected override void OnButtonClick(string name) {
        switch (name) {
            case "ButtonPlay":
                Debug.Log("Play clicked!!");
                break;
            case "ButtonExit":
                Debug.Log("Exit clicked!!!");
                break;
        }
    }
    public override void OnShow() {
        Debug.Log("LoginPanel show");
    }
    public override void OnHide() {
        Debug.Log("LoginPanel hide");
    }
}
