
using UnityEngine;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "DebugPrint Task",
        menuName = "Surveillance Tasks/Debug Print Task"
    )]
    public class DebugPrintTask : Task {
        [SerializeField]
        private string textToPrint = "Hi, I'm a test task!";

        public override object Execute(object input = null) {
            Debug.Log(textToPrint);

            return input;
        }
    }
}
