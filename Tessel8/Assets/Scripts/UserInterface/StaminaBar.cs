using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class StaminaBar : MonoBehaviour
    {
        public GameObject staminaBar;
        public GameObject staminaBG;

        public bool isAlwaysVisible;

        [Tooltip("Time in seconds until fade effect starts")]
        public float waitSecondsToFade = 1;
        
        [Tooltip("Fade duration in seconds")]
        public float fadeDurationSeconds;

        private RectTransform _staminaBarRect;

        private Image _staminaBarImage;
        private Image _staminaBGImage;

        private float _staminaBarStartingXScale;
        private float _staminaBarStartingAlpha;
        private float _staminaBGStartingAlpha;

        private bool _isOutOfStamina;

        private Coroutine _fadeCoroutine;

        void Awake()
        {
            _staminaBarRect = staminaBar.GetComponent<RectTransform>();
            _staminaBarImage = staminaBar.GetComponent<Image>();
            _staminaBGImage = staminaBG.GetComponent<Image>();

            _staminaBarStartingXScale = _staminaBarRect.localScale.x;
            _staminaBarStartingAlpha = _staminaBarImage.color.a;
            _staminaBGStartingAlpha = _staminaBGImage.color.a;

            _isOutOfStamina = false;

            if (!isAlwaysVisible)
            {
                FadeImage(_staminaBarImage, _staminaBarStartingAlpha);
                FadeImage(_staminaBGImage, _staminaBGStartingAlpha);
            }
        }

        public void OnAction(float actionCostPercent)
        {
            var localScale = _staminaBarRect.localScale;
            var newXScaleVal = localScale.x - _staminaBarStartingXScale * actionCostPercent;
            newXScaleVal = newXScaleVal >= 0 ? newXScaleVal : 0;

            if (newXScaleVal <= 0)
            {
                _isOutOfStamina = true;
            }
            else
            {
                _isOutOfStamina = false;
                localScale = new Vector3(newXScaleVal, localScale.y, localScale.z);
                _staminaBarRect.localScale = localScale;
            }

            if (!isAlwaysVisible)
            {
                if (_fadeCoroutine != null)
                {
                    StopCoroutine(_fadeCoroutine);
                }
    
                ResetImageAlpha(_staminaBarImage, _staminaBarStartingAlpha);
                ResetImageAlpha(_staminaBGImage, _staminaBGStartingAlpha);
                _fadeCoroutine = StartCoroutine(FadeStaminaBar());
            }
        }
        
        
        
        private IEnumerator FadeStaminaBar()
        {
            yield return new WaitForSeconds(waitSecondsToFade);
            
            var staminaBarFadeAmountPerTick = _staminaBarStartingAlpha / fadeDurationSeconds;
            var staminaBGFadeAmountPerTick = _staminaBGStartingAlpha / fadeDurationSeconds;
            
            for (var timer = fadeDurationSeconds; timer > 0; timer -= Time.deltaTime)
            {
                FadeImage(_staminaBarImage, staminaBarFadeAmountPerTick * Time.deltaTime);
                FadeImage(_staminaBGImage, staminaBGFadeAmountPerTick * Time.deltaTime);
                yield return null;
            }     
        }

        private void ResetImageAlpha(Image image, float startingAlpha)
        {
            var imageColor = image.color;
            image.color = new Color(imageColor.r,imageColor.g, 
                imageColor.r, startingAlpha);
        }

        public void ResetStaminaBar()
        {
            var localScale = _staminaBarRect.localScale;
            _staminaBarRect.localScale = new Vector3(_staminaBarStartingXScale, localScale.y, localScale.z);
            _isOutOfStamina = false;
        }

        private void FadeImage(Image image, float fadeAmount)
        {
            var imageColor = image.color;
            image.color = new Color(imageColor.r,imageColor.g, 
                imageColor.r, imageColor.a - fadeAmount);
        }
    }
}
