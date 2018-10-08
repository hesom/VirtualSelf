using System.Collections;
using System.Collections.Generic;
using Leap;
using RoboRyanTron.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.XR;
using VirtualSelf.GameSystems;

namespace VirtualSelf
{
//    [System.Serializable]
//    public class UnityEventDynamic : UnityEvent<Scene>{}
    
    [CreateAssetMenu]
    public class SceneSwitcher : ScriptableObject
    {
        public Transform PortalPrefab;
        public Material renderPlaneMaterialLeft;
        public Material renderPlaneMaterialRight;
        public Transform PortalCameraPrefab;

        public LevelCodes levelCodes;
        public KeycodesList keycodes;
        public Keycode resetCode;

        public UnityEvent onPortalTraversed;
//        public UnityEventDynamic onPortalTraversed2;
        public UnityEvent onPortalDespawned;

        private GameObject renderPlaneLeft;
        private GameObject renderPlaneRight;
        private GameObject colliderPlane;
        public bool VRMode = false;
        private Camera portalCameraLeft;
        private Camera portalCameraRight;
        private Camera leftCamera;
        private Camera rightCamera;
        private bool portalSpawned = false;
        private string currentPortalScene;
        private GameObject currentPortal;
        private Room currentPortalRoom;
        public Dictionary<GameObject, int> objectLayer { get; set; }
        public Dictionary<Camera, LayerMask> cameraCullingLayermasks { get; set; }
        public Dictionary<MirrorScript, LayerMask> mirrorMasks { get; set; }

        private string loadSceneName = null;

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Reset private variables to default values because scriptable objects are persistent
            renderPlaneLeft = null;
            renderPlaneRight = null;
            colliderPlane = null;
            portalCameraLeft = null;
            portalCameraRight = null;
            portalSpawned = false;
            currentPortalScene = null;
            currentPortal = null;
            loadSceneName = null;
            leftCamera = null;
            rightCamera = null;
            objectLayer = null;
            cameraCullingLayermasks = null;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void NextLevel(string sequence)
        {
            if(sequence == resetCode.CodeString)
            {
                // reload current scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }

            var allMappings = keycodes.GetKeycodeFromCodeString(sequence);

            string newLevel;
            Room roomReference;
            if (allMappings.IsPresent()) {
                var roomMapping = allMappings.Get();
                newLevel = roomMapping.RoomReference.Scene.SceneName;
                roomReference = roomMapping.RoomReference;
                if (!roomMapping.KeycodeReference.IsDiscovered) {
                    return;
                }
            }
            else {
                return;
            }
            
            if (newLevel != null)
            {
                loadSceneName = newLevel;
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene s = SceneManager.GetSceneAt(i);
                    if (s.name == newLevel)
                    {
                        return; // Scene already loaded
                    }
                }
                if (PortalActive())
                {
                    SceneManager.UnloadSceneAsync(GetCurrentPortalScene());
                }
                SceneManager.LoadSceneAsync(newLevel, LoadSceneMode.Additive);
                currentPortalRoom = roomReference;
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
                    // build layer hash table
                    objectLayer.Add(o, o.layer);
                    Camera cam = o.GetComponent<Camera>();
                    if (cam != null)
                    {
                        cameraCullingLayermasks.Add(cam, cam.cullingMask);
                        var behindPortalCullingMask = cam.GetComponent<BehindPortalCullingMask>();
                        cam.cullingMask = behindPortalCullingMask?.cullingMaskBehindPortal ?? cam.cullingMask;
                    }
                    MirrorScript mirror = o.GetComponent<MirrorScript>();
                    if(mirror != null)
                    {
                        mirrorMasks.Add(mirror, mirror.ReflectLayers);
                        mirror.ReflectLayers = o.GetComponent<BehindPortalMirrorMask>()?.mirrorMaskBehindPortal ?? mirror.ReflectLayers;
                    }

                    foreach (var child in o.GetComponentsInChildren<Transform>())
                    {
                        objectLayer.Add(child.gameObject, child.gameObject.layer);
                        Camera childCam = child.GetComponent<Camera>();
                        if(childCam != null)
                        {
                            cameraCullingLayermasks.Add(childCam, childCam.cullingMask);
                        }
                    }

                    LayerUtils.SetEnabledRecursive<Leap.Unity.Interaction.InteractionBehaviour>(o, false);
                    LayerUtils.SetLayersRecursive(o, "Behind Portal");
                    LayerUtils.ProcessLightRecursive(o, "Behind Portal");
                }
            }
            else
            {
                return;
            }
            if (!this.PortalActive())
            {
                Transform portalCameraObjLeft = null;
                Transform portalCameraObjRight = null;
                if (!VRMode)
                {
                    Camera mainCamera = Camera.main;
                    portalCameraObjLeft = Instantiate(PortalCameraPrefab, mainCamera.transform);
                }
                else
                {
                    portalCameraObjLeft = Instantiate(PortalCameraPrefab, leftCamera.transform);
                    portalCameraObjRight = Instantiate(PortalCameraPrefab, rightCamera.transform);
                    portalCameraRight = portalCameraObjRight.GetComponent<Camera>();
                }
                portalCameraLeft = portalCameraObjLeft.GetComponent<Camera>();


                if (portalCameraLeft.targetTexture != null)
                {
                    portalCameraLeft.targetTexture.Release();
                }

                portalCameraLeft.targetTexture = new RenderTexture(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 24);
                renderPlaneMaterialLeft.mainTexture = portalCameraLeft.targetTexture;

                if (VRMode)
                {
                    if(portalCameraRight.targetTexture != null)
                    {
                        portalCameraRight.targetTexture.Release();
                    }
                    portalCameraRight.targetTexture = new RenderTexture(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 24);
                    renderPlaneMaterialRight.mainTexture = portalCameraRight.targetTexture;
                    
                    var renderPortalScriptLeft = leftCamera.GetComponent<RenderPortal>();
                    var renderPortalScriptRight = rightCamera.GetComponent<RenderPortal>();
                    renderPortalScriptLeft.portalCamera = portalCameraLeft.GetComponent<Camera>();
                    renderPortalScriptRight.portalCamera = portalCameraRight.GetComponent<Camera>();
                }

                Transform portalObj = Instantiate(PortalPrefab);
                var portalSpawn = GameObject.FindGameObjectWithTag("PortalSpawn");
                if(portalSpawn != null && portalSpawn.scene == SceneManager.GetActiveScene()){
                    portalObj.transform.position = portalSpawn.transform.position;
                    portalObj.transform.rotation = portalSpawn.transform.rotation;
                }
                LayerUtils.SetLayersRecursive(portalObj.gameObject, "Master");
                SceneManager.MoveGameObjectToScene(portalObj.gameObject, SceneManager.GetSceneByBuildIndex(0));
                renderPlaneLeft = portalObj.Find("PortalMesh").Find("RenderPlaneLeft").gameObject;
                renderPlaneLeft.layer = LayerMask.NameToLayer("LeftEye");
                if (VRMode)
                {
                    renderPlaneRight = portalObj.Find("PortalMesh").Find("RenderPlaneRight").gameObject;
                    renderPlaneRight.layer = LayerMask.NameToLayer("RightEye");
                    renderPlaneRight.SetActive(true);
                }
                
                colliderPlane = portalObj.Find("PortalMesh").Find("ColliderPlane").gameObject;

                renderPlaneLeft.SetActive(true);
                colliderPlane.SetActive(true);
                PortalTeleporter teleporter = colliderPlane.GetComponent<PortalTeleporter>();
                teleporter.PortalCameraLeft = portalCameraObjLeft;
                if (VRMode)
                {
                    teleporter.PortalCameraRight = portalCameraObjRight;
                }

                Animation portalAnimation = portalObj.GetComponentInChildren<Animation>();
                portalAnimation.Play("PortalUp");
                NotifyPortalSpawn(portalObj.gameObject);
            }
            
