using UnityEngine;

namespace Services
{
    public interface IDraggable
    {
        public void OnStartDrag();
        public void OnDrag(Vector3 mousePosition);

        public void OnStopDrag(Vector3 mousePosition);
    }
}