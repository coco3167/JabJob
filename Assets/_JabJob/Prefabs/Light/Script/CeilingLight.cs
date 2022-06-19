using UnityEngine;

namespace _JabJob.Prefabs.Light.Script
{
    public class CeilingLight : MonoBehaviour
    {
        public GameObject Light;
        public MeshRenderer LightRenderer;

        public void Enable()
        {
            Light.SetActive(true);
            // Enable emmision
            LightRenderer.material.EnableKeyword("_EMISSION");
            LightRenderer.material.SetFloat("_EmissionScale", 0.4f);
        }
        
        public void Disable()
        {
            Light.SetActive(false);
            LightRenderer.material.DisableKeyword("_EMISSION");
            LightRenderer.material.SetFloat("_EmissionScale", 0);
        }
    }
}
