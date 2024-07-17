using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFiles
{
    public int levelsUnlocked;

    public SaveFiles (int levelsUnlocked)
    {
        this.levelsUnlocked = levelsUnlocked;
    }
}
