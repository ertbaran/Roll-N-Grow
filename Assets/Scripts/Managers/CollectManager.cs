using UnityEngine;
using DG.Tweening;
using TMPro;
using GameManagerNamespace;
using System.Collections.Generic;
using static UnityEngine.ParticleSystem;
using UnityEngine.UI;

public class CollectManager : MonoBehaviour
{
    public static CollectManager Instance = null;

    [SerializeField] Vector3 growthAmount;
    [SerializeField] float scoreIncreaseAmount, graterShrinkFraction;

    [Header("Designations")]
    [SerializeField] ParticleSystem _particleCollect;
    [SerializeField] ParticleSystem _graterCollect;
    [SerializeField] ParticleSystem _destroyParticle;
    [SerializeField] TMP_Text _scoretext;
    [SerializeField] Transform _transform;
    [SerializeField] AudioClip[] _collectClips;
    [SerializeField] AudioClip _transitionClip;
    [SerializeField] AudioClip _graterClip;
    [SerializeField] GameObject _timeline;
    [SerializeField] public Image _oldScoreImage;   // Gamemanager'dan alýnabilir
    [SerializeField] Sprite _newScoreImage;

    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;

    [HideInInspector]
    public Material _juice;

    [HideInInspector]
    public float collectedFruitCount;

    public float totalFruitCount;
    float score;

    Vector3 growedScale;
    Vector3 tempShrink;
    Vector3 graterShrinkVector;
    MinMaxGradient currentFruitGradient;

    [HideInInspector]
    public List<float> collectedFruits = new List<float>();

    void Start()
    {
        _juice = Finish.Instance._juice.material;
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
        graterShrinkVector = new Vector3(graterShrinkFraction / 700, graterShrinkFraction / 700, graterShrinkFraction / 700);

        totalFruitCount = GameObject.FindGameObjectsWithTag("CollectableFruit").Length;
    }

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.gameState == GameManager.GameState.Started)
        {
            if (other.gameObject.CompareTag("CollectableFruit"))
            {
                other.gameObject.SetActive(false);
                GrowUp();
                collectedFruitCount++;
                score += scoreIncreaseAmount;
                ScoreToText();

                SoundManager.Instance.PlaySoundEffect(_collectClips[Random.Range(0, _collectClips.Length)]);

                currentFruitGradient = GameManager.Instance.fruitTypes[GameManager.Instance.currentFruitID]._gradient;

                var main = _particleCollect.main;
                main.startColor = currentFruitGradient;

                _particleCollect.Play();
            }


            if (other.gameObject.CompareTag("Ad"))
            {
                other.gameObject.SetActive(false);
                StartCoroutine(AdManager.Instance.AdPowerUpCollect());
            }


            if (other.gameObject.CompareTag("TransitionArea"))
            {
                SoundManager.Instance.PlaySoundEffect(_transitionClip);
                ParticleSystem transitionParticle = Instantiate(Finish.Instance._finishParticles, _transform.position, Quaternion.identity);
                GameManager.Instance.currentFruitID++;
                transitionParticle.Play();
                _oldScoreImage.sprite = _newScoreImage;
                _meshFilter.mesh = GameManager.Instance.fruitTypes[GameManager.Instance.currentFruitID]._mesh;
                _meshRenderer.materials = GameManager.Instance.fruitTypes[GameManager.Instance.currentFruitID]._materials;

                collectedFruits.Add(collectedFruitCount);
                collectedFruitCount = 0;
            }


            if (other.gameObject.CompareTag("Obstacle"))
            {
                other.gameObject.SetActive(false);
                _destroyParticle.Play();

                if (GameManager.isShrinking)
                {
                    GameManager.Instance.Dead();
                }
            }


            if (other.gameObject.CompareTag("Finish"))
            {
                collectedFruits.Add(collectedFruitCount);
                GameManager.Instance.Finished();
                _timeline.SetActive(true);
            }
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Grater"))
        {
            if (GameManager.isShrinking)
            {
                tempShrink = GameManager.Instance.shrinkAmount;
                GameManager.Instance.shrinkAmount += graterShrinkVector;
                _graterCollect.Play();
                SoundManager.Instance.PlaySoundEffect(_graterClip);
            }
        }
    }


    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Grater"))
        {
            GameManager.Instance.shrinkAmount = tempShrink;
            SoundManager.Instance.PauseSoundEffect();
            _graterCollect.Stop();
        }
    }



    void ScoreToText()
    {
        _scoretext.text = score.ToString();
        _scoretext.DOColor(Color.red, .2f).OnComplete(() => _scoretext.DOColor(Color.black, .2f));
        _scoretext.transform.DOScale(Vector3.one * 1.5f, .1f).OnComplete(() => _scoretext.transform.DOScale(Vector3.one, .1f));
    }

    void GrowUp()
    {
        growedScale = _transform.localScale + growthAmount;
        _transform.DOScale(growedScale, 0.1f);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

    }
}
