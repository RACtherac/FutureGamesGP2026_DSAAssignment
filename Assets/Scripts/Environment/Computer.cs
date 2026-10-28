using Environment.Tasks;
using UnityEngine;

namespace Environment {
    public class Computer : MonoBehaviour, IInteractable {
        [Header("Employee Task")]
        [SerializeField] private Task testTask;

        [Header("Terminal")]
        [SerializeField] private ComputerTerminal computerTerminal;

        public void OnInteract() {
            computerTerminal.OpenTerminal(testTask);
        }
    }
}
