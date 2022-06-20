using _JabJob.Prefabs.Player.Scripts;
using Acron0;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndZone : MonoBehaviour
{
    public AudioSource carAudioSource;
    public AudioSource audioSource;
    public Image fadeImage;
    
    private bool _isEndZone = false;
    private float _time;
    private bool _finalAudioPlayed = false;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        _isEndZone = true;
    }

    private void Update()
    {
        if (!_isEndZone)
            return;
        
        _time += Time.deltaTime;

        _time = Mathf.Min(_time, 10f);
        
        float uiEasedTimeFadeOut = Easings.CubicEaseInOut(Mathf.Min(_time, 1f) / 1f);
        
        if (uiEasedTimeFadeOut > 0.75f)
            MovementController.Instance.CanMove = false;
        
        float easedTimeCarAudioIdle = 1f - Easings.CubicEaseInOut(Mathf.Min(_time, 2f) / 2f);
        fadeImage.color = new Color(0.1f, 0.1f, 0.1f, uiEasedTimeFadeOut);
        carAudioSource.volume = easedTimeCarAudioIdle;

        if (_time > 2f && !_finalAudioPlayed)
        {
            audioSource.Play();
            _finalAudioPlayed = true;
        }

        if (_finalAudioPlayed && !audioSource.isPlaying)
        {
            SceneManager.LoadScene("_JabJob/Scenes/EndScene");
        }
    }
}
