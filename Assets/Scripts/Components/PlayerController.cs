﻿using UnityEngine;
using System.Collections;
using SupHero;
using SupHero.Model;

namespace SupHero.Controllers {
    public class PlayerController : MonoBehaviour {

        // Variables
        public Player player;
        private GameObject playerUI;
        private Vector3 moveVector;
        private Vector3 rotation;

        // Components
        private WeaponController weapon;
        private ZoneController zone;
        private Rigidbody playerRigidbody;

        // Events
        public delegate void dieAction(Player player);
        public event dieAction OnDie;

        void Awake() {
            // For standalone init, like for test scene
            player = new Hero(1);
            player.inputType = InputType.KEYBOARD;
        }

        // Use this for initialization
        void Start() {
            playerRigidbody = GetComponent<Rigidbody>();
            weapon = GetComponentInChildren<WeaponController>();
            zone = GetComponentInParent<ZoneController>();
        }

        // Update is called once per frame
        void Update() {
            if (player.isAlive) {
                // Record move input
                moveVector = getMovementVector();
                // Record rotate input
                rotation = getRotation();
                // Record actions
                getActions();
            }
            else {
                die();
            }
        }

        void FixedUpdate() {
            // Moving
            if (moveVector != null && moveVector != Vector3.zero) {
                moveVector = moveVector.normalized * player.speed * Time.deltaTime;
                playerRigidbody.MovePosition(transform.position + moveVector);
            }
            // Turning
            if (rotation != null && rotation != Vector3.zero) {
                Quaternion rotate = Quaternion.LookRotation(rotation * Time.deltaTime);
                playerRigidbody.MoveRotation(rotate);
            }
        }

        public void takeDamage(int damage) {
            player.takeDamage(damage);
        }

        public void setPlayer(Player player) {
            this.player = player;
        }

        public void setUI(GameObject ui) {
            playerUI = ui;
        }

        public void die() {
            if (OnDie != null) {
                player.die();
                OnDie(player);
            }
        }

        private void getActions() {
            bool useWeapon = false;
            switch (player.inputType) {
                case InputType.KEYBOARD:
                    useWeapon = Input.GetButton("Fire1");
                    break;
                case InputType.GAMEPAD:
                    float rightBumper = Input.GetAxis(Utils.getControlForPlayer("R2", player.gamepadNumber));
                    useWeapon = (rightBumper > 0f);
                    break;
                default:
                    break;
            }
            if (useWeapon) weapon.useWeapon();
        }

        private Vector3 getMovementVector() {
            Vector3 movement = Vector3.zero;
            float h, v;
            switch (player.inputType) {
                case InputType.KEYBOARD:
                    h = Input.GetAxis("Horizontal");
                    v = Input.GetAxis("Vertical");
                    movement = new Vector3(h, 0f, v);
                    break;
                case InputType.GAMEPAD:
                    h = Input.GetAxis(Utils.getControlForPlayer("LeftStickX", player.gamepadNumber));
                    v = Input.GetAxis(Utils.getControlForPlayer("LeftStickY", player.gamepadNumber));
                    movement = new Vector3(h, 0f, v);
                    break;
                default:
                    break;
            }
            return movement;
        }

        private Vector3 getRotation() {
            Vector3 rotation = Vector3.zero;
            switch (player.inputType) {
                case InputType.GAMEPAD:
                    float x = Input.GetAxis(Utils.getControlForPlayer("RightStickX", player.gamepadNumber));
                    float z = Input.GetAxis(Utils.getControlForPlayer("RightStickY", player.gamepadNumber));
                    if (x != 0f && z != 0f) {
                        rotation = new Vector3(x, 0f, z);
                    }
                    break;
                case InputType.KEYBOARD:
                    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit floorHit;
                    if (Physics.Raycast(camRay, out floorHit)) {
                        Vector3 playerToMouse = floorHit.point - transform.position;
                        playerToMouse.y = 0f;
                        rotation = playerToMouse;
                    }
                    break;
                default:
                    break;
            }
            return rotation;
        }
    }
}