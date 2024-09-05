using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParent : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    void Update()
    {
        transform.position = followTarget.position;
    }
}