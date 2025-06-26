using UnityEngine;

namespace View.GameTable
{
    public interface IEnvironmentFactory
    {
        void LoadAssets();
        GameObject CreateEnvironmentObject(float noise, Vector3 position, Transform parent);
    }
}