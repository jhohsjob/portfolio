//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;


//public class UIBattleTopContext
//{
//    public IBattleController battleController;
//    public IPopupService popupService;
//    public ISceneLoader sceneLoader;
//}

//public class UIBattleTop : MonoBehaviour
//{
//    private UIBattleTopContext _context;

//    [SerializeField]
//    private Button _btnPause;

//    [SerializeField]
//    private TextMeshProUGUI _txtGold;
//    [SerializeField]
//    private TextMeshProUGUI _txtElementWater;
//    [SerializeField]
//    private TextMeshProUGUI _txtElementForest;
//    [SerializeField]
//    private TextMeshProUGUI _txtElementFire;

//    private void Awake()
//    {
//        _txtGold.text = "0";

//        _btnPause.onClick.AddListener(OnClickPause);

//        EventHelper.AddEventListener(EventName.AddElement, OnAddElement);
//    }

//    private void OnDestroy()
//    {
//        EventHelper.RemoveEventListener(EventName.AddElement, OnAddElement);
//    }

//    public void InitDependencies(UIBattleTopContext context)
//    {
//        _context = context;
//    }

//    public void Bind(BattleRewardRuntime reward)
//    {
//        reward.onGoldChanged += HandleGoldChanged;
//    }

//    private void OnClickPause()
//    {
//        _context.popupService.ShowPopup<UIPausePopup>(PopupName.UIPausePopup, onLoadedCallback: popup =>
//        {
//            popup.Init(HandleBattleResume, _context.sceneLoader);
//        });
//    }

//    private void HandleBattleResume()
//    {
//        _context.battleController.SetStatus(BattleStatus.Running);
//    }

//    private void HandleGoldChanged(int gold)
//    {
//        _txtGold.text = gold.ToString();
//    }

//    private void OnAddElement(object sender, object data)
//    {
//        if (data is Dictionary<ElementType, int> elements == false)
//        {
//            return;
//        }

//        foreach (var element in elements)
//        {
//            switch (element.Key)
//            {
//                case ElementType.Water:
//                    _txtElementWater.text = element.Value.ToString();
//                    break;

//                case ElementType.Forest:
//                    _txtElementForest.text = element.Value.ToString();
//                    break;

//                case ElementType.Fire:
//                    _txtElementFire.text = element.Value.ToString();
//                    break;
//            }
//        }
//    }
//}
