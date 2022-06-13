using System.Collections.Generic;
using Acron0;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _JabJob.Prefabs.Password_Pad.Scripts
{
	internal enum PasswordPadState
	{
		Idle,
		WaitForGoodCharEndAnim,
		WaitForBadCharEndAnim,
		WaitForTimerEndAnim,
		Complete
	}
	
	public class PasswordPad : MonoBehaviour
	{
		[Header("Animation Elements")]
		public MeshRenderer emissivePlane;
		public TextMeshProUGUI passwordText;
		public MeshFilter timer;
		
		[Header("Animation Properties")]
		public int timerMeshPoints = 20;
		public float timerMeshInnerRadius = 0.015f;
		public float timerMeshOuterRadius = 0.02f;
		public Color badCharacterColor = Color.red;
		public Color goodCharacterColor = Color.green;
		public float emissionForce = 0.75f;
		public Easings.Functions emissionPulseFunction = Easings.Functions.CubicEaseInOut;
		public float characterResultAnimDuration = 1.5f;
		public float timerDuration = 10f;

		[Header("Logic Properties")]
		public string password;
		public UnityEvent onPasswordComplete;
		
		[Header("Sound Properties")]
		public AudioClip incorrectLetterSound;
		public AudioClip correctLetterSound;
		public AudioClip waitForTimerTickSound;
		public AudioClip timerEndSound;
		public AudioSource audioSource;

		private float _easingTime = 0f;
		private float[,] _timerMeshCoordinates;
		private PasswordPadState _state = PasswordPadState.Idle;
		private string _passwordBuffer = "";
		private readonly int _baseColorHash = Shader.PropertyToID("_BaseColor");
		private readonly int _emissionColorHash = Shader.PropertyToID("_EmissionColor");

		private void Start()
		{
			_timerMeshCoordinates = new float[timerMeshPoints, 5];
			
			float step = Mathf.PI * 2 / timerMeshPoints;
			for (var i = 0; i < timerMeshPoints; i++)
			{
				float angle = i * step;
				
				_timerMeshCoordinates[i, 0] = angle;
				
				_timerMeshCoordinates[i, 1] = Mathf.Cos(angle) * timerMeshInnerRadius;
				_timerMeshCoordinates[i, 2] = Mathf.Sin(angle) * timerMeshInnerRadius;
				_timerMeshCoordinates[i, 3] = Mathf.Cos(angle) * timerMeshOuterRadius;
				_timerMeshCoordinates[i, 4] = Mathf.Sin(angle) * timerMeshOuterRadius;
			}
			
			GenerateTimerMesh(0f);
		}

		private void Update()
		{
			float easedTime;
			Color color;
			
			switch (_state)
			{
				case PasswordPadState.Idle:
					break;
				case PasswordPadState.Complete:
					_easingTime += Time.deltaTime;
					easedTime = Mathf.Sin(_easingTime * 4f);
					
					color = Color.Lerp(Color.white, goodCharacterColor, easedTime);
					
					emissivePlane.material.SetColor(_baseColorHash, color);
					emissivePlane.material.SetColor(_emissionColorHash, color * emissionForce);
					
					break;
				case PasswordPadState.WaitForBadCharEndAnim:
					_easingTime += Time.deltaTime;
					
					easedTime = _easingTime / characterResultAnimDuration < 0.5f
						? Easings.Interpolate(_easingTime / characterResultAnimDuration * 2f, emissionPulseFunction)
						: Easings.Interpolate(1f - (_easingTime / characterResultAnimDuration - 0.5f) * 2f, emissionPulseFunction);
					
					color = Color.Lerp(Color.white, badCharacterColor, easedTime);
					
					emissivePlane.material.SetColor(_baseColorHash, color);
					emissivePlane.material.SetColor(_emissionColorHash, color * emissionForce);
					
					if (_easingTime >= characterResultAnimDuration)
					{
						_easingTime = 0f;
						audioSource.PlayOneShot(incorrectLetterSound);
						_state = PasswordPadState.WaitForTimerEndAnim;
						audioSource.clip = waitForTimerTickSound;
						audioSource.loop = true;
						audioSource.Play();
						_passwordBuffer = _passwordBuffer.Substring(0, _passwordBuffer.Length - 1);
						passwordText.text = _passwordBuffer;
						
						GenerateTimerMesh(1f);
					}
					
					break;
				case PasswordPadState.WaitForGoodCharEndAnim:
					_easingTime += Time.deltaTime;
					
					easedTime = _easingTime / characterResultAnimDuration < 0.5f
						? Easings.Interpolate(_easingTime / characterResultAnimDuration * 2f, emissionPulseFunction)
						: Easings.Interpolate(1f - (_easingTime / characterResultAnimDuration - 0.5f) * 2f, emissionPulseFunction);
					
					color = Color.Lerp(Color.white, goodCharacterColor, easedTime);
					
					emissivePlane.material.SetColor(_baseColorHash, color);
					emissivePlane.material.SetColor(_emissionColorHash, color * emissionForce);
					
					if (_easingTime >= characterResultAnimDuration)
					{
						_easingTime = 0f;
						audioSource.PlayOneShot(correctLetterSound);
						
						if (_passwordBuffer.Length >= password.Length)
						{
							_state = PasswordPadState.Complete;
							passwordText.text = "OK";
							onPasswordComplete.Invoke();
						}
						else
						{
							_state = PasswordPadState.Idle;
						}
					}
					break;
				case PasswordPadState.WaitForTimerEndAnim:
					_easingTime += Time.deltaTime;

					float t = _easingTime / timerDuration;
					
					GenerateTimerMesh(1f - t);
					
					if (_easingTime >= timerDuration)
					{
						_easingTime = 0f;
						_state = PasswordPadState.Idle;
						audioSource.clip = null;
						audioSource.loop = false;
						
						audioSource.PlayOneShot(timerEndSound);
						
						GenerateTimerMesh(0f);
					}
					break;
			}
		}

		public void EnterCharacter(string character)
		{
			if (_state != PasswordPadState.Idle)
				return;
			
			if (_passwordBuffer.Length >= password.Length)
				return;

			_state = password.StartsWith(_passwordBuffer + character)
				? PasswordPadState.WaitForGoodCharEndAnim
				: PasswordPadState.WaitForBadCharEndAnim;
			
			_passwordBuffer += character;
			
			passwordText.text = _passwordBuffer;
		}

		private void GenerateTimerMesh(float percentage)
		{
			float clampedPercentage = Mathf.Clamp01(percentage);
			
			Mesh mesh = new Mesh();
			
			List<Vector3> vertices = new List<Vector3>();
			List<int> triangles = new List<int>();

			if (clampedPercentage != 0f)
			{
				float angle = percentage * Mathf.PI * 2;

				for (int i = 0; i < _timerMeshCoordinates.GetLength(0); i++)
				{
					if (_timerMeshCoordinates[i, 0] > angle)
						break;
				
					vertices.Add(new Vector3(_timerMeshCoordinates[i, 1], _timerMeshCoordinates[i, 2], 0));
					vertices.Add(new Vector3(_timerMeshCoordinates[i, 3], _timerMeshCoordinates[i, 4], 0));
				}
				
				vertices.Add(new Vector3(Mathf.Cos(angle) * timerMeshInnerRadius, Mathf.Sin(angle) * timerMeshInnerRadius, 0));
				vertices.Add(new Vector3(Mathf.Cos(angle) * timerMeshOuterRadius, Mathf.Sin(angle) * timerMeshOuterRadius, 0));
				
				for (int i = 0; i < vertices.Count - 2; i += 2)
				{
					triangles.Add(i);
					triangles.Add(i + 2);
					triangles.Add(i + 1);

					triangles.Add(i + 1);
					triangles.Add(i + 2);
					triangles.Add(i + 3);
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
			
			mesh.RecalculateNormals();
			
			timer.mesh = mesh;
		}
	}
}
