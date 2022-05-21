using System;
using Unity.Mathematics;
using UnityEngine;

namespace DrawInstance
{
    public class InstanceDrawer : MonoBehaviour
    {
        public int count;
        public float boundsLength = 100f;
        public Mesh mesh;
        public Material mat;
        private Material matInstance;
        private ComputeBuffer colorBuffer;
        private ComputeBuffer matrixBuffer;
        private MaterialPropertyBlock propertyBlock;
        private readonly int colorProperty = Shader.PropertyToID("_Colors");
        private readonly int matrixProperty = Shader.PropertyToID("_Matrices");
        private Bounds bounds;

        void OnEnable()
        {
            matInstance = Instantiate(mat);
            bounds = new Bounds(Vector3.zero, boundsLength * Vector3.one);
            propertyBlock = new MaterialPropertyBlock();
            GenerateTempData();
        }

        void OnDisable()
        {
            colorBuffer?.Release();
            matrixBuffer?.Release();
            colorBuffer = null;
            matrixBuffer = null;
            propertyBlock = null;
        }

        private void OnValidate()
        {
            if (enabled && Tests())
            {
                OnDisable();
                OnEnable();
            }
        }

        private bool Tests()
        {
            return (colorBuffer != null && matrixBuffer != null);
        }

        void LateUpdate()
        {
            if (colorBuffer == null || matrixBuffer == null)
            {
                return;
            }

            if (!colorBuffer.IsValid() || !matrixBuffer.IsValid())
            {
                return;
            }

            Draw();
        }

        void Draw()
        {
            Graphics.DrawMeshInstancedProcedural(mesh, 0, matInstance, bounds, colorBuffer.count, propertyBlock);
        }

        public void SetBuffers(Vector4[] colors, Matrix4x4[] matrices)
        {

            colorBuffer?.Release();
            colorBuffer = null;



            matrixBuffer?.Release();
            matrixBuffer = null;


            colorBuffer = new ComputeBuffer(colors.Length, 16);
            matrixBuffer = new ComputeBuffer(matrices.Length, 64);



            colorBuffer.SetData(colors);
            matrixBuffer.SetData(matrices);
            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }

            propertyBlock.SetBuffer(colorProperty, colorBuffer);
            propertyBlock.SetBuffer(matrixProperty, matrixBuffer);

            // Hack from: https://forum.unity.com/threads/computebuffer-warning-when-component-uses-executeineditmode.844648/
            // I am explicitly releasing buffers above before creating and explicitly releasing in OnDisable(), but the GC is
            // trying to collect/finalize them anyway.
            GC.SuppressFinalize(colorBuffer);
            GC.SuppressFinalize(matrixBuffer);
        }

        private void GenerateTempData()
        {
            var cols = new Vector4[3];
            var mats = new Matrix4x4[3];
            cols[0] = Color.red;
            cols[1] = Color.white;
            cols[2] = Color.blue;
            mats[0] = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.identity, Vector3.one);
            mats[0] = Matrix4x4.TRS(new Vector3(1f, 1f, 0f), Quaternion.identity, Vector3.one);
            mats[0] = Matrix4x4.TRS(new Vector3(2f, 2f, 0f), Quaternion.identity, Vector3.one);
            SetBuffers(cols, mats);
        }
    }
}