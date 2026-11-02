using System.Collections.Generic;


public class GachaResponse
{
    public NetworkResult result;
    public GachaPVO gachaPVO;
    public UserPVO userPVO;
    public Dictionary<int, MercenaryPVO> mercenaryPVOs;
    public List<RewardBase> rewards;
}
