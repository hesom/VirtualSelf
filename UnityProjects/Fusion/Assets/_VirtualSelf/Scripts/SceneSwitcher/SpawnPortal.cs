using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VirtualSelf
{
    public class SpawnPortal : MonoBehaviour
    {

        public string loadSceneName;
        public Transform PortalPrefab;
        public Material portalFrameMaterial;
        public Material renderPlaneMaterial;
        public Transform PortalCameraPrefab;
        public SceneSwitcher sceneSwitcher;

        private GameObject renderPlane;
        private GameObject colliderPlane;
        private Camera portalCamera;

        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene s = SceneManager.GetSceneAt(i);
                    if (s.name == loadSceneName)
                    {
                        return; // Scene already loaded
                    }
                }
                if (sceneSwitcher.PortalActive())
                {
                    SceneManager.UnloadSceneAsync(sceneSwitcher.GetCurrentPortalScene());
                }
                SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SceneManager.GetSceneByName(loadSceneName) != scene)
            {
                return;
            }
            if (scene == SceneManager.GetSceneByName(loadSceneName))
            {       
                var rootObjects = scene.GetRootGameObjects();
                foreach (var o in rootObjects)
                {
                    LayerUtils.SetEnabledRecursive<Leap.Unity.Interaction.InteractionBehaviour>(o, false);
                    LayerUtils.SetLayersRecursive(o, "Behind Portal");
                    LayerUtils.ProcessLightRecursive(o, "Behind Portal");
                }
            }
            else
            {
                return;
            }
            if (!sceneSwitcher.PortalActive())
            {
                Camera mainCamera = Camera.main;
                Transform portalCameraObj = Instantiate(PortalCameraPrefab, mainCamera.transform);
                portalCamera = portalCameraObj.GetComponent<Camera>();

                if (portalCamera.targetTexture != null)
                {
                    portalCamera.targetTexture.Release();
                }
                portalCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
                renderPlaneMaterial.mainTexture = portalCamera.targetTexture;


                Transform portalObj = Instantiate(PortalPrefab);
                var portalSpawn = GameObject.FindGameObjectWithTag("PortalSpawn");
                if(portalSpawn != null && portalSpawn.scene == SceneManager.GetActiveScene()){
                    portalObj.transform.position = portalSpawn.transform.position;
                    portalObj.transform.rotation = portalSpawn.transform.rotation;
                }
                LayerUtils.SetLayersRecursive(portalObj.gameObject, "Master");
                SceneManager.MoveGameObjectToScene(portalObj.gameObject, SceneManager.GetSceneByBuildIndex(0));
                renderPlane = portalObj.Find("PortalMesh").Find("RenderPlaneLeft").gameObject;
                renderPlane.layer = LayerMask.NameToLayer("Default");
                colliderPlane = portalObj.Find("PortalMesh").Find("ColliderPlane").gameObject;

                renderPlane.SetActive(true);
                colliderPlane.SetActive(true);
                PortalTeleporter teleporter = colliderPlane.GetComponent<PortalTeleporter>();
                teleporter.PortalCameraLeft = portalCameraObj;

                Animation portalAnimation = portalObj.GetComponentInChildren<Animation>();
                portalAnimation.Play("PortalUp");

                if (portalFrameMaterial != null)
                {
                    Transform frame = portalObj.Find("PortalMesh").Find("Frame");
                    foreach (Transform child in frame)
                    {
                        MeshRenderer renderer = child.GetComponent<MeshRenderer>();
                        renderer.material = portalFrameMaterial;
                    }
                }
                sceneSwitcher.NotifyPortalSpawn(portalObj.gameObject);
            }

            sceneSwitcher.NotifyPortalSceneSwitch(loadSceneName);
        }
    }
}


