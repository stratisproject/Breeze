using System;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using NTumbleBit.ClassicTumbler;

namespace Breeze.TumbleBit.Client
{
    public class TumblingState : IStateMachine
    {
        private const string StateFileName = "tumblebit_state.json";

        [JsonProperty("tumblerParameters")]
        public ClassicTumblerParameters TumblerParameters { get; set; }

        [JsonProperty("tumblerUri")]
        public Uri TumblerUri { get; set; }

        [JsonProperty("startHeight")]
        public int StartHeight { get; set; }

        [JsonProperty("lastBlockReceivedHeight")]
        public int LastBlockReceivedHeight { get; set; }

        [JsonProperty("destinationWalletName")]
        public string DestinationWalletName { get; set; }

        /// <inheritdoc />
        public void Save()
        {
            File.WriteAllText(GetStateFilePath(), JsonConvert.SerializeObject(this));
        }

        /// <inheritdoc />
        public IStateMachine Load()
        {
            return LoadState();
        }

        /// <inheritdoc />
        public void Delete()
        {
            var stateFilePath = GetStateFilePath();
            File.Delete(stateFilePath);
        }

        /// <summary>
        /// Loads the saved state of the tumbling execution to the file system.
        /// </summary>
        /// <returns></returns>
        public static TumblingState LoadState()
        {
            var stateFilePath = GetStateFilePath();
            if (!File.Exists(stateFilePath))
            {
                return null;
            }
            
            // load the file from the local system
            return JsonConvert.DeserializeObject<TumblingState>(File.ReadAllText(stateFilePath));
        }

        /// <summary>
        /// Gets the file path of the file containing the state of the tumbling execution.
        /// </summary>
        /// <returns></returns>
        private static string GetStateFilePath()
        {
            string defaultFolderPath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                defaultFolderPath = $@"{Environment.GetEnvironmentVariable("AppData")}\Breeze\TumbleBit";
            }
            else
            {
                defaultFolderPath = $"{Environment.GetEnvironmentVariable("HOME")}/.breeze/TumbleBit";
            }

            // create the directory if it doesn't exist
            Directory.CreateDirectory(defaultFolderPath);
            return Path.Combine(defaultFolderPath, StateFileName);
        }
    }
}
