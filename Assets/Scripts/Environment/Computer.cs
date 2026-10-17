
using Environment.Tasks;
using UnityEngine;

namespace Environment {
    public class Computer : MonoBehaviour, IInteractable {
        [Header("Employee Task")]
        [SerializeField] private Task testTask;

        [Header("Terminal")]
        [SerializeField] private ComputerTerminal computerTerminal;

        public void OnInteract() {
            if(computerTerminal == null) {
                Debug.LogError(
                    "Computer: ComputerTerminal is not assigned!",
                    this
                );
                return;
            }

            if(testTask == null) {
                Debug.LogError(
                    "Computer: Employee task is not assigned!",
                    this
                );
                return;
            }

            computerTerminal.OpenTerminal(testTask);
        }
    }
}
