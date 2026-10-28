using System;

namespace Environment.Tasks {
    [Serializable]
    public class EmployeeData {
        public string name;
        public int employeeId;
        public bool hasEngineRoomClearance;
        public string lastEntranceInEngineRoom;
        public string lastExitFromEngineRoom;
    }

    [Serializable]
    public class EmployeeList {
        public EmployeeData[] employees;
    }
}
