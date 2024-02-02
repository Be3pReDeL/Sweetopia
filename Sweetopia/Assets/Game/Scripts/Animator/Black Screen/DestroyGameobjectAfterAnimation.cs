using UnityEngine;

[OPS.Obfuscator.Attribute.DoNotObfuscateClass]
public class DestroyGameobjectAfterAnimation : StateMachineBehaviour {
    [OPS.Obfuscator.Attribute.DoNotRename]
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Destroy(animator.gameObject);
    }
}
