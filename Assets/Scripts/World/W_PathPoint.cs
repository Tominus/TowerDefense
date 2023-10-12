using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class W_PathPoint : MonoBehaviour
{
    [SerializeField] private SpriteRenderer render = null;

    public void DeactivateRender()
    {
        render.enabled = false;
    }
}