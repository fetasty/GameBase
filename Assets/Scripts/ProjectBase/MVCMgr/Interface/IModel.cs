using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModel
{
    string Name { get; }
    bool Save();
    bool Load();
}
