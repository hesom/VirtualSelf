using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VirtualSelf
{
    public class PortalTeleporter : MonoBehaviour
    {
        public SceneSwitcher sceneSwitcher;
        private bool playerIsOverlapping = false;

        /// <returns>The name of the scene that is displayed on the portal</returns>
        public Transform PortalCameraLeft { get; set; }
        public Transform PortalCameraRight { get; set; }

        // Update is called once per frame
        void LateUpdate()
        {
            if (playerIsOverlapping)
            {
                Transform player = Player.GetTransform();
                Vector3 portalToPlayer = player.position - transform.position;
                float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

                // If this is true: The player has moved across the portal
                if (dotProduct > 0f)
                {
                    // Teleport him!
                    var scene = SceneManager.GetSceneByName(sceneSwitcher.GetCurrentPortalScene());
                    var rootObjects = scene.GetRootGameObjects();
                    foreach (var o in rootObjects)
                    {
                        LayerUtils.SetLayersRecursive(o, "Default");
                        LayerUtils.ProcessLightRecursive(o, "Default");
                        LayerUtils.SetEnabledRecursive<Leap.Unity.Interaction.InteractionBehaviour>(o, true);
                    }

                    var portal = sceneSwitcher.GetCurrentPortal();
                    var renderPlaneLeft = portal.transform.Find("PortalMesh").Find("RenderPlaneLeft");
                    var renderPlaneRight = portal.transform.Find("PortalMesh").Find("RenderPlaneRight");
                    var colliderPlane = portal.transform.Find("PortalMesh").Find("ColliderPlane");
                    renderPlaneLeft.gameObject.SetActive(false);
                    renderPlaneRight.gameObject.SetActive(false);
                    colliderPlane.gameObject.SetActive(false);
                    var fader = portal.GetComponent<Fader>();
                    fader.StartFade();
                    sceneSwitcher.NotifyPortalTraversed();

                    Scene activeScene = SceneManager.GetActiveScene();
                    SceneManager.UnloadScene(activeScene);
                    SceneManager.SetActiveScene(scene);
                    Destroy(PortalCameraLeft.gameObject);
                    if(PortalCameraRight != null)
                        Destroy(PortalCameraRight.gameObject);
                    sceneSwitcher.NotifyPortalDespawn();

                }
                playerIsOverlapping = false;
            }
        }

        /// <summary>
        /// OnTriggerEnter is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                playerIsOverlapping = true;
            }
        }

        /// <summary>
        /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
        /// </summary>
        /// <param name="other">The other Collider involved in this collision.</param>
        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                playerIsOverlapping = false;
            }
        }
    }

}

