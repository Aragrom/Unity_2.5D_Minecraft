using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public FullScreenMode fullScreenMode = FullScreenMode.ExclusiveFullScreen;

    // Start is called before the first frame update
    void Start()
    {
        Screen.fullScreenMode = fullScreenMode;

        //PlayerSettings.resizableWindow = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetAndUpdateScreenMode()
    { 
    
    }

    void SaveSettings()
    { 
    
    }

    void LoadSettings()
    { 
    
    }
}
