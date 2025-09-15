using UnityEngine;


public class IEnumeratorTool : MonoSingleton<IEnumeratorTool>
{
    WaitForSeconds m_waitForOneSecond = new WaitForSeconds(1.0f);

    public WaitForSeconds waitForOneSecond
    {
        get { return m_waitForOneSecond; }
    }

    public WaitForSeconds waitForHalfSecond { get; } = new WaitForSeconds(0.5f);

    public WaitForSeconds waitForZeroSecond { get; } = new WaitForSeconds(0.1f);

    protected override void OnAwake()
    {
    }
}