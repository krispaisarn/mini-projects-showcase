using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

namespace ShowCase.AR
{
    public class ImageTracking : MonoBehaviour
    {
        [SerializeField] private ARTrackedImageManager _arImageTrackManager;

        public void OnEnable()
        {
            _arImageTrackManager.trackedImagesChanged += OnImageChanged;
        }

        public void OnDisable()
        {
            _arImageTrackManager.trackedImagesChanged -= OnImageChanged;
        }

        void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
            {
                UpdateInfo(trackedImage);
            }

            foreach (var trackedImage in eventArgs.updated)
                UpdateInfo(trackedImage);
        }

        void UpdateInfo(ARTrackedImage trackedImage)
        {
            if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.None || trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
            {
                trackedImage.gameObject.SetActive(false);
            }
            else
            {
                trackedImage.gameObject.SetActive(true);
            }

        }
    }
}