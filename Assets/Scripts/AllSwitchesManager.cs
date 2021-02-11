using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AllSwitchesManager : MonoBehaviour
{
    // 直前まで押されていたスイッチの番号
    int lastSwitchNum = -1;

    SwitchBehavior sB01, sB02, sB03;

    void Awake()
    {
        try { sB01 = GameObject.FindGameObjectWithTag("SwitchHub01").GetComponent<SwitchBehavior>(); }
        catch (NullReferenceException) { }
        try { sB02 = GameObject.FindGameObjectWithTag("SwitchHub02").GetComponent<SwitchBehavior>(); }
        catch (NullReferenceException) { }
        try { sB03 = GameObject.FindGameObjectWithTag("SwitchHub03").GetComponent<SwitchBehavior>(); }
        catch (NullReferenceException) { }
    }

    public void OtherCororSwitchesOFF(int pressedSwitchNum)
    {
        // 直前に他のスイッチがONになっていた場合、そのスイッチをOFFにする
        if(lastSwitchNum != -1 && lastSwitchNum != pressedSwitchNum)
        {
            if(lastSwitchNum == 1)
            {
                sB01.SwitchBool = false;
                sB01.SwitchOFFProcess();
            }
            else if (lastSwitchNum == 2)
            {
                sB02.SwitchBool = false;
                sB02.SwitchOFFProcess();
            }
            else if (lastSwitchNum == 3)
            {
                sB03.SwitchBool = false;
                sB03.SwitchOFFProcess();
            }
        }
    }

    public int LastSwitchNum
    {
        set { lastSwitchNum = value; }
        get { return lastSwitchNum; }
    }
}
