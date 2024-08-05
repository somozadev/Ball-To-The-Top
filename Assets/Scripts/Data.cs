using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[System.Serializable]
public class Data
{
    public string Time;
    public float XPos;
    public float YPos;
    public float ZPos;

    public float XRot;
    public float YRot;
    public float ZRot;

    public Data()
    {
        ResetData();
    }

    public bool HasSavedData() =>
        (Time != "00:00:00" || XPos != 0 || YPos != 0 || ZPos != 0 || XRot != 0 || YRot != 0 || ZRot != 0);


    public void ResetData()
    {
        Time = "00:00:00";
        XPos = 0;
        YPos = 0;
        ZPos = 0;
        XRot = 0;
        YRot = 0;
        ZRot = 0;
    }

    public Data(Vector3 position, Vector3 rotation, string time)
    {
        Time = time;
        XPos = position.x;
        YPos = position.y;
        ZPos = position.z;
        XRot = rotation.x;
        YRot = rotation.y;
        ZRot = rotation.z;
    }
}