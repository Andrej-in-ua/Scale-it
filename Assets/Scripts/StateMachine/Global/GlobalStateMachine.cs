using StateMachine.Base;
using StateMachine.Global.States;

namespace StateMachine.Global
{
    public class GlobalStateMachine : StateMachineBase
    {
        private GlobalStateMachine(BootState.Factory bootStateFactory, MainState.Factory mainStateFactory)
        {
            Add(bootStateFactory.Create(this));
            Add(mainStateFactory.Create(this));
        }
    }
}
