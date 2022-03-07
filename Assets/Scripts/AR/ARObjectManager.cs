using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShowCase.AR
{
    public class ARObjectManager : MonoSingleton<ARObjectManager>
    {
        public List<ARObject> aRObjectList = new List<ARObject>();
        public bool hasMoreThanOne = false;
        [SerializeField] private float _colorDistinctValue = 0.15f;
        [SerializeField] private float _timeToReachTarget = 2f;

        private List<Color> _colors = new List<Color>();

        public Color GetRandomColor()
        {
            Color rColor = RandomColor();

            if (_colors.Count == 0)
            {
                return rColor;
            }

            bool isColorDistinct = false;
            int tryCount = 0;
            while (!isColorDistinct)
            {

                isColorDistinct = false;

                foreach (var item in _colors)
                {
                    float h1 = GetHue(rColor);
                    float h2 = GetHue(item);
                    if (Mathf.Abs(h1 - h2) > _colorDistinctValue)
                    {
                        isColorDistinct = true;
                    }
                    else
                    {
                        isColorDistinct = false;
                        break;
                    }
                }

                rColor = RandomColor();

                // Escape loop when exceeds limit
                tryCount++;
                if (tryCount >= 100)
                    isColorDistinct = true;
            }

            Color RandomColor()
            {
                return Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);
            }

            float GetHue(Color color)
            {
                float H, S, V;

                Color.RGBToHSV(color, out H, out S, out V);
                return H;
            }

            return rColor;
        }

        void Update()
        {
            foreach (var item in aRObjectList)
                item.UpdateObject();
        }

        private void SetObjectDestination()
        {
            if (aRObjectList.Count > 1)
            {
                for (int i = 0; i < aRObjectList.Count; i++)
                {
                    int nextIndex = i + 1 < aRObjectList.Count ? i + 1 : 0;
                    aRObjectList[i].SetDestination(aRObjectList[nextIndex].originTransform, _timeToReachTarget);
                }
            }
        }

        public void RegisterObject(ARObject aRObject)
        {
            aRObjectList.Add(aRObject);
            _colors.Add(aRObject.objectColor);
            SetObjectDestination();
            hasMoreThanOne = aRObjectList.Count > 1;
        }

        public void DeRegisterObject(ARObject aRObject)
        {
            aRObject.MoveBack();
            aRObjectList.Remove(aRObject);
            _colors.Remove(aRObject.objectColor);
            SetObjectDestination();
            if (aRObjectList.Count == 1)
            {
                aRObjectList[0].MoveBack();
            }
        }
    }
}