using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModel : IModel
{
    public string Name { get; set; }
    public virtual bool Save() { return false; }
    public virtual bool Load() { return false; }
}
