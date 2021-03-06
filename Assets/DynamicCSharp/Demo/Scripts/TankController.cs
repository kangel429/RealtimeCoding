﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace DynamicCSharp.Demo
{
    internal enum TankEventType
    {
        Rotate,
        Move,
        Shoot,
        HorizonMove,
    }

    internal struct TankEvent
    {
        // Public
        public TankEventType eventType;
        public float eventValue;

        // Constructor
        public TankEvent(TankEventType type, float value = 0)
        {
            this.eventType = type;
            this.eventValue = value;
        }
    }

    /// <summary>
    /// The main controller for the tank.
    /// </summary>
    public abstract class TankController : MonoBehaviour
    {
        // Private
        private Queue<TankEvent> tankTasks = new Queue<TankEvent>();
        private bool crash = false;

        // Public        
        /// <summary>
        /// The prefab that is used to fire a shell.
        /// </summary>
        [HideInInspector]
        public GameObject bulletObject = null;
        /// <summary>
        /// How fast the tank can move forwards and backwards.
        /// </summary>
        [HideInInspector]
        public float moveSpeed = 1;
        /// <summary>
        /// How fast the tank can rotate in any direction.
        /// </summary>
        [HideInInspector]
        public float rotateSpeed = 3;

        public Sprite playBuSprite;
        public Image buttonPlayimg;
        public string nextStage;

        public Vector3 tank;

        bool crashEnemy = false;
        //public Vector3 tank
        //{
        //    get
        //    {
        //        return transform.position;
        //    }
        //    set
        //    {
        //        transform.position = value;
        //    }
        //}
        // Methods
        /// <summary>
        /// The method that must be implemented to control the tank.
        /// </summary>
        public abstract void TankMain();

        /// <summary>
        /// Called by Unity.
        /// </summary>
        /// <param name="collision">The collider that was hit</param>
        public void OnCollisionEnter2D(Collision2D collision)
        {
            Collider2D collider = collision.collider;

            // Check for crashed into walls
            if (collider.name == "DamagedWall" || collider.name == "Wall")
                crash = true;
            if (collider.tag == "Goal" ){

                SceneManager.LoadScene(nextStage);
            }

            if (collider.tag == "enemy")
            {
                crashEnemy = true;


            }
        }


        /// <summary>
        /// Runs the current controlling code.
        /// </summary>
        public void RunTank()
        {
            // Start the routine
            StartCoroutine(RunTankRoutine());
        }

        public bool TellCrash()
        {
            // Add a move forward task
            return crashEnemy;
        }
        /// <summary>
        /// Move the tank forward by the specified amount.
        /// </summary>
        /// <param name="amount">The amount of tiles to move forward</param>
        public void MoveForward(float amount = 1)
        {
            // Add a move forward task
            tankTasks.Enqueue(new TankEvent(TankEventType.Move, amount));
        }
        public void HorizonMoveForward(float amount = 1)
        {
            // Add a move forward task
            tankTasks.Enqueue(new TankEvent(TankEventType.HorizonMove, amount));
        }
        /// <summary>
        /// Move the tank backward by the specified amount.
        /// </summary>
        /// <param name="amount">The amount of tiles to move backward</param>
        public void MoveBackward(float amount = 1)
        {
            // Add a move back task
            tankTasks.Enqueue(new TankEvent(TankEventType.Move, -amount));
        }

        /// <summary>
        /// Rotate the tank clockwise by the specified degrees.
        /// </summary>
        /// <param name="amount">The amount of degrees to rotate</param>
        public void RotateClockwise(float amount = 90)
        {
            // Add a rotate clockwise task
            tankTasks.Enqueue(new TankEvent(TankEventType.Rotate, amount));
        }

        /// <summary>
        /// Rotate the tank counter-clockwise by the specified degrees.
        /// </summary>
        /// <param name="amount">The amount of degrees to rotate</param>
        public void RotateCounterClockwise(float amount = 90)
        {
            // Add a rotate counter clockwise task
            tankTasks.Enqueue(new TankEvent(TankEventType.Rotate, -amount));
        }

        /// <summary>
        /// Fire a projectile from the tank.
        /// </summary>
        public void Shoot(float amount = 0)
        {
            // Add a shoot event
            tankTasks.Enqueue(new TankEvent(TankEventType.Shoot, amount));
        }
        float xx = 0;
        float yy = 0;
        private IEnumerator RunTankRoutine()
        {
            // Call the main method
            TankMain();
            Debug.Log("Ddddddd" + tank);
         
            if(tank.y != yy)
            {
                yield return StartCoroutine(MoveRoutine(tank.y));
                tank.y = yy;
            }

            if (tank.x != xx)
            {
                yield return StartCoroutine(HorizontalMoveRoutine(tank.x));
                tank.x = xx;
            }


            //transform.Translate(tank);
            //transform.position = Vector2.MoveTowards(transform.position, tank, 2);

            // Process all tank tasks in order

            while (tankTasks.Count > 0)
            {
                // Check for a crash
                if(crash == true)
                {
                    Debug.Log("Crashed!");
                    tankTasks.Clear();
                    buttonPlayimg.sprite = playBuSprite;
                    yield break;
                }

                // Get an item
                TankEvent e = tankTasks.Dequeue();

                switch (e.eventType)
                {
                    case TankEventType.Move:
                        {
                            // Move the tank
                            yield return StartCoroutine(MoveRoutine(e.eventValue));
                            break;
                        }
                    case TankEventType.HorizonMove:
                        {
                            // Move the tank
                            yield return StartCoroutine(HorizontalMoveRoutine(e.eventValue));
                            break;
                        }
                    case TankEventType.Rotate:
                        {
                            // Rotate the tank
                            yield return StartCoroutine(RotateRoutine(e.eventValue));
                            break;
                        }

                    case TankEventType.Shoot:
                        {
                            yield return StartCoroutine(ShootRoutine(e.eventValue));
                            break;
                        }
                }
            }
          

            
            buttonPlayimg.sprite = playBuSprite;

            //transform.Translate(new Vector3(0, 0, 0));


        }

        private IEnumerator MoveRoutine(float amount)
        {
            // Get the target position
            Vector2 destination = transform.localPosition + (transform.up * amount);
            
            // Loop until we reach our target
            while(Vector2.Distance(transform.localPosition, destination) > 0f)
            {
                // Check for a crash
                if (crash == true)
                    yield break;

                // Move towards the target
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, destination, Time.deltaTime * moveSpeed);

                // Wait for next frame
                yield return null;
            }
        }
        private IEnumerator HorizontalMoveRoutine(float amount)
        {
            // Get the target position
            Vector2 destination = transform.localPosition + (transform.right * amount);

            // Loop until we reach our target
            while (Vector2.Distance(transform.localPosition, destination) > 0f)
            {
                // Check for a crash
                if (crash == true)
                    yield break;

                // Move towards the target
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, destination, Time.deltaTime * moveSpeed);

                // Wait for next frame
                yield return null;
            }
        }

        private IEnumerator RotateRoutine(float amount)
        {
            // Get the target rotation
            Quaternion target = Quaternion.Euler(0, 0, transform.eulerAngles.z - amount);

            // Loop until we rotate to our target
            while(transform.rotation != target)
            {
                // Check for a crash
                if (crash == true)
                    yield break;

                // Find the angle delta
                float delta = Mathf.Abs(transform.eulerAngles.z - target.eulerAngles.z);

                // CHeck for fail case
                if (delta < 0.2f)
                    yield break;

                // Rotate towards the target
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, Time.deltaTime * (rotateSpeed * 60));

                // Wait for next frame
                yield return null;
            }
        }

        private IEnumerator ShootRoutine(float vv)
        {
            Vector2 startPos = transform.position+ new Vector3(vv,0,0) + (transform.up * 0.8f);

            // Fire a shell
            TankShell shell = TankShell.Shoot(bulletObject, startPos, transform.up);

            // Wait for the shell to be destroyed
            while(shell.Step() == false)
            {
                // Wait for next frame
                yield return null;
            }

            // Destroy the shell
            shell.Destroy();
        }

    }
}
