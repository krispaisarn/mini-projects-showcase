using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ShowCase.Snake
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public int score;
        public bool isGameOver = false;
        [SerializeField] Snake _snake;
        [SerializeField] SpawnFood _spawnFood;
        [SerializeField] TMPro.TextMeshProUGUI _scoreText;
        [SerializeField] GameObject _gameOverObject;
        [SerializeField] Button _restartButton;
        private string _baseScoreText;

        private void Start()
        {
            _baseScoreText = _scoreText.text;
            SetScore(score);

            _restartButton.onClick.AddListener(RestartGame);
        }

        private void OnDestroy()
        {
            _restartButton.onClick.RemoveAllListeners();
        }

        public void AddScore(int addedScore)
        {
            score += addedScore;
            SetScore(score);
        }

        private void SetScore(int amount)
        {
            _scoreText.text = string.Format(_baseScoreText, amount.ToString());
        }

        public void OnGameOver()
        {
            _gameOverObject.SetActive(true);
            isGameOver = true;
        }

        public void RestartGame()
        {
            isGameOver = false;
            score = 0;
            SetScore(score);

            _snake.Reset();
            _spawnFood.Reset();

            _gameOverObject.SetActive(false);
        }
    }
}