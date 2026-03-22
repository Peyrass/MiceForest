using UnityEngine;
using DG.Tweening;

public class SC_Coin : MonoBehaviour
{
    [SerializeField] private Vector3 endRotation;
    [SerializeField] private float endPosition;
    [SerializeField] private float duration;
    [SerializeField] private Transform coin;   
    
    void Start()
    {
        coin.DOMoveY(endPosition, duration).SetLoops(-1, LoopType.Yoyo);
        coin.DORotate(endRotation, duration, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart);
    }
}
