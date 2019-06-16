namespace XNuvem.Environment
{
    public interface IHostEvents
    {
        /// <summary>
        ///     Fired when initialize the shell
        /// </summary>
        void OnInitialize();

        /// <summary>
        ///     Fired when terminate the shell
        /// </summary>
        void OnTerminate();
    }
}