using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Services.Analytics;
using Unity.Services.Mediation;

namespace GameManagerNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance = null;
        [Header("Roll Controls")]
        [SerializeField] float rollspeed;
        [SerializeField] float maxRollSpeed;

        [Range(1, 1000)]
        [Tooltip("High Value -> Slower Shrinking \nRecommended 700")]
        [SerializeField] float shrinkFraction;

        [SerializeField] public Rigidbody _rigidbody;
        Transform _playerTransform;

        public enum GameState { Started, Paused, Dead, Finished }
        public GameState gameState;

        [HideInInspector]
        public int currentFruitID = 0;

        [SerializeField] public FruitTypes[] fruitTypes;
        public static bool isShrinking;

        [Tooltip("Default: -9.81")]
        [SerializeField] float gravityScale;

        [Header("Designations")]
        [SerializeField] Image _gameOverPanel;
        [SerializeField] Image _playPanel;
        [SerializeField] AudioClip _gameOverClip;
        [SerializeField] Transform _blender;
        [SerializeField] public GameObject _topPanel;
        [SerializeField] GameObject _learn;


        [HideInInspector]
        public Vector3 shrinkAmount;


        private void Awake()
        {
            Setup();
        }

        void Start()
        {
            Time.timeScale = 0;
            gameState = GameState.Paused;
            shrinkAmount = Vector3.one / shrinkFraction;
            _playerTransform = _rigidbody.gameObject.transform;
        }

        private void FixedUpdate()
        {
            Roll();
        }

        #region GameState Methods
        public void Started()
        {
            gameState = GameState.Started;
            _rigidbody.freezeRotation = false;
            _rigidbody.velocity = AdManager.Instance.tempVelocity;
            PlayerControl.Instance.canMove = true;
            _rigidbody.isKinematic = false;
        }

        public void Paused()
        {
            gameState = GameState.Paused;
            Time.timeScale = 0;
            PlayerControl.Instance.canMove = false;
            SoundManager.Instance.PauseMusic();
        }

        public void Dead()
        {
            if (AdManager.Instance._ad.ad.AdState == AdState.Loaded)
            {
                Paused();
                AdManager.Instance._ad.ShowAd();
            }
            if (_playerTransform.localScale.x <= 6f)
            {
                _playerTransform.DOScale(Vector3.one * 17, 0.1f);
            }
            

            gameState = GameState.Dead;
            _gameOverPanel.gameObject.SetActive(true);
            _rigidbody.isKinematic = true;
            SoundManager.Instance.PauseMusic();
            SoundManager.Instance.PlaySoundEffect(_gameOverClip);
        }

        public void Finished()
        {
            gameState = GameState.Finished;
            PlayerControl.Instance.canMove = false;
            _rigidbody.AddForce((_blender.position - _playerTransform.position) / 2, ForceMode.VelocityChange);
        }
        #endregion

        void Roll()
        {
            if (gameState == GameState.Started && _playerTransform.localScale.x >= 0.1f && PlayerControl.Instance.canMove)
            {
                _rigidbody.AddForce(rollspeed * Vector3.forward, ForceMode.Acceleration);
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxRollSpeed);

                Vector3 tempScale = _playerTransform.localScale -= shrinkAmount;
                _playerTransform.DOScale(tempScale, 0.1f);

            }
            else if (_playerTransform.localScale.x < 6f)
            {
                Dead();
            }
        }

        public void Play()
        {
            StartCoroutine(StartGame());
        }

        public void Restart()
        {
            StopAllCoroutines();
            DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            // Unity Analytics | Custom event
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            AnalyticsService.Instance.CustomData("restartClicked", parameters);
        }

        public void NextLevel()
        {
            StopAllCoroutines();
            DOTween.KillAll();

            if (AdManager.Instance._ad.ad.AdState == AdState.Loaded)
            {
                Paused();
                AdManager.Instance._ad.ShowAd();
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void Continue()
        {
            if (AdManager.Instance._ad.ad.AdState == AdState.Loaded)
            {
                Paused();
                AdManager.Instance._ad.ShowAd();
            }
            _gameOverPanel.gameObject.SetActive(false);
            StartCoroutine(AdManager.Instance.AdPowerUpCollect());
        }

        IEnumerator StartGame()
        {
            _playPanel.gameObject.SetActive(false);
            _topPanel.gameObject.SetActive(true);
            _topPanel.transform.DOLocalMoveY(_topPanel.transform.localPosition.y - 300, 0.7f).SetEase(Ease.OutBack);

            Time.timeScale = 1;
            Debug.Log("Game starts in 1 seconds");
            yield return new WaitForSeconds(1);
            gameState = GameState.Started;
            isShrinking = true;
            _rigidbody.isKinematic = false;

            if (!PlayerPrefs.HasKey("FirstPlay"))
            {
                StartCoroutine(HowToPlay());
            }

        }

        IEnumerator HowToPlay()
        {
            _learn.SetActive(true);
            yield return new WaitForSeconds(3);
            _learn.SetActive(false);
            PlayerPrefs.SetString("FirstPlay", "Yes");
            PlayerPrefs.Save();
        }

        private void Setup()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            Physics.gravity = new Vector3(0, gravityScale, 0);
            Application.targetFrameRate = 30;   //Default FrameRate for mobile
            Time.timeScale = 0;
        }

    }
}