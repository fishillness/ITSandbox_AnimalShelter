using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasObjectController : MonoBehaviour
{
    [Serializable]
    private class CanvasObjectPosition
    {
        public Vector2 MinAnchors;
        public Vector2 MaxAnchors;
        public Vector2 Position;
        public Vector2 Scale = Vector2.one;
    }

    [Serializable]
    private class CanvasObject
    {
        public RectTransform CanvasObjectRectTransform;
        public CanvasObjectPosition PortraitOrientationPosition;
        public CanvasObjectPosition LandscapeOrientationPosition;

        public void MovingTheCanvasObjectToLandscapeOrientation()
        {
            CanvasObjectRectTransform.anchorMin = LandscapeOrientationPosition.MinAnchors;
            CanvasObjectRectTransform.anchorMax = LandscapeOrientationPosition.MaxAnchors;
            CanvasObjectRectTransform.anchoredPosition = LandscapeOrientationPosition.Position;
            CanvasObjectRectTransform.localScale = LandscapeOrientationPosition.Scale;
        }
        public void MovingTheCanvasObjectToPortraitOrientation()
        {
            CanvasObjectRectTransform.anchorMin = PortraitOrientationPosition.MinAnchors;
            CanvasObjectRectTransform.anchorMax = PortraitOrientationPosition.MaxAnchors;
            CanvasObjectRectTransform.anchoredPosition = PortraitOrientationPosition.Position;
            CanvasObjectRectTransform.localScale = PortraitOrientationPosition.Scale;
        }
    }

    [SerializeField] private CanvasObject[] m_CanvasObjects;

    private bool isLandscape;
    private bool isPortrait;

    private void Update()
    {
        if (Screen.width > Screen.height)
        {
            if (isLandscape == false)
            {
                ActivateLandscapeOrientation();
                isLandscape = true;
                isPortrait = false;
            }
            
        }
        else
        {
            if (isPortrait == false)
            {
                ActivatePortraitOrientation();
                isPortrait = true;
                isLandscape = false;
            }
        }
    }

    private void ActivatePortraitOrientation()
    {
        for (int i = 0; i < m_CanvasObjects.Length; i++)
        {
            m_CanvasObjects[i].MovingTheCanvasObjectToPortraitOrientation();
        }
    }
    private void ActivateLandscapeOrientation()
    {
        for (int i = 0; i < m_CanvasObjects.Length; i++)
        {
            m_CanvasObjects[i].MovingTheCanvasObjectToLandscapeOrientation();
        }
    }

    
}
