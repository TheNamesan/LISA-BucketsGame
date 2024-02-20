using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace TUFF
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("References")]
        public Camera cam;
        public SceneProperties si;
    
        [Header("Camera Pixel Perfect Offset")]
        private const float pixelPerfectOffsetX = 0.075f;
        private const float pixelPerfectOffsetY = 0.03f;
    
        float timeOffset = 1;
    
        float camHalfHeight;
        float camHalfWidth;
        Vector3 previousPosition;

        [Header("Parallax")]
        [Header("Parallax Speed")]
        public float parallaxHorizontalBaseSpeed = 100f;
        public float parallaxVerticalBaseSpeed = 35f;

        [Tooltip("Reference to the Scene's Background GameObject. The background's position will Lerp between the Scene's min and max size.")]
        public Transform background;
        public Vector2 parallaxOffset = new Vector2();
        public List<Vector2> individualParallaxOffset = new List<Vector2>();
        [Tooltip("Reference to the Scene's Parallax GameObjects. The parallax will scroll at a lower speed the higher the GameObject's Z position is.")]
        public List<SpriteRenderer> parallaxElements;

        [Header("Fixed Parallax")]
        [Tooltip("Moves like the background, but for parallax elements.")]
        public List<SpriteRenderer> fixedParallaxElements;

        public List<Vector3> originalParallaxPos = new List<Vector3>();
        
        SpriteRenderer backgroundSpr;

        public UnityEvent<bool> onCameraFollowingToggle = new();
    
        [HideInInspector] public bool disableCameraFollow;
        [HideInInspector] public Vector3 orgPosition;
        private Vector2 min;
        private Vector2 max;
        Tween tween;

        private void Awake()
        {
            if (!cam) cam = GetComponent<Camera>();
            if (background && !backgroundSpr) backgroundSpr = background.GetComponent<SpriteRenderer>();
        }
        void Start()
        {
            previousPosition = transform.position;
            GetParallaxOriginalPosition();
            UpdateCamera();
        }
    
        void LateUpdate()
        {
            UpdateCamera();
        }
    
        public void GetParallaxOriginalPosition()
        {
            originalParallaxPos.Clear();
            for (int i = 0; i < parallaxElements.Count; i++)
            {
                if (parallaxElements[i] == null) 
                {
                    originalParallaxPos.Add(Vector2.zero);
                    continue;
                }
                var position = parallaxElements[i].transform.position;
                originalParallaxPos.Add(new Vector2(position.x, position.y));
            }
        }
        private void UpdateCamera()
        {
            Vector3 startpos = transform.position;
            if (!FollowerInstance.player) return;
            if (!FollowerInstance.player.controller) return;
            Vector3 endpos = FollowerInstance.player.controller.transform.position;
            endpos.z = transform.position.z;
            camHalfHeight = (2f * cam.orthographicSize) / 2;
            camHalfWidth = camHalfHeight * cam.aspect;
            //Debug.Log($"W: {camHalfWidth}, H: {camHalfHeight}");
            min = new Vector2(Mathf.Min(si.min.x, si.max.x), Mathf.Min(si.min.y, si.max.y));
            max = new Vector2(Mathf.Max(si.min.x, si.max.x), Mathf.Max(si.min.y, si.max.y));
            float minPosX = min.x + camHalfWidth;
            float maxPosX = max.x - camHalfWidth;
            float minPosY = min.y + camHalfHeight;
            float maxPosY = max.y - camHalfHeight;
            
            if (!disableCameraFollow) transform.position = endpos;

            // Clamp Position
            Vector2 clampMinPos = new Vector2(minPosX + pixelPerfectOffsetX, minPosY + pixelPerfectOffsetY);
            Vector2 clampMaxPos = new Vector2(maxPosX - pixelPerfectOffsetX, maxPosY - pixelPerfectOffsetY);
            if (clampMinPos.x > clampMaxPos.x)
            {
                if (clampMinPos.x > camHalfWidth) clampMinPos.x = camHalfWidth;
                if (clampMaxPos.x < camHalfWidth) clampMaxPos.x = camHalfWidth;
            }
            if (clampMinPos.y > clampMaxPos.y)
            { 
                if (clampMinPos.y > camHalfHeight) clampMinPos.y = camHalfHeight;
                if (clampMaxPos.y < camHalfHeight) clampMaxPos.y = camHalfHeight;
            }
            transform.position = new Vector3
                (
                    Mathf.Clamp(transform.position.x, clampMinPos.x, clampMaxPos.x),
                    Mathf.Clamp(transform.position.y, clampMinPos.y, clampMaxPos.y),
                    transform.position.z
                );

            UpdateParallax(clampMinPos.x, clampMaxPos.x, clampMinPos.y, clampMaxPos.y);
        }
    
        private void UpdateParallax(float minPosX, float maxPosX, float minPosY, float maxPosY)
        {
            
            if(background != null)
            {
                float posX = Mathf.Lerp(min.x + backgroundSpr.bounds.size.x * 0.5f, max.x - backgroundSpr.bounds.size.x * 0.5f,
                    Mathf.InverseLerp(minPosX, maxPosX, transform.position.x));
                float posY = Mathf.Lerp(min.y + backgroundSpr.bounds.size.y * 0.5f, max.y - backgroundSpr.bounds.size.y * 0.5f,
                    Mathf.InverseLerp(minPosY, maxPosY, transform.position.y));

                background.position = new Vector3(
                    posX,
                    posY,
                    background.position.z
                    );
            }
            for (int i = 0; i < fixedParallaxElements.Count; i++)
            {
                if (fixedParallaxElements[i] == null) continue;

                Vector2 size = fixedParallaxElements[i].bounds.size;
                float posX = Mathf.Lerp(min.x + size.x * 0.5f, max.x - size.x * 0.5f,
                    Mathf.InverseLerp(minPosX, maxPosX, transform.position.x));

                //float posY = Mathf.Lerp(si.min.y + size.y * 0.5f, si.max.y - size.y * 0.5f,
                //    Mathf.InverseLerp(minPosX, maxPosX, transform.position.x));
                float posY = fixedParallaxElements[i].transform.position.y;

                fixedParallaxElements[i].transform.position = new Vector3(
                    posX,
                    posY,
                    fixedParallaxElements[i].transform.position.z
                    );
            }
            for (int i = 0; i < parallaxElements.Count; i++)
            {
                if (parallaxElements[i] == null) continue;
                float positionZ = parallaxElements[i].transform.position.z == 0 ? 1 : parallaxElements[i].transform.position.z; // Avoid division by zero
                float scaleX = parallaxHorizontalBaseSpeed / positionZ; //The higher the z position, the slower the parallax will go.
                float scaleY = parallaxVerticalBaseSpeed / positionZ; //The higher the z position, the slower the parallax will go.

                float offsetX = transform.position.x * 0.1f * scaleX - (minPosX * 0.1f * scaleX);
                offsetX -= parallaxElements[i].bounds.size.x * 0.5f - camHalfWidth;
                Vector2 indParallaxOffset = Vector2.zero;
                if (i < individualParallaxOffset.Count) 
                    indParallaxOffset = individualParallaxOffset[i];
                float posX = transform.position.x - offsetX + parallaxOffset.x + indParallaxOffset.x;

                float offsetY = (transform.position.y - minPosY) * 0.1f * scaleY;
                float posY = originalParallaxPos[i].y - offsetY + parallaxOffset.y + indParallaxOffset.y;

                parallaxElements[i].transform.position = new Vector3(
                    posX,
                    posY, 
                    parallaxElements[i].transform.position.z);
                
            }
            previousPosition = transform.position;
        }

        public void DisableCameraFollow(bool input)
        {
            disableCameraFollow = input;
            onCameraFollowingToggle.Invoke(!input);
        }
    
        public void MoveCamera(CameraMove cameraMove)
        {
            bool rememberToEnableCamera = false;
            DisableCameraFollow(true);
            KillTween();
            Vector3 target = Vector3.zero;
            switch(cameraMove.moveCameraType)
            {
                case MoveCameraType.MoveDelta:
                    target = new Vector3(transform.position.x + cameraMove.moveDelta.x,
                        transform.position.y + cameraMove.moveDelta.y, transform.position.z);
                    break;
    
                case MoveCameraType.MoveToWorldPosition:
                    target = new Vector3(cameraMove.targetWorldPosition.x,
                        cameraMove.targetWorldPosition.y, transform.position.z);
                    break;
    
                case MoveCameraType.MoveToTransformPosition:
                    target = new Vector3(cameraMove.targetTransform.position.x,
                        cameraMove.targetTransform.position.y, transform.position.z);
                    break;
    
                case MoveCameraType.ReturnToPlayer:
                    target = new Vector3(FollowerInstance.player.controller.transform.position.x,
                        FollowerInstance.player.controller.transform.position.y, transform.position.z);
                    rememberToEnableCamera = true;
                    break;
            }
            target = ClampVector(target);
            MoveCameraToTarget(cameraMove, rememberToEnableCamera, target);
        }
    
        private void MoveCameraToTarget(CameraMove cameraMove, bool rememberToEnableCamera, Vector3 target)
        {
            tween = transform.DOMove(
                                    target,
                                    cameraMove.timeDuration)
                                    .SetEase(cameraMove.easeType)
                                    .OnComplete(() =>
                                    {
                                        KillTween();
                                        if (rememberToEnableCamera) DisableCameraFollow(false);
                                        cameraMove.onMovementEnd?.Invoke();
                                    });
        }
    
        public void ShakeCamera(CameraShake cameraShake)
        {
            DisableCameraFollow(true);
            KillTween();
            if(!cameraShake.enableFadeOut) orgPosition = transform.position;
            tween = transform.DOShakePosition(
                    cameraShake.timeDuration,
                    new Vector3(cameraShake.shakeStrength.x, cameraShake.shakeStrength.y, 0),
                    cameraShake.vibrato,
                    cameraShake.randomness,
                    cameraShake.snapping,
                    cameraShake.enableFadeOut
                )
                .OnComplete(() =>
                    {
                        if (!cameraShake.enableFadeOut) transform.position = orgPosition;
                        KillTween();
                        DisableCameraFollow(cameraShake.disableCameraFollow);
                        cameraShake.onShakeEnd?.Invoke();
                    }
                );
        }
        Vector3 ClampVector(Vector3 vector)
        {
            if (!si) return vector;
            vector = new Vector3
                (
                    Mathf.Clamp(vector.x, si.min.x + camHalfWidth, si.max.x - camHalfWidth),
                    Mathf.Clamp(vector.y, si.min.y + camHalfHeight, si.max.y - camHalfHeight),
                    transform.position.z
                );
    
            return vector;
        }
        void KillTween()
        {
            tween.Kill();
            tween = null;
        }
    }
}
