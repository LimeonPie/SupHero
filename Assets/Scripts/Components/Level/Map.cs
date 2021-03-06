﻿using UnityEngine;
using System.Collections.Generic;

namespace SupHero.Components.Level {
    public class Map : MonoBehaviour {

        //private ZoneController zone;
        public GameObject transfer;
        
        public bool generateRoute = false;
        public int length = 1;

        public List<Plate> plates { get; private set; } // All the plates
        public Plate battleField { get; private set; } // Current plate where hero is

        void Awake() {
            //zone = GetComponent<ZoneController>();
        }

        void Start() {
            if (generateRoute && length > 0) {
                createRoute(length);
            }
            createDrops();
        }

        void Update() {

        }

        public void constructZone(int length) {
            createRoute(length);
        }

        private void createDrops() {
            // Weapon
            initWeaponDrop();
            // Item
            initItemDrop();
            // Supply
            initSupplyDrop();
        }

        private void initWeaponDrop() {
            EnvironmentData data = Data.Instance.getEnvByTheme(Theme.FOREST);
            Timer weapon = gameObject.AddComponent<Timer>();
            weapon.time = data.weaponDropTime;
            weapon.OnEnd += delegate () {
                int weaponId = Data.Instance.getRandomWeaponId();
                GameObject dropInstance = Instantiate(data.dropPrefab) as GameObject;
                Drop drop = dropInstance.GetComponent<Drop>();
                drop.createDropLimitedAmount(Entity.WEAPON, weaponId, 2);
                battleField.Drop(drop);
                initWeaponDrop();
            };
            weapon.Launch();
        }

        private void initItemDrop() {
            EnvironmentData data = Data.Instance.getEnvByTheme(Theme.FOREST);
            Timer item = gameObject.AddComponent<Timer>();
            item.time = data.itemDropTime;
            item.OnEnd += delegate () {
                int itemId = Data.Instance.getRandomItemId();
                GameObject dropInstance = Instantiate(data.dropPrefab) as GameObject;
                Drop drop = dropInstance.GetComponent<Drop>();
                drop.createDropLimitedAmount(Entity.ITEM, itemId, 2);
                battleField.Drop(drop);
                initItemDrop();
            };
            item.Launch();
        }

        private void initSupplyDrop() {
            EnvironmentData data = Data.Instance.getEnvByTheme(Theme.FOREST);
            Timer supply = gameObject.AddComponent<Timer>();
            supply.time = data.supplyDropTime;
            supply.OnEnd += delegate () {
                int supplyId = Data.Instance.getRandomSupplyId();
                GameObject dropInstance = Instantiate(data.dropPrefab) as GameObject;
                Drop drop = dropInstance.GetComponent<Drop>();
                drop.createDropLimitedTime(Entity.SUPPLY, supplyId, 20f);
                battleField.Drop(drop);
                initSupplyDrop();
            };
            supply.Launch();
        }

        public List<PlateData> getPlatesByType(PlateData[] data, Side side) {
            List<PlateData> result = new List<PlateData>();
            foreach (PlateData dict in data) {
                List<Connector> connectors = dict.prefab.GetComponent<Plate>().getAllConnectors();
                if (connectors.Find(x => x.type == side) != null) {
                    result.Add(dict);
                }
            }
            return result;
        }

        public void createRoute(int length) {
            // New list of plates
            plates = new List<Plate>();
            EnvironmentData data = Data.Instance.getEnvByTheme(Theme.FOREST);
            PlateData[] options = data.plates;

            if (options.Length == 0) {
                Debug.Log("Plates data is empty");
                return;
            }

            // Assume, that first is always exist 
            GameObject plateInstance = Instantiate(options[0].prefab) as GameObject;
            plateInstance.transform.SetParent(transform);
            plateInstance.transform.position = transform.position;
            Plate plate = plateInstance.GetComponent<Plate>();
            // Shift base plate to north
            plateInstance.transform.Translate(plate.north.transform.localPosition);
            plate.southConnector.isFree = false;
            // The first plate will be the first battlefield
            battleField = plate;
            plate.plateData = options[0];
            plates.Add(plate);

            for (int index = 1; index < length; index++) {

                // Choosing free connector
                Plate last = plates[index - 1].GetComponent<Plate>();
                List<Connector> free = last.getFreeConnectors();
                Connector chosen = Utils.getRandomElement(free);
                List<PlateData> solutions = getPlatesByType(options, chosen.compatibleType);

                if (solutions.Count > 0) {
                    PlateData plateData = Utils.getRandomElement(solutions);
                    plateInstance = Instantiate(plateData.prefab) as GameObject;
                    plateInstance.transform.SetParent(transform);
                    plate = plateInstance.GetComponent<Plate>();

                    plate.connectTo(chosen);
                    chosen.isFree = false;

                    // Place transfer at last
                    if (index == (length - 1)) {
                        GameObject transferInstance = Instantiate(transfer) as GameObject;
                        transferInstance.transform.SetParent(transform);
                        plateInstance.GetComponent<Plate>().placeTransfer(transferInstance);
                    }

                    // When hero steps on this plate, made it current battlefield
                    plate.OnHeroCome += delegate () {
                        battleField = plate;
                    };

                    plate.plateData = plateData;
                    plates.Add(plate);
                }
            }
        }

        private void generateLandscape() {
            foreach (Plate plate in plates) {
                plate.generateObjects(Theme.FOREST);
            }
        }
    }
}
