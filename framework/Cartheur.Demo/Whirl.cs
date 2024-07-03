
using Cartheur.Demo;

namespace Bph
{
    public static class RueTheWhirl
    {
        public static int NumberOfWhirls = 0;
        public enum States
        {
            Zero, One, Two, Three, Four
        }
        /// <summary>
        /// The actions to perform, given the ratio between form and function.
        /// </summary>
        /// <returns></returns>
        public static string ActionStateZero()
        {
            // The zeroth-rest state.
            if (NumberOfWhirls == 0)
            {
                NumberOfWhirls++;
                return "Beginning my whirl.";
            }
            else
            {
                return "Resting in my zeroth state.";
            }
            
        }
        /// <summary>
        /// Actions the state one.
        /// </summary>
        /// <returns>The thinking is changing to Task<>.</returns>
        public static string ActionStateOne()
        {
            return Tasks.DockerStats("0");
        }
        public static string ActionStateTwo()
        {
            //return "I am now in state two on the whirl.";
            return Tasks.CreateAgentTask();
        }
        public static string ActionStateThree()
        {
            //return "I am now in state three on the whirl.";
            return Tasks.CreateAgentTask();
        }
        public static string ActionStateFour()
        {
            NumberOfWhirls++;
            return "I am now in state four on the whirl.";
        }
        /// <summary>
        /// The central notifier of the current state of the whirl.
        /// </summary>
        /// <value>
        /// The state of the current.
        /// </value>
        public static string CurrentState { get; set; }

        public static string ActionController()
        {
            if (CurrentState == "Zero")
            {
                return ActionStateZero();
            }
            if (CurrentState == "One")
            {
                return ActionStateOne();
            }
            if (CurrentState == "Two")
            {
                return ActionStateTwo();
            }
            if (CurrentState == "Three")
            {
                return ActionStateThree();
            }
            if (CurrentState == "Four")
            {
               return ActionStateFour();
            }
            else { return ActionStateZero(); }
        }
    }
}