            NotifyPortalSceneSwitch(loadSceneName);
        }

        /// <summary>
        /// Call this method when you spawn a new portal. Make sure to call NotifyPOrtalSceneSwitch too
        /// </summary>
        /// <param name="currentPortal">The GameObject of the new portal</param>
		public void NotifyPortalSpawn(GameObject currentPortal)
        {
            portalSpawned = true;
            this.currentPortal = currentPortal;
        }

        /// <summary>
        /// Call this method when the scene inside the portal changes
        /// </summary>
        /// <param name="currentPortalScene">Name of the new scene inside the portal</param>
        public void NotifyPortalSceneSwitch(string currentPortalScene)
        {
            portalSpawned = true;
            this.currentPortalScene = currentPortalScene;
        }

        /// <summary>
        /// Call this method when the portal is deleted (manually or because its scene was unloaded)
        /// </summary>
		public void NotifyPortalDespawn()
        {
            portalSpawned = false;
            onPortalDespawned.Invoke();
        }

        /// <summary>
        /// Is there a portal currently active?
        /// </summary>
		public bool PortalActive()
        {
            return portalSpawned;
        }

        /// <summary>
        /// Current scene displayed inside the portal
        /// </summary>
        /// <returns>Name of the scene</returns>
		public string GetCurrentPortalScene()
        {
            return currentPortalScene;
        }

        /// <summary>
        /// Currently active portal. Only valid if PortalActive is also true
        /// </summary>
        /// <returns>GameObject of the current portal</returns>
        public GameObject GetCurrentPortal()
        {
            return currentPortal;
        }

        public void NotifyPortalTraversed()
        {
            onPortalTraversed.Invoke();
            currentPortalRoom.HasBeenVisited = true;
//            onPortalTraversed2.Invoke(SceneManager.GetActiveScene());
        }

        public void SetLeftCamera(Camera camera)
        {
            leftCamera = camera;
        }

        public void SetRightCamera(Camera camera)
        {
            rightCamera = camera;
        }
    }
}


