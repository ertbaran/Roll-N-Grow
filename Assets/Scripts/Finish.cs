using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.Services.Analytics;
using TMPro;
using GameManagerNamespace;

public class Finish : MonoBehaviour
{
    public static Finish Instance = null;

    [SerializeField] public ParticleSystem _finishParticles;
    [SerializeField] Image _finishPanel;
    [SerializeField] Button _topRestartButton;
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] public Renderer _juice;
    [SerializeField] CollectManager _collectManager;
    [SerializeField] AudioClip _mixerSound;
    [SerializeField] AudioClip _collectSound;
    [SerializeField] Color _fruitColor;

    [SerializeField] AudioSource _musicSource;

    float fill = -1;
    List<float> collectedFruits = new List<float>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
            _topRestartButton.gameObject.SetActive(false);
            SoundManager.Instance.PlaySoundEffect(_collectSound);

            _musicSource.DOFade(0.3f, 7);

            _finishParticles.Play();

            _scoreText.text = $"{_collectManager.collectedFruitCount} / {_collectManager.totalFruitCount}";

            // Unity Analytics | Custom event
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            AnalyticsService.Instance.CustomData("finishedLevel", parameters);
        }
    }

    public void Fill()
    {
        collectedFruits = CollectManager.Instance.collectedFruits;
        foreach (var fruitCount in collectedFruits)
        {
            //_juice.material.SetFloat("fillAmount", _juice.material.GetFloat("fillAmount") + 4 / CollectManager.Instance.totalFruitCount * fruitCount);
            float fruitPercent = _juice.material.GetFloat("fillAmount") + 8 / CollectManager.Instance.totalFruitCount * fruitCount; // Default: 4

            DOTween.To(() => fill, x => fill = x, fruitPercent, 5).OnUpdate(() =>
            {
                _juice.material.SetFloat("fillAmount", fill);
                Debug.Log(fruitPercent);
            }).OnComplete(() => _finishPanel.transform.DOLocalMoveY(0, 0.7f).SetEase(Ease.OutBack));
            GameManager.Instance._topPanel.SetActive(false);
        }

        SoundManager.Instance.PlaySoundEffect(_mixerSound);
        _juice.material.DOColor(_fruitColor, 2);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

