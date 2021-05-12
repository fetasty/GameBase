using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMgr : Singleton<ModelMgr>
{
    private Dictionary<string, BaseModel> modelDic = new Dictionary<string, BaseModel>();
    public void RegistModel(BaseModel model)
    {
        if (model == null) { return; }
        if (modelDic.ContainsKey(model.Name))
        {
            throw new System.Exception($"Try to register model with name {model.Name} which already exist!");
        }
        modelDic[model.Name] = model;
    }
    public void UnregistModel(string modelName)
    {
        if (modelDic.ContainsKey(modelName))
        {
            modelDic.Remove(modelName);
        }
    }
    public T GetModel<T>(string modelName) where T : BaseModel
    {
        BaseModel model = null;
        modelDic.TryGetValue(modelName, out model);
        return model as T;
    }
}
