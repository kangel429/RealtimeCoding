using UnityEngine;
using UnityEngine.UI;

namespace DynamicCSharp.Demo
{
    /// <summary>
    /// Responsible for the tank demo gameplay.
    /// </summary>
    public sealed class TankManager : MonoBehaviour
    {
        // Private
        private ScriptDomain domain = null;
        private Vector2 startPosition;
        private Quaternion startRotation;
        
        private const string newTemplate = "BlankTemplate";
        private const string exampleTemplate = "ExampleTemplate";

        // Public
        /// <summary>
        /// The shell prefab that tanks are able to shoot.
        /// </summary>
        public string nextStage;
        public GameObject bulletObject;
        /// <summary>
        /// The tank object that can be controlled via code.
        /// </summary>
        public GameObject tankObject;

        public Sprite playBuSprite;
        public Image buttonPlayimg;

        // Methods
        /// <summary>
        /// Called by Unity.
        /// </summary>
        public void Awake()
        {
            // Create our script domain
            
            // if (ScriptDomain.Active == null) {
            //     Debug.Log(" Domain Active null");
            //     domain = ScriptDomain.CreateDomain("ScriptDomain", true);
                
            // }else {
            //     // ScriptDomain.reset();
             
            // //    domain = ScriptDomain.CreateDomain("ScriptDomain", true);
            //     // domain = ScriptDomain.CreateDomain("ScriptDomain", true);
            //     Debug.Log(" Domain Active not null");
            // }
            if (tankObject == null ) {
                Debug.Log(" tttttt null ");
            }else {
                Debug.Log(" tttttt not null ");
            }

           
            domain = ScriptDomain.CreateDomain("ScriptDomain", true);
            
            // if (ScriptDomain.sandx == null) {
            //     domain = ScriptDomain.CreateDomain("ScriptDomain", true);
            //     Debug.Log(" sand box null");
            // }else {
            //     Debug.Log(" snad box not null");
            // }




            // Find start positions
            startPosition = tankObject.transform.position;
            startRotation = tankObject.transform.rotation;

            // Add listener for new button
            CodeUI.onNewClicked += (CodeUI ui) =>
            {
                // Load new file
                ui.codeEditor.text = Resources.Load<TextAsset>(newTemplate).text;
            };

            // Add listener for example button
            CodeUI.onLoadClicked += (CodeUI ui) =>
            {
                // Load example file
                ui.codeEditor.text = Resources.Load<TextAsset>(exampleTemplate).text;
            };

            CodeUI.onCompileClicked += (CodeUI ui) =>
            {
                // Try to run the script
                RunTankScript(ui.codeEditor.text);
            };
        }

        /// <summary>
        /// Resets the demo game and runs the tank with the specified C# code controlling it.
        /// </summary>
        /// <param name="source">The C# sorce code to run</param>
        public void RunTankScript(string source)
        {
            //if (tankObject != null)
            //{
                if (tankObject.GetComponent<TankController>() == null ){
                    Debug.Log(" tank null ");
                }else {
                         TankController old = tankObject.GetComponent<TankController>();

                    if (old != null)
                        Destroy(old);
                    RespawnTank();
                    Debug.Log(" tank not null ");
                }
               
                // Strip the old controller script
                


                // Reposition the tank at its start position
                
            //}
            // Compile the script
            ScriptType type = domain.CompileAndLoadScriptSource(source);

            if (type == null)
            {
                Debug.Log("ddddddd"+domain.GetErrorLineValue());
                Debug.LogError("Compile failed");
                return;
            }

            // Make sure the type inherits from 'TankController'
            if (type.IsSubtypeOf<TankController>() == true)
            {
                // Attach the component to the object
                ScriptProxy proxy = type.CreateInstance(tankObject);

                // Check for error
                if(proxy == null)
                {
                    // Display error
                    Debug.LogError(string.Format("Failed to create an instance of '{0}'", type.RawType));
                    return;
                }
                // Assign the bullet prefab to the 'TankController' instance
                proxy.Fields["playBuSprite"] = playBuSprite;
                proxy.Fields["buttonPlayimg"] = buttonPlayimg;
                proxy.Fields["bulletObject"] = bulletObject;
                proxy.Fields["nextStage"] = nextStage;
                // Call the run tank method
                proxy.Call("RunTank");
            }
            else
            {
                Debug.LogError("The script must inherit from 'TankController'");
            }
        }

        /// <summary>
        /// Resets the tank at its starting location.
        /// </summary>
        public void RespawnTank()
        {
            //if (tankObject != null)
            //{
                tankObject.transform.position = startPosition;
            tankObject.transform.rotation = startRotation;
            //}
        }
    }
}
