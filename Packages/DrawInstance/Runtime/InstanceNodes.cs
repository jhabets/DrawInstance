using UnityEngine;

namespace DrawInstance
{
    public class InstanceNodes : MonoBehaviour
    {
        public InstanceDrawer drawer;
        protected Vector4[] colorArray;
        protected Matrix4x4[] matrixArray;

        private void OnEnable()
        {
            AddListeners();
            GenerateInstanceData();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        protected virtual void AddListeners()
        {
        }

        protected virtual void RemoveListeners()
        {
        }

        private void OnValidate()
        {
            if (enabled && Tests())
            {
                OnDisable();
                OnEnable();
            }
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                GenerateInstanceData();
            }
        }

        protected bool Tests()
        {
            return true;
        }

        protected virtual void GenerateInstanceData()
        {
            var up = Vector3.up;
            var one = Vector3.one;
            int count = 5;
            colorArray = new Vector4[count];
            matrixArray = new Matrix4x4[count];
            var rand = new Unity.Mathematics.Random(19061976);
            for (int i = 0; i < count; i++)
            {

                var color = new Vector4(rand.NextFloat(), rand.NextFloat(), rand.NextFloat(), 1f);
                var position = new Vector3(i*2f, 0, 0);
                var rotation = Quaternion.AngleAxis(10f * i, up);
                var scale = one;
                var matrix = Matrix4x4.TRS(position, rotation, scale);
                colorArray[i] = color;
                matrixArray[i] = matrix;
            }
            drawer.SetBuffers(colorArray, matrixArray);
        }
    }

}