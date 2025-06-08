using Services.Input;
using UnityEngine;

namespace View.GameTable
{
    public class PortView : MonoBehaviour, IDraggable
    {
        public ConnectionView Connection;

        public int Priority => 2;
    }
}