namespace Breeze.TumbleBit.Client
{
    public interface IStateMachine
    {
        /// <summary>
        /// Saves the state of the current tumbling session.
        /// </summary>
        void Save();

        /// <summary>
        /// Loads the saved state of the tumbling execution to the file system.
        /// </summary>
        /// <returns></returns>
        void LoadStateFromMemory();

        /// <summary>
        /// Deletes the state of the current tumbling session..
        /// </summary>
        void Delete();

        /// <summary>
        /// Updates this the state of the current tumbling session.
        /// </summary>
        void Update();
    }
}
