using UnityEngine;

public class SixtyFPS : MonoBehaviour
{
    private void OnEnable(){
        Application.targetFrameRate = 60;
    }
}
