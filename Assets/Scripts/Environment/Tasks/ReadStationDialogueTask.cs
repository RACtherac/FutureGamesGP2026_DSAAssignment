using System;
using System.IO;
using Environment.Tasks;
using UnityEngine;

namespace Environment.Tasks {
    [CreateAssetMenu(
        fileName = "ReadStationDialogueTask",
        menuName = "Surveillance Tasks/Read Station Dialogue"
    )]
    public class ReadStationDialogueTask : Task {
        [SerializeField]
        private string fileName = "helper_dialogue.json";

        public override object Execute(object input = null) {
            string filePath = Path.Combine(
                Application.streamingAssetsPath,
                fileName
            );
            Debug.Log("[Station AI] Looking for JSON at: " + filePath);

            if(!File.Exists(filePath)) {
                Debug.LogError(
                    "Station AI dialogue file was not found: " +
                    filePath
                );
                return null;
            }

            try {
                string json = File.ReadAllText(filePath);

                StationDialogueFile data =
                    JsonUtility.FromJson<StationDialogueFile>(json);

                if(data == null || data.nodes == null) {
                    Debug.LogError(
                        "Station AI dialogue JSON is empty or invalid."
                    );
                    return null;
                }

                Debug.Log(
                    "Station AI dialogue loaded. Nodes: " +
                    data.nodes.Length
                );

                return data;
            }
            catch(Exception exception) {
                Debug.LogError(
                    "Failed to read Station AI dialogue."
                );
                Debug.LogException(exception);
                return null;
            }
        }
    }
}
