using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace _JabJob.Prefabs.Player_UI.Scripts
{
	public class CursorColorUpdater : MonoBehaviour
	{
		public int cursorRadius = 12;
	
		public Camera mainCamera;

		private Image _cursorImage;
		private float _targetGrayscale = 0f;
		private float _currentGrayscale = 0f;
		private Texture2D _texture;

		private void Start()
		{
			_cursorImage = GetComponent<Image>();
			mainCamera ??= Camera.main;
		
			StartCoroutine(UpdateCursorColor());
		}

		private void Update()
		{
			_currentGrayscale = Mathf.Lerp(_currentGrayscale, _targetGrayscale, Time.deltaTime * 5f);
			
			_cursorImage.color = new Color(_currentGrayscale, _currentGrayscale, _currentGrayscale, 1f);
		}

		IEnumerator UpdateCursorColor()
		{
			while (true)
			{
				yield return new WaitForSeconds(0.1f);
				yield return new WaitForEndOfFrame();

				_texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
				_texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
				_texture.Apply();
			
				Vector2Int center = new Vector2Int(_texture.width / 2, _texture.height / 2);

				float grayScaleCenter = _texture.GetPixel(center.x, center.y).grayscale;
				float grayScaleRight = _texture.GetPixel(center.x + cursorRadius, center.y).grayscale;
				float grayScaleLeft = _texture.GetPixel(center.x - cursorRadius, center.y).grayscale;
				float grayScaleUp = _texture.GetPixel(center.x, center.y + cursorRadius).grayscale;
				float grayScaleDown = _texture.GetPixel(center.x, center.y - cursorRadius).grayscale;

				float grayscale = (grayScaleCenter + grayScaleRight + grayScaleLeft + grayScaleUp + grayScaleDown) / 5f;
				
				_targetGrayscale = grayscale < 0.5f ? 1f : 0f;
			}
		}
	}
}
