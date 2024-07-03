
namespace Cartheur.Demo
{
    public static class Tasks
    {
        /// <summary>
        /// Is the noisy pod task running?
        /// </summary>
        public static bool ComputationTaskRunning;
        /// <summary>
        /// Is the prometheus pod running?
        /// </summary>
        public static bool PrometheusPodTaskRunning;
        /// <summary>
        /// Is the agent running?
        /// </summary>
        public static bool AgentRunning;
        /// <summary>
        /// Is the agent task running?
        /// </summary>
        public static bool AgentTaskRunning;
        /// <summary>
        /// Spawns a noisy pod with the default set of yaml values.
        /// </summary>
        /// <returns></returns>
        public static string SpawnMatrix(int loops)
        {
            if(ComputationTaskRunning)
            {
                return "Matrix computation pod already running.";
            }
            if (loops != 0)
            {
                MatrixContainer.Compute.SpinComputation(loops);
                ComputationTaskRunning = true;
                return "Computation running with " + loops.ToString() + " matrix iterations.";
            }
            
            
            return "Matrix computation pod spawned.";
        }
        /// <summary>
        /// Spawns a prometheus pod.
        /// </summary>
        /// <returns></returns>
        public static string SpawnPrometheusPod()
        {
            if (PrometheusPodTaskRunning)
            {
                return "Prometheus pod already running.";
            }
            PrometheusPodTaskRunning = true;
            return "Prometheus pod spawned.";
        }
        /// <summary>
        /// Spawns an agent.
        /// </summary>
        /// <returns></returns>
        public static string SpawnAgent()
        {
            if (AgentRunning)
            {
                return "The modal is running.";
            }
            AgentRunning = true;
            return "An modal has been spawned.";
        }
        /// <summary>
        /// Create a task for the agent to execute.
        /// </summary>
        /// <returns></returns>
        public static string CreateAgentTask()
        {
            if (AgentTaskRunning)
            {
                return "An modal task is running.";
            }
            AgentTaskRunning = true;
            return "Agent modal completed.";
        }
    }
}
