using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using GameControllerNameSpace;
using Unity.Ad.Interstıtıal;

public class CollectController : MonoBehaviour
{
    [SerializeField] private Vector3 growthAmount;
    [SerializeField] private float scoreIncreaseAmount;
    [SerializeField] private float adPowerUpTime;
    [SerializeField] private TMP_Text _scoretext;
    [SerializeField] private ParticleSystem _particleCollect;
    [SerializeField] private AudioClip[] _audioClips;

    private float score;
    private Vector3 growedScale;
    private Transform _transform;
    private InterstıtıalAd _ad;

    private void Start()
    {
        _transform = transform;
        AdSetUp();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tomato"))
        {
            other.gameObject.SetActive(false);
            GrowUp();

            score += scoreIncreaseAmount;
            ScoreToText();

            SoundManager.Instance.PlaySoundEffect(_audioClips[Random.Range(0, _audioClips.Length)]);
            _particleCollect.Play();
        }
        if (other.gameObject.CompareTag("Ad"))
        {
            other.gameObject.SetActive(false);
            GameManager.isShrinking = false;
            StartCoroutine(AdPowerUp());
        }
        if (other.gameObject.CompareTag("Obstacle"))
        {
            GameManager.gameState = GameManager.GameState.Death;
            Debug.Log("Can gidecek veya oyun yeniden başlayacak. Ayarlanması yapılır.");
            // State'in durumuna Can gitti veya kaybetti vs. de eklenip state ona çevrilince otomatik GameOver çıkması sağlanabilir.
        }
    }



    IEnumerator AdPowerUp()
    {
        GameManager.gameState = GameManager.GameState.Paused;

        _ad.ShowAd();

        yield return new WaitWhile(() => GameManager.gameState == GameManager.GameState.Paused);
        Debug.Log("Reklam Bitti");

        yield return new WaitForSeconds(adPowerUpTime);
        GameManager.isShrinking = true;
    }

    private void ScoreToText()
    {
        _scoretext.text = score.ToString();
        _scoretext.DOColor(Color.red, .2f).OnComplete(() => _scoretext.DOColor(Color.black, .2f));
        _scoretext.transform.DOScale(Vector3.one * 1.5f, .1f).OnComplete(() => _scoretext.transform.DOScale(Vector3.one, .1f));
    }

    private void GrowUp()
    {
        growedScale = _transform.localScale + growthAmount;
        _transform.DOScale(growedScale, 0.1f);
    }

    private void AdSetUp()
    {
        _ad = new InterstıtıalAd();
        _ad.InitServices();
        _ad.SetupAd();
    }
}
