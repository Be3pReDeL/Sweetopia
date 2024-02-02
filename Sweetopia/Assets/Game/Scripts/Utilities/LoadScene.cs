using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [OPS.Obfuscator.Attribute.DoNotRename]
    public static void LoadNextScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    } 

    [OPS.Obfuscator.Attribute.DoNotRename]
    public static void LoadPreviousScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    } 

    [OPS.Obfuscator.Attribute.DoNotRename]
    public static void LoadSceneByIndex(int index){
        SceneManager.LoadScene(index);
    } 

    [OPS.Obfuscator.Attribute.DoNotRename]
    public static void LoadSceneByRelativeIndex(int index){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + index);
    } 
}
