using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides global access to data specific for Level 1
/// </summary>
public class Level1DataManager : LevelDataManager
{
    
    public static new LevelDataManager Instance
    {
        get
        {
            if (_i == null || !(_i is Level1DataManager)) _i = (Instantiate(Resources.Load("Managers/Level 1 Data Manager")) as GameObject).GetComponent<Level1DataManager>();
            return _i;
        }
    }


}
