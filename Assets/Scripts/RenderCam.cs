using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SFB;

public class RenderCam : MonoBehaviour
{
    [SerializeField] Camera _targetCam;
    [SerializeField] public int _screenShotWidth = 2048;
    [SerializeField] public int _screenShotHeight = 2048;
    [SerializeField] TMPro.TextMeshProUGUI _infoText;

    [Header("Thumbnail")]
    [SerializeField] Image _thumbnailImage;
    [SerializeField] RectTransform _thumbnailRect;
    [SerializeField] GameObject _hideButtonObject;
    [SerializeField] Button _hideButton;
    [SerializeField] float _defaultThumbnailSize = 100;

    private string _savePath;
    private bool _takeScreenShot = false;
    private bool _changePath = false;
    private string _text;

    private void Awake()
    {
        _text = _infoText.text;

        _savePath = Application.persistentDataPath;

        _infoText.text = string.Format(_text, _savePath);

        _hideButton.onClick.AddListener(HideLastestScreenshot);
    }

    private void OnDestroy()
    {
        _hideButton.onClick.RemoveAllListeners();
    }

    private void LateUpdate()
    {
        _takeScreenShot |= Input.GetKeyDown(KeyCode.Return);
        if (_takeScreenShot)
        {
            _takeScreenShot = false;
            TakeScreenShot();
        }

        _changePath |= Input.GetKeyDown(KeyCode.Space);
        if (_changePath)
        {
            _changePath = false;

#if !UNITY_EDITOR
            StandaloneFileBrowser.OpenFolderPanelAsync("Select Save Location", "", true, (string[] paths) => { WritePathResult(paths); });

            void WritePathResult(string[] paths)
            {
                if (paths.Length == 0)
                {
                    return;
                }

                _savePath = "";
                foreach (var p in paths)
                {
                    _savePath += p;
                }

                _infoText.text = string.Format(_text, _savePath);
                _watcher.Path = _savePath;
            }
#else
            Debug.LogWarning("File dialog only appears on build!!!");
#endif
        }

        if (_watchedDirchanged)
        {
            if (_thumbnailRect.gameObject.activeSelf)
                ShowLatestScreenshot();
            _watchedDirchanged = false;
        }
    }

    private void TakeScreenShot()
    {
        RenderTexture rt = new RenderTexture(_screenShotWidth, _screenShotHeight, 24);
        _targetCam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(_screenShotWidth, _screenShotHeight, TextureFormat.RGB24, false);
        _targetCam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, _screenShotWidth, _screenShotHeight), 0, 0);
        screenShot.Apply();

        _targetCam.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(_screenShotWidth, _screenShotHeight);

        File.WriteAllBytes(filename, bytes);

        // Open after saved
        filename = filename.Replace(" ", "%20");
        Application.OpenURL(@"file:///" + filename);

        // Create thumbnail of latest screenshot
        CreateThumbnail();

        string ScreenShotName(int width, int height)
        {
            return string.Format("{0}/screen_{1}x{2}_{3}.png",
                                  _savePath,
                                  width, height,
                                  System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        }

        void CreateThumbnail()
        {
            Rect rec = new Rect(0, 0, _screenShotWidth, _screenShotWidth);
            _thumbnailRect.gameObject.SetActive(true);
            Sprite newSprite = Sprite.Create(screenShot, rec, Vector2.zero);
            _thumbnailImage.sprite = newSprite;
        }
    }

    private void ShowLatestScreenshot()
    {
        _thumbnailRect.sizeDelta = Vector2.one * Screen.height;
        _hideButtonObject.SetActive(true);
    }

    private void HideLastestScreenshot()
    {
        _thumbnailRect.sizeDelta = Vector2.one * _defaultThumbnailSize;
        _hideButtonObject.SetActive(false);
    }

    #region  FileWatcher
    private bool _watchedDirchanged;
    private FileSystemWatcher _watcher;

    private void OnEnable()
    {
        _watcher = new FileSystemWatcher(_savePath);

        _watcher.NotifyFilter = NotifyFilters.Attributes |
      NotifyFilters.CreationTime |
      NotifyFilters.FileName |
      NotifyFilters.LastAccess |
      NotifyFilters.LastWrite |
      NotifyFilters.Size |
      NotifyFilters.Security;

        // Add event
        _watcher.Changed += OnChanged;
        _watcher.Created += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Renamed += OnChanged;

        // Begin watching
        _watcher.EnableRaisingEvents = true;
    }

    private void OnDisable()
    {
        if (_watcher != null)
        {
            _watcher.Changed -= OnChanged;
            _watcher.Dispose();
        }
    }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        _watchedDirchanged = true;
    }
    #endregion FileWatcher
}
