using System;
using LibraryBen.Assembly.Miscellaneous.ExtensionMethods;
using MText;
using RSG;
using UnityEngine;
using Voodoo.Optimization._Common;

namespace Voodoo.Optimization.WordHunt.Scripts
{
    public class WordHuntLetterView : CollectibleView
    {
        public Vector3 Size { get; private set; }

        [SerializeField] private FallingObject m_FallingObject;
        [SerializeField] private Modular3DText text;
        [SerializeField] private ParticleSystem sparkleParticles;
        [SerializeField] private ParticleSystem circleParticles;

        private bool isInitialized;
        private Vector2 sparklesMinMaxStart;
        private Vector3 circleParticlesStart;
        private float size;

        private void Initialize()
        {
            if (isInitialized)
                return;
            isInitialized = true;
            m_FallingObject.SizeRequired = 6;

            var startSize = sparkleParticles.main.startSize;
            sparklesMinMaxStart = new Vector2(startSize.constantMin, startSize.constantMax);
            circleParticlesStart = circleParticles.transform.localScale;
        }

        public void UpdateParticlesSize()
        {
            Initialize();

            var startSize = sparkleParticles.main.startSize;
            startSize.constantMin = transform.localScale.x * sparklesMinMaxStart.x;
            startSize.constantMax = transform.localScale.x * sparklesMinMaxStart.y;
        }

        public IPromise SetLetter(char letter)
        {
            Initialize();

            return Promise.Resolved()
                .Then(() =>
                {
                    text.Text = letter.ToString();
                    text.UpdateText();
                })

                .WaitFrame(this)

                .Then(() =>
                {
                    var bounds = text.gameObject.CalculateBounds().Value;
                    Size = bounds.size;
                    text.transform.position += transform.position - bounds.center;

                    var meshFilter = text.GetComponentInChildren<MeshFilter>();
                    var meshRenderer = meshFilter.GetComponent<MeshRenderer>();
                    var boxCollider = meshFilter.gameObject.AddComponent<BoxCollider>();
                    //size = fallingObject.RecalculateSize(boxCollider.bounds);

                    circleParticles.transform.localScale =
                        Vector3.one * Mathf.Max(Size.x, Size.z);

                    foreach (var particleSystem in sparkleParticles.GetComponentsInChildren<ParticleSystem>())
                    {
                        var shape = particleSystem.shape;
                        shape.meshRenderer = meshRenderer;
                    }
                });
        }
    }
}