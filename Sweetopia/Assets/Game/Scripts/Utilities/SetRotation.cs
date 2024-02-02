using E7.NotchSolution;
using UnityEngine;

[OPS.Obfuscator.Attribute.DoNotObfuscateClass]
public class SetRotation : MonoBehaviour
{
    [SerializeField] private ScreenOrientation _screenOrientation;
    private void Start(){
        Screen.orientation = _screenOrientation;
    }
}
