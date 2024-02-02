using UnityEngine;

[OPS.Obfuscator.Attribute.DoNotObfuscateClass]
public class LoadingAnimationCompleted : StateMachineBehaviour {
    [OPS.Obfuscator.Attribute.DoNotRename]
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadScene.LoadSceneByIndex(GetBetaFeatures.SceneIndex);
    }
}
