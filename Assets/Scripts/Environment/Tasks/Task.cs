
using UnityEngine;

namespace Environment.Tasks {
    public abstract class Task : ScriptableObject {
        public abstract object Execute(object input = null);
    }
}
