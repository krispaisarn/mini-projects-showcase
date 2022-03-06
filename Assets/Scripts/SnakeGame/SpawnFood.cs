using UnityEngine;
using System.Collections;

namespace ShowCase.Snake
{
    public class SpawnFood : MonoSingleton<SpawnFood>
    {
        public Pool foodPool;
        [Header("Prefab")]
        [SerializeField] private GameObject foodPrefab;
        [SerializeField] private GameObject _foodPoolRoot;
        [Header("Attribute")]
        [SerializeField] private int _foodCount;
        [SerializeField] private int _superFoodTriggerCounter = 5;
        [SerializeField] float _spawnInterval = 3f;
        [Header("Border Sprites")]
        [SerializeField] private Transform _borderTop;
        [SerializeField] private Transform _borderBottom;
        [SerializeField] private Transform _borderLeft;
        [SerializeField] private Transform _borderRight;
        private float _time;

        // Use this for initialization
        void Start()
        {
            foodPool = new Pool("Food", foodPrefab, _foodPoolRoot, 10, PoolInflationType.INCREMENT);
        }

        void Update()
        {
            if (GameManager.Instance.isGameOver)
                return;

            _time += Time.deltaTime;
            while (_time >= _spawnInterval)
            {
                Spawn();
                _time -= _spawnInterval;
            }
        }

        void Spawn()
        {
            // x position between left & right border
            int x = (int)Random.Range(_borderLeft.position.x,
                                      _borderRight.position.x);

            // y position between top & bottom border
            int y = (int)Random.Range(_borderBottom.position.y,
                                      _borderTop.position.y);

            _foodCount++;
            GameObject fObject = foodPool.NextAvailableObject(true);
            fObject.transform.position = new Vector2(x, y);

            fObject.GetComponent<Food>().SetFood(_foodCount % _superFoodTriggerCounter == 0);
        }

        public void OnAteFood(Food food)
        {
            food.gameObject.SetActive(false);
            foodPool.ReturnObjectToPool(food);
        }

        public void Reset()
        {
            foodPool.ResetPool();
        }
    }
}