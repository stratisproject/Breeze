namespace Breeze.TumbleBit.Client
{
    public interface IStateMachine
    {
        /// <summary>
        /// Saves the state of the current tumbling session.
        /// </summary>
        void Save();

        /// <summary>
        /// Loads the state of the current tumbling session.
        /// </summary>
        /// <returns></returns>
        IStateMachine Load();

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
