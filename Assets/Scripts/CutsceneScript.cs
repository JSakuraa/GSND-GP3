using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoSceneTransition : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName = "GameScene"; // Change to your game scene name
    [SerializeField] private bool allowSkip = true;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Subscribe to video finished event
        videoPlayer.loopPointReached += OnVideoFinished;

        Debug.Log("Video scene started, waiting for video to finish...");
    }

    void Update()
    {

    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished, loading next scene...");
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}