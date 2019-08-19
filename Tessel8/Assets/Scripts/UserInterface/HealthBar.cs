using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class HealthBar : MonoBehaviour
    {
        public GameObject healthBar;
        public GameObject healthBG;

        public bool isAlwaysVisible;

        [Tooltip("Time in seconds until fade effect starts")]
        public float waitSecondsToFade = 1;
        
        [Tooltip("Fade duration in seconds")]
        public float fadeDurationSeconds;

        private RectTransform _healthBarRect;

        private Image _healthBarImage;
        private Image _healthBGImage;

        private float _healthBarStartingXScale;
        private float _healthBarStartingAlpha;
        private float _healthBGStartingAlpha;

        private Coroutine _fadeCoroutine;

        void Awake()
        {
            _healthBarRect = healthBar.GetComponent<RectTransform>();
            _healthBarImage = healthBar.GetComponent<Image>();
            _healthBGImage = healthBG.GetComponent<Image>();

            _healthBarStartingXScale = _healthBarRect.localScale.x;
            _healthBarStartingAlpha = _healthBarImage.color.a;
            _healthBGStartingAlpha = _healthBGImage.color.a;

            if (!isAlwaysVisible)
            {
                FadeImage(_healthBarImage, _healthBarStartingAlpha);
                FadeImage(_healthBGImage, _healthBGStartingAlpha);
            }
        }

        public void OnHit(float damagePercent)
        {
            var localScale = _healthBarRect.localScale;
            var newXScaleVal = localScale.x - _healthBarStartingXScale * damagePercent;
            newXScaleVal = newXScaleVal >= 0 ? newXScaleVal : 0;
            newXScaleVal = newXScaleVal > _healthBarStartingXScale ? _healthBarStartingXScale : newXScaleVal;
            localScale = new Vector3(newXScaleVal, localScale.y, localScale.z);
            _healthBarRect.localScale = localScale;

            if (!isAlwaysVisible)
            {
                if (_fadeCoroutine != null)
                {
                    StopCoroutine(_fadeCoroutine);
                }
    
                ResetImageAlpha(_healthBarImage, _healthBarStartingAlpha);
                ResetImageAlpha(_healthBGImage, _healthBGStartingAlpha);
                _fadeCoroutine = StartCoroutine(FadeHealthBar());
            }
        }
        
        private IEnumerator FadeHealthBar()
        {
            yield return new WaitForSeconds(waitSecondsToFade);
            
            var healthBarFadeAmountPerTick = _healthBarStartingAlpha / fadeDurationSeconds;
            var healthBGFadeAmountPerTick = _healthBGStartingAlpha / fadeDurationSeconds;
            
            for (var timer = fadeDurationSeconds; timer > 0; timer -= Time.deltaTime)
            {
                FadeImage(_healthBarImage, healthBarFadeAmountPerTick * Time.deltaTime);
                FadeImage(_healthBGImage, healthBGFadeAmountPerTick * Time.deltaTime);
                yield return null;
            }     
        }

        private void ResetImageAlpha(Image image, float startingAlpha)
        {
            var imageColor = image.color;
            image.color = new Color(imageColor.r,imageColor.g, 
                imageColor.r, startingAlpha);
        }

        public void ResetHealthBar()
        {
            var localScale = _healthBarRect.localScale;
            _healthBarRect.localScale = new Vector3(_healthBarStartingXScale, localScale.y, localScale.z);
        }

        private void FadeImage(Image image, float fadeAmount)
        {
            var imageColor = image.color;
            image.color = new Color(imageColor.r,imageColor.g, 
                imageColor.r, imageColor.a - fadeAmount);
        }
    }
}