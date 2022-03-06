using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShowCase.Snake
{
    public class Food : PoolObject
    {
        public bool isSuperFood;
        public int score = 1;
        [SerializeField] private int _baseScore = 1;
        [SerializeField] private int _superScore = 5;
        [SerializeField] private Color _baseFoodColor;
        [SerializeField] private Color _superFoodColor_0;
        [SerializeField] private Color _superFoodColor_1;
        [SerializeField] private Transform _foodTransform;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        public void SetFood(bool isSuper)
        {
            _foodTransform.localEulerAngles = Vector3.zero;

            isSuperFood = isSuper;
            score = isSuper ? _superScore : _baseScore;

            _foodTransform.localScale = isSuper ? Vector3.one * 2 : Vector3.one;

            _spriteRenderer.color = isSuper ? _superFoodColor_0 : _baseFoodColor;
        }

        private void Update()
        {
            if (!isSuperFood)
                return;

            _foodTransform.Rotate(Vector3.forward, 180 * Time.deltaTime);

            float t = Mathf.PingPong(Time.time * 1, 1.0f);
            _spriteRenderer.color = Color.Lerp(_superFoodColor_0, _superFoodColor_1, t);
        }
    }
}