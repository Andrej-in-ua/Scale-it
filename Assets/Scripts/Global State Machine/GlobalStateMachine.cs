using UnityEditor.SceneManagement;
using UnityEngine;

public class GlobalStateMachine : StateMachineBase
{
    private GlobalStateMachine(BootState.Factory bootStateFactory, MainState.Factory mainStateFactory)
    {
        Add(bootStateFactory.Create(this));
        Add(mainStateFactory.Create(this));
    }
}
