using UnityEngine;


public class ActorSubView : MonoBehaviour
{
    protected Renderer _renderer;

    public void SetSprite(Renderer renderer)
    {
        _renderer = renderer;
    }
}
