using UnityEngine;

namespace WMG
{
    [System.Serializable]
    public struct ValueRange
    {
        public float Min;
        public float Max;

        public ValueRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Clamp(float value)
        {
            if (value < Min)
                value = Min;
            else if (value > Max)
                value = Max;

            return value;
        }

        public void Clamp(ref float value)
        {
            value = Clamp(value);
        }

        public bool IsInRange(float value) => value >= Min && value <= Max;

        public float Random()
        {
            return UnityEngine.Random.Range(Min, Max);
        }
    }
}
