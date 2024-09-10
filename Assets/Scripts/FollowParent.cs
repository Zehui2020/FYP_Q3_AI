using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParent : MonoBehaviour
{
    public enum FollowType
    {
        Position,
        LocalScale
    }
    public FollowType followType;

    [SerializeField] private Transform followTarget;

    void Update()
    {
        switch (followType)
        {
            case FollowType.Position:
                transform.position = followTarget.position;
                break;
            case FollowType.LocalScale:
                transform.localScale = followTarget.localScale;
                break;
        }   
    }
}