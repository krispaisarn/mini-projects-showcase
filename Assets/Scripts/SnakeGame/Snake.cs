using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShowCase.Snake
{
    public class Snake : MonoBehaviour
    {
        [Header("Tail")]
        public Pool tailPool;
        [SerializeField] GameObject _tailPrefab;
        [SerializeField] GameObject _tailPoolRoot;

        [Header("Attribute")]
        [SerializeField] float _moveInterval = 0.15f;
        private bool _ate = false;
        private bool _isDied = false;
        private Vector2 _dir = Vector2.right;
        private List<Transform> _tails = new List<Transform>();
        private float _time;
        private Food _ateFood;

        // Use this for initialization
        void Start()
        {
            tailPool = new Pool("Tail", _tailPrefab, _tailPoolRoot, 10, PoolInflationType.INCREMENT);
        }

        void Update()
        {
            if (_isDied)
                return;

            _time += Time.deltaTime;
            while (_time >= _moveInterval)
            {
                Move();
                _time -= _moveInterval;
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (_isDied)
                return;

            if (Input.GetKey(KeyCode.D))
                _dir = Vector2.right;
            else if (Input.GetKey(KeyCode.S))
                _dir = -Vector2.up;
            else if (Input.GetKey(KeyCode.A))
                _dir = -Vector2.right;
            else if (Input.GetKey(KeyCode.W))
                _dir = Vector2.up;
        }

        void Move()
        {
            if (_isDied)
                return;

            Vector2 v = transform.position;
            transform.Translate(_dir);

            if (_ate)
            {
                OnAte(_ateFood, v);
            }
            else if (_tails.Count > 0)
            {
                _tails.Last().position = v;

                _tails.Insert(0, _tails.Last());
                _tails.RemoveAt(_tails.Count - 1);
            }
        }

        private void OnAte(Food food, Vector2 v)
        {
            _ate = false;
            GameManager.Instance.AddScore(food.score);

            GameObject tObject = tailPool.NextAvailableObject(true);
            tObject.transform.position = v;

            _tails.Insert(0, tObject.transform);
        }

        public void Reset()
        {
            tailPool.ResetPool();

            _tails.Clear();

            transform.position = new Vector3(0, 0, 0);

            _isDied = false;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Food"))
            {
                _ateFood = col.GetComponent<Food>();
                _ate = true;

                SpawnFood.Instance.OnAteFood(_ateFood);
            }
            else
            {
                _isDied = true;
                GameManager.Instance.OnGameOver();
            }
        }
    }
}