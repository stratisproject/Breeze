using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NTumbleBit.ClassicTumbler;
using NTumbleBit.PuzzlePromise;
using NTumbleBit.PuzzleSolver;

namespace Breeze.TumbleBit.Client
{
    public class TumblingState : IStateMachine
    {
        private const string StateFileName = "tumblebit_state.json";
        
        [JsonProperty("tumblerParameters")]
        public ClassicTumblerParameters TumblerParameters { get; set; }

        [JsonProperty("tumblerUri")]
        public Uri TumblerUri { get; set; }
        
        [JsonProperty("lastBlockReceivedHeight")]
        public int LastBlockReceivedHeight { get; set; }

        [JsonProperty("destinationWalletName")]
        public string DestinationWalletName { get; set; }

        public IList<Session> Sessions { get; set; }

        public TumblingState()
        {
            this.Sessions = new List<Session>();
        }

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

        /// <inheritdoc />
        public void Update()
        {
            // get a list of cycles we expect to have at this height
            var cycles = this.TumblerParameters.CycleGenerator.GetCycles(this.LastBlockReceivedHeight);

            var states = cycles.SelectMany(c => this.Sessions.Where(s => s.StartCycle == c.Start)).ToList();
            foreach (var state in states)
            {
                try
                {
                    // create a new session to be updated
                    var session = new Session();
                    if (state.NegotiationClientState != null)
                    {
                        session.StartCycle = state.NegotiationClientState.CycleStart;
                        session.ClientChannelNegotiation = new ClientChannelNegotiation(this.TumblerParameters, state.NegotiationClientState);
                    }
                    if (state.PromiseClientState != null)
                        session.PromiseClientSession = new PromiseClientSession(this.TumblerParameters.CreatePromiseParamaters(), state.PromiseClientState);
                    if (state.SolverClientState != null)
                        session.SolverClientSession = new SolverClientSession(this.TumblerParameters.CreateSolverParamaters(), state.SolverClientState);

                    // update the session
                    session.Update();

                    // replace the updated session in the list of existing sessions
                    int index = this.Sessions.IndexOf(state);
                    if (index != -1)
                    {
                        this.Sessions[index] = session;
                    }

                    this.Save();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        
        public void CreateNewSession(int start)
        {
            this.Sessions.Add(new Session { StartCycle = start });
            this.Save();
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



    public class Session
    {
        public int StartCycle { get; set; }

        public ClientChannelNegotiation.State NegotiationClientState { get; set; }

        public PromiseClientSession.State PromiseClientState { get; set; }

        public SolverClientSession.State SolverClientState { get; set; }

        public ClientChannelNegotiation ClientChannelNegotiation { get; set; }

        public SolverClientSession SolverClientSession { get; set; }

        public PromiseClientSession PromiseClientSession { get; set; }

        public void Update()
        {

        }
    }
}
