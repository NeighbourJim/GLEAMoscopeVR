using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLEAMoscopeVR.Settings;

public class CHANGELANGTEST : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            if(SettingsManager.Instance.CurrentLanguageSetting == LanguageSetting.English)
            {
                SettingsManager.Instance.SetLanguageSetting(LanguageSetting.Italian);
            }
            else
            {
                SettingsManager.Instance.SetLanguageSetting(LanguageSetting.English);
            }
        }
    }
}
