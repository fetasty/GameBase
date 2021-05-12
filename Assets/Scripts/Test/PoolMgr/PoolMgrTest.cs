using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgrTest : MonoBehaviour
{
    void Start()
    {
        InputMgr.Instance.ListenMouse = true;
        MessageCenter.Instance.AddHandle<int>(BaseConst.Msg_MouseDown, OnMouseBtnDown);
    }
    private void OnDestroy()
    {
        MessageCenter.Instance.RemoveHandle<int>(BaseConst.Msg_MouseDown, OnMouseBtnDown);
    }
    private void OnMouseBtnDown(int mouse)
    {
        Camera c = Camera.main;
        switch (mouse)
        {
            case 0:
                {
                    PoolMgr.Instance.Get("Prefab/Sphere", (obj) =>
                    {
                        obj.transform.position = c.transform.position;
                        var rig = obj.GetComponent<Rigidbody>();
                        rig.AddForce(c.transform.forward * 20, ForceMode.VelocityChange);
                    });
                }
                break;
            case 1:
                {
                    PoolMgr.Instance.Get("Prefab/Cube", (obj) =>
                    {
                        obj.transform.position = c.transform.position;
                        var rig = obj.GetComponent<Rigidbody>();
                        rig.AddForce(c.transform.forward * 20, ForceMode.VelocityChange);
                    });
                }
                break;
            case 2:
                {
                    //PoolMgr.Instance.Clear("Prefab/Cube");
                    PoolMgr.Instance.ClearAll();
                }
                break;
        }
    }
}
