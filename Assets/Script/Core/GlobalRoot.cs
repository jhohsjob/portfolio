using UnityEngine;


// Global 오브젝트(루트)에 부착. 앱 수명 오브젝트의 참조만 제공한다.
// 보존(DontDestroyOnLoad)과 중복 파괴는 App.EnsureCreated가 담당한다.
public class GlobalRoot : MonoBehaviour
{
    [SerializeField]
    private SoundRoot _soundRoot;
    [SerializeField]
    private Transform _popupCanvas;

    public SoundRoot SoundRoot => _soundRoot;
    public Transform PopupCanvas => _popupCanvas;
}