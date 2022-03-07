using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShowCase.AR
{
    public class ARObject : MonoBehaviour
    {
        public bool isMoving = false;
        public Transform originTransform;
        public Color objectColor;
        [SerializeField] private Renderer _renderer;
        private float _t;
        private Vector3 _startPosition;
        private Transform _targetTransform;
        private Transform _currentTargetTransform;
        private float _timeToReachTarget = 2f;

        void OnEnable()
        {
            this.transform.position = originTransform.position;
            objectColor = ARObjectManager.Instance.GetRandomColor();
            _renderer.material.color = objectColor;
            ARObjectManager.Instance.RegisterObject(this);
        }

        private void OnDisable()
        {
            this.transform.position = originTransform.position;
            ARObjectManager.Instance.DeRegisterObject(this);
        }


        public void UpdateObject()
        {
            if (_currentTargetTransform == null || !isMoving)
                return;

            _t += Time.deltaTime / _timeToReachTarget;
            transform.position = Vector3.Lerp(this.transform.position, _currentTargetTransform.position, _t);

            //MoveBack and forth
            if (_t >= 1)
            {
                _t = 0;
                if (_currentTargetTransform == _targetTransform)
                    _currentTargetTransform = originTransform;
                else
                {
                    if (ARObjectManager.Instance.hasMoreThanOne)
                        _currentTargetTransform = _targetTransform;
                    else
                        isMoving = false;
                }
            }
        }

        public void SetDestination(Transform destinationT, float time)
        {
            isMoving = true;
            _t = 0;
            _startPosition = transform.position;
            _timeToReachTarget = time;
            _currentTargetTransform = destinationT;
            _targetTransform = destinationT;
        }

        public void MoveBack()
        {
            _t = 1 - _t;
            _targetTransform = originTransform;
        }
    }
}